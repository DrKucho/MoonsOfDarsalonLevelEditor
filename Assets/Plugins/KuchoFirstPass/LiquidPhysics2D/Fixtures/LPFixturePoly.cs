using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Represents a polygon shaped fixture in the liquidfun simulation</summary>
public class LPFixturePoly : LPFixture
{
	[Tooltip("This poly shapes rotation relative to the body")]
	public float Rotation;
	
	[Range(3,8)]	
	[Tooltip("How many sides this poly has. Note: if you change this it will reset this shape to a regular polygon")]
	public int NumberOfSides = 5;
	[Tooltip("Size of this poly shape")]
	public float radius = 0.25f;
	
//	[SerializeField][HideInInspector]
//	private Vector2 LastOffset;
	[SerializeField][HideInInspector] 
	private float LastRotation;
	[SerializeField][HideInInspector]
	private int LastNumberOfSides = 5;
	[SerializeField][HideInInspector]
	private float LastRadius = 0.25f;

}