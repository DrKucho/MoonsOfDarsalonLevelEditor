using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class TransformExtras : MonoBehaviour {

//	public bool absolutePos;
//	public Vector3 worldPos;
//	Vector3 oldPos;
	public Vector3 worldScale;

	Transform[] parents;

	void OnValidate(){
		Update();
	}
	void Awake(){
		OnTransformParentChanged();
	}
	void Update(){
//		if (absolutePos)
//		{
//			transform.position = worldPos;
//			worldPos = transform.position;
//		}
//		else
//		{
//			worldPos = transform.position;
//			transform.position = worldPos;
//			ReadScale();
//		}
//		oldPos = worldPos;
		if (isActiveAndEnabled)
		{
			ReadScale();
		}
	}
	void OnTransformParentChanged(){
		parents = GetComponentsInParent<Transform>();
	}
	void ReadScale(){
		worldScale = transform.localScale;
		if (parents != null)
		{
			foreach (Transform t in parents) {
				worldScale.x *= t.localScale.x;
				worldScale.y *= t.localScale.y;
				worldScale.z *= t.localScale.z;
			}
		}
	}
}
