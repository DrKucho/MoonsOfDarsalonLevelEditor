// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "_Kucho/Sprites-Default HSV (Green)"
{
     Properties
     {
		 _MainTex ("Sprite Texture", 2D) = "white" {}
		 _Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
         _HueShift("HueShift", Range(-180, 180)) = 0
         _Sat("Saturation", Range(0,2)) = 1
         _Val("Value", Range (0,2)) = 1
         _Alpha ("Alpha", Range(0,1)) = 1
     }
 
 
 
 
 
     SubShader
     {
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Fog { Mode Off }
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile _ PIXELSNAP_ON
			#include "UnityCG.cginc"
			
			sampler2D _MainTex;
            half4 _Color;
			half _HueShift;
			half _Sat;
			half _Val;
            half _Alpha;







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
		
			half3 shift_col(half3 RGB, half3 shift)
	        {
	        	half3 RESULT = half3(RGB);
	        	half VSU = shift.z*shift.y*cos(shift.x * 0.0174532925); //3.14159265/180);
	       		half VSW = shift.z*shift.y*sin(shift.x * 0.0174532925); //3.14159265/180);
	            RESULT.x = (.299*shift.z+.701*VSU+.168*VSW)*RGB.x
	                     + (.587*shift.z-.587*VSU+.330*VSW)*RGB.y
	                     + (.114*shift.z-.114*VSU-.497*VSW)*RGB.z;

//	            RESULT.y = (.299*shift.z-.299*VSU-.328*VSW)*RGB.x
//	                     + (.587*shift.z+.413*VSU+.035*VSW)*RGB.y
//	                     + (.114*shift.z-.114*VSU+.292*VSW)*RGB.z;
//
//	    		RESULT.z = (.299*shift.z-.3*VSU+1.25*VSW)*RGB.x
//	                     + (.587*shift.z-.588*VSU-1.05*VSW)*RGB.y
//	                     + (.114*shift.z+.886*VSU-.203*VSW)*RGB.z;

				RESULT.y = RGB.y;
				RESULT.z = RGB.z;


	         	return (RESULT);
	        }
	         			
			half4 SampleSpriteTexture (half2 uv)
	        {
				half4 color = tex2D (_MainTex, uv);
				return color;
			}
			
			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.color = IN.color * _Color;
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
                col.rgb = shift_col(col, half3(_HueShift, _Sat, _Val));
                col.a *= _Alpha;
             
                return col;
            }
		ENDCG
		}
	}
     Fallback "Particles/Alpha Blended"
 }