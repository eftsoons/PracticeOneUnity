using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;


public class DroneManager : MonoBehaviour
{
    public static DroneManager Instance;
    public int score = 0;
    public TextMeshProUGUI TextComponentScoreNum;
    public TextMeshProUGUI TextComponentSpeedNum;
    public TextMeshProUGUI TextComponentBatteryLifeNum;
    public GameObject DroneObject;
    public float currentFlySpeed = 0f;
    public DateTime endDroneTime;
    public bool isEndTimeNow = false;
    private float timer = 0f;
    public AudioSource audioSource;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        GameObject gameObject = new GameObject("DroneManager");
        gameObject.AddComponent<DroneManager>();
        DontDestroyOnLoad(gameObject);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        DroneObject = GameObject.Find("Drone");

        if (!DroneObject)
        {
            Destroy(gameObject);

            return;
        }

        audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.loop = true;

        AudioClip clip = Resources.Load<AudioClip>("Music/RumbleFpv");

        audioSource.clip = clip;

        audioSource.Play();

        GameObject TextObjectScoreNum = GameObject.Find("ScoreNum");
        TextMeshProUGUI newTextObjectScoreNum = TextObjectScoreNum.GetComponent<TextMeshProUGUI>();

        TextComponentScoreNum = newTextObjectScoreNum;
        TextComponentScoreNum.text = score.ToString();

        GameObject TextObjectSpeedNum = GameObject.Find("SpeedNum");
        TextMeshProUGUI newTextObjectSpeedNum = TextObjectSpeedNum.GetComponent<TextMeshProUGUI>();

        TextComponentSpeedNum = newTextObjectSpeedNum;
        TextComponentSpeedNum.text = currentFlySpeed.ToString();

        GameObject TextObjectBatteryLifeNum = GameObject.Find("BatteryLifeNum");
        TextMeshProUGUI newTextObjectBatteryLifeNum = TextObjectBatteryLifeNum.GetComponent<TextMeshProUGUI>();

        TextComponentBatteryLifeNum = newTextObjectBatteryLifeNum;
    }

    void Update()
    {
        if (!isEndTimeNow)
        {
            endDroneTime = DateTime.UtcNow.AddSeconds(ConfigManager.Instance.configDrone.batteryLife);
            isEndTimeNow = true;
        }

        double secondsEnd = (endDroneTime - DateTime.UtcNow).TotalSeconds;

        if (secondsEnd <= 0)
        {
            Destroy(gameObject);
            SceneManager.LoadScene("Main");
        }

        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            TextComponentBatteryLifeNum.text = secondsEnd.ToString();
            timer = 0;
        }
    }

    public void addScore(int addScore)
    {
        score += addScore;
        TextComponentScoreNum.text = score.ToString();
    }

    public void updateCurrentFlySpeed(float newCurrentFlySpeed)
    {
        currentFlySpeed = newCurrentFlySpeed;
        TextComponentSpeedNum.text = currentFlySpeed.ToString();
    }
}
