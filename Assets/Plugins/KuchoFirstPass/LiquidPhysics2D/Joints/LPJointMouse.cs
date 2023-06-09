﻿using UnityEngine;
using System.Collections;
using System;

/// <summary>This is like a rope joint that always tries to drag Body B to the target point
/// You must specify a BodyA for the joint to be created successfully but apart from that it has no influence</summary>
public class LPJointMouse : LPJoint
{	
	[Tooltip("The maximum force this joint can exert")]
	public float MaximumForce = 10000f;
	[Tooltip("How much damping this joint has")]
	public float DampingRatio = 0.6f;
	[Tooltip("This joints natural frequency in hertz. Note: it is recommended to keep this slower than the simulation timestep")]
	public float Frequency = 4f;
	[Tooltip("How close to the target should this joint attempt to bring BodyB")]
	public float Accuracy = 0.1f;
	

	
	
	void OnDrawGizmos()
	{		
		Gizmos.DrawIcon(transform.position,@"LiquidPhysics2D/Icon_mouse",false);							
	}
}
