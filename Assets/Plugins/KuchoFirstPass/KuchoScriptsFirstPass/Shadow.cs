using UnityEngine;
using System.Collections;

public class Shadow : MonoBehaviour {

	
	public Transform main;
	public Vector3 offset;
	
	
	public void Update () {
		if (main)
		{
			transform.rotation = main.rotation;
			transform.position = main.position + offset;
		}
	}
}
