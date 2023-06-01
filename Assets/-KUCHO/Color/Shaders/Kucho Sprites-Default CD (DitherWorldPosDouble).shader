// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Kucho/Sprites-Default CD (DitherWorldPosDouble)"
{
     Properties
     {
		 _MainTex ("Sprite Texture", 2D) = "white" {}
		 _Color ("Tint", Color) = (1,1,1,1)
         _DitherColor ("DitherColor", Color) = (1,1,1,0.025)
         _DitherColorLuma ("DitherColorLuma", Color) = (1,1,1,0.025)
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
            half4 _DitherColorLuma;

			half _Contrast;
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
                col.rgb *= col.a;
                col.rgb = (col.rgb - 0.5f) * (_Contrast) + 0.5f; // contrast

                col.rgb = DitheringWorldDouble(IN.texcoord1, col, _DitherColor, _DitherColorLuma);  // Dithering basandose en la posicion en el mundo  

                col.a *= _Alpha;   
              
                return col;
            }
		ENDCG
		}
	}
 }