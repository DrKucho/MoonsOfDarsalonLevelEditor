using UnityEngine;
using System.Collections;

using UnityEngine.Serialization;

[ExecuteInEditMode]
public class SpritePlane : MonoBehaviour {

//	public TileGrid tileGrid;
	public bool updateOnEnableOnValidate = true;
	public enum Type
	{
		None,
		FarBackground,
		Background2,
		Background1,
		Zero,
		Foreground2,
		Foreground1,
		Coverground2,
		CoverLightPlane,
		Coverground1,
	}
	public Type type = Type.Background2;
	public Type[] randomType;
	[Range(0f, 1f)] public float plane = 0.5f;
	public float offset = 0;
	 [SerializeField] float privOffset = 0;
	public Renderer rend; // para que lo usen otros
    [HideInInspector] public GrowAnimator grow;

	void OnValidate(){
		if (isActiveAndEnabled & updateOnEnableOnValidate)
		{
			MyUpdate(0);
		}
		if (!rend)
			rend = GetComponent<Renderer>();
	}
	void OnEnable(){
		if (updateOnEnableOnValidate)
			MyUpdate(0);
	}
	
    public void RandomizeMyPlane(){
        RandomizeMyPlane(0, 1);
    }
    public void RandomizeMyPlane(float min, float max){
		if (randomType != null && randomType.Length > 0)
		{
			int r = Random.Range(0, randomType.Length);
			type = randomType[r];
		}
		plane = Random.Range(min, max); 
		MyUpdate();
	}
	public void MyUpdate(){
		MyUpdate(0);
	}
	public void MyUpdate(float extraOffset){
		RangeFloat region = new RangeFloat();
		SpritePlanes spritePlanes = WorldMap.spritePlanes;
		float gridOffset = 0;

		if (!spritePlanes)
			spritePlanes = FindObjectOfType<SpritePlanes>();
		if (spritePlanes != null)
		{	
			region = spritePlanes.GetRegionByType(type);
			TransformHelper.SetPosZ(transform, Mathf.Lerp(region.min, region.max, plane) + extraOffset + offset + privOffset);
            if (!grow)
                grow = GetComponent<GrowAnimator>();
            if (grow)
                grow.OnSpritePlaneChanged(plane);
        }
	}
	
	void Update_And_Update_Children(){
		BroadcastMessage("MyUpdate");
	}
	public bool IsCoverLight(){
		if (transform.position.z < WorldMap.spritePlanes.lightCoverPlane)
			return true;
		return false;
	}
}
