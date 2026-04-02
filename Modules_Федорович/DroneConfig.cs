using System;
using System.Collections.Generic;
using UnityEngine;



public class DroneConfig : ScriptableObject
{
    public int maxSpeed = 0;
    public int acceleration = 0;
    public int rotationSpeed = 0;
    public int batteryLife = 0;
    public int obstaclePenalty = 0;
}