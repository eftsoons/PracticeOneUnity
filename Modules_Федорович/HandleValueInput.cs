using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HandleValueInput : MonoBehaviour
{
    public TMP_InputField InputMaxSpeed;
    public TMP_InputField InputAcceleration;
    public TMP_InputField InputRotationSpeed;
    public TMP_InputField InputBatteryLife;
    public TMP_InputField InputObstaclePenalty;

    void Start()
    {
        if (ConfigManager.Instance != null)
        {
            InputMaxSpeed.text = ConfigManager.Instance.configDrone.maxSpeed.ToString();
            InputAcceleration.text = ConfigManager.Instance.configDrone.acceleration.ToString();
            InputRotationSpeed.text = ConfigManager.Instance.configDrone.rotationSpeed.ToString();
            InputBatteryLife.text = ConfigManager.Instance.configDrone.batteryLife.ToString();
            InputObstaclePenalty.text = ConfigManager.Instance.configDrone.obstaclePenalty.ToString();

            InputMaxSpeed.onValueChanged.AddListener(setMaxSpeed);
            InputAcceleration.onValueChanged.AddListener(setAcceleration);
            InputRotationSpeed.onValueChanged.AddListener(setRotationSpeed);
            InputBatteryLife.onValueChanged.AddListener(setBatteryLife);
            InputObstaclePenalty.onValueChanged.AddListener(setObstaclePenalty);
        }
    }

    public void setMaxSpeed(string num)
    {
        if (int.TryParse(num, out int result))
        {
            ConfigManager.Instance.configDrone.maxSpeed = result;
        }
    }

    public void setAcceleration(string num)
    {
        if (int.TryParse(num, out int result))
        {
            ConfigManager.Instance.configDrone.acceleration = result;
        }
    }

    public void setRotationSpeed(string num)
    {
        if (int.TryParse(num, out int result))
        {
            ConfigManager.Instance.configDrone.rotationSpeed = result;
        }
    }

    public void setBatteryLife(string num)
    {
        if (int.TryParse(num, out int result))
        {
            ConfigManager.Instance.configDrone.batteryLife = result;
        }
    }

    public void setObstaclePenalty(string num)
    {
        if (int.TryParse(num, out int result))
        {
            ConfigManager.Instance.configDrone.obstaclePenalty = result;
        }
    }
}
