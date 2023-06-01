using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Represents a chain shaped fixture in the liquidfun simulation</summary>
public class LPFixtureChainShape : LPFixture
{
	[Tooltip("This chainshapes rotation relative to the body")]
	public float Rotation;
	[Tooltip("Should this shape form a closed loop?")]
	public bool IsLoop = false;
	
//	[SerializeField][HideInInspector]
//	private Vector2 LastOffset;
	[SerializeField][HideInInspector]
	private float LastRotation;	


}
