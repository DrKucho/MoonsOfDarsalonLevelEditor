using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


/// <summary>
/// Represents a box shaped fixture in the liquidfun simulation</summary>
public class LPFixtureBox : LPFixture
{
	[SerializeField][HideInInspector] Vector2 _Size = new Vector2(0.5f,0.5f);
	 public Vector2 Size{
		get{return _Size;}
		set{
			_Size = value;
			if (_Size != LastSize)
			{
				CreateBoxPointsFromScratch();
			}
		}
	}
	[SerializeField][HideInInspector] float _Rotation;
	 public float Rotation{
		get{return _Rotation;}
		set{
			_Rotation = value;
			if (_Rotation != LastRotation)
			{
				LPShapeTools.RotatePoints(pointsList, _Rotation - LastRotation, Offset); 
				LastRotation = _Rotation;
			}
		}
	}

	[SerializeField][HideInInspector]
	private float LastRotation;
	
	[SerializeField][HideInInspector]
	private Vector2 LastSize;

	
	public void CreateBoxPointsFromScratch(){
		pointsList = LPShapeTools.Force4Points(pointsList);
		LPShapeTools.MakeBoxPoints(pointsList, _Size); // hace un cuadrado que siempre esta en el centro y sin rotacion
		LPShapeTools.OffsetPoints(Offset, pointsList);
		LPShapeTools.RotatePoints(pointsList, _Rotation, Offset);
		CopyPointsListToPointsCopy();
		LastSize = _Size;
	}
}
