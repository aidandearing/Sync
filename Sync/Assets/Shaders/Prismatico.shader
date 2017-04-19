Shader "Characters/Prismatico"
{
	Properties
	{
		_Colour("Colour", Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
		_Detail("Detail", 2D) = "gray" {}
		_Metallic("Metallic", Range(0,1)) = 1.0
		_Smoothness("Smoothness", Range(0,1)) = 1.0
		_RimExpression("Rim Expression", Range(0,1)) = 1
		_RimColour("Rim Colour", Color) = (1,1,1,1)
		_RimPower("Rim Power", Float) = 1
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(.002, 0.03)) = .005
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface surf Standard
		#include "UnityCG.cginc"

		struct Input
		{
			float2 uv_MainTex;
			float4 screenPos;
			float3 worldPos;
			float3 viewDir;
		};

		sampler2D _MainTex;
		//float4 _MainTex_ST;
		sampler2D _Detail;
		float4 _Detail_ST;

		float _Metallic;
		float _Smoothness;

		half4 _Colour;

		half4 _RimColour;
		half _RimExpression;
		float _RimPower;

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			float2 screenUV = IN.screenPos.xy / IN.screenPos.w;

			half4 texUVColour = tex2D(_MainTex, IN.uv_MainTex) * _Colour;
			half4 texScreenColour = tex2D(_Detail, screenUV * _Detail_ST.xy + _Detail_ST.zw);

			o.Albedo = lerp(texScreenColour.rgb, texUVColour.rgb, texUVColour.a);

			half rimFactor = pow(1.0 - saturate(dot(normalize(IN.viewDir), o.Normal)), _RimPower);
			o.Emission = lerp(lerp(texScreenColour.rgb, float3(0, 0, 0), texUVColour.a), _RimColour, min(1 - texUVColour.a, min(rimFactor, _RimExpression)));

			o.Metallic = lerp(0, _Metallic, texUVColour.a);
			o.Smoothness = lerp(0, _Smoothness, texUVColour.a);
		}
		ENDCG

		UsePass "Toon/Basic Outline/OUTLINE"
	}
	Fallback "Diffuse"
}