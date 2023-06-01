using UnityEngine;
using System.Collections;

[System.Serializable]
public class GndResolution {
	public Vector2 realResolution;
	public Vector2 arcadeResolution;
	public float zoom;

	public void CopyTo(GndResolution to){
		to.realResolution = realResolution;
		to.arcadeResolution = arcadeResolution;
		to.zoom = zoom;
	}
}

