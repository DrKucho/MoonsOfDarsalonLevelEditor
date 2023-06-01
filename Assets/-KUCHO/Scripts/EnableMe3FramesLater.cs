using UnityEngine;
using System.Collections;

public class EnableMe3FramesLater : MonoBehaviour {

	public GameObject go;

	void Awake(){
		go.SetActive(false);
	}
	void OnEnable () {
        KuchoEvents.onSceneWasLoaded3FramesAfter += Switch;
	}
	void OnDisable () {
        KuchoEvents.onSceneWasLoaded3FramesAfter -= Switch;
	}
	void Switch() {
		go.SetActive(true);
	}
}
