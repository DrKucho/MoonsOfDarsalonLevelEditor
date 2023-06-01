using UnityEngine;
using System.Collections;

public class SwitchGameObjects : MonoBehaviour {

	
	[Header("ADEMAS RESETEO TRANSFORM ONENABLE, ESTE GO SHIFTABLE TRANSFORM DEBE ESTAR A CERO, asi que por si acaso lo muevo por error esto lo arregla")]
	public GameObject[] GO;
	
	public void OnEnable(){
		transform.localPosition = Constants.zero3;
	}
	public void Switch(){
		foreach (GameObject g in GO){
			g.SetActive(!g.activeSelf);
		}
	}
}
