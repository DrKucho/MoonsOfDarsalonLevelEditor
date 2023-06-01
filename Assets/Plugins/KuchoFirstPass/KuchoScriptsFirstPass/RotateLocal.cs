using UnityEngine;
using System.Collections;
//using System.Net.Configuration;

public class RotateLocal : MonoBehaviour
{
	public Vector3 axis;
	public float rotationSpeed = 0.1f; 
	
	public void Update ()
	{
		var rot = transform.localRotation.eulerAngles;
		transform.Rotate(axis, rotationSpeed * KuchoTime.unityDeltaTime, Space.Self);
	}
}
