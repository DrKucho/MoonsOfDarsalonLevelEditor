using UnityEngine;
using System.Collections;

public class NoInternetScreen : MonoBehaviour {

	public float quitDelay;

	void OnEnable () {
		Invoke("ExitGame", quitDelay);
	}
	void ExitGame(){
		Application.Quit();
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#endif
	}
}

