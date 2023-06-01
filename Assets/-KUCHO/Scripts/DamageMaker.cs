using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Serialization;

public class DamageMaker : MonoBehaviour {

	// AJUSTALOS COLLIDERS PARA DAÑAR A UNO U OTRO EQUIPO, PERO NO HACE EL DAÑO EN SI, ESTE SE HACE POR DAMAGE RECIVER , ALLI SALTAN LAS COLISIONES
	public bool debug = false;
	public bool collidersEnabledAtStart = false;
	public float colliderRadius = 1f;
	public ArmyType armyType = ArmyType.GoodGuys;
	public ArmyType hurt = ArmyType.All;
	float timeToDisableCollider;
	public EnergyManager eM;
	public Collider2D goodGuysAttackCol;
	public Collider2D badGuysAttackCol;
	public Collider2D[] allColliders;
	CC cC;

	[Header("TAMAÑO DINAMICO CON THRUST MYUPDATE")]
	public float inputOffset = -0.05f;
	public Vector3 inputToPosMax;
	[Range (0,200)] public RangeFloat sizeX;
	[Range (0,200)] public RangeFloat sizeY;
	public List<Collider2D> collidersToIgnore = new List<Collider2D>();
	Vector3 originalPos;
	Vector3 _inputToPosMax;
	[ReadOnly2Attribute] public float input;
	[ReadOnly2Attribute] public Transform myTransform;
	
	

	private float previousInput = 1;


}
