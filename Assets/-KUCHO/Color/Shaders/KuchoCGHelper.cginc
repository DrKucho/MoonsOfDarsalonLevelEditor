
struct posAndCoord 
{
	half4 pos    : POSITION;
	half4 color     : COLOR; 
	float2 texcoord0 : TEXCOORD0; 
};
struct posAndTwoCoords 
{
	half4 pos    : SV_POSITION;
	half4 color     : COLOR; 
	float2 texcoord0 : TEXCOORD0; 
	half2 texcoord1 : TEXCOORD1; 
}; 

struct posTwoCoordsAndHSVMults
{
	half4 pos    : SV_POSITION; 
	half4 color     : COLOR; 
	float2 texcoord0 : TEXCOORD0; 
	half2 texcoord1 : TEXCOORD1;
	half2 VsuVsw	: TEXCOORD5;
	half4 valueMults  :COLOR1;
	half4 valueMults2 :COLOR2;
};

posTwoCoordsAndHSVMults CalculateThingsForHSVFastProcessing(half3 _HueShift, half _Sat, half _Val, posTwoCoordsAndHSVMults OUT)   
{
	OUT.VsuVsw.x = _Val * _Sat * cos(_HueShift * 0.0174532925); 
	OUT.VsuVsw.y = _Val * _Sat * sin(_HueShift * 0.0174532925); 
	OUT.valueMults.x = .299 * _Val;
	OUT.valueMults.y = .587 * _Val;
	OUT.valueMults.z = .114 * _Val;
	OUT.valueMults.w = .114 * OUT.VsuVsw.x;
	OUT.valueMults2.x = .588 * OUT.VsuVsw.x; 
	return (OUT);
}

float2 GetObjectPosition2()  { return float2(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3]); }
float3 GetObjectPosition3()  { return float3(unity_ObjectToWorld[0][3], unity_ObjectToWorld[1][3], unity_ObjectToWorld[2][3]); }
float3 GetObjectUp()        { return normalize(unity_ObjectToWorld[1].xyz); }
float3 GetPixelPosition3(float2 uv) {return mul ((float4x4)unity_ObjectToWorld, uv );} 
half3 ShiftHSV_Fast(half3 RGB, posTwoCoordsAndHSVMults i) 
{
	half3 RESULT = half3(RGB);
	half VSU = i.VsuVsw.x;
	half VSW = i.VsuVsw.y;
    
    RESULT.x = (i.valueMults.x + .701 * VSU + .168 * VSW) * RGB.x
             + (i.valueMults.y - i.valueMults2.x + .330 * VSW) * RGB.y  
             + (i.valueMults.z - i.valueMults.w - .497 * VSW) * RGB.z; 

    RESULT.y = (i.valueMults.x - .299 * VSU - .328 * VSW) * RGB.x
             + (i.valueMults.y + .413 * VSU + .035 * VSW) * RGB.y
             + (i.valueMults.z - i.valueMults.w + .292 * VSW) * RGB.z;

	RESULT.z = (i.valueMults.x - .3   * VSU + 1.25 * VSW) * RGB.x
             + (i.valueMults.y - i.valueMults2.x - 1.05 * VSW) * RGB.y 
             + (i.valueMults.z + .886 * VSU - .203 * VSW) * RGB.z;

 	return (RESULT);
}

half3 ShiftHSV(half3 RGB, half3 shift) 
{
	half3 RESULT = half3(RGB);
	half value0 = shift.x * 0.0174532925;
	half value00 = shift.z * shift.y;
 	half VSU = value00 * cos(value0);
    half VSW = value00 * sin(value0);
    half value1 = .299 * shift.z;
    half value2 = .587 * shift.z;
    half value3 = .114 * shift.z;
    half value4 = .114 * VSU;
    half value5 = .5875 * VSU;
     
    RESULT.x = (value1 + .701 * VSU + .168 * VSW) * RGB.x
             + (value2 -   value5   + .330 * VSW) * RGB.y 
             + (value3 -   value4   - .497 * VSW) * RGB.z;

    RESULT.y = (value1 - .299 * VSU - .328 * VSW) * RGB.x
             + (value2 + .413 * VSU + .035 * VSW) * RGB.y
             + (value3 -   value4   + .292 * VSW) * RGB.z;

	RESULT.z = (value1 - .3   * VSU + 1.25 * VSW) * RGB.x
             + (value2 -   value5   - 1.05 * VSW) * RGB.y 
             + (value3 + .886 * VSU - .203 * VSW) * RGB.z;

 	return (RESULT);
}
half Luma (half3 color)
{
	return 0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b;
}
half GetDitherMultiplier (half2 coords, half2 texelSize)
{
    return 0;
} 
half3 DitheringWorld(float2 pos, half4 color, half4 ditherColor)  
{
	return color.rgb;  
}
half3 DitheringWorldIncludingSemiTransparentPixels(float2 pos, half4 color, half4 ditherColor)  
{
	return color.rgb;  
}
half3 HorizontalDitheringWorld(float2 pos, half4 color, half4 ditherColor)  
{
	return color.rgb;  
}
half3 VerticalDitheringWorld(float2 pos, half4 color, half4 ditherColor)  
{
	return color.rgb;  
}
half3 DitheringWorldLuma(float2 pos, half4 color, half4 ditherColor)  
{
	return color.rgb;  
}
half3 HorizontalDitheringWorldLuma(float2 pos, half4 color, half4 ditherColor)  
{
	return color.rgb;  
}
half3 VerticalDitheringWorldLuma(float2 pos, half4 color, half4 ditherColor)  
{
	return color.rgb;  
}
half3 DitheringWorldDouble(float2 pos, half4 color, half4 ditherColor, half4 ditherColorLuma)  
{
	return color.rgb;  
}
half3 HorizontalDitheringWorldDouble(float2 pos, half4 color, half4 ditherColor, half4 ditherColorLuma)  
{
	return color.rgb;  
}
half3 VerticalDitheringWorldDouble(float2 pos, half4 color, half4 ditherColor, half4 ditherColorLuma)   
{
	return color.rgb;  
}
half3 DitheringWorldTriple(float2 pos, half4 color, half4 ditherColorDarks, half4 ditherColorMids, half4 ditherColorBrights)  
{
    return color.rgb;  
}
half3 DitheringWorldQuad(float2 pos, half4 color, half4 darkColor, half4 midColor2, half4 midColor1, half4 brightColor)  
{
	return color;
}
half3 DitheringWorldQuadForGreyScaleColor(float2 pos, half4 color, half luma, half4 darkColor, half4 midColor2, half4 midColor1, half4 brightColor)  
{
    return color;
}

half3 DitheringScreen(half2 pos, half4 color, half4 ditherColor)
{
	return color.rgb; 
}

half3 DitheringTex(half2 texCoord, half2 texSize, half4 color, half4 ditherColor)  
{
	return color.rgb; 
}
half3 DitheringTexQuad(half2 texCoord, half2 texSize, half4 color, half4 darkColor, half4 midColor2, half4 midColor1, half4 brightColor)  
{
	return color.rgb; 
} 
