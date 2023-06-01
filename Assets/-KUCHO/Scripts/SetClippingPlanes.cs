using UnityEngine;
using System.Collections;


public class SetClippingPlanes : MonoBehaviour {

	public Camera cam;
	public SpritePlane.Type getThisIncluded;

	void OnValidate(){
		if (!cam)
			cam = GetComponent<Camera>();
	}
	void Start () {
		Set();
	}
	
	void Set(){
		if (WorldMap.spritePlanes)
		{
			RangeFloat region = WorldMap.spritePlanes.GetRegionByType(getThisIncluded);
			cam.farClipPlane = region.max - transform.position.z + WorldMap.spritePlanes._minSeparation;
		}
	}
}
