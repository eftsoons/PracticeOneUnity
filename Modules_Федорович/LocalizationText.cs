using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LocalizationText : MonoBehaviour
{
    private TextMeshProUGUI Text;
    private string textStart;
    private string textOld;
    void Start()
    {
        Text = GetComponent<TextMeshProUGUI>();
        textStart = Text.text;
    }

    void Update()
    {
        if (LocalizationManager.Instance != null)
        {
            if (textOld != LocalizationManager.Instance.getText(textStart))
            {
                Text.text = LocalizationManager.Instance.getText(textStart);
                textOld = LocalizationManager.Instance.getText(textStart);
            }
        }
    }
}
