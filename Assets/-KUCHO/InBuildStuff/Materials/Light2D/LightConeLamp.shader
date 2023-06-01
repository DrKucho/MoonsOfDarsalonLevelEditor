
Shader "_Kucho/Light2D/Cone Lamps" {
Properties {
	_MainTex ("Light texture", 2D) = "white" {}
	_ObstacleMul ("Obstacle Mul", Float) = 500
	_EmissionColorMul ("Emission color mul", Float) = 1
}
SubShader {	
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

	LOD 100
	Blend OneMinusDstColor One 
	Blend DstColor SrcColor
	BlendOp Max 
	Cull Off
	ZWrite Off
	Lighting Off

	Pass {  
		CGPROGRAM
			#define PATH_TRACKING_SAMPLES 30 
			#pragma target 3.0
			#pragma multi_compile ORTHOGRAPHIC_CAMERA PERSPECTIVE_CAMERA
			
			#include "UnityCG.cginc"
			#include "Assets/Plugins/KuchoFirstPass/Light2D/Resources_no/Shaders/LightBase.cginc" // all code is here
			
			#pragma vertex light2d_fixed_vert
			#pragma fragment light2_fixed_frag
		ENDCG
	}
}

Fallback "Light2D/Light 20 Points"

}