using UnityEngine;
using System.Collections;

using UnityEngine.Serialization;


public class AI_Turret : MonoBehaviour {

	public bool debug;
	public Vision vision;
	public HingeJoint2D hinge;

	public Rigidbody2D baseBody;
	public Rigidbody2D turretBody;
    public bool hingeIsCenterOfTurretBodyMass = true;
	public GameObject disableOnFlip;
	public float resetDuration = 1;
	//public float resetTime = 1; // cuando estamos bloqueados entramos en modo reset durante unos segundos en el que el caño va al reves
	[Range (0,100)]public float rotationSpeed; // esto no es una variable AI, deberia estar en un controller
	public bool invertRotationDirection;
	[Range (0,400)]public float maxRotationSpeed = 30f; // esto no es una variable AI, deberia estar en un controller
	[Range (0,1)] public float angleDiffToSpeedFactor = 1;
	[Range (0,1)] public float baseBodySpeedToRotationSpeedFactor = 0f; // esto no es una variable AI, deberia estar en un controller
	[ReadOnly2Attribute] public float finalRotSpeed;
	[Header("")]
	public int aimAngleOffset = 0; // esto no es una variable AI, deberia estar en un controller
    public float missShotAngleOffset = 4;
    [ReadOnly2Attribute] public float _missShotAngleOffset; // lo fija AI segun hayamos acertado ene l blanco o no
	public float minAngleToShot = 2f;
	[ReadOnly2Attribute] public bool angleGoodToShot = false;

    [Header("BASE TO TURRET")]
    [HideInInspector][ReadOnly2Attribute] public SmoothFloat baseBodyToTurretBodyRotDiff;
    [Header("CANNON TO LIMITS")]
    [HideInInspector][ReadOnly2Attribute] public float turretToLowerLimit;
    [HideInInspector][ReadOnly2Attribute] public float turretToHigherLimit;
    [HideInInspector][ReadOnly2Attribute] public float tToLL_turrToTarget;
    [HideInInspector][ReadOnly2Attribute] public float tToHL_turrToTarget;
	[Header("CANNON TO TARGET")]
    [HideInInspector][ReadOnly2Attribute] public Vector2 vectorCannonRotationNorm;
    [HideInInspector][ReadOnly2Attribute] public float absTurretAngleToTarget;
    [HideInInspector][ReadOnly2Attribute] public float turretAngleToTarget;
	[Header("BASE TO TARGET")]
    [HideInInspector][ReadOnly2Attribute] public Vector2 vectorBaseBodyRotationNorm;
    [HideInInspector][ReadOnly2Attribute] public float absBaseBodyAngleToTarget;
    [HideInInspector][ReadOnly2Attribute] public float baseBodyAngleToTarget;
	[Header("FINAL AIM ANGLE")]
    [HideInInspector][ReadOnly2Attribute] public float finalAimAngle;
    [HideInInspector][ReadOnly2Attribute] public float rotationDirection;
	[Header("BLIND ANGLE")]
    [ReadOnly2Attribute] public bool blinded;
    public AI_BlindArea blindArea;
	public float blindAngle = 360;
    [HideInInspector][ReadOnly2Attribute] public float blindDiff;
    [HideInInspector][ReadOnly2Attribute] public float maxBlindDiff;
    [HideInInspector][ReadOnly2Attribute] public float blindAngleAdd;
    [Header("RESET")]
    public  float lockedTimeToDoReset = 1;
    [ReadOnly2Attribute] public  bool turretIsLocked;
    [ReadOnly2Attribute] public bool resetMode;
    [ReadOnly2Attribute] public float resetModeDir;
    [Header("WRONG DIR FIX")]
    [ReadOnly2Attribute] public bool wrongDirFixing;
    public float wrongDirFixAngle = 140;
    [HideInInspector][ReadOnly2Attribute] public float wrongDirAngleAdd;


	Vector2 rotationCenter;
	Vector2 delta; // la dstancia que hay al objetivo usada en las rutinas de apuntar
	Vector2 hingePos;
	float baseRotation;
	float angleToPlayer;

	float moveAngle;

	
}
