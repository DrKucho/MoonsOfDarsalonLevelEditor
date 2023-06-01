using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaInverseLerp : MonoBehaviour {

    public float a = 0;
    public float b = 100;
    public float t = 90;
    void OnValidate(){
        if (isActiveAndEnabled)
        {
            float r = Mathf.InverseLerp(a, b, t);
            print(r);
                
        }
    }

}
