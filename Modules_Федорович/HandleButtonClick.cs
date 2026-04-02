using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleButtonClick : MonoBehaviour
{

    public void handleSetRuTranslition()
    {
        LocalizationManager.Instance.currentLanguageType = LanguageType.ru;
    }

    public void handleSetEnTranslition()
    {
        LocalizationManager.Instance.currentLanguageType = LanguageType.en;
    }

    public void handleOpenMain()
    {
        SceneManager.LoadScene("Main");
    }

    public void handleOpenSettings()
    {
        SceneManager.LoadScene("Settings");
    }

    public void handleOpenLocalization()
    {
        SceneManager.LoadScene("Localization");
    }
}
