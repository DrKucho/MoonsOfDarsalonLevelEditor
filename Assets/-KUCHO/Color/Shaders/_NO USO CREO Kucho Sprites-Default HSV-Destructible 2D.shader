// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Kucho/Sprites-Default-HSV-Destructible 2D"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_AlphaTex ("Alpha Tex", 2D) = "white" {}
		_AlphaScale ("Alpha Scale", Vector) = (1,1,0,0)
		_AlphaOffset ("Alpha Offset", Vector) = (0,0,0,0)
		_Sharpness ("Sharpness", float) = 1.0
		_Color ("Tint", Color) = (1,1,1,1)
		_HueShift("HueShift", Range(-180, 180)) = 0
        _Sat("Saturation", Range(0,2)) = 1
        _Val("Value", Range (0,2)) = 1
        _Alpha ("Alpha", Range(0,1)) = 1
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
				#pragma vertex Vert
				#pragma fragment Frag
				#pragma multi_compile DUMMY 
				#include "UnityCG.cginc"
				#include "Assets/-KUCHO/Color/Shaders/KuchoCGHelper.cginc"  

				
				sampler2D _MainTex;
				sampler2D _AlphaTex;
				half     _Sharpness;
				half4    _Color;
				half2    _AlphaScale;
				half2    _AlphaOffset;
				
				half _HueShift;
				half _Sat;
				half _Val;
				half _Alpha;

				
				struct a2v
				{
					half4 vertex    : POSITION;
					half4 color     : COLOR;
					half2 texcoord : TEXCOORD0;
				};
				
				struct v2f
				{
					half4 vertex    : SV_POSITION;
					half4 color     : COLOR;
					half2 texcoord : TEXCOORD0;
					half2 texcoord1 : TEXCOORD1;
				};
			
			void Vert(a2v IN, out v2f OUT)
			{
				OUT.vertex    = UnityObjectToClipPos(IN.vertex);
				OUT.color     = IN.color * _Color;
				OUT.texcoord = IN.texcoord;
				OUT.texcoord1 = (IN.texcoord - _AlphaOffset) * _AlphaScale;
			}
			
			void Frag(v2f IN, out half4 o:COLOR0)
			{
				half4 col = tex2D(_MainTex, IN.texcoord);// sample sprite Texture 
				col *= IN.color; // Apply Tint
                col.rgb *= col.a;
                col.rgb = ShiftHSV(col, half3(_HueShift, _Sat, _Val)); // Hue Sat And Value 
				
				col.a = 1.0f;
				half4 alphaTex = tex2D(_AlphaTex, IN.texcoord1);
				
				// Clip the alpha if it's outside the range
				half2 clipUV = abs(IN.texcoord1 - 0.5f);
				
				alphaTex.a *= max(clipUV.x, clipUV.y) <= 0.5f ? 1.0f : 0.0f;
				
				// Multiply the color
				o.rgba = col * IN.color;
				
				// Apply alpha tex
				o.a *= saturate(0.5f + (alphaTex.a - 0.5f) * _Sharpness);
				
				// Premultiply alpha
				o.rgb *= o.a;
			}

			ENDCG
		}
	}
}
