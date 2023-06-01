using UnityEngine;
using System.Collections;

public class ShaderProp : MonoBehaviour {

	public static int _Color;
	public static int _ColorSpecial;
	public static int _ColorSpecialAdd;
	public static int _ColorSpecialMult;
    public static int _AdditiveColor;
    public static int _MultiplicativeColor;
    public static int _ColorDithering;
	public static int _Hue;
    public static int _Sat;
    public static int _Cont;
	public static int _Val;
	public static int _Alpha;
	public static int _Tint;
	public static int _MainTex;
	public static int _AlphaTex;
	public static int _AlphaTex1;
	public static int _AlphaTex2;
	public static int _AlphaTex3;
    public static int _TexB;
    public static int _TexM;
    public static int _TexF;
    public static int _ColorRamp;
	public static int _TexSize;

	public static int _AlphaScale;
	public static int _AlphaOffset;
	public static int _Sharpness;

	public static int _Add;
	public static int _Luminosity;
    public static int _GlassColorDestination;
    public static int _GlassGreyScaleBoost;
    public static int _GlassColorBoost;
    public static int _GlassGreyScaleShift;
    public static int _SkyColorIntensity;

	public static int _SkyColor;
	public static int _RealSaturatedSkyColor;
	public static int _RealSaturatedSkyColorLuminance;
	public static int _SunColor;
	public static int _CosmosColor;
	public static int _AtmosColor;

	public static int _PixelZoom;

	public static int _CameraPos_MapSize;
	public static int _Angle;
	public static int _Projected_Directional_Dissapear_At_Night;
	public static int _EditorPreGain;
	
	public static int _ObstacleMul;
	public static int _EmissionColorMul;


	#if UNITY_EDITOR
	void OnValidate(){ // necesario si no los valores se quedan a cero cada vez que compilo
		Initialise();
	}
	#endif

	void Awake(){
		Initialise();
	}
	public static void Initialise() {
		_Color = Shader.PropertyToID("_Color");
		_ColorSpecial = Shader.PropertyToID("_ColorSpecial");
		_ColorSpecialAdd = Shader.PropertyToID("_ColorSpecialAdd");
        _ColorSpecialMult = Shader.PropertyToID("_ColorSpecialMult");
        _AdditiveColor = Shader.PropertyToID("_AdditiveColor");
        _MultiplicativeColor = Shader.PropertyToID("_MultiplicativeColor");
        _ColorDithering = Shader.PropertyToID("_ColorDithering");
		_Hue = Shader.PropertyToID("_HueShift");
        _Sat = Shader.PropertyToID("_Sat");
        _Cont = Shader.PropertyToID("_Contrast");
		_Val = Shader.PropertyToID("_Val");
		_Alpha = Shader.PropertyToID("_Alpha");
		_Tint = Shader.PropertyToID("_Tint");
		_MainTex = Shader.PropertyToID("_MainTex");
		_AlphaTex = Shader.PropertyToID("_AlphaTex");
		_AlphaTex1 = Shader.PropertyToID("_AlphaTex1");
		_AlphaTex2 = Shader.PropertyToID("_AlphaTex2");
        _AlphaTex3 = Shader.PropertyToID("_AlphaTex3");
        _TexB = Shader.PropertyToID("_TexB");
        _TexM = Shader.PropertyToID("_TexM");
        _TexF = Shader.PropertyToID("_TexF");
		_ColorRamp = Shader.PropertyToID("_ColorRamp");
		_TexSize = Shader.PropertyToID("_TexSize");

		_AlphaScale = Shader.PropertyToID("_AlphaScale");
		_AlphaOffset = Shader.PropertyToID("_AlphaOffset");
		_Sharpness = Shader.PropertyToID("_Sharpness");

		_Add = Shader.PropertyToID("_Add");
		_Luminosity = Shader.PropertyToID("_Luminosity");
        _GlassColorDestination = Shader.PropertyToID("_GlassColorDestination");
        _GlassGreyScaleBoost = Shader.PropertyToID("_GlassGreyScaleBoost");
        _GlassColorBoost = Shader.PropertyToID("_GlassColorBoost");
        _GlassGreyScaleShift = Shader.PropertyToID("_GlassGreyScaleShift");
        _SkyColorIntensity = Shader.PropertyToID("_SkyColorIntensity");
		_SkyColor = Shader.PropertyToID("_SkyColor");
		_RealSaturatedSkyColor = Shader.PropertyToID("_RealSaturatedSkyColor");
		_RealSaturatedSkyColorLuminance = Shader.PropertyToID("_RealSaturatedSkyColorLuminance");
		_SunColor = Shader.PropertyToID("_SunColor");
		_CosmosColor = Shader.PropertyToID("_CosmosColor");
		_AtmosColor = Shader.PropertyToID("_AtmosColor");
		
		_PixelZoom = Shader.PropertyToID("_PixelZoom");

		_CameraPos_MapSize = Shader.PropertyToID("_CameraPos_MapSize");
		_Angle = Shader.PropertyToID("_Angle");
		_Projected_Directional_Dissapear_At_Night = Shader.PropertyToID("_Projected_Directional_Dissapear_At_Night");
		
		_ObstacleMul = Shader.PropertyToID("_ObstacleMul");
		_EmissionColorMul = Shader.PropertyToID("_EmissionColorMul");


	}
}
