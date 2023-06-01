using UnityEngine;
using System.Collections;

public class DisplayResolutions : MonoBehaviour {

	Resolution[] res;
	public Vector2 pos =  new Vector2(20,20);
	public float lineHeight = 15;
	void Start () {
		res = Screen.resolutions;
	}
	
	void Update () {
		res = Screen.resolutions;
	}

	void OnGUI_disabled(){
		var x = pos.x;
		var y = pos.y;
		var w = 200;
	
		for (int i = 0; i < res.Length; i++)
		{
			var r = res[i];
			GUI.Label( new Rect( x, y, x + w, y + 10 ),  r.width + "x" + r.height + "@" + r.refreshRate);
			y += lineHeight;
		}
	}
}
