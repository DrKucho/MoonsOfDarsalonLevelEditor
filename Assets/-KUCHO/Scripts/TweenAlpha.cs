using UnityEngine;
using System.Collections.Generic;

using UnityEditor;


public class TweenAlpha : MonoBehaviour
{

	public Component changing;
	[HideInInspector] public SpriteRenderer spr;
	[HideInInspector] public SWizSprite tkSpr;
	[HideInInspector] public Renderer rend;
	Color color;

    public bool randomColorOnEnable;
    bool RandomColorOnEnable() { return randomColorOnEnable; }
    bool HueCycle() { return hueCycle; }
    bool NotHueCycle() { return !hueCycle; }
    bool RandomColorOnEnableAndHueCycle() { return randomColorOnEnable & hueCycle; }
    bool RandomColorOnEnableAndNotHueCycle() { return randomColorOnEnable & !hueCycle; }


    public bool hueCycle;
	public int hueCycleInc;
    [Range(-180,180)] public RangeFloat randomHueRange;
    [Range(0,1)] public RangeFloat randomSaturationRange;
    [Range(0,1)] public RangeFloat randomValueRange;
    [Range(0,1)] public RangeFloat randomAlphaRange;
    [Range(0,1)] public Color randomColor;
    [Range(0,1)] public float min;
    [Range(0,1)] public float max;
	public float inc;
	public bool slowOnHigherValues;
	public float slowMult = 1;
public float skyTextureLightnessFactor = 0;

	public void InitialiseInEditor()
	{
		spr = GetComponent<SpriteRenderer>();
		if (spr)
		{
			changing = spr;
		}
		else
		{
			tkSpr = GetComponent<SWizSprite>();
			if (tkSpr)
			{
				changing = tkSpr;
			}
			else
			{
				rend = GetComponent<Renderer>();
				if (rend)
				{
					changing = rend;
				}
			}
		}
	}
	
    float oldHue;


}
