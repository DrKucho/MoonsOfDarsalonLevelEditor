using UnityEngine;
using System.Collections;

public class DestroyOnCollision : MonoBehaviour {
	public string destroyThisTag ="PickUp";

	void OnCollisionEnter2D(Collision2D col){
		if (col.collider.CompareTag(destroyThisTag)) Destroy(col.gameObject); 
	}
}
