
Shader "_Kucho/Terrain/MOD Background Terrain (Gray to FourColor And Proyected Shadows)"
{
	Properties
	{
		_MainTex ("Sprite Texture", 2D) = "white" {}
		_AlphaTex ("Alpha Tex", 2D) = "white" {}
		_AlphaTex2 ("Alpha Tex2", 2D) = "white" {}
		_TextAspectRatio ("Sprite Texture AspectRatio", float) = 1
		_TexSize ("Sprite Tex Size", Vector) = (1000,1000,0,0)

        _PreGain("Don't Touch Must be 1", Range (1,1)) = 1 
        _PreShift("Don't Touch Must be 0", Range (0,0)) = 0 

        
        _BrightColor ("Useless In Level Editor", Color) = (1,1,1,1)
		_Mid1Color ("Useless In Level Editor", Color) = (1,1,1,1)
		_Mid2Color ("Useless In Level Editor", Color) = (1,1,1,1)
		_DarkColor ("Useless In Level Editor", Color) = (0,0,0,0)
        _nothing ("", int) = 0 // spacer
        _DitherColorBrights ("Useless In Level Editor", Color) = (0,0,0,0)
        _DitherColorMids1 ("Useless In Level Editor", Color) = (0,0,0,0)
        _DitherColorMids2 ("Useless In Level Editor", Color) = (0,0,0,0)
        _DitherColorDarks ("Useless In Level Editor", Color) = (0,0,0,0)
        

        _AllBorderShadowsIntensity("Useless In Level Editor", Range (0,1)) = 0.3 

        _Top_Distance ("Useless In Level Editor", Range(0,10)) = 0.002 
        _Top_Intensity ("Useless In Level Editor", Range(0,10)) = 1

        _Bottom_Distance ("Useless In Level Editor", Range(0,10)) = 0.002 
        _Bottom_Intensity ("Useless In Level Editor", Range(0,10)) = 1

        _Side_Distance ("Useless In Level Editor", Range(0,10)) = 0.002 
        _Side_Intensity ("Useless In Level Editor", Range(0,10)) = 1

        _General_Clamp("Useless In Level Editor", Range(0,1)) = 0

        _Angle ("Useless In Level Editor", Range (-360,360)) = 0
        _AllProjectedShadowsIntensity("Useless In Level Editor", Range (0,1)) = 0.3 

        _Projected_Top_Distance("Useless In Level Editor", Range(0,10)) = 0.002
        _Projected_Top_Intensity ("Useless In Level Editor", Range(0,1)) = 1
        _Projected_Diagonal_Distance ("Useless In Level Editor", Range(0,10)) = 4 
        _Projected_Diagonal_Intensity ("Useless In Level Editor", Range(0,1)) = 1
        _Projected_Bottom_Distance ("Useless In Level Editor", Range(0,10)) = 0.002 
        _Projected_Bottom_Intensity ("Useless In Level Editor", Range(0,1)) = 1
        _Projected_Directional_Distance ("Useless In Level Editor", Range(0,10)) = 0.002 
        _Projected_Directional_Intensity ("Useless In Level Editor", Range(0,1)) = 1
        _Projected_Directional_DissapearAtNight ("Useless In Level Editor", Range(0,1)) = 1
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
				#pragma target 3.0
				half _Long_Steps;

				#pragma vertex Vert
				#pragma fragment Frag
				#pragma multi_compile DUMMY PIXELSNAP_ON
				#include "UnityCG.cginc"
				#include "Assets/-KUCHO/Color/Shaders/KuchoCGHelper.cginc"  

				sampler2D _MainTex;
				sampler2D _AlphaTex;
				sampler2D _AlphaTex2;
				float4 	 _CameraPos_MapSize;

				half 	 _TextAspectRatio;
				float4	 _TexSize;
				float4   _MainTex_TexelSize;
                
                half    _PreGain;
                half    _PreShift;

			struct v2f
			{
				float4 pos    : SV_POSITION;
				half4 color : Color;
				float2 texcoord0 : TEXCOORD0;
				float2 worldPos : TEXCOORD1; 
				float2 directionalDist	: TEXCOORD2;
				float2 diagonalDist : TEXCOORD3;
			};

			void Vert(posAndCoord IN, out v2f OUT)  
			{
				OUT.pos    = UnityObjectToClipPos(IN.pos);
				OUT.color     = IN.color;
				OUT.texcoord0 = IN.texcoord0;
				OUT.worldPos = mul (unity_ObjectToWorld, IN.pos);
			}
			void Frag(v2f i, out half4 o:COLOR0) 
			{ 
				float2 pixelPos = i.texcoord0;
				half4 alphaTex; 
				half a  = tex2D(_MainTex, pixelPos).a; 
                o.rgb = a * (_PreGain ) + (_PreShift );
				o.a = step(0.003, a); 
				half alpha = o.a;
				o.rgba *= i.color;
				o.rgb *= 0.55; // compensacion a ojimetro
				o.a = alpha;   
				o.rgb *= o.a;  
				  
			} 
			ENDCG  
		}
	}
}
