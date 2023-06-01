// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Kucho/LightGlare"
{
     Properties
     {
		 _MainTex ("Sprite Texture", 2D) = "white" {}
		 _Color ("Tint", Color) = (1,1,1,1)
         _Dithering ("Dither", Range (0,1)) = 1
         _LowAlphaBoost ("LowAlphaDitherBoost", Range (0,1)) = 1
         _LowAlphahreshold ("LowAlphaDitherThreshold", Range (0,1)) = 1
         _AlphaCut ("AlphaCut", Range (0,1)) = 1
         _AlphaCutMult ("AlphaCutMult", Range (0,1)) = 1
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
			#include "UnityCG.cginc"
			#include "Assets/-KUCHO/Color/Shaders/KuchoCGHelper.cginc" 

			sampler2D _MainTex;
            half4 _Color;
            half _Dithering;
            half _AlphaCut;
            half _LowAlphaBoost;
            half _AlphaCutMult;
            half _LowAlphahreshold;
			
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
                //half alpha01 = step(0.5, col.a);// * _Alpha;
                //col.rgb *= col.a;
            	//col.rgb *= col.a;

            	//col.a *= alpha_sw;
				col *= IN.color; // Apply Tint el cual incluye alpha ambien luego hay que hacer esto antes que el alpha cut
            	half alpha_sw = step (_AlphaCut, col.a * _AlphaCutMult); // creo switch para alphas bajos


            	 //DITHER ESPECIAL LUMA CON BOOS PARA ALPHAS BAJOS
					half pairMult = step(frac((IN.texcoord1.x + IN.texcoord1.y + 0.01) / 2), 0.5);// devuelve 0 o 1 correctamente incluso si el pixel esta en posicion con decimales, y + 0.001 subsana el error de redondeo para cuando estamos en una pisicion entera!
					//pairMult *= alpha_sw;
					//half boost = ((1 - col.a) * _LowAlphaBoost);
					//boost *= boost * boost;
					//boost *= pairMult;// col.a;
					half normalDitheting =  pairMult * _Dithering * alpha_sw;// + boost;
				
            	half boost = ((_LowAlphahreshold - col.a) * _LowAlphaBoost) *  pairMult;

				col.a -= (normalDitheting + boost);
            	col.a *= alpha_sw;
            	col.rgb *= col.a;
            	
                return col;
            }
		ENDCG
		}
	}
 }