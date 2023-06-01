// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Kucho/Sprites-Default HSVC (Explosions)"
{
     Properties
     {
		 _MainTex ("Sprite Texture", 2D) = "white" {}
		 _Color ("Tint", Color) = (1,1,1,1)
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
        ZWrite Off Lighting Off Cull Off Fog { Mode Off }
        //Blend SrcAlpha OneMinusSrcAlpha // este mantiene intensidad constante en cielo pero hace que se transparente un poco la pared
        //Blend One One
        //Blend OneMinusDstColor One 
        //Blend DstColor Zero
        //Blend DstColor SrcColor
        Blend One OneMinusSrcAlpha // este mezcla mejor las transparencias entre si y con el cielo 
     	//Blend SrcAlpha DstAlpha // el que tenia para la linterna	
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
			half _HueShift;
			half _Sat;
			half _Val;
			half _Contrast;
            half _Alpha;

			void Vert(posAndCoord IN, out posTwoCoordsAndHSVMults OUT)  
			{
				OUT.pos = UnityObjectToClipPos(IN.pos); 
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
                col.a *= _Alpha;
             
                return col;
            }
		ENDCG
		}
	}
 }