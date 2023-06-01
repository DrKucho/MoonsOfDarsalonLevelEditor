using UnityEngine;
using System.Collections;

public class ToggleRendererOnKey : MonoBehaviour {

	public string _ = "LA TECLA DE CAMBIO ES F12";
	public bool validate;
	public Renderer rend;

	void OnValidate(){
		rend = GetComponent<Renderer>();
	}
	void Update(){
        if (Constants.isDebugBuild && Input.GetKeyDown(KeyCode.Print))// Keyboard.current.f12Key.wasPressedThisFrame)// Input.GetKeyDown(KeyCode.Print))
			rend.enabled = !rend.enabled;
	}
}
