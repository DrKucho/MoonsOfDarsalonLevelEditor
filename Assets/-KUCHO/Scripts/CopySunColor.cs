using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Serialization;


public class CopySunColor : MonoBehaviour {

	public enum ColorToMix {PickedManually, ThisSunAndSky, GameSun, GameSky, GameCosmos, GameAtmos, ThisTintMult, ThisTintAdd, WorldMapSkyColor, WorldMapNightColor }//TODO implementar en moons of darsalon MOD
    [System.Serializable]
    public class ColorReader{
        public ColorMixer sourceColor;
        [Range(0, 1)] public float saturation = 1f;
        public AnimationCurve curve;
        public float curveMult;
        public float curveAdd;
        public Color color;
        public HSLColor hslColor;

        public void Process(float input){
            hslColor = color; // conversion de RGB a HSL
            hslColor.s *= saturation;
            color = hslColor; // no necesito castear en plan (Color) ?
            float c = curve.Evaluate(input) * curveMult + curveAdd;
            color.a = c;
        }
    }
	[System.Serializable]
	public class ColorMixer{
		public bool enabled = true;
		public ColorToMix getColor1From;
		[Range(0,1)] public float saturation1 = 1f;
        public Color color1;
		public ColorToMix getColor2From;
		[Range(0,1)] public float saturation2 = 1f;
		public Color color2;
		public bool heightAbsoluteMode = false;
		public bool invertColorMix = false;
		public bool lockColorMix = true;
		[Range(-1,1)] public float heightColorMixFactor = 0f;
		[Range(-1,3)] public float heightColorMixAdder = 0f;
		[Range(0,1)] public float colorMix = 1f;
        [ReadOnly2Attribute] public Color finalColor;
//		public bool basedOnZ = false;
//		public float zShift;
//		[Range(-0.005f,0.005f)] public float zFactor;

		public void Mix(float heigthFactor, float absHeightFactor){
			HSLColor hslSunColor = new HSLColor();
			if (saturation1 < 1){
				hslSunColor = color1; // conversion de RGB a HSL
				hslSunColor.s *= saturation1;
				color1 = hslSunColor; // no necesito castear en plan (Color) ?
			}
			if (saturation2 < 1){
				hslSunColor = color2; // conversion de RGB a HSL
				hslSunColor.s *= saturation2;
				color2 = hslSunColor; // no necesito castear en plan (Color) ?
			}
			if (lockColorMix == false)
			{
				float hc;
				if (heightAbsoluteMode) hc = absHeightFactor;
				else hc = heigthFactor;
				colorMix = hc * heightColorMixFactor + heightColorMixAdder;
				if (invertColorMix) colorMix = 1 - colorMix;
			}
			finalColor = Color.Lerp (color1, color2, colorMix);
		}
	}
	
	public float updateRate = 0.0333333333f; // 30fps
    [ReadOnly2Attribute] public float heigth11;
    [ReadOnly2Attribute] public float absHeight01;
    [ReadOnly2Attribute] public float day01;
    [ReadOnly2Attribute] public float night01;

	[Header ("---")]
	public bool heightAbsoluteMode = false;
	public bool heightNegativeIsZero = false;

	private Color sunColor; // solo para verlo en inspector ...? sigo usando esto?

	[Header ("to be used by other mixers")]
	public ColorMixer sunAndSky;
	[Header ("TINT1 (COSMOS) -> Shader 'Tint (Cosmos)' or SprRenderer Color")]
	public ColorMixer mainTint;
    [Header("to be added to TINT1 Before Apply")]
    public bool mainTintAddEnabled = false;
    bool MainTintAddEnabled() { return mainTintAddEnabled; }

    public ColorMixer mainTintAdd;

    [Range(0,1)] public float mainTintAddFactor;

    [ReadOnly2Attribute] public Color mainTintFinalColor;

	[Header ("---LIGHTNESS / DARKENING")]
    [ReadOnly2Attribute] public float sunIntensity;
    [ReadOnly2Attribute] public float nightIntensity;
	public float sunIntensityFactor;
	public bool heightAbsoluteModeForLightness = false;
	public bool modLightness = false;
    public AnimationCurve lightnessCurve;

    [Range(-1,1)] public float heightLightnessFactor = 0f;
	[Range(-1,1)] public float heightLightnessAdder = 0f;
	//[Range(-1,1)] public float atmosAlphaFactor = 0f;
	//public float lightnessMin;
	//public float lightnessMax;

	public bool lightenBasedOnZ;
    [Range(-0.005f, 0.005f)] public float lightnessZFactor;

    public float lightnessZShift;
    [ReadOnly2Attribute] public float lightness = 1f;
    private Color lightnessColor; // para sumarlo comodamente

    [Header("Doesn't show Z depth Lightness/Darkening")]
    [ReadOnly2Attribute] public Color finalColorToSprites; // el que colos que se va a aplicar a los sprites

	[Header ("---TRANSPARENCY")]
	public bool heightNegativeIsZeroTransparency = false;
	public bool lockTransParency = false;
	[Range(-1,1)]
	public float heightTransparencyFactor = 0f;
	[Range(-1,1)]
	public float heightTransparencyAdder = 0f;
	[Range(-1,1)]
	public float finalTransparency;
	public enum WhatToChange {Renderers, SpriteRenderers, SWiz_Sprites}

    [Header ("TINT 2 (Atmos & Sky) ---- to be set at Shader _ColorSpecial ----")]
	public ColorMixer specialTint;
	public bool specialTintResetIfDisabled = true;
	[Range(0,80)] public float specialTintAdd;
	[Range(0,200)] public float specialTintMult;
	public bool specialTintZbased;
	[Range(0,0.00005f)] public float specialTintZFactor;
	public float specialTintZshift;

    [Header("SAT CONT VAL ----")]
    public bool modSaturationAndContrast = false;
    bool ModSaturationAndContrast() { return modSaturationAndContrast; }
    [Tooltip("Set To First Plane Z")]
    public float firstPlaneZ; // fijar a mano el primer plano parallax
    [Range(-4, 4)] public float saturationAdd; 
    [Range(0,0.005f)] public float saturationZ_Factor;
    [Range(-4, 4)] public float contrastAdd; 
    [Range(0, 0.005f)] public float contrastZ_Factor;
    [Range(-4, 4)] public float valueAdd; 
    [Range(-0.005f, 0.005f)] public float valueZ_Factor;

    [Header ("DITHERING TINT ---- to be set at Shader _ColorDithering ----")]
    public ColorMixer ditheringTint;
    public bool ditheringTintResetIfDisabled = true;
//    [Range(0,80)] public float ditheringTintAdd;
//    [Range(0,200)] public float ditheringTintMult;
    public bool ditheringTintZbased;
    [Range(0,0.0005f)] public float ditheringTintZFactor;
    public float ditheringTintZshift;

	[Header ("---SPRITES OR RENDERERS")]
	public WhatToChange whatToChange = WhatToChange.SWiz_Sprites;
	public Renderer[] renderers;
	public SpriteRenderer[] spriteRenderers;
	public SWizSprite[] SWizSprites;

	private bool renderersHaveColorSpecial;
	private bool renderersHaveHSVC;
    private bool renderersHaveDitheringColor;
	//private int _Color; // no lo uso
	private int _ColorSpecial;
	private int _ColorSpecialMult;
	private int _ColorSpecialAdd;

	void Awake () {
		Initialize();
		Game.onFindLevelSpecificStuff += OnFindLevelSpecificStuff;
	}
	void OnDestroy(){
		Game.onFindLevelSpecificStuff -= OnFindLevelSpecificStuff;
	}
	void OnVaLidate(){
		Initialize();
        MyUpdate();
	}
	void OnFindLevelSpecificStuff(){
		if(Game.sun != null){
			if (this == null)
			{
				print ("SI LLEGA AQUI ES POR QUE EL DELEGATE/EVENT HA LLAMADO AQUI PERO LA INSTANCIA DE ESTE SCRIPT YA NO EXISTE, POR ESO AÑADI UN ELIMINAR DEL DELEGATE/EVENT EN ONDESTROY");
			}
		}
		else
		{
			this.enabled = false;
		}
	}
	void Initialize(){
		renderersHaveColorSpecial = false;
        renderersHaveDitheringColor = false;
        switch (whatToChange)
        {
            case (WhatToChange.SWiz_Sprites):
                SWizSprites = GetComponentsInChildren<SWizSprite>();
                if (renderers != null && renderers.Length > 0)
                    renderers = null;
                if (spriteRenderers != null && spriteRenderers.Length > 0)
                    spriteRenderers = null;
                break;
            case (WhatToChange.Renderers):
                renderers = GetComponentsInChildren<Renderer>();
                if (renderers != null && renderers.Length > 0)
                {
	                var r = renderers[0];
	                var m = r.sharedMaterial;
                    if (m.HasProperty("_ColorSpecial"))
                        RenderersHaveColorAdd();
                    if (m.HasProperty(ShaderProp._ColorDithering))
                        renderersHaveDitheringColor = true;
                    if (m.HasProperty(ShaderProp._Hue) && m.HasProperty(ShaderProp._Sat) && m.HasProperty(ShaderProp._Val) && m.HasProperty(ShaderProp._Cont))
	                    renderersHaveHSVC = true;
                }
                if (SWizSprites != null && SWizSprites.Length > 0)
                    SWizSprites = null;
                if (spriteRenderers != null && spriteRenderers.Length > 0)
                    spriteRenderers = null;
                break;
            case (WhatToChange.SpriteRenderers):
                spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
                if (spriteRenderers != null && spriteRenderers.Length > 0)
                {
                    if (spriteRenderers[0].sharedMaterial.HasProperty("_ColorSpecial"))
                        RenderersHaveColorAdd();
                    if (spriteRenderers[0].sharedMaterial.HasProperty(ShaderProp._ColorDithering))
                        renderersHaveDitheringColor = true;
                }
                if (renderers != null && renderers.Length > 0)
                    renderers = null;
                if (SWizSprites != null && SWizSprites.Length > 0)
                    SWizSprites = null;
                break;
        }
	}
	void RenderersHaveColorAdd(){
		renderersHaveColorSpecial = true;
//		_Color = Shader.PropertyToID("_Color"); // no lo uso
		_ColorSpecial = Shader.PropertyToID("_ColorSpecial");
		_ColorSpecialAdd = Shader.PropertyToID("_ColorSpecialAdd");
		_ColorSpecialMult = Shader.PropertyToID("_ColorSpecialMult");
	}

	public Color GetColorFromEnum(ref Color color, ColorToMix ctm){
		switch(ctm){
			case(ColorToMix.PickedManually):
				break;
			case(ColorToMix.ThisSunAndSky):
				color = sunAndSky.finalColor;
				break;
			case(ColorToMix.GameSun):
				color = Game.sun.actualColor;
				break;
			case(ColorToMix.GameSky):
				color = Game.skyManager.skyColor;
				break;
			case(ColorToMix.GameCosmos):
				color = Game.skyManager.cosmosColor;
				break;
			case(ColorToMix.GameAtmos):
				color = Game.skyManager.atmosColor;
				break;
			case(ColorToMix.ThisTintMult):
				color = mainTint.finalColor;
				break;
			case(ColorToMix.ThisTintAdd):
				color = specialTint.finalColor;
				break;
			case (ColorToMix.WorldMapSkyColor): //TODO implementar en moons of darsalon MOD
				color = WorldMap.instance.skyColor;//TODO implementar en moons of darsalon MOD
				break;
			case (ColorToMix.WorldMapNightColor): //TODO implementar en moons of darsalon MOD
				color = WorldMap.instance.nightColor;//TODO implementar en moons of darsalon MOD
				break;
			
		}
		return Color.magenta; // no debe llegar aqui nunca
	}
	public void MyUpdate(){
		heigth11 = Game.sun.elipse.heightFactor;
		absHeight01 = Mathf.Abs(heigth11);
        day01 = Mathf.Clamp01(heigth11);
        night01 = Mathf.Clamp01(-heigth11);

        if (heightNegativeIsZero && heigth11 < 0) heigth11 = 0f;
		
		if (lockTransParency == false)
		{
			if (heightNegativeIsZeroTransparency && heigth11 < 0)
                finalTransparency = heightTransparencyAdder; 
			else
                finalTransparency = heigth11 * heightTransparencyFactor + heightTransparencyAdder;
		}

		if (sunAndSky.enabled){
			GetColorFromEnum(ref sunAndSky.color1, sunAndSky.getColor1From);
			GetColorFromEnum(ref sunAndSky.color2, sunAndSky.getColor2From);
			sunAndSky.Mix(heigth11, absHeight01);
		}

		if (mainTint.enabled){
			GetColorFromEnum(ref mainTint.color1, mainTint.getColor1From);
			GetColorFromEnum(ref mainTint.color2, mainTint.getColor2From);
			mainTint.Mix(heigth11, absHeight01);
			mainTintFinalColor = mainTint.finalColor;
		}
		else mainTintFinalColor = Color.black;

		if (mainTintAdd.enabled){
			GetColorFromEnum(ref mainTintAdd.color1, mainTintAdd.getColor1From);
			GetColorFromEnum(ref mainTintAdd.color2, mainTintAdd.getColor2From);
			mainTintAdd.Mix(heigth11, absHeight01);
			mainTintFinalColor += mainTintAdd.finalColor * mainTintAddFactor;
		}

		sunIntensity = Game.sun.finalIntensity;
		if (Game.skyLight) nightIntensity = Game.skyLight.finalIntensity;

		if (modLightness)
		{
            //float hl;
            //if(heightAbsoluteModeForLightness) hl = absHeight01;
            //else hl = heigth11;
            //lightness = (hl * heightLightnessFactor) + heightLightnessAdder + (mainTintFinalColor.a * atmosAlphaFactor) + Mathf.Lerp(sunIntensity, nightIntensity, sunAndSky.colorMix) * sunIntensityFactor;
            //lightness = Mathf.Clamp(lightness, lightnessMin, lightnessMax);

            lightness = lightnessCurve.Evaluate(heigth11) * heightLightnessFactor + heightLightnessAdder;
			lightnessColor = new Color (lightness, lightness, lightness, 0);
			finalColorToSprites = mainTintFinalColor + lightnessColor;
		}
		else
            finalColorToSprites = mainTintFinalColor;

		finalColorToSprites.a = finalTransparency;

		if (specialTint.enabled){
			GetColorFromEnum(ref specialTint.color1, specialTint.getColor1From);
			GetColorFromEnum(ref specialTint.color2, specialTint.getColor2From);
			specialTint.Mix(heigth11, absHeight01);
		}

        if (ditheringTint.enabled)
        {
            GetColorFromEnum(ref ditheringTint.color1, ditheringTint.getColor1From);
            GetColorFromEnum(ref ditheringTint.color2, ditheringTint.getColor2From);
            ditheringTint.Mix(heigth11, absHeight01);
        }

		int n;
		for (n = 0; n < SWizSprites.Length; n++)
		{
			SWizSprites[n].color = finalColorToSprites;
		}
		for (n = 0; n < renderers.Length; n++)
		{
			var r = renderers[n];
			var m = r.sharedMaterial;

			m.color = finalColorToSprites; // tint1 cosmos se aplica a material property _Color
			if (renderersHaveHSVC)
			{
				m.SetFloat(ShaderProp._Hue, WorldMap.instance.cosmosHueShift);
				var map = WorldMap.instance;
				if (!WorldMap.instance.useDayAndNightCSV)
				{
					m.SetFloat(ShaderProp._Sat, map.cosmosSaturation);
					m.SetFloat(ShaderProp._Val, map.cosmosBright);
					m.SetFloat(ShaderProp._Cont, map.cosmosContrast);
				}
				else
				{
					float v = heigth11 - 0.5f;
					m.SetFloat(ShaderProp._Sat, Mathf.Lerp(map.cosmosAtNightSaturation, map.cosmosSaturation, v));
					m.SetFloat(ShaderProp._Val, Mathf.Lerp(map.cosmosAtnightBright, map.cosmosBright, v));
					m.SetFloat(ShaderProp._Cont, Mathf.Lerp(map.cosmosAtNightContrast, map.cosmosContrast, v));
				}
			}

			if (renderersHaveColorSpecial)
			{
//				float zFactor = (spriteRenderers.transform.position.
//				renderers[n].sharedMaterial.SetColor(_ColorSpecial, tintAdd.finalColor);
				ApplySpecialAndDitheringTints(renderers[n].transform.position.z, renderers[n].sharedMaterial);
			}
		}
		for (n = 0; n < spriteRenderers.Length; n++)
		{	
			SpriteRenderer spr = spriteRenderers[n];
			Color darkenedColor = finalColorToSprites;
			if (lightenBasedOnZ)
			{

                float zfactor = (spr.transform.position.z - lightnessZShift);
                float darkening = zfactor * lightnessZFactor;;
				darkenedColor *= darkening + 1;
				darkenedColor.a = finalColorToSprites.a;
			}
			spriteRenderers[n].color = darkenedColor;
			if (renderersHaveColorSpecial)
			{
//				float amount = tintAddAmount;
//				if (tintAddZbased) amount = (spr.transform.position.z - tintAddZshift) * tintAddZFactor;
				ApplySpecialAndDitheringTints(spr.transform.position.z, spriteRenderers[n].sharedMaterial);
			}
//						spriteRenderers[n].sharedMaterial.SetColor(_ColorSpecial, tintAdd.finalColor);
		}
	}
	void ApplySpecialAndDitheringTints(float posZ, Material mat){
		float amount = 1;
		if (specialTintZbased)
            amount = (posZ - specialTintZshift) * specialTintZFactor;
		if (renderersHaveColorSpecial)
		{
			if (specialTint.enabled)
			{
				mat.SetColor(_ColorSpecial, specialTint.finalColor);
				mat.SetFloat(_ColorSpecialAdd, amount * specialTintAdd);
				mat.SetFloat(_ColorSpecialMult, amount * specialTintMult);
			}
			else if (specialTintResetIfDisabled)
			{
				mat.SetColor(_ColorSpecial, Color.white);
				mat.SetFloat(_ColorSpecialAdd, 0);
				mat.SetFloat(_ColorSpecialMult, 0);
			}
		}
        if (renderersHaveDitheringColor)
        {
            if (ditheringTint.enabled)
            {
                Color color = ditheringTint.finalColor;
                if (ditheringTintZbased)
                {
                    color.a = (posZ - ditheringTintZshift) * ditheringTintZFactor;
                }
                mat.SetColor(ShaderProp._ColorDithering, color);
            }
            else if (ditheringTintResetIfDisabled)
            {
                mat.SetColor(ShaderProp._ColorDithering, Constants.transparentBlack);
            }
        }
        if(modSaturationAndContrast)
        {
            float f = firstPlaneZ - posZ;
            mat.SetFloat(ShaderProp._Sat,  f * saturationZ_Factor + saturationAdd);
            mat.SetFloat(ShaderProp._Cont, f * contrastZ_Factor + contrastAdd);
            mat.SetFloat(ShaderProp._Val,  f * valueZ_Factor + valueAdd);
        }
    }
}
