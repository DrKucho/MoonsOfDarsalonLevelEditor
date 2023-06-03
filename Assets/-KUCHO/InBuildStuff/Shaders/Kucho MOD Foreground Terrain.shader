
Shader "_Kucho/Terrain/MOD Foreground Terrain"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_DefaultTex ("Default Sprite Tex", 2D) = "white" {}
		_AlphaTex ("Alpha Tex", 2D) = "white" {}

		_AlphaScale ("Must Be 1,1,0,0", Vector) = (1,1,0,0)
		_AlphaOffset ("Must Be 0,0,0,0", Vector) = (0,0,0,0)
		_HueShift("Will Be Ignored, Don't Touch", Range(-180, 180)) = 0
        _Sat("Will Be Ignored, Don't Touch", Range(0,2)) = 1
        _Val("Will Be Ignored, Don't Touch", Range (0,3)) = 1
        _Contrast("Will Be Ignored, Don't Touch", Range (0,4)) = 1
        _Color ("Will Be Ignored, Don't Touch", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
				#define LONG_SHADOW_STEPS 7
				#define SHORT_SHADOW_STEPS 3
				#define BOTTOM_SHADOW_STEPS 5
				#pragma vertex Vert
				#pragma fragment Frag
				#pragma multi_compile DUMMY
				#include "UnityCG.cginc"
				#include "Assets/-KUCHO/Color/Shaders/KuchoCGHelper.cginc" 

				sampler2D _MainTex;
				sampler2D _AlphaTex;
//				half     _Sharpness;
				half4    _Color;
				half2    _AlphaScale;
				half2    _AlphaOffset;
				
				half _HueShift;
				half _Sat;
				half _Val;
				half _Contrast;


			void Vert(posAndCoord IN, out posTwoCoordsAndHSVMults OUT)  
			{
				OUT.pos    = UnityObjectToClipPos(IN.pos);
				OUT.color     = IN.color * _Color;
				OUT.texcoord0 = IN.texcoord0;
				OUT.texcoord1 = (IN.texcoord0 - _AlphaOffset) * _AlphaScale;
				OUT.valueMults2.y = 0; // inicializacion para evitar warning
				OUT.valueMults2.z = 0; // inicializacion para evitar warning
				OUT.valueMults2.w = 0; // inicializacion para evitar warning
				
				OUT = CalculateThingsForHSVFastProcessing(_HueShift, _Sat, _Val, OUT); 
			}

			void Frag(posTwoCoordsAndHSVMults i, out half4 o:COLOR0)
			{
				half4 alphaTex;
				half blurredAlpha;

				half4 mainTex  = tex2D(_MainTex, i.texcoord0);

            	mainTex.rgb = ShiftHSV_Fast(mainTex, i);
            	mainTex.rgb = (mainTex.rgb - 0.5f) * (_Contrast) + 0.5f;

				alphaTex = tex2D(_AlphaTex, i.texcoord1);
				
				o.rgba = mainTex * i.color;
				
				o.a = alphaTex.a;
				
				o.rgb *= o.a;

			}
			ENDCG
		}
	}
}
