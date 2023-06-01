using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintContact : MonoBehaviour {

	public LPBody body;
	void Start () {
		body.onParticleContactDelegate += DoIt;
		body.onParticleContactDelegateIsNotNull = true;
	}
	
	// Update is called once per frame
	void DoIt (LPParticleSystem partSys, LPSystemFixPartContact contact) {
		print("FRM=" + Time.frameCount + " Time= " + Time.time + "CONTACTO");
	}
}
