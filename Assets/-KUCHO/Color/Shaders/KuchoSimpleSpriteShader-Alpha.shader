// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Kucho/SimpleSprites_Alpha" {
 
     Properties {
         _MainTex ("Texture", 2D) = "white" {}
         _Alpha ("Alpha", Range(0,1)) = 1
     }
 
     SubShader {
         Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType" = "Transparent" }
         ZWrite Off
         Blend SrcAlpha OneMinusSrcAlpha
         Cull Off
 
         Pass
         {
             CGPROGRAM
             #pragma vertex vert
             #pragma fragment frag
             #pragma target 2.0
             #include "UnityCG.cginc"
 
             float _Alpha;
  
             struct v2f {
                 float4  pos : SV_POSITION;
                 float2  uv : TEXCOORD0;
             };
 
             float4 _MainTex_ST;
 
             v2f vert (appdata_base v)
             {
                 v2f o;
                 o.pos = UnityObjectToClipPos (v.vertex);
                 o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                 return o;
             }
 
             sampler2D _MainTex;
 
             half4 frag(v2f i) : COLOR
             {
                 half4 col = tex2D(_MainTex, i.uv);
                 col.a *= _Alpha;
                 return col; 
             }
 
             ENDCG
         }
     }
     Fallback "Particles/Alpha Blended" 
 }