using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderedVisibleFlag : MonoBehaviour {

	public bool visible = false;
	void OnBecameVisible(){
		visible = true;
	}
	void OnBecameInvisible(){
		visible = false;
	}
}
