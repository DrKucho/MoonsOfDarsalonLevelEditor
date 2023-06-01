using UnityEngine;
using System.Collections;

public class RotateMe : MonoBehaviour {

	
	public float rotationSpeed = 0.1f;
	
	public void Update () {
		TransformHelper.SetEulerAngleZ(transform, transform.rotation.eulerAngles.z + rotationSpeed);
	}
}
