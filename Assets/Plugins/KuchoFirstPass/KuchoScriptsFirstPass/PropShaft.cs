using UnityEngine;
using System.Collections;

public class PropShaft : MonoBehaviour {

	public Transform tire;
	public float rotationOffset = 0f;
	private float angle;
	
	public void Update () {
		angle = GetAngle(transform.position, tire.position) + rotationOffset;
		TransformHelper.SetEulerAngleZ(transform, angle);
	}
	public float GetAngle(Vector2 self, Vector2 target){
		var delta = target - self;
		return (Mathf.Atan2( delta.y, delta.x) * Mathf.Rad2Deg);
	}
}
