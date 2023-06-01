using UnityEngine;
using System.Collections;

public class CheckeredFlagScroll : MonoBehaviour {

	public Vector2 inc;
	public Scroll copyInc;
	public Vector2 pos;
	public Vector2 offset;
	public Vector2 limit;
	public bool snapToRealPixel = true;
	
	private Transform t;
	
	public void Start(){ //  print(this + "START ");

		pos = offset;
		t = transform;
	}
	public void Update(){ //  print (this + " UPDATE ");
		if (copyInc) inc = copyInc.inc;
		pos += inc;
		if (Mathf.Abs(pos.x) > limit.x) pos.x = offset.x;
		if (Mathf.Abs(pos.y) > limit.y) pos.y = offset.y;
	
		if (snapToRealPixel)
		{
			var newPosition = t.position;
			newPosition.y = SnapTo.ZoomFactor(newPosition.y);
			newPosition.x = SnapTo.ZoomFactor(newPosition.x);
			t.position = newPosition;
		}
		else
		{
			t.localPosition = new Vector3(pos.x, t.localPosition.y, t.localPosition.z);
			t.localPosition = new Vector3(t.localPosition.x, pos.y, t.localPosition.z);
		}
	}
}
