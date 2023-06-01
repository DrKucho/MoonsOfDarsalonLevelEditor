using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class SwitchGameObjectAfterDelay : MonoBehaviour {

	//[TextArea ()]
	public string notes = "";
	public bool debug = false;
	public EnableOrDisable action = EnableOrDisable.Disable_Close;
	public DoItAt when = DoItAt.Start;
	public GameObject target;
	public float seconds = 0;
	public int frames = 0;
	
}
