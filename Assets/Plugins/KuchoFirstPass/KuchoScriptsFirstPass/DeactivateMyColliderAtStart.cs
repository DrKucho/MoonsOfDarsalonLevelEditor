using UnityEngine;
using System.Collections;

public class DeactivateMyColliderAtStart : MonoBehaviour {

	
	public void Start(){
		GetComponent<Collider2D>().enabled = false;
	}
}
