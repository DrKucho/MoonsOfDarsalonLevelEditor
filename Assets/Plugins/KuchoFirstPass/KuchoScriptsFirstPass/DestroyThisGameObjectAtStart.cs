using UnityEngine;
using System.Collections;

public class DestroyThisGameObjectAtStart : MonoBehaviour {

	
	public float delay = 0f;
	
	public void Start(){
		Destroy(gameObject, delay);
	}
}
