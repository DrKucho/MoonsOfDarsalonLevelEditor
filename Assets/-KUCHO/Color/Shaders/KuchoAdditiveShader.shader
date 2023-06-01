// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// unlit, vertex colour, alpha blended
// cull off
// Funciona bien con los alphas en el cielo 

Shader "_Kucho/AdditiveVertexColor" 
{
	Properties 
	{
		_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}

//		_ColorMult ("Color Mult" , Range(0,2)) = 1
//		_ColorAdd ("Color Add", Range(-1,1)) = 0
//		_AlphaMult ("Alpha Mult" , Range(0,2)) = 1
//		_AlphaAdd ("Alpha Add", Range(-1,1)) = 0

	}
	
	SubShader
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		ZWrite Off Lighting Off Cull Off Fog { Mode Off }

//		Blend DstColor Zero // Multiplicative
//		Blend DstColor SrcColor // 2x Multiplicative
//		Blend SrcAlpha OneMinusSrcAlpha // Traditional transparency
//		Blend One OneMinusSrcAlpha // Premultiplied transparency
//		Blend OneMinusDstColor One // Soft Additive
		Blend One One // Additive - funciona bien sobre el cielo al contrario que el plugin por defecto de unity
//		Blend One OneMinusSrcAlpha


		LOD 110
		
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			half4 _Color;
//			half _ColorMult;
//			half _ColorAdd;
//			half _AlphaMult;
//			half _AlphaAdd;

			struct appdata_t
			{
				half4 vertex   : POSITION;
				half4 color    : COLOR;
				half2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				half4 vertex   : SV_POSITION;
				half4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};

			half4 SampleSpriteTexture (half2 uv) 
	        {
				half4 color = tex2D (_MainTex, uv);
				return color;
			}
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.color = IN.color;
				OUT.texcoord = IN.texcoord;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT; 
			}


            half4 frag(v2f IN) : COLOR
            {
				half4 col = SampleSpriteTexture (IN.texcoord) * IN.color;
                col.rgb *= col.a;
             
                return col;
            }
			
			ENDCG
		} 
	}
}
