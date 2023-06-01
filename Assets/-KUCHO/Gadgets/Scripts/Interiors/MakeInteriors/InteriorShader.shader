// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// unlit, vertex colour, alpha blended
// cull off

Shader "_Kucho/InteriorShader" 
{
	Properties 
	{
		 _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}

		 _Color ("Tint (Just For Alpha)", Color) = (1,1,1,1)

         _Col1("Color1", Color) = (1,1,1,1)
         _Col1Range("Color1Range" , Range(0,2)) = 0.5
         _Col1Dest("Color1Destination", Color) = (1,1,1,1)
         _Col1Boost("Color1Boost" , Range(1,5)) = 1

         _nothing ("", int) = 0 // spacer
         _Col2("Color2", Color) = (1,1,1,1)
         _Col2Range("Color2Range" , Range(0,2)) = 0.5
         _Col2Dest("Color2Destination", Color) = (1,1,1,1)
         _Col2Boost("Color2Boost" , Range(1,5)) = 1

         _nothing ("", int) = 0 // spacer
         _Col3("Color3", Color) = (1,1,1,1)
         _Col3Range("Color3Range" , Range(0,2)) = 0.5
         _Col3Dest("Color3Destination", Color) = (1,1,1,1)
         _Col3Boost("Color3Boost" , Range(1,5)) = 1


		 _nothing ("", int) = 0 // spacer
         _GlassCol("GlassColor", Color) = (1,1,1,1)
         _GlassColRange("GlassColorRange" , Range(0,2)) = 0.5
         _GlassColDest("GlassColorDestination", Color) = (1,1,1,1)
         _GlassColBoost("GlassColorBoost" , Range(0,1)) = 1
         _GlassGreyScaleBoost("GlassGreyScaleBoost", Range(0,2)) = 0.5
         _GlassGreyScaleShift("GlassGreyScaleShift",  Range(-1,1)) = 1
         _SkyColor("SkyColor", Color) = (1,1,1,1)
         _SkyColorIntensity("SkyColorIntensity" , Range(0,1)) = 1



//         _HueShift("HueShift", Range(-180, 180)) = 0
//         _Sat("Saturation", Range(0,2)) = 1
//         _Val("Value", Range (0,4)) = 1

         _nothing ("", int) = 0 // spacer
         _Contrast("_Contrast", Range (0,2)) = 1
         _Add("Add", Range (-1,2)) = 0
//         _Alpha ("Alpha", Float) = 1
	}
	
    SubShader {
    	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Lighting Off Cull Off Fog { Mode Off }
//		Blend SrcAlpha OneMinusSrcAlpha
		Blend One OneMinusSrcAlpha
		LOD 110
		
        Pass {
            Name "ColorReplacementAlpha"
           
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"

                half4 _Color;
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
				half _GlassColBoost;

				half _GlassGreyScaleBoost;
				half _GlassGreyScaleShift;
				half4 _SkyColor;
				half _SkyColorIntensity;

				half _Contrast;

				half _Add;
//                half _Alpha;


                struct v2f
                {
                    half4  pos : SV_POSITION;
                    half4  color : COLOR;
                    half2  uv : TEXCOORD0; 
                };

                v2f vert (appdata_tan v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.texcoord.xy;
                    return o;
                }

                half GetSwitch(half4 col, half range, half4 streCol)
                {
                	half4 colDist = col - streCol;
					half dist = abs(colDist.r) + abs(colDist.g) + abs(colDist.b);
					return step (dist, range);
                }

                half4 frag(v2f i) : COLOR
                {
                    half4 result;
                    half4 trueCol =  tex2D(_MainTex, i.uv);
//                    half4 halfCol = trueCol * 0.5;
                    half  maxComp = max (trueCol.r, trueCol.g);
                    maxComp = max (maxComp, trueCol.b);
                    half mult = 1 / maxComp;
                    half4 streCol = trueCol * mult;
                    half4 greyScale = (trueCol.r + trueCol.g + trueCol.b) / 3;
                    half  alpha = step(0.5, trueCol.a) * _Color.a;

                    half col1_sw = GetSwitch(_Col1, _Col1Range, streCol);
					half col2_sw = GetSwitch(_Col2, _Col2Range, streCol);
					half col3_sw = GetSwitch(_Col3, _Col3Range, streCol);
					half sky_sw  = GetSwitch(_GlassCol, _GlassColRange, streCol); 



					half main_sw = step (col1_sw + col2_sw + col3_sw + sky_sw, 0.5); // pone a 1 si todos los demas son cero y al contrario

					result.rgb  = trueCol * main_sw;
					result.rgb += greyScale * _Col1Dest * _Col1Boost * col1_sw;
					result.rgb += greyScale * _Col2Dest * _Col2Boost * col2_sw;
					result.rgb += greyScale * _Col3Dest * _Col3Boost * col3_sw;
					result .rgb += greyScale * _GlassColDest * _GlassColBoost * sky_sw;
					half4 glassGrey = (greyScale + _GlassGreyScaleShift) * _GlassGreyScaleBoost;
					result.rgb += ( glassGrey + (_SkyColor * _SkyColorIntensity)) * sky_sw;

					result.rgb = (result.rgb - 0.5f) * (_Contrast) + 0.5f;
			
                    result.rgb += _Add;
                    // hacer esto asi junto con Blend One OneMinusSrcAlpha es lo que hace que al ser transparente no haga transparente a los sprites que estan detras y se vea el cielo
                    result.rgb *= alpha;
                    result.a = alpha;

                    return result;
                }
            ENDCG
        }
    }
}