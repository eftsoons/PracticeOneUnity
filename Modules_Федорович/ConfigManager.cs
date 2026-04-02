using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ConfigManager : MonoBehaviour
{
    public static ConfigManager Instance;
    public DroneConfig configDrone;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        GameObject gameObject = new GameObject("ConfigManager");
        gameObject.AddComponent<ConfigManager>();
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

        if (configDrone == null) configDrone = ScriptableObject.CreateInstance<DroneConfig>();
    }
}
