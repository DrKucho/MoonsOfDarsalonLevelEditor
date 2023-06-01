using UnityEngine;
using System.Collections;

public class AudioSourceInspector : MonoBehaviour {

	public Vector2 guiPos;
	
	public AudioSource aS;
	
	public void OnEnable () {
		aS = GetComponent<AudioSource>();
	}
	
	public void OnGUI_NO(){
		
			var y = guiPos.y;
			var x = guiPos.x;
			GUI.Label(new Rect(x, y, 200, 20), System.String.Format("isPlaying: {0}", aS.isPlaying)); y += 20;
			GUI.Label(new Rect(x, y, 200, 20), System.String.Format("volume: {0}", aS.volume)); y += 20;
			GUI.Label(new Rect(x, y, 200, 20), System.String.Format("pitch: {0}", aS.pitch)); y += 20;
			GUI.Label(new Rect(x, y, 200, 20), System.String.Format("pan: {0}", aS.panStereo)); y += 20;
			GUI.Label(new Rect(x, y, 200, 20), System.String.Format("clip: {0}", aS.clip)); y += 20;
		}
}
