using UnityEngine;
using System.Collections;

public class MakeMeInvisibleAtStart : MonoBehaviour {

	
	public void Start(){
		GetComponent<Renderer>().enabled = false;
	}
}
