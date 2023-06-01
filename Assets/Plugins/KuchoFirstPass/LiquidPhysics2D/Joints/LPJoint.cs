using UnityEngine;
using System;

public abstract class LPJoint : LPThing
{
    [Tooltip("Should this joint spawn on play")]
    public bool SpawnOnPlay = true;
	[Tooltip("Gameobject with 1st body attached to this joint")]
	public GameObject BodyA;
	[Tooltip("Gameobject with 2nd body attached to this joint")]
	public GameObject BodyB;
	[Tooltip("Should the bodies connected by this joint collide with each other?")]
	public bool CollideConnected = false;
	
	protected LPManager lpman;


	
	
}
