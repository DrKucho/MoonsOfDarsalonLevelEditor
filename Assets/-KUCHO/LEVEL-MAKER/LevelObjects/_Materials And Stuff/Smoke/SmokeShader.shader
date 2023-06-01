// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Kucho/SmokeShader" {
 
     Properties {
         _MainTex ("Texture", 2D) = "white" {}
         _Color ("Tint", Color) = (1,1,1,1)
         _Contrast("_Contrast", Range (0,2)) = 1
         _Alpha ("Alpha", Range(0,2)) = 1
     }
 
     SubShader {
         Tags
         {
	         "Queue"="Transparent"
	         "IgnoreProjector"="True"
	         "RenderType" = "Transparent"
         }
         ZWrite Off         
        //Blend One One
         //Blend SrcAlpha OneMinusSrcAlpha //hace extraños 
        //Blend One  //  parece solo aclarar, si tiens color oscuro se hace transparente
        //Blend OneMinusDstColor One // parece solo aclarar, si tiens color oscuro se hace transparente
        //Blend DstColor Zero // extraño efecto mascara con el mesh entero, hace desaparecer la roca y se ve el cielo que hay detras
        //Blend DstColor SrcColor // extraño efecto mascara con el mesh entero, hace desaparecer la roca y se ve el cielo que hay detras
         Blend One OneMinusSrcAlpha // muy oscuro sobre roca y mas claro sobre cielo
         Cull Off
 
         Pass
         {
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma target 2.0
             #include "UnityCG.cginc"

            half _Contrast;
            half _Alpha;
  			half4 _Color;

  			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

             struct v2f {
                 float4  pos : SV_POSITION;
                 float4 color    : COLOR; 
                 float2  uv : TEXCOORD0;
                 float2 worldPos : TEXCOORD1;
             };
 
             float4 _MainTex_ST;
 
             v2f vert (appdata_t v)
             {
                 v2f o;
                 o.pos = UnityObjectToClipPos (v.vertex);
                 o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                 o.color = v.color * _Color;
                 return o;
             }
 
             sampler2D _MainTex;
 
             half4 frag(v2f i) : COLOR
             {
                 half4 col = tex2D(_MainTex, i.uv) * i.color;

                 col *= _Color;
                 col.rgb *= col.a;
                 col.rgb = (col.rgb - 0.5f) * (_Contrast) + 0.5f;
                 col.rgb *= col.a;
                 
                 col.a *= _Alpha;
                 return col;
             }
 
             ENDCG
         }
     }
     Fallback "Particles/Alpha Blended" 
 }