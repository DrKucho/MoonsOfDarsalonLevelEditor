using UnityEngine;
using System.Collections;


public class CopyPosition : MonoBehaviour {

	public bool debug;
	public Transform transformToCopy;
	public string _transformToCopy = "";
	public bool followCam;
	public bool smoothed = false;
	public Vector2 speed = new Vector2(0.2f, 0.2f);
	public bool copyXY = true;
	public bool copyZ = true;
    public bool lockRotation = false;
	public Vector3 offset;
    public Transform compensateRotationWith;
	public DoItAt doItAt = DoItAt.EveryFrame;
	public bool DoItOnValidate = true;
	public bool DoItOnLevelWasLoaded = true;
	Vector2 smoothedPos;
	Vector2 previousPos; // solo para smoothed
	[HideInInspector] public Transform myTransform;

	bool IsSmoothed(){
		return smoothed;
	}
    Quaternion originalRotation;
	
}
