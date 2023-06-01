using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System;


public class TriggerColliders : MonoBehaviour {


	public bool debug = false;
	//public Aim aim; // LO COMENTO PORQUE NO NECESITO QUE TRIGGER COLLIDERS SE SERIALICE , SOLO QUE EXISTA
	public Collider2D detectableCollider; // el collider en este GO , normalmente el que se usa para que otros me detecten
	public float rotationSpeed = 10;

	
	protected Transform t;
	[ReadOnly2Attribute] public int detectableColliderOriginalLayer = -1;
	[ReadOnly2Attribute] public string detectableColliderOriginalTag = ""; 

	public delegate void MyOnTriggerEnter2D(Collider2D col);
	public MyOnTriggerEnter2D myOnTriggerEnter2D;
	public delegate void MyOnTriggerExit2D(Collider2D col);
	public MyOnTriggerExit2D myOnTriggerExit2D;

	public CC cC;
	public VehicleInput vehicle;

	bool IsPlaying(){
		return Application.isPlaying;
	}
	public virtual void InitialiseInEditor(){
		cC = GetComponentInParent<CC>();
		detectableCollider = GetComponent<Collider2D>();// ha de ser el de este mismo GO siempre porque asi funcioona la Obtencion de este script , en multitud de scripts asumo que TriggerColliders esta siempre en el Go del DetectableCollider

		vehicle = GetComponentInParent<VehicleInput>();
		if (!detectableCollider)
			detectableCollider = GetComponent<Collider2D>();
		if (detectableCollider)
		{
			detectableColliderOriginalLayer = detectableCollider.gameObject.layer;
			detectableColliderOriginalTag = detectableCollider.gameObject.tag;
		}
	}
	
}
