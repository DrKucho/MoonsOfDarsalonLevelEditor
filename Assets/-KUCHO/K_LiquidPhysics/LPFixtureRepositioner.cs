using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LPFixtureRepositioner : MonoBehaviour {

	public bool validateMe;
	public LPFixture fix;
	public Transform copyPosOf;
	public Transform[] makeScaleXWith;

	Vector3 previousPos;
	float previousScaleX;
	Vector2 originalFixOffset;
	
}
