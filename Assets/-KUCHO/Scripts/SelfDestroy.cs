using UnityEngine;
using System.Collections;
using System.Collections.Generic;



public class SelfDestroy : MonoBehaviour {

	public RandomFloat lifeTime;
	public float actualLifeTime;
	public float vanishSpeed = 0.01f; 
	[HideInInspector][SerializeField] SWizSprite sprite;
	[HideInInspector][SerializeField]  Item item;

	public void InitialiseInEditor()
	{
		if (!item)
			item = GetComponent<Item>();
		if (!item)
			item = GetComponentInParent<Item>();
		if (!sprite)
			sprite = GetComponent<SWizSprite>();
		if (!GetComponent<PickUpDropper>()) // estos son cajas y optras mierdas , no pieces que pueden ser destruidos por pieceskiller
			gameObject.tag = "Piece";
		
	}
	
	
}
