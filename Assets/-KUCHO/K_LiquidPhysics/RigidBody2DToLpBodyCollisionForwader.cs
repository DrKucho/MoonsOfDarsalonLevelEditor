using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RigidBody2DToLpBodyCollisionForwader : MonoBehaviour {

	public Rigidbody2D rb;
	public LPBody lpBody;
	IntPtr bodyPtr;
	public float forceMultiplier = 1;

//	void Start () {
//		bodyPtr = lpBody.GetPtr();
//	}
//	
//
//	void OnCollisionEnter2D(Collision2D col) {
//
//	}
//	void OnCollisionStay2D(Collision2D col) {
//		for (int i = 0; i < col.contacts.Length; i++)
//		{
//			ContactPoint2D c = col.contacts[i];
//			Vector2 force = new Vector2 (c.relativeVelocity.x * c.normal.x, c.relativeVelocity.y * c.normal.y) * forceMultiplier * c.separation;
//			LPAPIBody.ApplyForceToBody(bodyPtr, force.x, force.y, c.point.x, c.point.y, true);
//			print(force);
//		}
//	}
}
