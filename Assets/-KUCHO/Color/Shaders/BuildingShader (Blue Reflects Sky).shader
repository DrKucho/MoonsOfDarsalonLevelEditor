// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// unlit, vertex colour, alpha blended
// cull off

Shader "_Kucho/BuildingShader (Blue Reflects Sky)" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}


         _GreyColors("GreyColors", Color) = (1,1,1,1)
         _nothing ("", int) = 0 // spacer

         _Red("Red", Color) = (1,1,1,1)
         _RedBoost("RedBoost" , Range(1,2)) = 1
         _Magenta ("Magenta", Color) = (1,1,1,1)
         _MagentaBoost("MagentaBoost", Range (1,2)) = 1 
         _Orange ("Orange", Color) = (1,1,1,1)
         _OrangeBoost("OrangeBoost", Range (1,2)) = 1
         _nothing ("", int) = 0 // spacer

         _Green("Green", Color) = (1,1,1,1)
         _GreenBoost("GreenBoost" , Range(1,2)) = 1
         _Apple ("Apple", Color) = (1,1,1,1)
         _AppleBoost("AppleBoost", Range (1,2)) = 1
         _Turquoise ("Turquoise", Color) = (1,1,1,1)
         _TurquoiseBost("TurquoiseBoost", Range (1,2)) = 1 
         _nothing ("", int) = 0 // spacer

         _Blue("Blue", Color) = (1,1,1,1)
         _BlueBoost("GreenBoost" , Range(1,2)) = 1
         _Cyan ("Cyan", Color) = (1,1,1,1)
         _CyanBoost("CyanBoost", Range (1,2)) = 1
         _Purple ("Purple", Color) = (1,1,1,1)
         _PurpleBoost("PurpleBoost", Range (1,2)) = 1
         _nothing ("", int) = 0 // spacer

         _GlassColorShift("GlassColorShift",  Range(-1,1)) = 1
         _GlassColorIntensity("GlassColorIntensity" , Range(0,1)) = 1
         _SkyColorIntensity("SkyColorIntensity" , Range(0,1)) = 1


         _SkyColor("SkyColor", Color) = (1,1,1,1)

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
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
               
                struct v2f
                {
                    half4  pos : SV_POSITION;
                    half2  uv : TEXCOORD0;
                };
 
                v2f vert (appdata_tan v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.texcoord.xy;
                    return o;
                }
               
                sampler2D _MainTex;
				half4 _NormalColors;
				half4 _GreyColors;
				half4 _Green;
				half  _GreenBoost;
				half4 _Red;
				half  _RedBoost;
				half4 _Blue;
				half  _BlueBoost;
				half4 _Orange;
				half  _OrangeBoost;
				half4 _Apple;
				half  _AppleBoost;
				half4 _Turquoise;
				half  _TurquoiseBoost;
				half4 _Cyan;
				half  _CyanBoost;
				half4 _Magenta;
				half  _MagentaBoost;
				half4 _Purple;
				half  _PurpleBoost;

				half4 _SkyColor;
				half _GlassColorShift;
				half _GlassColorIntensity;
				half _SkyColorIntensity;
                half _HueShift;
				half _Sat;
				half _Val;
				half _Add;
                half _Alpha;

	            half3 shift_col(half3 RGB, half3 shift) // HSV OPTIMIZADO
	            {
	            	half3 RESULT = half3(RGB);
	            	half value0 = shift.x * 0.0174532925;//3.14159265/180);
	            	half value00 = shift.z * shift.y;
	             	half VSU = value00 * cos(value0);
	                half VSW = value00 * sin(value0);
	                half value1 = .299 * shift.z;
	                half value2 = .587 * shift.z;
	                half value3 = .114 * shift.z;
	                half value4 = .114 * VSU;
	                half value5 = .5875 * VSU;
	                 
	                RESULT.x = (value1 + .701 * VSU + .168 * VSW) * RGB.x
	                         + (value2 -   value5   + .330 * VSW) * RGB.y // value5 era .587 * VSU
	                         + (value3 -   value4   - .497 * VSW) * RGB.z;

	                RESULT.y = (value1 - .299 * VSU - .328 * VSW) * RGB.x
	                         + (value2 + .413 * VSU + .035 * VSW) * RGB.y
	                         + (value3 -   value4   + .292 * VSW) * RGB.z;
	 
	        		RESULT.z = (value1 - .3   * VSU + 1.25 * VSW) * RGB.x
	                         + (value2 -   value5   - 1.05 * VSW) * RGB.y // value5 era .588 * VSU
	                         + (value3 + .886 * VSU - .203 * VSW) * RGB.z;

	             	return (RESULT);
	            }
                half4 frag(v2f i) : COLOR
                {
                // RESULT
                    half4 result;
                    half4 mainCol =  tex2D(_MainTex, i.uv);
                    half4 halfCol = mainCol * 0.5;
                    half4 greyScale = (mainCol.r + mainCol.g + mainCol.b) / 3;
//                    half3 shiftedColor = shift_col(mainCol.rgb, half3(_HueShift, _Sat, _Val));
                    half alpha = step(0.5, mainCol.a) * _Alpha;


//                    half blue_sw = step(mainCol.a, 0.99);    // si este vale 0 el otro vale 1

					half BlueBiggerThanRed = 1 - step (mainCol.b, mainCol.r);
					half BlueBiggerThanGreen = 1 - step (mainCol.b, mainCol.g);
					half GreenBiggerThanRed = 1 - step (mainCol.g, mainCol.r);
					half GreenBiggerThanBlue = 1 - step (mainCol.g, mainCol.b);
					half RedBiggerThanGreen = 1 - step (mainCol.r, mainCol.g);
					half RedBiggerThanBlue = 1 - step (mainCol.r, mainCol.b);
					half RedEqualsGreen = step(abs(mainCol.r - mainCol.g),0);// si son iguales se pone a 1 
					half GreenEqualsBlue = step(abs(mainCol.g - mainCol.b),0);// si son iguales se pone a 1 
					half BlueEqualsRed = step(abs(mainCol.b - mainCol.r),0);// si son iguales se pone a 1
					half BlueBiggerThanHalfGreen = step (halfCol.g , mainCol.b);
					half BlueBiggerThanHalfRed   = step (halfCol.r , mainCol.b);
					half GreenBiggerThanHalfBlue = step (halfCol.b , mainCol.g);
					half GreenBiggerThanHalfRed  = step (halfCol.r , mainCol.g);
					half RedBiggerThanHalfBlue   = step (halfCol.b , mainCol.r);
					half RedBiggerThanHalfGreen  = step (halfCol.g , mainCol.r);


					// Blue Test / blue_sw // si b es mas grande que r y mas grande que g
					//half blue_sw = saturate(test1 + test2);
					half blue_sw = step (0.6, saturate((BlueBiggerThanRed + BlueBiggerThanGreen) * 0.5));
					// Green Test
                    half green_sw = step (0.6, saturate((GreenBiggerThanRed + GreenBiggerThanBlue) * 0.5));
//                  blue_sw *=  abs(1 - green_sw); // si ambos blue y green dan positivo solo uno tiene preferencia el green
                    // Red Test / red_sw
                    half red_sw = step (0.6, saturate((RedBiggerThanGreen + RedBiggerThanBlue) / 2));

                    // Cyan Test
                    half cyan_sw = blue_sw * GreenBiggerThanHalfBlue;
                    cyan_sw = saturate (cyan_sw + GreenEqualsBlue);

                    // TurquoiseTest
                    half turquoise_sw = green_sw * BlueBiggerThanHalfGreen;

                    // AppleGreenTest
                    half apple_sw = green_sw * RedBiggerThanHalfGreen;
                    apple_sw = saturate (apple_sw + RedEqualsGreen);

                    // OrangeTest
                    half orange_sw = red_sw * GreenBiggerThanHalfRed;

                    // MagentaTest
                    half magenta_sw = red_sw * BlueBiggerThanHalfRed;

                    // PurpleTest
                    half purple_sw = blue_sw * RedBiggerThanHalfBlue;
                    purple_sw = saturate (purple_sw + BlueEqualsRed);

                    // Yellow Test / yellow_sw
//                    test1 = 1 - step (mainCol.r, mainCol.b);
//                    test2 = 1 - step (mainCol.g, mainCol.b);
       
                 
                 	// Grey test / grey_sw

					half grey_sw =  step (3, RedEqualsGreen + GreenEqualsBlue + BlueEqualsRed);


					half mainCol_sw = step (blue_sw + grey_sw + green_sw + red_sw, 0.5); // pone a 1 si todos los demas son cero y al contrario
					green_sw *= step (apple_sw + turquoise_sw, 0.5); // pone a cero si hay de los otros
					blue_sw *= step (cyan_sw + purple_sw, 0.5); // pone a cero si hay de los otros
					red_sw *= step (orange_sw + magenta_sw, 0.5); // pone a cero si hay de los otros

//                    result.rgb = shiftedColor * _NormalColors * mainCol_sw; // el color real si no es transparente ni negro-gris-blanco se aplica aqui

//                    result.rgb  = mainCol * _GreyColors * grey_sw ; // colorea los pixeles negros y grises 
//                    result.rgb += greyScale * _Green * _GreenBoost * green_sw ; // colorea los pixeles verdes
//                    result.rgb += greyScale * _Red * _RedBoost * red_sw ; // colorea los pixeles rojos
//                    result.rgb += ((mainCol * _GlassColorIntensity) + (_SkyColor * _SkyColorIntensity)) * blue_sw; // aplica color CON transparencia sin replacement

					result.rgb  = mainCol * _GreyColors * grey_sw ; // colorea los pixeles negros y grises

					result.rgb += greyScale * _Red * _RedBoost * red_sw;
					result.rgb += greyScale * _Orange * _OrangeBoost * orange_sw;
					result.rgb += greyScale * _Magenta * _MagentaBoost * magenta_sw;

                    result.rgb += greyScale * _Green * _GreenBoost * green_sw ; // colorea los pixeles verdes
					result.rgb += greyScale * _Apple * _AppleBoost * apple_sw;
					result.rgb += greyScale * _Turquoise * _TurquoiseBoost * turquoise_sw;

					result.rgb += greyScale * _Blue * _BlueBoost * blue_sw;
//					result.rgb += greyScale * _Cyan * _CyanBoost * cyan_sw;
					result.rgb += greyScale * _Purple * _PurpleBoost * purple_sw;

					result.rgb += (((greyScale + _GlassColorShift) * _GlassColorIntensity) + (_SkyColor * _SkyColorIntensity)) * cyan_sw;

                    result.rgb += _Add;
                    result.a = alpha;

                    return result;
                }
            ENDCG
        }
    }
}