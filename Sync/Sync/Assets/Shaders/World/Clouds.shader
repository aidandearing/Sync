Shader "World/Clouds/Alpha" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_Sun ("Sun", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGBA)", 2D) = "white" {}
		_Scattering("Scattering", Float) = 1.0
		_Density("Scattering Density", Float) = 1
		// Alpha Mode properties
		[KeywordEnum(Alpha, Stepped, HighPass, LowPass, Threshold, Range, RangeNormalized, Exponential)] _AlphaRangeMode("Alpha Mode", Float) = 0
		_AlphaRange("Alpha Range", Float) = 1
		_AlphaThreshold("Alpha Threshold", Float) = 0
	}
	SubShader {
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		Fog { Mode Off }
		Blend SrcAlpha OneMinusSrcAlpha
		//Cull Back
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert alpha
		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		#include "UnityCG.cginc"

		sampler2D _MainTex;

		struct Input 
		{
			float2 uv_MainTex;
		};

		fixed4 _Color;
		fixed4 _Sun;

		float _Scattering;

		float _Density;

		float _AlphaRangeMode;
		float _AlphaBlit;
		float _AlphaRange;
		float _AlphaThreshold;

		float random(float2 p) // Version 2
		{
			// e^pi (Gelfond's constant)
			// 2^sqrt(2) (Gelfond–Schneider constant)
			float2 r = float2(23.14069263277926, 2.665144142690225);
			//return fract( cos( mod( 12345678., 256. * dot(p,r) ) ) ); // ver1
			return frac(cos(dot(p, r)) * 123456.); // ver2
		}

		void surf (Input IN, inout SurfaceOutput o) {
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex + float2(random(IN.uv_MainTex), random(IN.uv_MainTex)) * _Scattering) * _Color;

			float alpha = c.a;

			// Stepped
			// Stepped reduces the number of alpha values to a specified range
			// 0 = well please don't
			// 1 = alpha will either be 1 or 0
			// 2 = alpha will have 3 steps, 0, 0.5, 1
			// etc.
			if (_AlphaRangeMode == 1)
			{
				alpha = floor(alpha * _AlphaRange) / _AlphaRange;
			}
			// High Pass
			// Masking will be applied to all pixels not greater than a specific mask alpha as specified by alpha threshold
			else if (_AlphaRangeMode == 2)
			{
				alpha = (alpha > _AlphaThreshold) ? alpha : alpha * _AlphaRange;
			}
			// Low Pass
			// Masking will be applied to all pixels greater than a specific mask alpha as specified by alpha threshold
			else if (_AlphaRangeMode == 3)
			{
				alpha = (alpha < _AlphaThreshold) ? alpha : alpha * _AlphaRange;
			}
			// Threshold
			// Masking will be applied to all pixels not greater than a specific mask alpha as specified by alpha threshold
			// But they will not be completely masked
			// All alpha values within alpha range of the alpha threshold will be 'soft' faded, that is as alpha falls away from the mask threshold it fades towards 0 along the range specified
			else if (_AlphaRangeMode == 4)
			{
				if (_AlphaRange > 0)
				{
					float t = (_AlphaThreshold - alpha);
					t = 1 - t / _AlphaRange;
					alpha = (alpha > _AlphaThreshold) ? 1 : lerp(0, 1, t);
				}
				else
					alpha = (alpha > _AlphaThreshold) ? 1 : 0;
			}
			// Range
			// Masking will be applied to all pixels not within a specific range of alphas from alpha range to alpha threshold
			else if (_AlphaRangeMode == 5)
			{
				alpha = (_AlphaRange - alpha < _AlphaThreshold) ? 1 : 0;
			}
			// Range normalized
			// Masking will be applied to all pixels not within a specific range of alphas from alpha range to alpha threshold
			// As well all non-masked pixels will have their alpha modified so that those closes to the alpha threshold are fully opaque and as they move toward alpha range they fade to 0
			else if (_AlphaRangeMode == 6)
			{
				float t = alpha - _AlphaRange;
				alpha = (t < _AlphaThreshold) ? t / (_AlphaThreshold * 2) : 0;
			}
			// Exponential
			// Modifies the alpha mask using an exponential function.
			else if (_AlphaRangeMode == 7)
			{
				alpha = pow(alpha, _AlphaRange);
			}

			float density = pow(1.0 - alpha, _Density);

			o.Albedo = c.rgb;// _Sun.rgb;
			o.Emission = (density * _Sun.rgb);
			o.Alpha = max(min(alpha,1),0);
		}
		ENDCG
	}
	FallBack "Transparent"
}
