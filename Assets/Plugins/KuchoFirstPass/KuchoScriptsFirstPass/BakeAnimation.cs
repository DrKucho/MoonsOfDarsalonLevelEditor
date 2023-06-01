using UnityEngine;
using System.Collections;

public class BakeAnimation : MonoBehaviour {

	private int counter = 0;
	public bool grabarFrame0 = false;
	public int cuantosFrames = 10;
	public string fileName = "";
	public bool capturar = false;
	
	public void Start(){

	
	}
	
	public void Update () {
		if(capturar){
			if ((grabarFrame0 && counter == 0) || counter > 0){
				var numero = counter.ToString();
				if (numero.Length == 1) numero = "0" + numero;
				ScreenCapture.CaptureScreenshot(fileName + numero + ".png");
			}
			counter ++;
			if (counter >= cuantosFrames)
				Debug.Break(); Debug.Log(this + "!!!!!!!!!!!!!!!!!!!!!!!!!");
		}
	}
}
