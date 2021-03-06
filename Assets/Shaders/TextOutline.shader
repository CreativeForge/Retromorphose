﻿Shader "Mobile/GUI (Colored)" {
	Properties {
		_MainTex ("Base", 2D) = "white" {}
		_MainColor ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_OutlineColor ("Outline Color", Color) = (0.5,0.5,0.5,1.0)
		_Outline ("Outline width", Float) = 0.01
	}
   
	CGINCLUDE
 
		#include "UnityCG.cginc"
 
		sampler2D _MainTex;
		fixed4 _MainColor;
	   
		half4 _MainTex_ST;
					   
		struct v2f {
			half4 pos : SV_POSITION;
			half2 uv : TEXCOORD0;
			fixed4 vertexColor : COLOR;
		};
 
		v2f vert(appdata_full v) {
			v2f o;
		   
			o.pos = mul (UNITY_MATRIX_MVP, v.vertex);  
			o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.vertexColor = v.color * _MainColor;
				   
			return o;
		}
	   
		fixed4 frag( v2f i ) : COLOR { 
			return tex2D (_MainTex, i.uv.xy) * i.vertexColor;
		}
   
	ENDCG
   
	SubShader {
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent+100"}
		Cull Off
		Lighting Off
		ZWrite Off
		ZTest Always
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
	   
		Pass {

			Cull Front
            ZWrite On
			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma glsl_no_auto_normalization


			ENDCG
		 
		}
			   
	}
	FallBack Off
}
