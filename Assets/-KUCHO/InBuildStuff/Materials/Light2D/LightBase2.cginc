
#ifndef LIGHT_BASE_INCLUDED
#define LIGHT_BASE_INCLUDED

#ifndef PATH_TRACKING_SAMPLES
#define PATH_TRACKING_SAMPLES 1
#endif

#pragma glsl_no_auto_normalization

struct light2d_fixed_data_t {
	float4 vertex : POSITION;
	float2 texcoord : TEXCOORD0;
	float4 color : COLOR0;
	float2 texcoord1 : TEXCOORD1;
};

struct light2d_fixed_v2f {
	float4 vertex : SV_POSITION;
	half2 texcoord : TEXCOORD0;
	half4 color : COLOR0;
	#ifdef PERSPECTIVE_CAMERA
	half2 texcoord1 : TEXCOORD1;
	float4 projVertex : COLOR1;
	float zDistance : TEXCOORD2;
	#else
	float2 thisPos : TEXCOORD2; 
	float2 centerPos : TEXCOORD1; 
	#endif
	half2 aspect : TEXCOORD3;
};
			
uniform sampler2D _ObstacleTex;
uniform sampler2D _MainTex;
uniform float _ObstacleMul;
uniform half _EmissionColorMul;
uniform half _Offset; 
uniform half _GlobalObstacleMultiplier; // KUCHO HACK

#if UNITY_UV_STARTS_AT_TOP
#define Y_MUL -1
#else
#define Y_MUL 1
#endif

light2d_fixed_v2f light2d_fixed_vert (light2d_fixed_data_t v)
{
	light2d_fixed_v2f o;
	o.vertex = v.vertex;
	return o;
}

half4 light2_fixed_frag (light2d_fixed_v2f i) : COLOR
{
	return 1;
}

#endif