// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FillTransparent" {
Properties
{
	_Color ("Fill", Color) = (1,1,1,1)
}
SubShader
{
        Pass {
           
            CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma fragmentoption ARB_precision_hint_fastest
                #include "UnityCG.cginc"
               
                struct v2f
                {
                    float4  pos : SV_POSITION;
                    float2  uv : TEXCOORD0;
                };
 
                v2f vert (appdata_tan v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.texcoord.xy;
                    return o;
                }
               
                sampler2D _MainTex;
                fixed4 _Color;
 
                float4 frag(v2f i) : COLOR
                {
                // RESULT
                    float4 result;
                    result = _Color;
                    return result;
                }
            ENDCG
        }
}

}