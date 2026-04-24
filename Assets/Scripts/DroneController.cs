using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class DroneController : MonoBehaviour
{
    [Header("Настройки полета")]
    public float flySpeed;
    public float verticalSpeed;
    public float turnSpeed;         // Скорость поворота (градусов в секунду)
    public float accelerationMult;
    public float brakingForce = 5f;

    [Header("Настройки пропеллеров")]
    [Tooltip("Список трансформов пропеллеров. Должно быть 4.")]
    public Transform[] propellerTransforms;
    [Tooltip("Максимальная скорость вращения пропеллеров (градусов в секунду).")]
    public float maxPropellerSpinSpeed = 1000f;
    [Tooltip("Базовая скорость вращения, когда дрон просто завис.")]
    public float idlePropellerSpinSpeed = 500f;
    [Tooltip("Вокруг какой локальной оси вращать пропеллеры?")]
    public Vector3 rotationAxis = Vector3.up; // Обычно Vector3.up (Y) или Vector3.forward (Z)

    [Header("Ввод с контроллеров")]
    public InputActionProperty leftStick;   // Vector2 (X - поворот, Y - высота)
    public InputActionProperty rightStick;  // Vector2 (Движение)
    public InputActionProperty trigger;     // Float (Ускорение)
    public InputActionProperty grip;        // Float (Торможение)

    private Rigidbody rb;
    private float yawRotation;              // Текущий накопленный угол поворота
    private float currentPropellerSpeed;     // Текущая визуальная скорость пропеллеров

    void Start()
    {
        flySpeed = ConfigManager.Instance.configDrone.maxSpeed;
        verticalSpeed = ConfigManager.Instance.configDrone.maxSpeed;
        turnSpeed = ConfigManager.Instance.configDrone.rotationSpeed;         // Скорость поворота (градусов в секунду)
        accelerationMult = ConfigManager.Instance.configDrone.acceleration;


        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.drag = 1f;
        rb.angularDrag = 2f;

        // Инициализируем начальный поворот
        yawRotation = transform.eulerAngles.y;

        // Простая проверка
        if (propellerTransforms == null || propellerTransforms.Length != 4)
        {
            Debug.LogError("DroneController: Пожалуйста, назначьте ровно 4 пропеллера в инспекторе!");
        }
    }

    void FixedUpdate()
    {
        // 1. Сбор данных
        Vector2 moveInput = rightStick.action.ReadValue<Vector2>();
        Vector2 altitudeAndTurnInput = leftStick.action.ReadValue<Vector2>();
        float boostValue = trigger.action.ReadValue<float>();
        float brakeValue = grip.action.ReadValue<float>();

        // 2. Логика поворота (Yaw) - Левый стик по горизонтали
        float turnInput = altitudeAndTurnInput.x;
        yawRotation += turnInput * turnSpeed * Time.fixedDeltaTime;

        float newCurrentFlySpeed = flySpeed * (1 + boostValue * accelerationMult);

        // 3. Расчет скоростей
        DroneManager.Instance.updateCurrentFlySpeed(newCurrentFlySpeed);

        // 4. Движение (Вперед/Вбок)
        Vector3 moveDir = (transform.forward * moveInput.y) + (transform.right * moveInput.x);
        rb.AddForce(moveDir * newCurrentFlySpeed, ForceMode.Acceleration);

        // 5. Высота (Левый стик Y)
        rb.AddForce(Vector3.up * altitudeAndTurnInput.y * verticalSpeed, ForceMode.Acceleration);

        // 6. Торможение
        if (brakeValue > 0.1f)
        {
            rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, brakeValue * brakingForce * Time.fixedDeltaTime);
        }

        // 7. Применение вращения корпуса и визуального наклона
        ApplyRotationAndTilt(moveInput);
    }

    void Update()
    {
        // Эффект вращения пропеллеров лучше делать в Update, а не в FixedUpdate,
        // так как это чисто визуальный эффект, не влияющий на физику.
        UpdatePropellerVisuals();
    }

    private void ApplyRotationAndTilt(Vector2 moveInput)
    {
        float maxTilt = 20f;
        float lerpSpeed = 5f;

        // Создаем целевой наклон (Pitch и Roll) на основе ввода правого стика
        Quaternion targetRotation = Quaternion.Euler(
            moveInput.y * maxTilt,      // Наклон вперед/назад
            yawRotation,               // Поворот вокруг оси Y
            -moveInput.x * maxTilt     // Наклон влево/вправо
        );

        // Плавный переход к целевому вращению
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.fixedDeltaTime * lerpSpeed));
    }

    private void UpdatePropellerVisuals()
    {
        if (propellerTransforms == null || propellerTransforms.Length != 4) return;

        // 1. Рассчитываем целевую скорость вращения.
        // За основу берем ввод высоты (левый стик Y) и ввод ускорения (триггер).
        float altitudeInput = leftStick.action.ReadValue<Vector2>().y;
        float boostInput = trigger.action.ReadValue<float>();

        // Насколько сильно пользователь "газует" (от 0 до 1)
        float throttleFactor = Mathf.Max(Mathf.Abs(altitudeInput), boostInput);

        // Рассчитываем визуальную скорость: idle + (зависимость от газа)
        float targetSpeed = idlePropellerSpinSpeed + (throttleFactor * (maxPropellerSpinSpeed - idlePropellerSpinSpeed));

        // Плавно изменяем текущую скорость вращения (для красоты)
        currentPropellerSpeed = Mathf.Lerp(currentPropellerSpeed, targetSpeed, Time.deltaTime * 5f);

        // 2. Вращаем каждый пропеллер
        foreach (Transform prop in propellerTransforms)
        {
            if (prop != null)
            {
                // Вращаем вокруг выбранной оси в локальных координатах
                prop.Rotate(rotationAxis, currentPropellerSpeed * Time.deltaTime, Space.Self);

                // Создаем пустой объект для хвоста, чтобы он не искажал меш дрона
                GameObject trailObj = new GameObject("DroneTrail");
                trailObj.transform.SetParent(prop);
                trailObj.transform.localPosition = Vector3.zero;

                TrailRenderer tr = trailObj.AddComponent<TrailRenderer>();

                // Настройка внешнего вида
                tr.time = 0.25f;           // Как долго живет след
                tr.startWidth = 0.1f;     // Ширина в начале
                tr.endWidth = 0f;         // Сужение в конце

                // Создаем материал программно (чтобы не был розовым/пустым)
                tr.material = new Material(Shader.Find("Sprites/Default"));
                tr.startColor = Color.white;
                tr.endColor = new Color(1, 1, 1, 0); // Уходит в прозрачность
            }
        }
    }
}