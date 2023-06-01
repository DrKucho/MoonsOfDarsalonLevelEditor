using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
    
#endif
using UnityEngine.Serialization;

[ExecuteInEditMode]
public class PatrolPoint : MonoBehaviour
{
    public PatrolPoint next;
    [HideInInspector] public float angleToNext;
    [HideInInspector] public Vector2 vectorToNext;
    public PatrolPoint previous;
    [HideInInspector] public float angleToPrevious;
    [HideInInspector] public Vector2 vectorToPrevious;
    [HideInInspector] public int myNumber;
    [HideInInspector] public TextMesh tm;
    [HideInInspector] public LineRenderer lineToNext;    
    [HideInInspector] public LineRenderer lineToPrevious;    
    private static List<PatrolPoint> staticPatrolPoints;
    private static float staticPatrolPointsUpdateTime;
    public void InitialiseInEditor()
    {
        tm = GetComponent<TextMesh>();
        lineToNext = GetComponent<LineRenderer>();
        var allLineRenderers = GetComponentsInChildren<LineRenderer>();
        GameObject child = null;
        if (allLineRenderers.Length == 1)
        {
            child = new GameObject("LineToPrevious");
            child.transform.parent = gameObject.transform;
            lineToPrevious = child.AddComponent<LineRenderer>();
        }
        else
        {
            foreach(LineRenderer l in allLineRenderers)
                if (l != lineToNext)
                    lineToPrevious = l;
        }
        lineToPrevious.transform.localPosition = new Vector3(0,0,-1);
        lineToPrevious.widthCurve = lineToNext.widthCurve;
        lineToPrevious.widthMultiplier = lineToNext.widthMultiplier * 0.25f;
        lineToPrevious.sharedMaterial = lineToNext.sharedMaterial;

        lineToPrevious.gameObject.layer = Layers.defaultLayer;
    }
    
    void DetachPrevious()
    {
        if (previous)
        {
            previous.next = null;
            previous = null;
        }
    }

    void Start_NO()
    {
        if (Application.isEditor || Constants.appIsLevelEditor)
        {
            var p = GetCloserPatrolPoint();
            previous = p;
            next = null;
        }
    }

    void Awake() // piensa que al duplicar un punto salta su awake
    {
        if (!Application.isPlaying)
            FindAndRebuildPatrolPointList();
    }

    void OnEnable()
    {
        if (staticPatrolPoints == null || staticPatrolPoints.Contains(this))
            FindAndRebuildPatrolPointList();
        else
            staticPatrolPoints.Add(this);
        
        if (!next && previous)
        {
            foreach (var p in staticPatrolPoints)
            {
                if (p.previous == previous && p.next == next) // tiene los mismos que yo? 
                {
                    if (p.transform.position == transform.position)// y estoy justo encima? HE SIDO DUPLICCADO DE ESTE
                    {
                        previous = p;
                        p.next = this;
                    }
                }
            }
        }
        
        staticPatrolPointsUpdateTime = Time.time;
    }

    private void OnDisable()
    {
        if (staticPatrolPoints != null)
        {
            staticPatrolPoints.Remove(this);
            staticPatrolPointsUpdateTime = Time.time;
        }
    }
    

    private Vector3 posOld;
    private Vector3 nextPosOld;
    private Vector3 previousPosOld;
    void Update()
    {
        #if UNITY_EDITOR
        if (UnityEditor.Selection.activeObject == gameObject)
        {
            UnityEditor.Tools.pivotMode = UnityEditor.PivotMode.Pivot;
        }
        #endif
        
        var pos = transform.position;
        pos.z = -25f;// para que siempre sea visible delante de las rocas y otras mierdas
        transform.position = pos;

        FixNumber();

        if (next)
        {
            if (next == this)
                next = null;
            else
                next.previous = this;
        }

        if (previous)
        {
            if (previous.next != this)
                previous = null;
        }

        if (transform.position != posOld || next && next.transform.position != nextPosOld)
        {
            lineToNext.SetPosition(0,transform.position);
            if (next)
            {
                nextPosOld = next.transform.position;
                lineToNext.positionCount = 2;
                lineToNext.SetPosition(1, next.transform.position);
                vectorToNext = next.transform.position - transform.position;
                angleToNext = KuchoHelper.AngleZ(vectorToNext) -90;// el -90 coloca el angulo 0 hacia arriba que es como lo usa GetNextPatrolPoint para comparar
            }
            else
                lineToNext.positionCount = 1;
            
        }
        
        if (transform.position != posOld || previous && previous.transform.position != previousPosOld)
        {
            if (lineToPrevious)
            {
                if (previous)
                {
                    lineToPrevious.SetPosition(0, previous.transform.position);
                    previousPosOld = previous.transform.position;
                    lineToPrevious.positionCount = 2;
                    lineToPrevious.SetPosition(1, transform.position);
                    vectorToPrevious = previous.transform.position - transform.position;
                    angleToPrevious = KuchoHelper.AngleZ(vectorToPrevious) -90;// el -90 coloca el angulo 0 hacia arriba que es como lo usa GetNextPatrolPoint para comparar
                }
                else
                    lineToPrevious.positionCount = 1;
            }

        }
        
        if (!previous)
        {
            for (int i = 0; i < staticPatrolPoints.Count; i++)
            {
                var otherPoint = staticPatrolPoints[i];
                if (otherPoint.next == this)
                    previous = otherPoint;
            }
        }

        // asignacion de next automatica , no tiene sentido , necesitamos puntos de finde cola sin next
        /*if (!next)
        {
            foreach (PatrolPoint p in staticPatrolPoints)
            {
                int n = StringHelper.ExtractNumber(p.name);
                if (n > 0)
                {
                    if (n == myNumber + 1)
                    {
                        next = p;
                    }
                }
            }
        }
        */

        if (next)
        {
            lineToNext.startWidth = 20;
            lineToNext.endWidth = 4;
            lineToNext.positionCount = 2;
            lineToNext.startColor = Color.blue;
            lineToNext.endColor = Color.white;
            lineToNext.SetPosition(0,transform.position);
            lineToNext.SetPosition(1,next.transform.position);
        }
    }

    void FindAndRebuildPatrolPointList()
    { 
        
        if (staticPatrolPoints != null)
            staticPatrolPoints.Clear();
        else
            staticPatrolPoints = new List<PatrolPoint>();
        
        var array = FindObjectsOfType<PatrolPoint>();
        staticPatrolPoints = array.OrderBy(go=>go.name).ToList();
        myNumber = staticPatrolPoints.IndexOf(this);
    }
    private void FixNumber()
    {
        #if UNITY_EDITOR
        var pm = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage();
        if (pm != null) // editando prefab de patrol points?
        {
            int parsedNumber = StringHelper.ExtractNumber(name);
            SetNumber(parsedNumber);
            return;
        }
        #endif

        var n = staticPatrolPoints.IndexOf(this);
        if (n < 0)
        {
            FindAndRebuildPatrolPointList();
            return;
        }

        if (myNumber != n)
        {
            SetNumber(n);
        }
        else // mynumber y el indice en la tabla son iguales
        {

            int parsedNumber = StringHelper.ExtractNumber(name);

            if (parsedNumber != n) // pero el nombre del go es diferente?
            {
                SetNumber(myNumber);
            }
            else
            {
                if (parsedNumber >= staticPatrolPoints.Count) // no deberia ocurrir nunca
                {
                    parsedNumber = staticPatrolPoints.Count - 1;
                    SetNumber(parsedNumber);
                }
            }

        }
    }

    void SetNumber(int n)
    {
        myNumber = n;
        name = "P" + myNumber.ToString();
        tm.text = name;
    }

    PatrolPoint GetCloserPatrolPoint()
    {
        Vector2 pos = transform.position;
        float distMag;
        float magToCloserPoint = float.MaxValue;

        foreach (PatrolPoint p in staticPatrolPoints)
        {
            if (p != this) // ignoro si es el point que el llamador tiene
            {
                Vector2 pPos = p.transform.position;
                Vector2 dist;
                dist.x = pPos.x - pos.x;
                dist.y = pPos.y - pos.y;
                distMag = dist.sqrMagnitude;
                if (distMag < magToCloserPoint)
                {
                    magToCloserPoint = distMag;
                    return p;
                }
            }
        }

        return null;
    }

    bool GetForward(Vector2 approachVector, bool currentforward)
    {
        float angleDiffToNext = Vector2.Angle(approachVector, vectorToNext);
        float angleDiffToPrevious = Vector2.Angle(approachVector, vectorToPrevious);
        if (currentforward)
        {
            if (angleDiffToNext < angleDiffToPrevious + 45) //+45 hace que solo si estamos muy enfilados a contrarias acepte cambiar de rumbo
                return true;
            else
                return false;
        }
        else
        {
            if (angleDiffToPrevious < angleDiffToNext + 45) //+45 hace que solo si estamos muy enfilados a contrarias acepte cambiar de rumbo
                return false;
            else
                return true;
        }
    }

    public static PatrolPoint GetNewCloserPatrolPoint(float myAngle, Vector2 pos , PatrolPoint current, ref bool forward)
    {
        if (staticPatrolPoints == null)
            return null;
        float magToCloserPoint = float.MaxValue;
        PatrolPoint closer = null;
        float distMag;
        foreach (PatrolPoint p in staticPatrolPoints)
        {
            if (p != current) // ignoro si es el point que el llamador tiene
            {
                Vector2 pPos = p.transform.position;
                Vector2 dist;
                dist.x = pPos.x - pos.x;
                dist.y = pPos.y - pos.y;
                distMag = dist.sqrMagnitude;
                if (distMag < magToCloserPoint)
                {
                    magToCloserPoint = distMag;
                    closer = p;
                }
            }
        }

        if (closer)
        {
            Vector2 myVector = KuchoHelper.DegreeToVector2(myAngle);
            Vector2 vectorToCloser;
            vectorToCloser.x = closer.transform.position.x - pos.x;
            vectorToCloser.y = closer.transform.position.y - pos.y;
            float angleToCloser = vectorToCloser.AngleZ();
            //float angleDiffToCloser = Math.Abs(myAngle - angleToCloser);
            float angleDiffToCloser = Vector2.Angle(myVector, vectorToCloser);

            PatrolPoint next = closer.next;
            if (!next)
                return closer;
            Vector2 vectorToNext;
            vectorToNext.x = next.transform.position.x - pos.x;
            vectorToNext.y = next.transform.position.y - pos.y;
            float angleToNext = vectorToNext.AngleZ();
            //float angleDiffToNext = Math.Abs(myAngle - angleToNext);
            float angleDiffToNext = Vector2.Angle(myVector, vectorToNext);


            PatrolPoint previous = closer.previous;
            if (!previous)
                return closer;
            Vector2 vectorToPrevious;
            vectorToPrevious.x = previous.transform.position.x - pos.x;
            vectorToPrevious.y = previous.transform.position.y - pos.y;
            float angleToNextReverse = vectorToPrevious.AngleZ();
            //float angleDiffToPrevious = Math.Abs(myAngle - angleToNextReverse);
            float angleDiffToPrevious = Vector2.Angle(myVector, vectorToPrevious);

            //AbsFloat closerToNextAngle = KuchoHelper.GetAngleToTarget(closer.transform.position, next.transform.position, Vector2.up);

            PatrolPoint winner = null;
            if (angleDiffToCloser < angleDiffToNext && angleDiffToCloser < angleDiffToPrevious)
            {
                winner = closer;
                forward = winner.GetForward(vectorToCloser, forward); 
            }
            else if (angleDiffToNext < angleDiffToCloser && angleDiffToNext < angleDiffToPrevious)
            {
                winner = next;
                forward = winner.GetForward(vectorToNext, forward); 
            }
            else if (angleDiffToPrevious < angleDiffToCloser && angleDiffToPrevious < angleDiffToNext)
            {
                winner = previous;
                forward = winner.GetForward(vectorToPrevious, forward); 
            }
            
            return winner;
        }

        /* Vector2 VectorFromCloserToNext;
             VectorFromCloserToNext.x = next.transform.position.x - closer.transform.position.x;
             VectorFromCloserToNext.y = next.transform.position.y - closer.transform.position.y;
 
             float angleDiff = Vector2.Angle(vectorToCloser, VectorFromCloserToNext);
 
             if (angleDiff < angleDiffToNextReverse)
             {
                 return closer;
             }
             else
             {
                 return next;
             }
 
         }
         */

        /*// calculo el punto medio entre el patro point y el siguiente para decidir si le doy el patrol point mas cercano o el siguiente
        if (closer)
        {
            Vector2 pppos = closer.transform.position;
            PatrolPoint next = PatrolPoint.GetNextPatrolPoint(closer, ref forward);
            Vector2 nxtpos = next.transform.position;
            Vector2 midPoint = (pppos + nxtpos) / 2;
            float magToNextPoint = ((pppos - nxtpos) / 2).sqrMagnitude;
            float magToMidPoint = midPoint.sqrMagnitude;
            // no es que sea perfecto, aun habrá ocasiones en las que retrocede al patro point cercano y luego da la vuelta , pero seran menos veces y menos evidentes
            if (magToCloserPoint < magToMidPoint)
                return closer;
            else
                return next;

            return next;
        }
        */
        return null;
    }

    public static PatrolPoint GetNextPatrolPoint(PatrolPoint current, ref bool forward )
    {
        if (forward)
        {
            if (current.next == null) // hemos pillado el ultimo?
            {
                forward = false;
                return current.previous;
            }
            else
            {
                return current.next;
            }
        }
        else // reverse
        {
            if (current.previous == null) // hemos pillado el ultimo?
            {
                forward = true;
                return current.next;
            }
            else
            {
                return current.previous;
            }
        }
    }
}
