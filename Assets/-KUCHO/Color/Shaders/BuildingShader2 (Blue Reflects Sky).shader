// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// unlit, vertex colour, alpha blended
// cull off

Shader "_Kucho/BuildingShader2 (Blue Reflects Sky)" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}


         _Col1("Color1", Color) = (1,1,1,1)
         _Col1Range("Color1Range" , Range(0,2)) = 0.5
         _Col1Dest("Color1Destination", Color) = (1,1,1,1)
         _Col1Boost("Color1Boost" , Range(1,2)) = 1

         _nothing ("", int) = 0 // spacer
         _Col2("Color2", Color) = (1,1,1,1)
         _Col2Range("Color2Range" , Range(0,2)) = 0.5
         _Col2Dest("Color2Destination", Color) = (1,1,1,1)
         _Col2Boost("Color2Boost" , Range(1,2)) = 1

         _nothing ("", int) = 0 // spacer
         _Col3("Color3", Color) = (1,1,1,1)
         _Col3Range("Color3Range" , Range(0,2)) = 0.5
         _Col3Dest("Color3Destination", Color) = (1,1,1,1)
         _Col3Boost("Color3Boost" , Range(1,2)) = 1


		 _nothing ("", int) = 0 // spacer
         _GlassCol("GlassColor", Color) = (1,1,1,1)
         _GlassColRange("GlassColorRange" , Range(0,2)) = 0.5
         _GlassColDest("GlassColorDestination", Color) = (1,1,1,1)
         _GlassColBoost("GlassColorIntensity" , Range(0,1)) = 1
         _GlassGreyScaleBoost("GlassGreyScaleBoost", Range(0,2)) = 0.5
         _GlassGreyScaleShift("GlassGreyScaleShift",  Range(-1,1)) = 1
         _SkyColorIntensity("SkyColorIntensity" , Range(0,1)) = 1

        _nothing ("", int) = 0 // spacer
		_DitherColor ("DitherColor", Color) = (1,1,1,0.025)




//         _HueShift("HueShift", Range(-180, 180)) = 0
//         _Sat("Saturation", Range(0,2)) = 1
//         _Val("Value", Range (0,4)) = 1
        _nothing ("", int) = 0 // spacer
         _Add("Add", Range (-1,2)) = 0
        _Alpha ("Alpha", Float) = 1
	}
	
    SubShader {
    	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha
		LOD 110
		
        Pass {
            Name "ColorReplacementAlpha"
           
            CGPROGRAM
                #pragma vertex Vert
                #pragma fragment Frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
				#include "Assets/-KUCHO/Color/Shaders/KuchoCGHelper.cginc" 
				#include "Assets/-KUCHO/Color/Shaders/SkyColors.cginc"
               
//                struct v2f
//                {
//                    half4  pos : SV_POSITION;
//                    half2  uv : TEXCOORD0;
//                };
 
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

				half4 _Col2;
				half  _Col2Range;
				half  _Col2Boost;
				half4 _Col2Dest;

				half4 _Col3;
				half  _Col3Range;
				half  _Col3Boost;
				half4 _Col3Dest;

				half4 _GlassCol;
				half  _GlassColRange;
				half4 _GlassColDest;
				half _GlassGreyScaleBoost;
				half _GlassGreyScaleShift;
				half _GlassColBoost;
				half _SkyColorIntensity;

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
//                    half4 halfCol = trueCol * 0.5;
                    half  maxComp = max (trueCol.r, trueCol.g);
                    maxComp = max (maxComp, trueCol.b);
                    half mult = 1 / maxComp;
                    half4 streCol = trueCol * mult;
                    half4 greyScale = (trueCol.r + trueCol.g + trueCol.b) / 3;
                    half  alpha = step(0.5, trueCol.a) * _Alpha;

                    half col1_sw = GetSwitch(_Col1, _Col1Range, streCol);
					half col2_sw = GetSwitch(_Col2, _Col2Range, streCol);
					half col3_sw = GetSwitch(_Col3, _Col3Range, streCol);
					half sky_sw  = GetSwitch(_GlassCol, _GlassColRange, streCol); 



					half main_sw = step (col1_sw + col2_sw + col3_sw + sky_sw, 0.5); // pone a 1 si todos los demas son cero y al contrario

					result.rgb  = trueCol * main_sw;
					result.rgb += greyScale * _Col1Dest * _Col1Boost * col1_sw;
					result.rgb += greyScale * _Col2Dest * _Col2Boost * col2_sw;
					result.rgb += greyScale * _Col3Dest * _Col3Boost * col3_sw;
					half4 glassGrey = (greyScale + _GlassGreyScaleShift) * _GlassGreyScaleBoost;
					half4 glassCol = _GlassColDest * _GlassColBoost;
					result.rgb += ( glassGrey + (_SkyColor * _SkyColorIntensity)) * sky_sw; 
                    result.a = alpha; // necesario hacerlo antes de llamar a dithering
					result.rgb = DitheringWorld(i.texcoord1, result, _DitherColor);  // Dithering basandose en la posicion en el mundo

                    result.rgb += _Add; 


                    return result;
                }
            ENDCG
        }
    }
}