Shader "World/Sky/Gradient" {
	Properties{
		_ColourZenith("Zenith Colour", Color) = (1,1,1,1)
		_EmissionZenith("Zenith Emission", Color) = (0,0,0,0)
		_ColourHorizon("Horizon Colour", Color) = (1,1,1,1)
		_EmissionZenith("Horizon Emission", Color) = (0,0,0,0)
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

		Blend SrcAlpha OneMinusSrcAlpha
		//Cull Back
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
#pragma surface surf Lambert alpha nofog
		// Use shader model 3.0 target, to get nicer looking lighting
#pragma target 3.0

#include "UnityCG.cginc"
		
	struct Input
	{
		float2 uv_MainTex;
	};

	fixed4 _ColourZenith;
	fixed4 _ColourHorizon;
	fixed4 _EmissionZenith;
	fixed4 _EmissionHorizon;

	float random(float2 p) // Version 2
	{
		// e^pi (Gelfond's constant)
		// 2^sqrt(2) (Gelfond–Schneider constant)
		float2 r = float2(23.14069263277926, 2.665144142690225);
		//return fract( cos( mod( 12345678., 256. * dot(p,r) ) ) ); // ver1
		return frac(cos(dot(p, r)) * 123456.); // ver2
	}

	void surf(Input IN, inout SurfaceOutput OUT) {
		// Albedo comes from a texture tinted by color
		float2 delta = float2(0.5, 0.5) - IN.uv_MainTex;
		float s = delta.x * delta.x + delta.y * delta.y;

		fixed4 c = lerp(_ColourZenith, _ColourHorizon, s);

		OUT.Albedo = c;
		OUT.Alpha = c.a;
		OUT.Emission = lerp(_EmissionZenith, _EmissionHorizon, s);
	}
	ENDCG
	}
		FallBack "Transparent"
}
