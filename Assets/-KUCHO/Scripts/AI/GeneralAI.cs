using UnityEngine;
using System.Collections;

[System.Serializable]
public class GeneralAI{
	public bool aI_Script = false;
	public float initialNoAttackTime = 3f;
	public float weight;
	public bool alwaysSeePlayer = false;
	public bool lookToTargetWhenFire = true;
	public bool fireOnlyAtFirstAnimFrame = false;
	public bool chaseTarget = true;
	public bool saySomethingOnTarget = true;
	public Transform target;
	public CC targetCC;
	public FollowSide inFrontOfTargetCC = FollowSide.Back;
	public Vector2 targetOffset;
	public Vector2 targetMargin;
	[Header ("------debug------")]
	public Vector2 distToTarget;
	public Transform realTarget; 
	public Vector2 realTargetPos; 
	public Vector2 realDistToTarget;
	[Header ("-----------------")]
	public Vector2 aimOffset;
	public int inFrontDistance = 0;
	public MinMax considerPlayerRandom;
	public bool considerPlayer = true;
	public Timer waitingTimer;
	public Timer timeToTurnBackIfNotMoving; 
	public float stopJumpForceDistance;

	public bool destroyIfOutOfScreen = false;
	public bool destroyIfOutOfVision = false;
	public bool destroyIfOutOfAmmo = false;
	public int outOfScreenOffsetInPixels = 0;

	public int points = 20;
}

