using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Profiling;
using UnityEngine.Serialization;

[System.Serializable]
public struct RadiaSearchPointResult
{
    public float validTime;
    [ReadOnly2Attribute] public Vector2 point;
    [ReadOnly2Attribute] public float setTime;
    
}

public class RadialSearch : MonoBehaviour
{
    public bool debug; // para ser activada haciendo debug

    public enum Status { Stopped, WaitingToBeProcessed, Ready }
    public Status status;
    public float defaultResiltPointValidTime = 10;
    public float shouldMakeNewSearchSeconds = 2;
    public int abandonTargetAfterSearchTries = 5;
    public bool ignoreDestructible;
    [ReadOnly2Attribute] public float lastSearchTime;
    [ReadOnly2Attribute] public Vector2 myPos;
    [Range(0, 400)] public RangeFloat range = new RangeFloat (30, 200);
    public float spotRadius = 20;
    public float angleInc = 5;
    [Range(0, 1)] public float distToAngleIncRatio;
    [ReadOnly2Attribute] public float currentAngle;
    [Range(1, 3)] public float wallDecRatio;
    public int howManyTries;
    //[ReadOnly2Attribute] public Collider2D targetCollider;
    //[ReadOnly2Attribute] public Transform targetTransform;
    [ReadOnly2Attribute] public Vector2 targetPos;
    //[ReadOnly2Attribute] public static CleanSpotLoopArray cleanSpotHistory = new CleanSpotLoopArray(80);
    [Header("--- RESULT")]
    public int previousCleanSpotCount;
    public int firstLineCastCount;
    public int secondLineCastCount;
    public float sucessLevel = 0;
    public SmoothFloat sucesses;
    public bool pointWithVisibilityFound = false;
    public RadiaSearchPointResult closerPointToTargetWithVisibility;
    public bool pointFound = false;
    public RadiaSearchPointResult closerPointToTarget;
    public RadiaSearchPointResult lastCleanAndCloserPoint; // segun tengo planteado el juego nadie quiere ir a este punto
    public RadiaSearchPointResult mostFarAwayPoint;
    public GameObject mark;
    public GameObject markSpecial;
    private List<GameObject> marks;
    ExpensiveTask myExpensiveTask;

}
