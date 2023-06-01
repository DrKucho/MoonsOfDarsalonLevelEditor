using UnityEngine;
using System.Collections;

public class AllChildsLayerGround : MonoBehaviour {

	
	public void Start(){ //  print(this + "START ");

			StartCoroutine(DoIt());
		//Invoke("DoIt", 0.3);
	}
	public IEnumerator DoIt () {
		yield return null;
		foreach( Transform t in transform){
			t.gameObject.layer = Layers.ground;
		}
	}
}
