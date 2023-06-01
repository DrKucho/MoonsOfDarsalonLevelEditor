using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SnapMeEachFrame : MonoBehaviour {

	public enum WhenToSnap{OnFixeUpdate, OnUpdate, OnLateUpdate}
	public bool debug = false; 
	public WhenToSnap when = WhenToSnap.OnLateUpdate;
	public bool localPositionReset = false;
	public bool basedOnMyParent = false;
	public MovementType movementType = MovementType.Transform;
	public Snap pixelSnapX = Snap.ArcadePixel;
	public Snap pixelSnapY = Snap.ArcadePixel;
	public Vector3 originalLocalPosition;
	private Rigidbody2D rb2D;
	

    public void DoIt()
    {
	    if (localPositionReset)
		    transform.localPosition = transform.localPosition;

	    if (movementType == MovementType.Rigidbody && rb2D)
	    {
		    rb2D.position = SnapTo.Pixel(transform.position, pixelSnapX, pixelSnapY);
	    }
	    else if (basedOnMyParent)
	    {
		    transform.position = SnapTo.Pixel(transform.parent.transform.position + originalLocalPosition, pixelSnapX, pixelSnapY);
	    }
	    else
	    {
		    transform.position = SnapTo.Pixel(transform.position, pixelSnapX, pixelSnapY);
	    }

	    if (debug)
		    print(Time.time + gameObject.name);
    }
}
