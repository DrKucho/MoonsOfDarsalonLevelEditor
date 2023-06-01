using UnityEngine;
using System.Collections;

public class RotateMyLocalY : MonoBehaviour {

	
	public float rotationSpeed = 0.1f; 
	
	public void Update () {
		TransformHelper.SetLocalEulerAngleY(transform, transform.localRotation.eulerAngles.y + rotationSpeed * KuchoTime.unityDeltaTime);
	}
}
