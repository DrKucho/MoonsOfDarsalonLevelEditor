using UnityEngine;
using System.Collections;
using System;

/// <summary>Simulates round or stright mechanical gears between two bodies</summary>
public class LPJointGear : LPJoint
{
	[Tooltip("The 1st joint component attached to this gear (Must be either revolute or prismatic)")]
	public LPJoint JointA;
	[Tooltip("The 2nd joint component attached to this gear (Must be either revolute or prismatic)")]
	public LPJoint JointB;
	[Tooltip("The mechanical gear ratio")]
	public float Ratio = 1;
	

	
	void OnDrawGizmos()
	{		
		Gizmos.DrawIcon(transform.position,@"LiquidPhysics2D/Icon_gear",false);							
	}
}
