// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Kucho/Sprites-Default - HSVCD-TintAdd"
{
     Properties
     {
		 _MainTex ("Sprite Texture", 2D) = "white" {}
		 _Color ("Tint", Color) = (1,1,1,1)
         _HueShift("HueShift", Range(-180, 180)) = 0
         _Sat("Saturation", Range(0,2)) = 1
         _Val("Value", Range (0,4)) = 1
         _Contrast("_Contrast", Range (0,2)) = 1
         _Alpha ("Alpha", Range(0,1)) = 1
         _ColorDithering ("DitherColor", Color) = (0,0,0,0)

         _ColorSpecial ("Tint Special", Color) = (1,1,1,1)
		 _ColorSpecialAdd ("Tint Special Add", Range(0,1)) = 0
		 _ColorSpecialMult ("Tint Special Mult", Range(0,4)) = 1
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
//		Blend One OneMinusSrcAlpha // el original pero crea bordes raros como de textura recortada al aplicar la suma de _ColorSpecial
		Blend SrcAlpha OneMinusSrcAlpha // funciona pero parece que tiende a saturar y brillar demasiado

		Pass
		{
		CGPROGRAM
			#pragma vertex Vert
			#pragma fragment Frag
			#include "UnityCG.cginc"
			#include "Assets/-KUCHO/Color/Shaders/SkyColors.cginc"
			#include "Assets/-KUCHO/Color/Shaders/KuchoCGHelper.cginc"  


			sampler2D _MainTex;
            half4 _Color;
			half _HueShift;
			half _Sat;
			half _Val;
			half _Contrast;
            half _Alpha;
            half4 _ColorDithering;
            half4 _ColorSpecial;
			half _ColorSpecialAdd;
			half _ColorSpecialMult;
			float4 _MainTex_TexelSize;

			half4 SampleSpriteTexture (half2 uv)
	        {
				half4 color = tex2D (_MainTex, uv);
				return color;
			}
			
			void Vert(posAndCoord IN, out posTwoCoordsAndHSVMults OUT)  
			{
				OUT.pos = UnityObjectToClipPos(IN.pos); 
				OUT.color = IN.color * _Color; // aplico el color independiente del sprite IN.color y _Color! ya que _Color es igual para todos los materiales 
				OUT.texcoord0 = IN.texcoord0;
				OUT = CalculateThingsForHSVFastProcessing(_HueShift, _Sat, _Val, OUT); 
			}

            half4 Frag(posTwoCoordsAndHSVMults IN) : COLOR
            {
				half4 col = SampleSpriteTexture (IN.texcoord0) * IN.color;
                col.rgb *= col.a;
//                col.rgb = shift_col(col, half3(_HueShift, _Sat, _Val));
            	col.rgb = ShiftHSV_Fast(col, IN); // HUE 
                col.a *= _Alpha;
                half4 col1;
                col1.rgb = col.rgb * _ColorSpecial.rgb * _ColorSpecialMult;
                col1.a = col.a;
                half4 col2;
                col2.rgb = col.rgb + _ColorSpecial.rgb * _ColorSpecialAdd; // cuidado , el alpha se me va a la mierda o algo raro pasa , por eso tengo que hacerlo en otro pase con otro modeo de Blend, al final descubri como hacerlo todo en un solo pase, cambiando el Blend mode

                col2.rgb = (col2.rgb - 0.5f) * (_Contrast) + 0.5f;
                col2.rgb += _ColorDithering.rgb * _ColorDithering.a * GetDitherMultiplier(IN.texcoord0, _MainTex_TexelSize.zw );

                col2.a = col.a;

                // borrame
//                col2.rgb = _ColorDithering.rgb * _ColorDithering.a * GetDitherMultiplier(IN);
//                return col2;
                return col1 + col2;
            }
		ENDCG
		}
	}
     Fallback "Particles/Alpha Blended"
 }