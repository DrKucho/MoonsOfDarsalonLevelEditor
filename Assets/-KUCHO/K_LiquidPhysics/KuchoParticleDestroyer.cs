using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using System;

public class KuchoParticleDestroyer : MonoBehaviour {

	public LPBody body;
	public int maxParticlesToDestroy;
	[SerializeField]
	ArrayOfIntArrays[] arrays;
	LPManager lpman;

}
