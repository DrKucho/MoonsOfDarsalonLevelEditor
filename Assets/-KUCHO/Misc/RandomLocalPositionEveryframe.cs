using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLocalPositionEveryframe : MonoBehaviour {

	public Vector2 min;
	public Vector2 max;

	void Update () {
		float x = Random.Range(min.x, max.x);
		float y = Random.Range(min.y, max.y);
		transform.localPosition = new Vector3(x, y, transform.localPosition.z);
	}
}
