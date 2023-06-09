using UnityEngine;
using System.Collections;
using System;

/// <summary>This joint keeps two bodies a certain distance apart as if they were connected by a rigid stick</summary>
public class LPJointDistance : LPJoint
 {		
	[Tooltip("How much damping this joint has")]
 	public float Dampingratio = 1f;
	[Tooltip("This joints natural frequency in hertz. Note: it is recommended to keep this slower than the simulation timestep")]
 	public float FrequencyInHertz = 4f;
	[Tooltip("Offset of the anchor point of this joint on bodyA relative to the body position")]
 	public Vector2 BodyAAnchorOffset; 
	[Tooltip("Offset of the anchor point of this joint on bodyB relative to the body position")]
	public Vector2 BodyBAnchorOffset; 
	
	
	void OnDrawGizmos()
	{		
		if (true)
		{
			Gizmos.color = LPColors.Joint;
			
			Gizmos.DrawLine(BodyA.transform.position+ LPShapeTools.RotatePoint((Vector3)BodyAAnchorOffset,
							BodyA.transform.rotation.eulerAngles.z,Constants.zero3) ,
			                BodyB.transform.position+ LPShapeTools.RotatePoint((Vector3)BodyBAnchorOffset,
			                BodyB.transform.rotation.eulerAngles.z,Constants.zero3)); 	
		}
		Gizmos.DrawIcon(transform.position,@"LiquidPhysics2D/Icon_distance",false);							
	}
}
