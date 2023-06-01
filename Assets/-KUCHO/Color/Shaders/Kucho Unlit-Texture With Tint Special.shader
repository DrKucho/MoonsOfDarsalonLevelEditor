// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'


Shader "_Kucho/Unlit-Texture With Tint Special)" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Tint (Cosmos)", Color) = (1,1,1,1)
		_ColorSpecial ("Tint Special (Atmos)", Color) = (1,1,1,1)
		_ColorSpecialAdd ("Tint Special (Atmos) Add", Range(0,5)) = 1.25
		
		 _HueShift("HueShift", Range(-180, 180)) = 0
         _Sat("Saturation", Range(0,2)) = 1
         _Val("Value", Range (0,2)) = 1
         _Contrast("_Contrast", Range (0,2)) = 1
	}
	
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag

				#include "UnityCG.cginc"
				#include "Assets/-KUCHO/Color/Shaders/KuchoCGHelper.cginc"  

				struct appdata {
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f {
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				float4 _Color;
				float4 _ColorSpecial;
				float _ColorSpecialAdd;

				half _HueShift;
				half _Sat;
				half _Val;
				half _Contrast;
			
				//v2f vert (appdata v) {
				//	v2f o;
				//	o.vertex = UnityObjectToClipPos(v.vertex);
				//	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//	return o;
				//}

				void Vert(posAndCoord IN, out posTwoCoordsAndHSVMults OUT)  
				{
					OUT.pos = UnityObjectToClipPos(IN.pos); 
					OUT.color = IN.color * _Color; // aplico el color independiente del sprite IN.color y _Color! ya que _Color es igual para todos los materiales 
					OUT.texcoord0 = TRANSFORM_TEX(IN.texcoord0, _MainTex);
					OUT = CalculateThingsForHSVFastProcessing(_HueShift, _Sat, _Val, OUT); 
				}
		

				//float4 frag (v2f i) : SV_Target {
				half4 Frag(posTwoCoordsAndHSVMults IN) : COLOR
				{
					float4 col = tex2D(_MainTex, IN.texcoord0) * _Color;

					col.rgb = ShiftHSV_Fast(col, IN); // Hue Sat And Value
					col.rgb = (col.rgb - 0.5f) * (_Contrast) + 0.5f; // contrast
					
					col += _ColorSpecial * _ColorSpecialAdd;			
					return col;
				}
			ENDCG
		}
	}
}
