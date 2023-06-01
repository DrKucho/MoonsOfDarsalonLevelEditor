using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour {


     void DoX()
    {
        TransformHelper.SetEulerAngleX(transform, Random.Range(0, 360));
    }
    
    void DoY()
    {
        TransformHelper.SetEulerAngleY(transform, Random.Range(0, 360));
    }
    
    void DoZ()
    {
        TransformHelper.SetEulerAngleZ(transform, Random.Range(0, 360));
    }
    
    void ZeroRotations()
    {
        transform.localEulerAngles = Vector3.zero;
    }
}
