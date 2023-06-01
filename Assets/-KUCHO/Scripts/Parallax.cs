using UnityEngine;
using System.Collections;

public class Parallax : MonoBehaviour {

	public bool debug = false;
	public GameObject[] parallaxElements;
	public Vector3[] myPositions;
	public Snap snapX = Snap.None;
	public int ghostsPlanes = 0;
	public float colorDarken = 0.001f;
	public bool turnOffIfEditor = false;
	private Vector3 camOffset;

}
