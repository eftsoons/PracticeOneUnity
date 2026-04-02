using System;
using System.Collections.Generic;
using UnityEngine;



public enum LanguageType
{
    ru,
    en
}

public class LanguageData : ScriptableObject
{
    public Dictionary<LanguageType, Dictionary<string, string>> languageDictionary = new Dictionary<LanguageType, Dictionary<string, string>>()
    {
        {
            LanguageType.ru, new Dictionary<string, string>
            {
                {"mainMenu", "Главное меню"},
                {"localization", "Локализация"},
                {"settings", "Настройки"},
                {"russian", "Русский"},
                {"english", "Английский"},
                {"maxSpeed", "Максимальная скорость"},
                {"acceleration", "Ускорение"},
                {"rotationSpeed", "Скорость поворота"},
                {"batteryLife", "Время работы батареи"},
                {"obstaclePenalty", "Штраф за препятствие"},
            }
        },
        {
            LanguageType.en, new Dictionary<string, string>
            {
                {"mainMenu", "Main menu"},
                {"localization", "Localization"},
                {"settings", "Settings"},
                {"russian", "Russian"},
                {"english", "English"},
                {"maxSpeed", "Max speed"},
                {"acceleration", "Acceleration"},
                {"rotationSpeed", "Rotation speed"},
                {"batteryLife", "Battery life"},
                {"obstaclePenalty", "Obstacle penalty"},

            }
        }
    };
}