using UnityEngine;
using System.Collections;

public class SnapMeAtStart : MonoBehaviour {

	public bool debug=false;
	public bool snapLocalPosition = false;
	public bool setZ = false;
	public float _setZ = 0f;
	
	public void OnEnable () {
		if (snapLocalPosition){
			transform.localPosition = SnapTo.Pixel(transform.localPosition, Snap.ArcadePixel, Snap.ArcadePixel);
			if(debug) print (this + "local pos snapped to = "+ transform.localPosition);
	
		}
		else{
			transform.position = SnapTo.Pixel(transform.position, Snap.ArcadePixel, Snap.ArcadePixel);
			if(debug) print (this + "world pos snapped to = "+ transform.position);
		}
		if (setZ) transform.position = new Vector3(transform.position.x, transform.position.y, _setZ);
	}
}
