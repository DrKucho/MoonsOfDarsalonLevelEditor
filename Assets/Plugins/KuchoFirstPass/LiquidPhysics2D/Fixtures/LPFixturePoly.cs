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
	void OnDrawGizmos()
	{
		if (NumberOfSides != LastNumberOfSides)
		{
			if (NumberOfSides > 3) // KUCHO HACK
				pointsList = LPShapeTools.makePolyPoints(NumberOfSides, radius);
			LastNumberOfSides = NumberOfSides;
			LastRotation = 0f;
			lastOffset = Constants.zero2;
		}
		if (radius != LastRadius)
		{
			pointsList = LPShapeTools.ChangeRadius(radius - LastRadius, pointsList, new Vector3(Offset.x, Offset.y)); 
			LastRadius = radius;
		}
		
		bool loop = true;
		if (DontDrawLoop)
			loop = false;
		
		if (!Application.isPlaying)
		{
			if (pointsList == null)
			{
				pointsList = LPShapeTools.makePolyPoints(NumberOfSides, radius);
			}
			LPShapeTools.DrawGizmos(Color.cyan, LPShapeTools.TransformPoints(transform, Constants.zero3, pointsList), loop);
		}
	}	
}