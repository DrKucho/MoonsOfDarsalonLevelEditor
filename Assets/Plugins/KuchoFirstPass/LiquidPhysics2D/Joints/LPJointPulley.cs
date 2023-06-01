using UnityEngine;
using System.Collections;
using System;

/// <summary>Simulates a joint connecting two bodies with a pulley system</summary>
public class LPJointPulley : LPJoint
{
	[Tooltip("The mechanical ratio of the pulley")]
	public float ratio = 1f;
	[Tooltip("Offset of BodyA's connection point to the pulley from the joint position")]
	public Vector2 BodyAGroundOffset = new Vector2(-1f,0f);
	[Tooltip("Offset of BodyB's connection point to the pulley from the joint position")]
	public Vector2 BodyBGroundOffset = new Vector2(1f,0f);
	[Tooltip("Offset of the anchor point of this joint on bodyA relative to the body position")]
	public Vector2 BodyAAnchorOffset = Constants.zero2;
	[Tooltip("Offset of the anchor point of this joint on bodyB relative to the body position")]
	public Vector2 BodyBAnchorOffset = Constants.zero2;

	
	void OnDrawGizmos()
	{			
		Gizmos.color = LPColors.Joint;	
		Gizmos.DrawLine(transform.position+(Vector3)BodyAGroundOffset ,transform.position+(Vector3)BodyBGroundOffset);		
		Gizmos.DrawWireSphere(transform.position+(Vector3)BodyAGroundOffset,0.15f);
		
		Gizmos.DrawLine(transform.position+(Vector3)BodyAGroundOffset,BodyA.transform.position+
		 LPShapeTools.RotatePoint((Vector3)BodyAAnchorOffset,BodyA.transform.rotation.eulerAngles.z,Constants.zero3));
		 			
		Gizmos.DrawWireSphere(transform.position+(Vector3)BodyBGroundOffset,0.15f);
		
		Gizmos.DrawLine(transform.position+(Vector3)BodyBGroundOffset ,BodyB.transform.position
		+ LPShapeTools.RotatePoint((Vector3)BodyBAnchorOffset,BodyB.transform.rotation.eulerAngles.z,Constants.zero3));
		
		Gizmos.DrawIcon(transform.position,@"LiquidPhysics2D/Icon_pulley",false);							
	}
}
