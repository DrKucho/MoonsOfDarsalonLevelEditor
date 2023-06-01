using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class LPParticlesDetector : MonoBehaviour {

	public float radius;
	public Vector2 offset;
	public int partSystemIndex = 1; // Liquids
	[ReadOnly2Attribute] public LPParticleSystem partSys;
	public int tooMuchLiquidThreshold = 20;
	[ReadOnly2Attribute] public int contactCount;
	[ReadOnly2Attribute]public bool tooMuchLiquid;
	public Color gizmoColor;
	[HideInInspector] public IntPtr shape ;
	
}
