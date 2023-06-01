using UnityEngine;
using System.Collections;
using System;

/// <summary>This joint creates friction between the two bodies. Can be used to approximate a 'top down' view</summary>
public class LPJointFriction : LPJoint
{	
	[Tooltip("maximum force for this joint")]
	public float MaximumForce = 10f;
	[Tooltip("maximum torque for this joint")]
	public float MaximumTorque = 10f;

	void OnDrawGizmos()
	{		
		Gizmos.DrawIcon(transform.position,@"LiquidPhysics2D/Icon_friction",false);							
    }
}
