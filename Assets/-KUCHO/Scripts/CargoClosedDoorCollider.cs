using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Serialization;


public class CargoClosedDoorCollider : MonoBehaviour {
	public float boundsThreshold = 5f; // por debajo de este temaño no sera empujado (asi evito cabezas)
	public float groundedPushForce = 4000;
	public float jumpingPushForce = 1000;
    public bool multiplyForceByMass = false;
	public Collider2D myCol;
	[ReadOnly2Attribute] public Rigidbody2D myBody;
	public List<ManInfo> applyForcesEveryFrame ;
	//public List<ManInfo> lockJump ;
    public bool disableGroundJoints;

    

	Vector2 pos;

	private float myBodyOffset;
	private float posXSign;
	

}
