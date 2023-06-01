using UnityEngine;
using System.Collections;

public class SetTransFormPosition : MonoBehaviour {

	public Vector3 position;

	void Start () {
		transform.position = position;
	}
}
