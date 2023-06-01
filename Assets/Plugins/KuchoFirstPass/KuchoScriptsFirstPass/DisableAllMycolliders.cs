using UnityEngine;
using System.Collections;

public class DisableAllMycolliders : MonoBehaviour {

	
	private Component[] components;
	private Collider2D[] colliders;
	
	public void Start(){
		components = GetComponentsInChildren<Collider2D>();	
		colliders = new Collider2D[components.Length];
		int i = 0;
		foreach (Collider2D col in components){
			colliders[i] = col;
			colliders[i].enabled = false;
			i++;
		}
	}
}
