// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Kucho/GrassShader HSV"
{
     Properties
     {
		 _MainTex ("Sprite Texture", 2D) = "white" {}
		 _Color ("Tint", Color) = (1,1,1,1)
     	_Gain ("Gain", Range(0, 2)) = 1
     	_SpriteColorFactor ("SpriteColor", Range(0	, 1)) = 0.5
     	_AlphaCut( "Alpha Cut" , Range (0,1)) = 0
         _AlphaPuch ("AlphaPush", Range(1,2)) = 1
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
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
            half4 _Color;
            half _AlphaPush;
            half _Gain;
            half _AlphaCut;
			half _SpriteColorFactor;

			struct appdata_t
			{
				half4 vertex   : POSITION;
				half4 color    : COLOR;
				half2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				half4 vertex   : SV_POSITION;
				half4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				
            	//half4  revColor;
            	//revColor.rgb = 2 -(IN.color.rgb * -_SpriteColor);
				//revColor.a = IN.color.a * _Alpha;
				//OUT.color = revColor * (_Color * _SpriteColor);// ?? MEZCLA DE COLOR INVERTIDO CON TINT

				half4 sprColor;
				sprColor.rgb = IN.color.rgb * _SpriteColorFactor;
				half shaderColorFactor = 1 - _SpriteColorFactor;
				half4 shaderColor;
				shaderColor.rgb = _Color.rgb * shaderColorFactor;
				OUT.color = (sprColor + shaderColor) * _Gain;
				OUT.color.a = ((IN.color.a * _SpriteColorFactor) + (_Color.a * shaderColorFactor)) * _AlphaPush;
				
				//OUT.color = IN.color * _Color; // asi era entes
				OUT.texcoord = IN.texcoord;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

            half4 frag(v2f IN) : COLOR
            {
            	half4 col;

            	col.ga = tex2D(_MainTex, IN.texcoord).ga; // pillo solo green
            	//col.g *= _Gain;
            	col.r = col.g; 
            	col.b = col.g; // igualado , ahora es blanco basado en green
            	half pixelAlpha = col.a;

            	col *= IN.color; // mezclo color de pixeles con color calculado en Vert
            	half alpha_sw = step (_AlphaCut, pixelAlpha); // creo switch para alphas bajos
            	col.a = saturate(pixelAlpha * alpha_sw * IN.color.a); // elimino alphas bajos
                col.rgb *= col.a;// * _Gain;
                return col;
            }
		ENDCG
		}
	}
     Fallback "Particles/Alpha Blended"
 }
