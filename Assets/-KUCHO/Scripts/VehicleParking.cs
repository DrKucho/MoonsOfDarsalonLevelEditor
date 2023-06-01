using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VehicleParking : MonoBehaviour
{

    static public List<VehicleParking> enabledInstances = new List<VehicleParking>();
    public bool vehicleDetected = false;
    [ReadOnly2Attribute] public Collider2D col;

    public enum Type
    {
        AllowBaseToOpen,
        StartMovingPLatform
    };

    public Type type = Type.AllowBaseToOpen;
    [ReadOnly2Attribute] public Base baseArrival;
    [ReadOnly2Attribute] public MovingPlatform movingPlatform;
    public static VehicleParking instance;

    public void InitialiseInEditor()
    {
        movingPlatform = GetComponentInParent<MovingPlatform>();
        col = GetComponentInChildren<Collider2D>();
        if (col)
            col.gameObject.layer = Layers.vision;
    }


    

    private VehicleInput vehicle;

}
