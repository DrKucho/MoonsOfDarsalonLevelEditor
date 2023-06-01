// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Kucho/Sprites-Default CBCSD (DitherWorldPos)"
{
     Properties
     {
		 _MainTex ("Sprite Texture", 2D) = "white" {}
		 _Color ("Tint", Color) = (1,1,1,1)
         _DitherColor ("DitherColor", Color) = (1,1,1,0.025)
         _Contrast("_Contrast", Range (0,2)) = 1
         _Brightness("_Brightness", Range (0,2)) = 1
         _Saturation("_Saturation", Range (0,2)) = 1 // no tengo claro que esto funcione
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
			#include "UnityCG.cginc"
			#include "Assets/-KUCHO/Color/Shaders/KuchoCGHelper.cginc" 

			sampler2D _MainTex;
            half4 _Color;
            half4 _DitherColor;

			half _Contrast;
			half _Brightness;
			half _Saturation;
            half _Alpha;
			
			void Vert(posAndCoord IN, out posAndTwoCoords OUT)  
			{
				OUT.pos = UnityObjectToClipPos(IN.pos); // pos = posicion en la pantalla ?
				OUT.texcoord1 = mul (unity_ObjectToWorld, IN.pos); // worldPosition!
				OUT.color = IN.color * _Color; // aplico el color independiente del sprite IN.color y _Color! ya que _Color es igual para todos los materiales 
				OUT.texcoord0 = IN.texcoord0;
			}

            half4 Frag(posTwoCoordsAndHSVMults IN) : COLOR
            {
				half4 col = tex2D(_MainTex, IN.texcoord0 );// sample sprite Texture 
				col *= IN.color; // Apply Tint
				float4 brtColor 	= tex2D(_MainTex, IN.texcoord0);
				float alpha = brtColor.a;
				brtColor.rgb *= _Brightness;

				float intensityf 	= dot(brtColor, float3(0.2125,0.7154,0.0721));
				col.rgb				= lerp(float3(0.5,0.5,0.5), lerp(float3(intensityf, intensityf, intensityf), brtColor, _Saturation), _Contrast);

                col.rgb = DitheringWorld(IN.texcoord1, col, _DitherColor);  // Dithering basandose en la posicion en el mundo  
                col.a = alpha;
                col.a *= _Alpha;   

                return col;
            }
		ENDCG
		}
	}
 }