using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionArea : MonoBehaviour
{
    [Range(0,1)] public float targetTimeScale = 0.25f;
    [Range(0, 10)] public float changeSpeed = 4f;

    [Header("CONDITIONS TO ENTER SLO MO")]
    public bool needsJump;
    public bool needsLeftOrRight;
    public float rightNeeded;
    public St statusNeeded;
    public St statusForbidden;
    public static bool playerInside;

}
