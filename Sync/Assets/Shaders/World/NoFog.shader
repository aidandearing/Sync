Shader "World/NoFog" {
	Properties{
		_MainTex("Texture", 2D) = "white" {}
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

	sampler2D _MainTex;

	void surf(Input IN, inout SurfaceOutput OUT) 
	{
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

		OUT.Albedo = c;
		//OUT.Emission = c;
		OUT.Alpha = c.a;
	}
	ENDCG
	}
		FallBack "Transparent"
}
