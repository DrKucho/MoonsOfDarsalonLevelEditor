using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class PatrolPoint2 : MonoBehaviour
{
    public PatrolPoint2 closerPoint;
    public float minDist = 150;
    public PatrolPoint2 previousPoint;
    [ReadOnly2Attribute] public float previousPointAngle ;
    [ReadOnly2Attribute] public float angleDiff;
    [ReadOnly2Attribute] public SWizTextMesh tm;
    [ReadOnly2Attribute] public LineRenderer line;    
    private static List<PatrolPoint2> staticPatrolPoints;
    public void InitialiseInEditor()
    {
        tm = GetComponent<SWizTextMesh>();
        line = GetComponent<LineRenderer>();
    }


    void Start()
    {
        closerPoint = null;
    }

    void OnEnable()
    {
        if (staticPatrolPoints == null)
            FindAndRebuildPatrolPointList();

        if (Application.isEditor || Constants.appIsLevelEditor)
        {
            FixName();
        }

        if (staticPatrolPoints == null || staticPatrolPoints.Contains(this))
            FindAndRebuildPatrolPointList();
        else
            staticPatrolPoints.Add(this);
    }

    private void OnDisable()
    {
        if (staticPatrolPoints != null)
        {
            staticPatrolPoints.Remove(this);
        }
    }

    private void FixName()
    {
        name = "PP";
        tm.text = name;
    }

    private Vector3 posOld;
    private Vector3 nextPosOld;
    void Update()
    {
        Get2CloserPatrolPoints();

        if (transform.position != posOld)
        {

                var l = line;
                var c = closerPoint;
                if (c)
                {
                    l.startWidth = 20;
                    l.endWidth = 4;
                    l.startColor = Color.blue;
                    l.endColor = Color.white;

                    if (l.positionCount != 2)
                        l.positionCount = 2;

                    l.SetPosition(0, transform.position);
                    l.SetPosition(1, c.transform.position);
                    if (l.transform != transform)
                        l.transform.localPosition = Constants.zero3;
                }
                else
                {
                    if (l.positionCount != 0)
                        l.positionCount = 0;
                }
            
        }
    }

    void FindAndRebuildPatrolPointList()
    { 
        
        if (staticPatrolPoints != null)
            staticPatrolPoints.Clear();
        else
            staticPatrolPoints = new List<PatrolPoint2>();
        
        var array = FindObjectsOfType<PatrolPoint2>();
        staticPatrolPoints = array.OrderBy(go=>go.name).ToList();
        
    }

    void Get2CloserPatrolPoints()
    {
        Vector2 pos = transform.position;
        float distMag;
        float magToCloserPoint = float.MaxValue;

        closerPoint = null;
        foreach (PatrolPoint2 p in staticPatrolPoints)
        {
            if (p != this) // ignoro si es el point que el llamador tiene
            {
                Vector2 pPos = p.transform.position;
                Vector2 dist;
                dist.x = pPos.x - pos.x;
                dist.y = pPos.y - pos.y;

                distMag = dist.magnitude;
                if (distMag < minDist)
                {
                    if (distMag < magToCloserPoint && ShouldIAttachToThisThisOtherPoint(p))
                    {
                        magToCloserPoint = distMag;
                        closerPoint = p;
                    }
                }
            }
        }
        if (closerPoint)
            closerPoint.previousPoint = this;
    }

    bool ShouldIAttachToThisThisOtherPoint(PatrolPoint2 other) 
    {
        if (other.closerPoint == this)
            return false;
        if (other.closerPoint && other.closerPoint.closerPoint == this)
            return false;
        return true;
    }
}
