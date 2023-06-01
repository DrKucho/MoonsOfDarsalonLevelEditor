using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchRenderers : MonoBehaviour {
	public Vector2 pos;
	public bool rendEnabled;

	void Update () {
		if (Input.GetKeyDown(KeyCode.Y))
		{
			var rends = GetComponentsInChildren<Renderer>();
			foreach (Renderer rend in rends)
			{
				rend.enabled = !rend.enabled;
				rendEnabled = rend.enabled;
			}
		}
	}
	void OnGUI_disabled(){
		float x = pos.x;
		float y = pos.y;
		GUI.Label(new Rect(x, y, 400, 20), System.String.Format("rends Enabled? {0}", rendEnabled));y += 20;
		GUI.Label(new Rect(x, y, 400, 20), System.String.Format("Trans Pos {0}", transform.localPosition));y += 20;
	}
}
