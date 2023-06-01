using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    
    
[ExecuteInEditMode]
public class AI_Test : MonoBehaviour
{
    public PatrolPoint patrolPoint;
    public float angleOffset  = -90;
    public float myAngle;
    public Vector2 myVector;
    public bool patrolDirIsForward;

    void Update()
    {
        GetNewCloserPatrolPoint();
    }

    
    public void GetNewCloserPatrolPoint()
    {
        myAngle = KuchoHelper.GetUsefullRotation(transform.eulerAngles.z) - angleOffset;
        myVector = KuchoHelper.DegreeToVector2(myAngle);
        patrolPoint = PatrolPoint.GetNewCloserPatrolPoint(myAngle, transform.position, null, ref patrolDirIsForward);
    }
}
