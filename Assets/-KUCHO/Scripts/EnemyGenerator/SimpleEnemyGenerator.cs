using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleEnemyGenerator : MonoBehaviour {

	public bool ignoreEnemyBirthLayerOnceIsborn;
	public bool collidersDeactivatedOnStart;
	public bool getZFromPlayer;
	public LookAt bornLookingTo;
	public float enemyEnergyMultiplier;

	[SerializeField] Gadget[] gadgets;
	public List<Item> items;


	public void InitialiseInEditor(){
		gadgets = GetComponentsInChildren<Gadget>();
	}
}
