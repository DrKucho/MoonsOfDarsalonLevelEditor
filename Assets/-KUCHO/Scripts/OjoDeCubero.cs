using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OjoDeCubero : MonoBehaviour {

    public float a = 1.5f;
    public static OjoDeCubero instance;

    void Awake () {
        instance = this;
	}
	
}
