using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


/// <summary>
/// Represents a box shaped particle group in the liquidfun simulation</summary>
public class LPParticleGroupBox : LPParticleGroup
{	
	[Tooltip("This boxes dimensions")]
//	public Vector2 Size = new Vector2(0.5f,0.5f);
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
	[Tooltip("This boxes rotation relative to the body")]
//	public float Rotation;
	[SerializeField][HideInInspector] float _Rotation;
	 public float Rotation{
		get{return _Rotation;}
		set{
			_Rotation = value;
			if (_Rotation != LastRotation)
			{
				float diff = _Rotation - LastRotation;
                LPShapeTools.RotatePoints(pointsList, diff, Offset);
				LastRotation = _Rotation;
			}
		}
	}

	[SerializeField][HideInInspector]
	private float LastRotation;
	
	[SerializeField][HideInInspector]
	private Vector2 LastSize;
	

	
	public void CreateBoxPointsFromScratch(){
		LPShapeTools.MakeBoxPoints(pointsList, _Size); // hace un cuadrado que siempre esta en el centro y sin rotacion
		LPShapeTools.OffsetPoints(Offset, pointsList);
		LPShapeTools.RotatePoints(pointsList, _Rotation, Offset);
		LastSize = _Size;
	}

	void OnDrawGizmos()
	{ 	
		if (isActiveAndEnabled)
		{
			if (Size != LastSize)
			{
				pointsList = LPShapeTools.MakeBoxPoints(Size);
				LastRotation = 0f;
				LastSize = Size;
			}
		
			if (Rotation != LastRotation)
			{
				LPShapeTools.RotatePoints(pointsList, Rotation - LastRotation, Constants.zero3); 
				LastRotation = Rotation;
			}		
		
			if (!Application.isPlaying)
			{
				if (pointsList == null)
				{
					pointsList = LPShapeTools.MakeBoxPoints(Size);
				}
				LPShapeTools.DrawGizmos(GetColors(), LPShapeTools.TransformPoints(transform, Constants.zero3, pointsList), true);
			}
			else
			{
				if (pointsList == null)
				{
					pointsList = LPShapeTools.MakeBoxPoints(Size);
				}
				LPShapeTools.DrawGizmos(GetColors(), LPShapeTools.TransformPoints(transformPointsWithThis, Constants.zero3, pointsList), true);// TODO , para que es el segundo parametro que siempre lo pongo a zero?
//				LPShapeTools.DrawGizmos(GetColors(), pointsCopy, true);// TODO , para que es el segundo parametro que siempre lo pongo a zero?
			}
				
		}
	}	
}