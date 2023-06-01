using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Serialization;

public class LampCone : MonoBehaviour {

	public bool debug = false;
	
	public OnShotEvent onShot;
	
	public OnStickEvent onStick;
	
	Vector2 originalCenterOfMass;
	Transform originalParent;
	Light2DManager light2D;
	Rigidbody2D rb2d;
	SWizSpriteAnimator anim;
	Item item;
	EnergyManager eM;
	
	[System.Serializable]
	public class OnShotEvent{
		public bool goEnabled = false;
		public LayerType layer = LayerType.Obstacle;
		public bool lightStatus = true;
		public bool setCenterOfMass = true;
		public Vector2 centerOfMass;
		public bool copyBulletPosVelRot = true;
		public bool unParent = true;
		public float unParentDelay = 0.001f;
		public Transform makeChildOf;
	}
	[System.Serializable]
	public class OnStickEvent{
		public bool lightStatus = true;
		public LayerType layer = LayerType.Obstacle;
		public bool playAnimation = true;
		public float localZ = 0f;
		public bool straightUp = false;
		public bool isKinematic = true;
	}
	
	private Rigidbody2D myBullet;

}
