using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

using UnityEngine.Serialization;

/// <summary>
/// Represents a polygon shaped particle group in the liquidfun simulation</summary>
public class LPParticleGroupPoly : LPParticleGroup
{	
	[Range(3,8)]
	[Tooltip("How many sides this poly has. Note: if you change this it will reset this shape to a regular polygon")]
	public int NumberOfSides = 5;
	[Tooltip("Size of this poly shape")]
	public MinMax _radius = new MinMax(0.25f, 0.25f); // la antigua property float radius de LP inicializaba a 0.25f
	public RandomFloat scale = new RandomFloat(); // la antigua property float radius de LP inicializaba a 0.25f
	float _scale = 0.25f;
	
	[SerializeField][HideInInspector] private int LastNumberOfSides;

	void OnDrawGizmos()
	{ 	
		if (NumberOfSides != LastNumberOfSides)
		{
			pointsList = LPShapeTools.makePolyPoints(NumberOfSides,_scale);
			LastNumberOfSides = NumberOfSides;
//			LastRadius = radius;
		}
		if (pointsListOriginal == null || pointsListOriginal.Length != pointsList.Count)
			CopyPointsListToOriginalPointsList();
		
		ScalePointList();	

		bool loop = true;
		if (DontDrawLoop)loop  = false;

		if (!Application.isPlaying)
		{
			if (pointsList == null)
			{
				pointsList = LPShapeTools.makePolyPoints(NumberOfSides, _scale);
			}
			LPShapeTools.DrawGizmos(GetColors(), LPShapeTools.TransformPoints(transform, Constants.zero3, pointsList), loop);
		}
		else
		{
			if (pointsList == null)
			{
				pointsList = LPShapeTools.makePolyPoints(NumberOfSides, _scale);
			}
			LPShapeTools.DrawGizmos(GetColors(), LPShapeTools.TransformPoints(transformPointsWithThis, Constants.zero3, pointsList), loop);
		}
	}
	
	void ScalePointList(){
		_scale = scale.GetValuePlusRandom();// RANGE CAN ONLY BE CALLED FROM MAIN THREAD, ya arreglé esto para otro tipo de particle group como lo hivce? POR QUE USABA INTS Y PARA INTS SE PUEDE USAR SYSTEM.RANDOM
		int pointsListCount = pointsList.Count; //Optimizacion: leer .Count es el doble de lento
		for (int i = 0; i < pointsListCount; i++)
		{
			pointsList[i] = pointsListOriginal[i] * _scale;
		}

	}
}