using System;
using UnityEngine;

using UnityEngine.UI;           // Для Image, CanvasScaler и GraphicRaycaster
using UnityEngine.EventSystems;
using System.Collections; // Иногда требуется для корректной работы UI

public class DroneTouching : MonoBehaviour
{
    private bool isPassed = false;
    public bool isAttack = false;
    public int score = 0;

    private void OnTriggerEnter(Collider other)
    {
        if ((!isPassed || isAttack) && other.GetComponentInParent<DroneController>() != null)
        {
            DroneTouchingPassed();
        }
    }

    private void DroneTouchingPassed()
    {
        isPassed = true;

        if (isAttack)
        {

            CreateSparksEffect();

            AudioClip clip = Resources.Load<AudioClip>("Music/Attack");

            DroneManager.Instance.audioSource.PlayOneShot(clip, 1.5f);

            AnalyticsManager.Instance.DroneCrashed();
            DroneManager.Instance.DroneObject.transform.position -= DroneManager.Instance.DroneObject.transform.forward * 5f;
        }
        else
        {
            TriggerFlash();

            AudioClip clip = Resources.Load<AudioClip>("Music/AddScore");

            DroneManager.Instance.audioSource.PlayOneShot(clip, 1.5f);

            SaveData.Instance.totalScore += score;

            AnalyticsManager.Instance.LevelCompleted(DateTime.UtcNow.ToString("HH:mm:ss yyyy-MM-dd"), score.ToString());
        }

        if (!isAttack || score != 0)
        {
            DroneManager.Instance.addScore(score);
        }
        else
        {
            DroneManager.Instance.addScore(-ConfigManager.Instance.configDrone.obstaclePenalty);
        }
    }

    public void CreateSparksEffect()
    {
        // Создаем объект и привязываем к дрону
        GameObject sparksObject = new GameObject("DynamicSparks");

        GameObject droneObject = DroneManager.Instance.DroneObject;

        sparksObject.transform.position = droneObject.transform.position;
        sparksObject.transform.SetParent(droneObject.transform);

        ParticleSystem ps = sparksObject.AddComponent<ParticleSystem>();

        // Настройка Main модуля
        ParticleSystem.MainModule main = ps.main;
        main.loop = false;
        main.playOnAwake = false;
        main.startLifetime = 0.5f;
        main.startSpeed = 7f;
        main.startSize = 0.1f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.stopAction = ParticleSystemStopAction.Destroy;

        // Настройка Emission (Всплеск)
        ParticleSystem.EmissionModule emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, 20) });

        // Настройка Shape
        ParticleSystem.ShapeModule shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;

        // ИСПРАВЛЕННЫЙ БЛОК: Настройка цвета
        ParticleSystem.ColorOverLifetimeModule colorModule = ps.colorOverLifetime;
        colorModule.enabled = true;

        Gradient grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.yellow, 0.0f), new GradientColorKey(Color.red, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );

        // В модуле ColorOverLifetime используется поле .color, а не .gradient
        colorModule.color = new ParticleSystem.MinMaxGradient(grad);

        // Настройка рендерера
        ParticleSystemRenderer psRenderer = ps.GetComponent<ParticleSystemRenderer>();
        psRenderer.material = new Material(Shader.Find("Sprites/Default"));

        ps.Play();
    }

    public void TriggerFlash()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        // 1. Создаем временный Canvas
        GameObject canvasObj = new GameObject("TempFlashCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        // Сортировка: отрисовываем поверх всего остального UI
        canvas.sortingOrder = 999;

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 2. Создаем белую картинку внутри
        GameObject imageObj = new GameObject("FlashImage");

        // ВАЖНО: false указывает Unity не пересчитывать мировые координаты в локальные
        imageObj.transform.SetParent(canvasObj.transform, false);

        Image img = imageObj.AddComponent<Image>();
        img.color = Color.white;

        // 3. Растягиваем на весь экран
        RectTransform rect = img.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;

        // Обнуляем все отступы и трансформации
        rect.offsetMin = Vector2.zero; // Лево и Низ
        rect.offsetMax = Vector2.zero; // Право и Верх
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;

        // 4. Анимация затухания
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            img.color = new Color(1, 1, 1, alpha);
            yield return null;
        }

        // 5. Удаляем весь Canvas
        Destroy(canvasObj);
    }
}