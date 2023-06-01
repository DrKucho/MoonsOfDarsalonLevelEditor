using UnityEngine;
using System.Collections;
using System;

/// <summary>Creates a fixed immobile connection between two bodies at a certain point.
/// Note that movement can occur between the bodies when large forces are applied to one of the bodies for example</summary>
public class LPJointWeld : LPJoint
{

	void OnDrawGizmos()
	{		
		Gizmos.DrawIcon(transform.position,@"LiquidPhysics2D/Icon_weld",false);				
	}
}
