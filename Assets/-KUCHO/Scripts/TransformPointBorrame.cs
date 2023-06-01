using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class TransformPointBorrame: MonoBehaviour
{
    public Vector3 offset;
    public GameObject placeThis;
    
    void Update(){
        if (isActiveAndEnabled & placeThis)
        {
            var newPos = transform.TransformPoint(offset);
            placeThis.transform.position = newPos;
        }
    }
}