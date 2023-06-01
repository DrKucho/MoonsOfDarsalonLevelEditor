// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*

Fast, low quality blur.

*/

Shader "_Kucho/Light2D/Kucho Fast Blur Light2D" {
Properties {
	_Distance ("Distance", Float) = 4 // blur distance in pixels
}

SubShader {
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
	LOD 100
	
	ZWrite Off
	//Blend SrcAlpha OneMinusSrcAlpha 

	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half2 texcoord : TEXCOORD0;
				half2 dist : TEXCOORD1;
			};

			uniform sampler2D _MainTex;
			uniform half _Distance;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				half2 dist = _Distance*(1.0/_ScreenParams.xy);
				o.dist = half2(dist.x, dist.y*0.707);
				return o;
			}

			half4 frag (v2f i) : COLOR
			{
				half2 dists[8] = 
				{ 
					half2(i.dist.x, 0), half2(-i.dist.x, 0), half2(0, i.dist.x), half2(0, -i.dist.x), 
					half2(i.dist.y, i.dist.y), half2(i.dist.y, -i.dist.y),
					half2(-i.dist.y, i.dist.y), half2(-i.dist.y, -i.dist.y)
				}; 

				half4 sum = 0;
				
				sum += tex2D(_MainTex, i.texcoord); // el pixel central
                
				//sum += tex2D(_MainTex, i.texcoord + dists[0]) * 4; // dcha
				//sum += tex2D(_MainTex, i.texcoord + dists[1]) * 4; // izq
				//sum += tex2D(_MainTex, i.texcoord + dists[2]) * 4; // arriba
				//sum += tex2D(_MainTex, i.texcoord + dists[3]) * 4; // abajo
                
				sum += tex2D(_MainTex, i.texcoord + dists[4]); // diago 1
				sum += tex2D(_MainTex, i.texcoord + dists[5]); // diago 2
				sum += tex2D(_MainTex, i.texcoord + dists[6]); // diago 3
				sum += tex2D(_MainTex, i.texcoord + dists[7]); // diago 4

				half4 tex = sum/5;
                
				return tex;
			}
		ENDCG
	}
}

}
