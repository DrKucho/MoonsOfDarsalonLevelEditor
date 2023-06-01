using UnityEngine;
using System.Collections;

public class DestroyAllCollidersAtStart : MonoBehaviour {

	
	public void Awake () {
	}
	public void OnApplicationQuit(){
		print (this + " BORRANDO COLLIDERS");
		Collider2D[] colliders = GetComponents<Collider2D>();
		foreach (Collider2D col in colliders){
			Destroy(col);
		}
		
	}
}
