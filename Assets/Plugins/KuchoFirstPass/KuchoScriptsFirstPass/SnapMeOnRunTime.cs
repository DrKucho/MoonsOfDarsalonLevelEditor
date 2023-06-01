using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SnapMeOnRunTime : MonoBehaviour {

	public enum WhenToSnap{OnFixeUpdate, OnUpdate, OnLateUpdate, OnEnable}
	public WhenToSnap when = WhenToSnap.OnLateUpdate;
	public Vector3 originalLocalPosition;
	public bool grabOriginalPosOnEnable = true;
	public bool localPositionReset = false;
	public bool basedOnMyParent = false;
	public MovementType movementType = MovementType.Transform;
	public Snap pixelSnapX = Snap.ArcadePixel;
	public Snap pixelSnapY = Snap.ArcadePixel;
	public Rigidbody2D rb2D;
	
	public void InitialiseInEditor () {
		rb2D = GetComponent<Rigidbody2D>();
	}

}
