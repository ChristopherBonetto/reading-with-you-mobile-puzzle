Shader "Custom/Diffuse-Reflection"
{
	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_ReflectColor("Reflection Color", Color) = (0,0,0,0.5)
		_MainTex("Base (RGB) RefStrength (A)", 2D) = "white" {}
	}

	SubShader{
		LOD 200
		Tags { "RenderType" = "Opaque" }

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;

		fixed4 _Color;
		fixed4 _ReflectColor;

		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 c = tex * _Color;
			o.Albedo = c.rgb;

			o.Emission = tex.rgb * _ReflectColor.rgb;
			o.Alpha = tex.a * _ReflectColor.a;
		}
	ENDCG
	}

	FallBack "Legacy Shaders/Reflective/VertexLit"
}
