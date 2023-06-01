using UnityEngine;
using System.Collections;


public class RigidBodyInspector : MonoBehaviour {

	public Vector2 guiPos;
	public bool showPos = true;
	public bool showVelVec = true;
	public bool showVelX = false;
	public bool showVelY = false;
	public bool showVelMagnitude = true;
	public bool showAngVel = true;
	public bool showRotation = true;
	public bool showCenterMass = true;
	public bool showAwake = true;
	public bool showSleeping = true;

	public bool changeSimulated = false;
	public bool simulated;
	private Rigidbody2D rb;

	public bool moverotation;
	public float rotation;
	
	public void Start(){
		rb = GetComponent<Rigidbody2D>();
	}

	void Update(){
		if (moverotation)
			rb.MoveRotation(rotation);
		if (changeSimulated)
			rb.simulated = simulated;
	}

	
	void ForceSleed(){
		rb.Sleep();
	}
    #if UNITY_EDITOR
	public void OnGUI(){
		var y = guiPos.y;
		var x = guiPos.x;
		if (showCenterMass) { GUI.Label(new Rect(x, y, 200, 20), System.String.Format("RB NAME: {0}", rb.gameObject.name)); y += 20;}
		if (showCenterMass) { GUI.Label(new Rect(x, y, 200, 20), System.String.Format("CenterMass: {0}", rb.centerOfMass)); y += 20;}
        if (showPos) { GUI.Label(new Rect(x, y, 200, 20), System.String.Format("Pos X: {0}", rb.position.x)); y += 20;}
        if (showPos) { GUI.Label(new Rect(x, y, 200, 20), System.String.Format("Pos Y: {0}", rb.position.y)); y += 20;}
		if (showVelVec) { GUI.Label(new Rect(x, y, 200, 20), System.String.Format("Vel: {0}", rb.velocity)); y += 20;}
		if (showVelX) { GUI.Label(new Rect(x, y, 200, 20), System.String.Format("VelocityX: {0}", rb.velocity.x)); y += 20;}
		if (showVelY) { GUI.Label(new Rect(x, y, 200, 20), System.String.Format("VelocityY: {0}", rb.velocity.y)); y += 20;}
		if (showVelMagnitude) { GUI.Label(new Rect(x, y, 200, 20), System.String.Format("VelMagnitude: {0}", rb.velocity.magnitude.ToString("F2"))); y += 20;}
		if (showAngVel) { GUI.Label(new Rect(x, y, 200, 20), System.String.Format("AngularVel: {0}", rb.angularVelocity.ToString("F2"))); y += 20;}
		if (showRotation) { GUI.Label(new Rect(x, y, 200, 20), System.String.Format("Rotation: {0}", rb.rotation)); y += 20;}
		if (showAwake) { GUI.Label(new Rect(x, y, 200, 20), System.String.Format("IsAwake: {0}", rb.IsAwake())); y += 20;}
		if (showAwake) { GUI.Label(new Rect(x, y, 200, 20), System.String.Format("Simulated: {0}", rb.simulated)); y += 20;}
//		if (showSleeping) { GUI.Label(new Rect(x, y, 200, 20), System.String.Format("IsSleeping: {0}", rb.IsSleeping())); y += 20;}
	}
    #endif
}
