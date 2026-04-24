using System;
using System.Collections.Generic;
using UnityEngine;



public class DroneConfig : ScriptableObject
{
    public float maxSpeed = 15f;
    public float acceleration = 2.5f;
    public float rotationSpeed = 100f;
    public float batteryLife = 100;
    public int obstaclePenalty = 100;
}