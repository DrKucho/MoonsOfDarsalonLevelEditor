using UnityEngine;
using System.Collections;


public class SpritePlaneManager : MonoBehaviour {

	[System.Serializable]
	public class SpritePlaneGroup{
		public string name;
		public SpritePlane.Type type;
		[Range(0f, 1f)] public float plane = 0.5f;
		[Range(0f, 0.001f)] public float heightMult;
	}

	public SpritePlaneGroup[] groups;

	
	void DoIt(){
		SpritePlane[] allPlanes = GetComponentsInChildren<SpritePlane>();
		foreach (SpritePlane plane in allPlanes)
		{
			foreach(SpritePlaneGroup grp in groups)
			{
				if (plane.gameObject.name.Contains(grp.name))
				{
					plane.type = grp.type;
					plane.plane = grp.plane + plane.transform.localPosition.y * grp.heightMult;
					plane.MyUpdate();
				}
			}
		}
	}
}
