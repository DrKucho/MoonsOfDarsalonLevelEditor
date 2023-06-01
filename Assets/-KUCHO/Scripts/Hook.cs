using System;
using UnityEngine;
using System.Collections;
using Unity.Collections;

public class Hook : MonoBehaviour {

	public int groundDistanceToDetach= -25;
	public CC hangingCC;
	public Collider2D grabbedByCollider; // para poder leer la Layer y decir frases pertinentes
	[ReadOnly2Attribute] public EnergyManager eM;
	[ReadOnly2Attribute] public CC cC;


}
