using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using System.IO;

[Serializable]
public class AnalyticsEvent
{
    public string eventName;
    public string timestamp;

    public AnalyticsEvent(string eventName)
    {
        this.eventName = eventName;
        this.timestamp = DateTime.UtcNow.ToString("HH:mm:ss yyyy-MM-dd");
    }
}

[Serializable]
public class AnalyticsEvent<T> : AnalyticsEvent
{
    public T data;

    public AnalyticsEvent(string eventName, T data) : base(eventName)
    {
        this.data = data;
    }
}

[Serializable]
public class AnalyticsEventLevelCompleted
{
    public string time;
    public string point;
    public AnalyticsEventLevelCompleted(string time, string point)
    {
        this.time = time;
        this.point = point;
    }
}

public class AnalyticsManager : MonoBehaviour
{
    public List<AnalyticsEvent> game_started = new List<AnalyticsEvent>() { };
    public List<AnalyticsEvent<AnalyticsEventLevelCompleted>> level_completed = new List<AnalyticsEvent<AnalyticsEventLevelCompleted>>() { };
    public List<AnalyticsEvent> drone_crashed = new List<AnalyticsEvent>() { };
    public List<AnalyticsEvent> game_closed = new List<AnalyticsEvent>() { };


    public static AnalyticsManager Instance;
    private string folderPath = Path.Combine(Application.dataPath, "Analytics");


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        GameObject gameObject = new GameObject("AnalyticsManager");
        gameObject.AddComponent<AnalyticsManager>();
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
        GameStarted();
        // LevelCompleted("asd", "1");
        // LevelCompleted("asd", "2");
        // DroneCrashed();
    }

    void OnApplicationQuit()
    {
        GameClosed();

        string filePath = Path.Combine(folderPath, $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}.json");
        string analyticsJson = JsonUtility.ToJson(this);
        Debug.Log($"Отправка данных: {analyticsJson}");
        File.WriteAllText(filePath, analyticsJson);
    }

    void GameStarted()
    {
        AnalyticsEvent Event = new AnalyticsEvent("game_started");

        game_started.Add(Event);
    }

    void LevelCompleted(string time, string point)
    {
        AnalyticsEventLevelCompleted saveData = new AnalyticsEventLevelCompleted(time, point);

        AnalyticsEvent<AnalyticsEventLevelCompleted> Event = new AnalyticsEvent<AnalyticsEventLevelCompleted>("level_completed", saveData);

        level_completed.Add(Event);
    }

    void DroneCrashed()
    {
        AnalyticsEvent Event = new AnalyticsEvent("drone_crashed");

        drone_crashed.Add(Event);
    }

    void GameClosed()
    {
        AnalyticsEvent Event = new AnalyticsEvent("game_closed");

        game_closed.Add(Event);
    }
}
