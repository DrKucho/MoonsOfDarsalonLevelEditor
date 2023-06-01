using UnityEngine;
using System.Collections;

public class UniqueGameObject : MonoBehaviour {

	
	public void Awake(){ //  print (this + " AWAKE ");
	    // hyper-advanced singleton implementation
	    if (FindObjectsOfType(typeof(Game)).Length > 1){
	        Debug.Log("UIM: Already found instance of script in scene; destroying.");
	        DestroyImmediate(gameObject);
	    }
	}
}
