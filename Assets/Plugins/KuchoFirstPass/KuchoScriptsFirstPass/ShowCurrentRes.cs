using UnityEngine;
using System.Collections;

public class ShowCurrentRes : MonoBehaviour {

	
	public Vector2 guiPosition;
	
	public void OnGUI_NO(){
		int y = 0;
		GUI.Label(new Rect(guiPosition.x, guiPosition.y + y, 300, 20), System.String.Format("Resolution {0}", Screen.currentResolution.width + " x " + Screen.currentResolution.height )); y += 20;
	}
}
