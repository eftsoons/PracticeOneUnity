using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public LanguageType currentLanguageType = LanguageType.ru;
    public Dictionary<LanguageType, Dictionary<string, string>> dataLanguage;
    public LanguageData languageData;
    public static LocalizationManager Instance;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Initialize()
    {
        GameObject gameObject = new GameObject("LocalizationManager");
        gameObject.AddComponent<LocalizationManager>();
        DontDestroyOnLoad(gameObject);
    }

    void Awake()
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

        if (languageData == null) languageData = ScriptableObject.CreateInstance<LanguageData>();
    }

    public string getText(string key)
    {

        return languageData.languageDictionary[currentLanguageType][key];
    }
}
