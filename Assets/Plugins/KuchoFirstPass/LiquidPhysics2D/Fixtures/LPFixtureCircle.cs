using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Represents a circle shaped fixture in the liquidfun simulation</summary>
public class LPFixtureCircle : LPFixture
{
	[Tooltip("Radius of this circle")] public float Radius = 0.25f;

	Vector3 Point;
	float AdjustedRadius;

}