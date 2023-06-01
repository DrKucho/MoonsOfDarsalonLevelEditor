using UnityEngine;
using System.Collections;
using System;

/// <summary>Prevents 2 bodies from going over a cetain distance apart as if they were connected by a rope</summary>
public class LPJointRope : LPJoint
{	
	[Tooltip("Offset of the anchor point of this joint on bodyA relative to the body position")]
	public Vector2 BodyAAnchorOffset; 
	[Tooltip("Offset of the anchor point of this joint on bodyB relative to the body position")]
	public Vector2 BodyBAnchorOffset; 
	[Tooltip("Lenght of the 'rope' (maximum distance the bodies can be apart)")]
	public float MaximumLenght = 2f;
	

	
	void OnDrawGizmos()
	{		
		if (true)
		{
			Gizmos.color = LPColors.Joint;
			
			Gizmos.DrawLine(BodyA.transform.position+ LPShapeTools.RotatePoint((Vector3)BodyAAnchorOffset,BodyA.transform.rotation.eulerAngles.z,Constants.zero3)  ,
			                BodyB.transform.position+ LPShapeTools.RotatePoint((Vector3)BodyBAnchorOffset,BodyB.transform.rotation.eulerAngles.z,Constants.zero3)); 	
		}
		Gizmos.DrawIcon(transform.position,@"LiquidPhysics2D/Icon_rope",false);							
	}
}
