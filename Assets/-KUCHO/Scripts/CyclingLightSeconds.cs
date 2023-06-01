using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using Light2D;


public class CyclingLightSeconds : MonoBehaviour {


	public int inc = 1;
	[Range(0,2)] public float runDelay = 0.5f; // es la que se usa en la corrutina para generar el delay/rate pero privada para que nadie la pueda cambiar
	[Range(0,2)] public float noLightTime = 0;
	[Range(0,1)] public float vanishSpeed = 1;

	bool vanishing;

	float[] alpha;
	int index;	
	bool runing;
    [ReadOnly2Attribute] public SWizSprite[] sprites;
    [ReadOnly2Attribute] public Light2DManager lightManager;

	public void InitialiseInEditor(){
        lightManager = GetComponentInChildren<Light2DManager>();
        sprites = GetComponentsInChildren<SWizSprite>();
    }



	private float nextChange = -1;
	private bool noLightTimeRuning;


}
