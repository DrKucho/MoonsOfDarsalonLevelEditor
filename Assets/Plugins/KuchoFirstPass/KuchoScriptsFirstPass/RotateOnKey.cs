using UnityEngine;
using System.Collections;

public class RotateOnKey : MonoBehaviour {

	
	public string key = "";
	public float angleStep = 22.5f;
	public bool rotateX = false;
	public bool rotateY = false;
	public bool rotateZ = false;
	private float rot;
	
	public void Start(){
		if (rotateX) rot = transform.localRotation.eulerAngles.x;
		if (rotateY) rot = transform.localRotation.eulerAngles.y;
		if (rotateZ) rot = transform.localRotation.eulerAngles.z;
	}
	
	public void Update () {
		if (Input.GetKeyDown(key))
		{
			rot += angleStep;
			if (rotateX) transform.Rotate(new Vector3(angleStep, 0, 0), Space.Self);
			if (rotateY) transform.Rotate(new Vector3(0, angleStep, 0), Space.Self);
			if (rotateZ) transform.Rotate(new Vector3(0, 0, angleStep), Space.Self);
		}
	}
}
