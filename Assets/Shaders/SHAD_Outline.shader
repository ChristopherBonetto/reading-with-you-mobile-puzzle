// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Outline"
{

	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_Outline("Outline Thickness", Range(1, 3)) = 1.03
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
	}

	CGINCLUDE
	#include "UnityCG.cginc"

	sampler2D _MainTex;
	half4 _MainTex_ST;

	half _Outline;
	half4 _OutlineColor;

	struct appdata {
		half4 vertex : POSITION;
		half4 uv : TEXCOORD0;
		half3 normal : NORMAL;
		fixed4 color : COLOR;
	};

	struct v2f {
		half4 pos : POSITION;
		half3 normal : NORMAL1;
		half2 uv : TEXCOORD0;
		fixed4 color : COLOR;
	};
	ENDCG

	SubShader
	{
		Tags
		{
			"RenderType" = "Opaque"
			"Queue" = "Transparent"
		}

		Pass
		{
			Name "OUTLINE"
			ZWrite off
			Cull Front
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			v2f vert(appdata v)
			{
				//float3 normal = v.normal.xyz * _Outline;

				v.vertex.xyz *= _Outline;
				
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.color = _OutlineColor;
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 o;
				o = i.color;
				return o;
			}
			ENDCG
		}

		Pass
		{
			Name "TEXTURE"

			Cull Back
			ZWrite On
			ZTest LEqual

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				return o;
			}

			fixed4 frag(v2f i) : COLOR
			{
				fixed4 o;
				o = tex2D(_MainTex, i.uv.xy);
				return o;
			}
			ENDCG
		}
	}
}
