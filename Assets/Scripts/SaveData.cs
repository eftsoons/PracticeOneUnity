using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
public class SaveData : MonoBehaviour
{
    public string version = "1.0";
    public string bestTime = "0";
    public int totalScore = 0;
    public string unlockedLevels = "0";
    private string folderPath = Path.Combine(Application.dataPath, "Saves");

    public static SaveData Instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        GameObject gameObject = new GameObject("SaveData");
        gameObject.AddComponent<SaveData>();
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
        string filePath = Path.Combine(folderPath, $"{version}.json");
        bool isExists = File.Exists(filePath);

        if (isExists)
        {
            string JsonString = File.ReadAllText(filePath);
            byte[] toDecode = Convert.FromBase64String(JsonString);
            string decodeJsonString = Encoding.UTF8.GetString(toDecode);

            JsonUtility.FromJsonOverwrite(decodeJsonString, this);
        }
        else
        {
            saveJson();
        }
    }

    public void saveJson()
    {
        string filePath = Path.Combine(folderPath, $"{version}.json");
        string JsonString = JsonUtility.ToJson(this);
        byte[] toEncode = Encoding.UTF8.GetBytes(JsonString);
        string encodeJsonString = Convert.ToBase64String(toEncode);

        File.WriteAllText(filePath, encodeJsonString);
    }
}
