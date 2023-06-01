using UnityEngine;
using System.Collections;

public class SnapMeTo : MonoBehaviour {

	//private Vector3 originalLocalPosition;
	public bool debug = false;
	public Transform attachPoint;
	
	public void Start(){
	}
	public void LateUpdate(){
		transform.localPosition = attachPoint.localPosition;
		if (debug) print (this +" "+ Time.time + " DENTRO DE SNAP ME TO " + attachPoint.position); 
	}
}
