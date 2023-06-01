using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TroopCarrierPseudoAi : MonoBehaviour {

	//[Inspect, TextArea(2,7)][Descriptor(0.5f, 1f, 0.5f)]
	public string INSTRUCTIONS = "TIENE QUE ESTAR EN UN GO CON RENDERER PARA QUE SALTE OnBecameVisible()";

	[HideInInspector] [SerializeField] ObjectSpawner[] gens ;
	public float refreshRate = 1;
	public Collider2D myGround;
	[HideInInspector] [SerializeField] AI_Flyer ai;
	[HideInInspector] [SerializeField] Item item;
	WaitForSeconds waitForSeconds;

	public void InitialiseInEditor(){
		gens = GetComponentsInChildren<ObjectSpawner>();
		ai = GetComponentInParent<AI_Flyer>();
		item = GetComponentInParent<Item>();
	}

}
