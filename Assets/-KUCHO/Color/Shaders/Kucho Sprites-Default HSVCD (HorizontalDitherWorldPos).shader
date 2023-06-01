﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Kucho/Sprites-Default HSVCD (HorizontalDitherWorldPos)" 
{
     Properties
     {
		 _MainTex ("Sprite Texture", 2D) = "white" {}
		 _Color ("Tint", Color) = (1,1,1,1)
         _DitherColor ("DitherColor", Color) = (1,1,1,0.025) 
         _HueShift("HueShift", Range(-180, 180)) = 0
         _Sat("Saturation", Range(0,2)) = 1
         _Val("Value", Range (0,2)) = 1
         _Contrast("_Contrast", Range (0,2)) = 1
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

			half _HueShift;
			half _Sat;
			half _Val;
			half _Contrast;
            half _Alpha;
            float4 _MainTex_TexelSize;
			
			void Vert(posAndCoord IN, out posTwoCoordsAndHSVMults OUT)  
			{
				OUT.pos = UnityObjectToClipPos(IN.pos); // pos = posicion en la pantalla ?
				OUT.texcoord1 = mul (unity_ObjectToWorld, IN.pos); // worldPosition!
				OUT.color = IN.color * _Color; // aplico el color independiente del sprite IN.color y _Color! ya que _Color es igual para todos los materiales 
				OUT.texcoord0 = IN.texcoord0;
				OUT = CalculateThingsForHSVFastProcessing(_HueShift, _Sat, _Val, OUT);   
			}

            half4 Frag(posTwoCoordsAndHSVMults IN) : COLOR
            {
				half4 col = tex2D(_MainTex, IN.texcoord0 );// sample sprite Texture 
				col *= IN.color; // Apply Tint
                col.rgb *= col.a;
                col.rgb = ShiftHSV_Fast(col, IN); // Hue Sat And Value
                col.rgb = (col.rgb - 0.5f) * (_Contrast) + 0.5f; // contrast
                col.rgb = HorizontalDitheringWorld(IN.texcoord1, col, _DitherColor);  // Dithering basandose en la posicion en el mundo  
                 
                col.a *= _Alpha;   
              
                return col;
            }
		ENDCG
		}
	}
 }