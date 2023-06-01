using UnityEngine;
using System.Collections;

public class DeactivateMyCamera : MonoBehaviour {

	public float seconds = 2.5f;
	public int frames = 0;
	private bool alreadyOnIt = false;
	
	public void Start(){
		if (alreadyOnIt == false) DecideWhichWayToDeactivate();
	}
	
	public void OnEnable(){
		if (alreadyOnIt == false) DecideWhichWayToDeactivate();
	}
	public IEnumerator FrameCountDown(){
		for ( int n = 0; n < frames; n++){
			yield return null;
		}
		Deactivate();
	}
	public void DecideWhichWayToDeactivate(){
		alreadyOnIt = true;
		if (seconds > 0) Invoke ("Deactivate", seconds);
		else if (frames > 0 ) StartCoroutine( FrameCountDown() );
		else Deactivate();
	}
	public void Deactivate(){
		alreadyOnIt = false;
		gameObject.GetComponent<Camera>().enabled = false;
	}
}
