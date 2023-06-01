using UnityEngine;
using System.Collections;


public class SpritePlanes : MonoBehaviour {
	[ReadOnly2Attribute] public float ground = 0;
	public float _minSeparation = 0.0001f;

	[Range(-200f, 200f)] public RangeFloat allPlanes = new RangeFloat(-200,200);
	[Range(-200f, 200f)] public RangeFloat interactive = new RangeFloat(-150,150);

	[Range(-200f, 200f)] public RangeFloat farBackground = new RangeFloat(150, 200);
	[ReadOnly2Attribute][Range(-200f, 200f)] public RangeFloat background = new RangeFloat(0, 150);
	[Range(-200f, 200f)] public RangeFloat background1 = new RangeFloat(0, 75);
	[Range(-200f, 200f)] public RangeFloat background2 = new RangeFloat(75, 150);
	[ReadOnly2Attribute][Range(-200f, 200f)] public RangeFloat foreground = new RangeFloat(-150, 0);
	[Range(-200f, 200f)] public RangeFloat foreground1 = new RangeFloat(-150, -75);
	[Range(-200f, 200f)] public RangeFloat foreground2 = new RangeFloat(-75, 0);
	[ReadOnly2Attribute][Range(-200f, 200f)] public RangeFloat coverground = new RangeFloat(-200, -150);
	[Range(-200f, 200f)] public RangeFloat coverground1 = new RangeFloat(-200, -175);
	[Range(-200f, 0)] public float lightCoverPlane; 
	[Range(-200f, 200f)] public RangeFloat coverground2 = new RangeFloat(-175, -150);
	[ReadOnly2Attribute][Range(-200f, 200)] public RangeFloat lightCover = new RangeFloat(-175.4f, -174.5f); // siempre tiene ancho de 1 !



	void OnValidate(){
		farBackground.max = allPlanes.max;
		farBackground.min = interactive.max;
		background2.max = interactive.max;
		background1.min = 0;
		foreground2.max = 0;
		background.min = 0;
		foreground.max = 0;
		background2.min = background1.max;
		foreground2.min = foreground1.max;
		foreground1.min = interactive.min;

		lightCoverPlane = ((coverground.max - coverground.min) / 2) + coverground.min;

		coverground1.min = allPlanes.min;
		coverground1.max = lightCoverPlane - 0.5f;

		coverground2.min = lightCoverPlane + 0.5f;
		coverground2.max = foreground1.min;

		coverground.max = interactive.min;
		coverground.min = allPlanes.min;

		background.min = background1.min;
		background.max = background2.max;

		foreground.min = foreground1.min;
		foreground.max = foreground2.max;

		lightCover.min = coverground1.max;
		lightCover.max = coverground2.min;


		var map = WorldMap.instance;
		if (map)
		{
			if (WorldMap.destructible && WorldMap.destructible.d2dSprite && WorldMap.indestructible && WorldMap.indestructible.d2dSprite && WorldMap.background && WorldMap.background.d2dSprite)
			{
				TransformHelper.SetPosZ(WorldMap.destructible.d2dSprite.transform, ground + _minSeparation);
				TransformHelper.SetPosZ(WorldMap.indestructible.d2dSprite.transform, ground);
				TransformHelper.SetPosZ(WorldMap.background.d2dSprite.transform, farBackground.max);
			}
		}
	}
	public SpritePlane.Type GetTypeByPosition(float z){
		if (KuchoHelper.IsInRange(background1, z))
			return SpritePlane.Type.Background1;
		else if (KuchoHelper.IsInRange(background2, z))
			return SpritePlane.Type.Background2;
		else if (KuchoHelper.IsInRange(foreground1, z))
			return SpritePlane.Type.Foreground1;
		else if (KuchoHelper.IsInRange(foreground2, z))
			return SpritePlane.Type.Foreground2;
		else if (KuchoHelper.IsInRange(coverground1, z))
			return SpritePlane.Type.Coverground1;
		else if (KuchoHelper.IsInRange(coverground2, z))
			return SpritePlane.Type.Coverground2;
		else if (KuchoHelper.IsInRange(new RangeFloat(coverground1.max, coverground2.min), z))
			return SpritePlane.Type.CoverLightPlane;
		else if (KuchoHelper.IsInRange(farBackground, z))
			return SpritePlane.Type.FarBackground;
		return SpritePlane.Type.None;
	}
	public RangeFloat GetRegionByType(SpritePlane.Type type){
		RangeFloat region = new RangeFloat();
		switch (type)
		{
			case (SpritePlane.Type.FarBackground):
				region = farBackground; 
				break;
			case (SpritePlane.Type.Background2):
				region = background2; 
				break;
			case (SpritePlane.Type.Background1):
				region = background1;
				break;
			case (SpritePlane.Type.Foreground2):
				region = foreground2;
				break;
			case (SpritePlane.Type.Foreground1):
				region = foreground1;
				break;
			case (SpritePlane.Type.Coverground1):
				region = coverground1;
				break;
			case (SpritePlane.Type.Coverground2):
				region = coverground2;
				break;
			case (SpritePlane.Type.CoverLightPlane):
				region = lightCover;
				break;
			case (SpritePlane.Type.Zero):
				region = new RangeFloat(-1,1);
				break;
		}
		return region;
	}
    public float GetMediumPointByType(SpritePlane.Type type){
        RangeFloat region = GetRegionByType(type);
        return region.min + region.max / 2;
    }
    public static float GetZ(SpritePlane.Type _type , float normValue){
        RangeFloat region = new RangeFloat();
        SpritePlanes spritePlanes = WorldMap.spritePlanes;
        float gridOffset = 0;
        region = spritePlanes.GetRegionByType(_type);
        return Mathf.Lerp(region.min, region.max, normValue);
    }
}
