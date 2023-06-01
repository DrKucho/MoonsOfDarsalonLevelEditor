using UnityEngine;
using System.Collections;


public class DecoPlaneConverter : MonoBehaviour {

	public float sourceMax = 3;
	public float sourceMin = 3;

	public float noZMax;
	public float noZMin;

	
	public void MoveAllMyDecorativeChildren(){
		var children = GetComponentsInChildren<Decorative>();
		foreach (Decorative child in children)
		{
			TransformHelper.SetPosZ(child.transform, NewZ(child.transform));
		}
	}

	float NewZ(Transform source){
		float z = source.position.z;
		var factor = 0f;
		RangeFloat r;
		if (z > 0)
		{
			factor = Mathf.InverseLerp(noZMax, sourceMax, z);
			r = new RangeFloat(WorldMap.spritePlanes.farBackground.min, WorldMap.spritePlanes.farBackground.max);
		}
		else
		{
			factor = 1 - Mathf.InverseLerp(noZMin, sourceMin, z);
			r = new RangeFloat(WorldMap.spritePlanes.coverground.min, WorldMap.spritePlanes.coverground.max);
		}
		return  Mathf.Lerp(r.min, r.max, factor);
	}
}
