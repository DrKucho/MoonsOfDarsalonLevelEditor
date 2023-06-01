using System.Collections;
using System.Collections.Generic;

using UnityEngine;
[ExecuteInEditMode]
public class DoorMovementExtension : MonoBehaviour
{
    public Transform topPoint;
    public Transform bottomPoint;
    Transform myTrans;

    void Awake()
    {
        myTrans = transform;
    }

    public void OnDoorMovement()
    {
        float midPoint = (topPoint.position.y - bottomPoint.position.y);
        float halfMidPoint = midPoint * 0.5f;
        Vector3 myScale = myTrans.localScale;
        myScale.y = Mathf.Abs(midPoint);
        myTrans.localScale = myScale;
        var myPos  = myTrans.position; 
        myPos.y = bottomPoint.position.y + halfMidPoint;
        myTrans.position = myPos;
    }
}
