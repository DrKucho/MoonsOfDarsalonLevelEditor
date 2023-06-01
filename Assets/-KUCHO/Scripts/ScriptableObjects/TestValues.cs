using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New TestValues", menuName = "Test Values", order = 51)]
public class TestValues : ScriptableObject
{
    public float testFloat1;
    public float testFloat2;
    public float testFloat3;
    public float testFloat4;
    public float testFloat5;
    public float testFloat6;
    public float testFloat7;
    public float testFloat8;
    public float testFloat9;
    public float testFloat10;
    public Vector3 testVector3_1;
    public Vector3 testVector3_2;
    public Vector3 testVector3_3;
    public Vector3 testVector3_4;
    public Vector3 testVector3_5;
    public Vector3 testVector3_6;
    
    private static TestValues _instance;
    public static TestValues instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameData.instance.testValues;
                if (!_instance)
                {
                    Debug.Log("NO ENCUENTRO TEST VALUES");
                }
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
}