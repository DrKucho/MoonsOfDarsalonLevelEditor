// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// unlit, vertex colour, alpha blended
// cull off

Shader "_Kucho/TreeShader" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}


         _Col1("LeavesOriginalColor", Color) = (1,1,1,1)
         _Col1Range("LeavesColorRange" , Range(0,2)) = 0.5
         _Col1Dest("LeavsColorDestination", Color) = (1,1,1,1)
         _Col1Boost("LeavesColorBright" , Range(0,2)) = 1
         _Col1Cont("LeavesContrast" , Range(0,2)) = 1

         _nothing ("", int) = 0 // spacer
         _Col2Dest("BarkColor", Color) = (1,1,1,1)
         _Col2Boost("BarkBright" , Range(0,2)) = 1
         _Col2Cont("BarkContrast" , Range(0,2)) = 1

        _nothing ("", int) = 0 // spacer
		_DitherColor ("DitherColor", Color) = (1,1,1,0.025)

        _nothing ("", int) = 0 // spacer
        _Add("Add", Range (-1,2)) = 0
        _Alpha ("Alpha", Float) = 1
	}
	
    SubShader {
    	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Lighting Off Cull Off Fog { Mode Off }
    	
    	//		Blend DstColor Zero // Multiplicative
//		Blend DstColor SrcColor // 2x Multiplicative
//		Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency (falla creando transparencias en background)
		Blend One OneMinusSrcAlpha // Premultiplied transparency (Funciona bien con las sombras sobre background , no crea transparencias fallidas)
		//Blend OneMinusDstColor One // Soft Additive
		//Blend One One // Additive - funciona bien sobre el cielo al contrario que el plugin por defecto de unity
    	
		LOD 110
		
        Pass {
           
            CGPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
				#include "Assets/-KUCHO/Color/Shaders/KuchoCGHelper.cginc" 
				#include "Assets/-KUCHO/Color/Shaders/SkyColors.cginc"
               
 
                void Vert(posAndCoord IN, out posAndTwoCoords OUT) 
                {
					OUT.pos = UnityObjectToClipPos(IN.pos); // pos = posicion en la pantalla ?
                    OUT.texcoord1 = mul (unity_ObjectToWorld, IN.pos); // world pos!
                    OUT.texcoord0 = IN.texcoord0.xy;  
                }
               
                sampler2D _MainTex;

				half4 _Col1;
				half  _Col1Range;
				half  _Col1Boost;
				half4 _Col1Dest;
				half _Col1Cont;

				half  _Col2Boost;
				half4 _Col2Dest;
				half _Col2Cont;

		
				half4 _DitherColor;

				half _Add;
                half _Alpha;

                half GetSwitch(half4 col, half range, half4 streCol)
                {
                	half4 colDist = col - streCol;
					half dist = abs(colDist.r) + abs(colDist.g) + abs(colDist.b);
					return step (dist, range);
                }

                half4 Frag(posAndTwoCoords i) : COLOR
                {
                    half4 result;
                    half4 trueCol =  tex2D(_MainTex, i.texcoord0);
                    half  maxComp = max (trueCol.r, trueCol.g);
                    maxComp = max (maxComp, trueCol.b);
                    half mult = 1 / maxComp;
                    half4 streCol = trueCol * mult;
                    half greyScale = (trueCol.r + trueCol.g + trueCol.b) / 3;
                	//half greyScale = Luma(trueCol.rgb);

                    //half  alpha = step(0.5, trueCol.a);

                    half col1_sw = GetSwitch(_Col1, _Col1Range, streCol);
					//half col2_sw = GetSwitch(_Col2, _Col2Range, streCol);
                	half col2_sw = step(col1_sw, 0.5f);// si este vale 0 el otro vale 1


					half main_sw = step (col1_sw + col2_sw, 0.5); // pone a 1 si todos los demas son cero y al contrario

					result.rgb  = trueCol * main_sw;
					half3 c1 = greyScale * _Col1Dest * _Col1Boost;
					half3 c2 = greyScale * _Col2Dest * _Col2Boost;

                	c1 = ((c1 - 0.5f) * _Col1Cont + 0.5f) * col1_sw; // contrast
                	c2 = ((c2 - 0.5f) * _Col2Cont + 0.5f) * col2_sw; // contrast
                	//result.rgb = c1 -0.25;

                	result.rgb += c1;
                	result.rgb += c2;
                	
                    //result.a = alpha; // necesario hacerlo antes de llamar a dithering
                	result.a = trueCol.a;
					result.rgb = HorizontalDitheringWorld(i.texcoord1, result, _DitherColor);  // Dithering basandose en la posicion en el mundo  

                	
                    result.rgb += _Add; 


                    return result;
                }
            ENDCG
        }
    }
}