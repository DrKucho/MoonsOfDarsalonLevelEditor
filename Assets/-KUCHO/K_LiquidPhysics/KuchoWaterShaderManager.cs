using UnityEngine;
using System.Collections;


public class KuchoWaterShaderManager : MonoBehaviour {
	[System.Serializable]
	public class WaterColorModifier
	{
		[Range(-0.5f, 0.5f)] public float byLevel;
		[Range(-0.5f, 0.5f)] public float bySat;
	}
	[System.Serializable]	
	public class WaterColorValues
	{
		[Range(-1, 1)] public float level;
		[Range(-1, 1)] public float sat;
		[Range(-1, 1)] public float alpha;
	}
	[System.Serializable]
	public class WaterColorModifierReducedRange
	{
		[Range(0, 1f)] public float level;
		[Range(0, 1f)] public float sat;
		[Range(0, 1f)] public float alpha;
	}

	[System.Serializable]
	public class WaterColorClamps
	{
		[Range(-1f, 1f)] public RangeFloat level;
		[Range(-1f, 1f)] public RangeFloat sat;
		[Range(-1f, 1f)] public RangeFloat alpha;
	}

	public int updateSkipsFrames;
	public Material mat;
	public Color baseColor;
	[Header("Sky Input")]
	public bool grabColorFromSky = true;
	public Color skyColor;
	[Range(0,1)] public float inputLuma;
	[Range(0,1)] public float inputLevel;
	[Range(0,1)] public float inputSat;

	public WaterColorModifierReducedRange _base;
//	[Header("Base Values")]
//	[Range(-4,4)] public float originalSkyColorAdd;
//	[Range(-4,4)] public float originalSkyLuminanceMult;
//	[Range(0,2)] public float originalAlphaMultiplier;

	public WaterColorModifier levelFactor;
	public WaterColorModifier saturationFactor;
	public WaterColorModifier alphaFactor;

	[Header(" --------------- ")]
	[Range(-360,360)] public float hueShift;

	public WaterColorValues preClamp;
	public WaterColorClamps clamps;
	public WaterColorValues postClamp;

	public Color finalColor;

	int counter = 0;
	void Start(){
//		originalSkyColorAdd = mat.GetFloat("_SkyColorAdd");
//		originalSkyLuminanceMult = mat.GetFloat("_SkyLuminanceMult");
//		originalAlphaMultiplier = mat.GetFloat("_AlphaMultiplier");
	}
	void OnValidate(){
		if (isActiveAndEnabled)
			DoIt();
	}
	void Update(){
		if (counter >= updateSkipsFrames)
		{
			DoIt();
			counter = 0;
		}
		else
		{
			counter++;
		}
	}

	void DoIt(){
		HSLColor hslSkyColor;

		if (grabColorFromSky && Game.skyManager)
		{
			skyColor = Game.skyManager.realSaturatedSkyColor;
			hslSkyColor = Game.skyManager.hslSkyColor;
				
			inputLuma = Game.skyManager.realSaturatedSkyColorHSL.l;
			inputLevel = hslSkyColor.l;
			inputSat = hslSkyColor.s;
		}

		preClamp.level = _base.level + (levelFactor.byLevel * inputLevel) + (levelFactor.bySat * inputSat);
		preClamp.sat = _base.sat + (saturationFactor.byLevel * inputLevel) + (saturationFactor.bySat * inputSat);
		preClamp.alpha = _base.alpha + (alphaFactor.byLevel * inputLevel) + (alphaFactor.bySat * inputSat);

		postClamp.level = Mathf.Clamp(preClamp.level, clamps.level.min, clamps.level.max);
		postClamp.sat = Mathf.Clamp(preClamp.sat, clamps.sat.min, clamps.sat.max);
		postClamp.alpha = Mathf.Clamp(preClamp.alpha, clamps.alpha.min, clamps.alpha.max);


		finalColor = baseColor + skyColor;

		HSLColor hslFinalColor = HSLColor.FromRGBA(finalColor);
		hslFinalColor.h += hueShift;
		hslFinalColor.s = postClamp.sat;
		hslFinalColor.l = postClamp.level;
		hslFinalColor.a = postClamp.alpha;

		finalColor = hslFinalColor.ToRGBA();

		mat.SetColor("_Color", finalColor);
	}
}
