
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class KuchoTransformFineEditor : MonoBehaviour {

    public Vector3 baseScale = Vector3.one;
    [Range(-0.1f, 0.1f)]public float scaleShift;
    [Range(-0.1f, 0.1f)]public float sxShift;
    [Range(-0.1f, 0.1f)]public float syShift;
    [Range(-0.1f, 0.1f)]public float szShift;

    public Vector3 basePos;
    [Range(-0.1f, 0.1f)]public float xShift;
    [Range(-0.1f, 0.1f)]public float yShift;
    [Range(-0.1f, 0.1f)]public float zShift;

    
    void Grab(){
        baseScale = transform.localScale;
        basePos = transform.localPosition;
    }

    void OnValidate(){
        if (isActiveAndEnabled)
        {
            transform.localScale = baseScale + new Vector3(scaleShift + sxShift, scaleShift + syShift, scaleShift + szShift);
            transform.localPosition = basePos + new Vector3(xShift, yShift, zShift);

        }
    }
}
