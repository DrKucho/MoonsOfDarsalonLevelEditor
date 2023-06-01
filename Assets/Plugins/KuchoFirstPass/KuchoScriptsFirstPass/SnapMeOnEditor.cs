using UnityEngine;
using System.Collections;

[ExecuteInEditMode]

public class SnapMeOnEditor : MonoBehaviour {

	
	public int[] validAngles = new int[] {0, 90, 180, 270};
	private int i = 0;
	
    #if UNITY_EDITOR

	public void Awake(){
		if (Application.isPlaying) this.enabled = false;
	}
	public void Update () {
		if (transform.parent)
			transform.localPosition = new Vector3(Mathf.RoundToInt(transform.localPosition.x), Mathf.RoundToInt(transform.localPosition.y), transform.localPosition.z);
		else
			transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), Mathf.RoundToInt(transform.position.y), transform.position.z);
	}
	public void Rotate(){
		i ++;
		if ( i >= validAngles.Length) i = 0;
		TransformHelper.SetEulerAngleZ(transform, validAngles[i]); 
	}
	//function SnapAngle(angle:float){
	//	if (angle > 180) angle -= 180;
	//	if 		( -45 <= angle && angle < 45 ) return 0;
	//	else if (  45 <= angle && angle < 135) return 90;
	//	else if ( 135 <= angle && angle < 225) return 180;
	//	else if ( 225 <= angle && angle < 315) return 270;
	//	else if (-135 <= angle && angle < -45) return -90;
	//	else if (-225 <= angle && angle <-135) return 180;
	//	else print(this + " ROTACION NO CALCULADA->" +angle);
	//	return 0;
	//}
    #endif

}
