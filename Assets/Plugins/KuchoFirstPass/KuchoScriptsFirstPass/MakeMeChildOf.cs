using UnityEngine;
using System.Collections;

public class MakeMeChildOf : MonoBehaviour {

	public GameObject parentGO;
	
	public void Start(){
		transform.parent = parentGO.transform;
	}
}
