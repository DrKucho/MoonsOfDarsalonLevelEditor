// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// unlit, vertex colour, alpha blended
// cull off

Shader "_Kucho/Glass Reflects Sky" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
		 // deteccion de color glass
         _GlassCol("GlassColor", Color) = (1,1,1,1)
         _GlassColRange("GlassColorRange" , Range(0,2)) = 0.5
         // modificacion del color glass
         _GlassColorDestination("GlassColorDestination", Color) = (1,1,1,1)
         _GlassColorBoost("GlassColorBoost" , Range(0,1)) = 1
         _GlassGreyScaleBoost("GlassGreyScaleBoost", Range(0,2)) = 0.5
         _GlassGreyScaleShift("GlassGreyScaleShift",  Range(-1,1)) = 1
//         _SkyColor("SkyColor", Color) = (1,1,1,1)
         _SkyColorIntensity("SkyColorIntensity" , Range(0,1)) = 1

         _nothing ("", float) = 0
         _Color ("Tint", Color) = (1,1,1,1)
         _DitherColor ("DitherColor", Color) = (1,1,1,0.025)
         _HueShift("HueShift", Range(-180, 180)) = 0
         _Sat("Saturation", Range(0,2)) = 1
         _Val("Value", Range (0,2)) = 1
         _Contrast("_Contrast", Range (0,2)) = 1

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
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
				#include "Assets/-KUCHO/Color/Shaders/KuchoCGHelper.cginc" 
				#include "Assets/-KUCHO/Color/Shaders/SkyColors.cginc"
               
//                struct v2f
//                {
//                    half4  pos : SV_POSITION;
//                    half2  uv : TEXCOORD0;
//                    half4  color : COLOR; // vertex color
//                };

//                v2f vert (appdata_tan v)
//                {
//                    v2f o;
//                    o.pos = UnityObjectToClipPos(v.vertex);
//                    o.uv = v.texcoord.xy;
//                    return o;
//                }
               
                sampler2D _MainTex;

				half4 _GlassCol;
				half  _GlassColRange;
				half4 _GlassColorDestination;
				half _GlassGreyScaleBoost;
				half _GlassGreyScaleShift;
				half _GlassColorBoost;
				half _SkyColorIntensity;

				half4 _Color;
                half4 _DitherColor;
				half _HueShift;
				half _Sat;
				half _Val;
				half _Contrast;

				half _Add;
                half _Alpha;

                void Vert(posAndCoord IN, out posTwoCoordsAndHSVMults OUT)  
				{
					OUT.pos = UnityObjectToClipPos(IN.pos); // pos = posicion en la pantalla ?
					OUT.texcoord1 = mul (unity_ObjectToWorld, IN.pos); // worldPosition!
					OUT.color = IN.color * _Color; // aplico el color independiente del sprite IN.color y _Color! ya que _Color es igual para todos los materiales 
					OUT.texcoord0 = IN.texcoord0;
					OUT = CalculateThingsForHSVFastProcessing(_HueShift, _Sat, _Val, OUT);   
				}

                half GetSwitch(half4 col, half range, half4 streCol)
                {
                	half4 colDist = col - streCol;
					half dist = abs(colDist.r) + abs(colDist.g) + abs(colDist.b);
					return step (dist, range);
                }

                half4 frag(posTwoCoordsAndHSVMults i) : COLOR
                {
                    half4 result;
                    half4 col =  tex2D(_MainTex, i.texcoord0);

                    half  maxComp = max (col.r, col.g);
                    maxComp = max (maxComp, col.b);
                    half mult = 1 / maxComp;
                    half4 streCol = col * mult;
					half sky_sw  = GetSwitch(_GlassCol, _GlassColRange, streCol); 

                    col *= i.color;
                	col.rgb = ShiftHSV_Fast(col, i); // Hue Sat And Value
                	col.rgb = (col.rgb - 0.5f) * (_Contrast) + 0.5f; // contrast
	                col.rgb = DitheringWorld(i.texcoord1, col, _DitherColor);  // Dithering basandose en la posicion en el mundo  
//                    half4 halfCol = col * 0.5;

                    half4 greyScale = (col.r + col.g + col.b) / 3;
                    half  alpha = step(0.5, col.a) * _Alpha;

		
					half main_sw = step (sky_sw, 0.5); // pone a 1 si todos los demas son cero y al contrario

					result.rgb  = col * main_sw;
					half4 glassGrey = greyScale * _GlassGreyScaleBoost + _GlassGreyScaleShift;
					half4 glassCol = glassGrey + _GlassColorDestination * _GlassColorBoost;
					result.rgb += ( glassCol + (_SkyColor * _SkyColorIntensity)) * sky_sw;

                    result.rgb += _Add;
                    result.a = alpha;

                    return result;
                }
            ENDCG
        }
    }
}