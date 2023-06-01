
using UnityEngine;
using System.Collections;


[ExecuteInEditMode]

public class SnapMeOnEditor2 : MonoBehaviour {

    public bool useLocalPosition = false;
	public Vector2 grid = new Vector2(15,15);
	public bool snapScale = true;

	public int[] validAngles = new int[] {0, 90, 180, 270};
	int i = 0;
	
	void Awake(){
		if (Application.isPlaying) this.enabled = false;
	}
	void Update () {
        if (useLocalPosition)
			transform.localPosition = GetSnapped(transform.localPosition);
		else
			transform.position = GetSnapped(transform.position);

		if (snapScale)
		{
			Vector3 scale = GetSnapped(transform.localScale);
			if (scale.x == 0)
				scale.x = 1;
			if (scale.y == 0)
				scale.y = 1;
			transform.localScale = scale;
		}
	}
	Vector3 GetSnapped(Vector3 pos){
		pos.x = Mathf.RoundToInt(pos.x / grid.x) * grid.x;
		pos.y = Mathf.RoundToInt(pos.y / grid.y) * grid.y;
		return pos;
	}
	
	void Rotate(){
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
}
