// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Kucho/Sprites-Default HSVC (DarkTint)"
{
     Properties
     {
		 _MainTex ("Sprite Texture", 2D) = "white" {}
         _DitherColor ("DitherColor", Color) = (1,1,1,0.025)
	     _TintShift("TintShift", Range(-1,0)) = 0
     	         _Mult1 ("Mult1", Range(0,20)) = 1

		 _Color ("Tint", Color) = (1,1,1,1)
		 //_DarkTint ("DarkTint", Color) = (1,1,1,1)
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
			#pragma multi_compile
			#include "UnityCG.cginc" 
			#include "Assets/-KUCHO/Color/Shaders/KuchoCGHelper.cginc"  

			
			sampler2D _MainTex;
            half4 _Color;
			half _TintShift;
            half4 _DitherColor;

            //half4 _DarkTint;
			half _HueShift;
			half _Sat;
			half _Val;
			half _Contrast;
            half _Alpha;
            half _Mult1;

			void Vert(posAndCoord IN, out posTwoCoordsAndHSVMults OUT)  
			{
				OUT.pos = UnityObjectToClipPos(IN.pos); 
				OUT.color = IN.color;// * _Color; // aplico el color independiente del sprite IN.color y _Color! ya que _Color es igual para todos los materiales 
				OUT.texcoord0 = IN.texcoord0;
				OUT.texcoord1 = mul (unity_ObjectToWorld, IN.pos); // worldPosition!
				OUT = CalculateThingsForHSVFastProcessing(_HueShift, _Sat, _Val, OUT); 
			}

            half4 Frag(posTwoCoordsAndHSVMults IN) : COLOR
            {
				half4 mainTextCol = tex2D(_MainTex, IN.texcoord0 );// sample sprite Texture
            	half transparent_sw = step(mainTextCol.a, 0.001);
            	half solid_sw = 1-transparent_sw;
            	half4 col = mainTextCol * solid_sw;
            	half4 transCol = mainTextCol * transparent_sw;

            	col.rgb = ShiftHSV_Fast(col, IN); // Hue Sat And Value

            	
				//col *= IN.color; // Apply Tint
            	//col += _DarkTint * col.r * col.a;
            	half i = saturate(1 - col.r + _TintShift) * col.r; // col.r = cutre luma
            	//col.rgb *= IN.color * i * IN.color.a * col.a;
            	col.rgb += IN.color * i * IN.color.a * _Mult1 * col.a;
				//col += IN.color * IN.color.a * col.r * col.a; // Apply Tint
            	col.rgb *= col.a;
            	

            	col.rgb = (col.rgb - 0.5f) * (_Contrast) + 0.5f; // contrast
            	col.rgb = DitheringWorld(IN.texcoord1, col, _DitherColor);  // Dithering basandose en la posicion en el mundo
            	col.rgb += transCol *  mainTextCol.a;
            	

                col.a *= _Alpha;
             
                return col;
            }
		ENDCG
		}
	}
 }