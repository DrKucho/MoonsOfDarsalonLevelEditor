using System;
using UnityEngine;
using System.Collections;
using System.ComponentModel;

	
//using System.Net.Configuration;

public class LocalShake : MonoBehaviour
{
    [Range(0,100)] public float speed = 0.1f;
    private float _speed;
    public Vector3 shift;
    [ReadOnly2Attribute] public Vector3 startPos;
    [ReadOnly2Attribute] public Vector3 endPos;
    private Transform myTransform;
    private void Awake()
    {
        myTransform = transform;
        startPos = myTransform.localPosition;
    }

    void OnEnable()
    {
        endPos = startPos + shift;
        speed = Mathf.Abs(speed);
        _speed = speed;
    }

    private void OnValidate()
    {
        _speed = speed; //?
    }

    private float t = 0;
    public void Update ()
    {
        t += _speed * KuchoTime.unityDeltaTime;
        if (t > 1)
        {
            t = 1;
            _speed *= -1;
        }
        else if (t < 0)
        {
            t = 0;
            _speed *= -1;
        }

        var newPos = Vector3.Lerp(startPos, endPos, t);
        myTransform.localPosition = newPos;

    }
}