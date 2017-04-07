Shader "Hidden/GlobalFog" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "black" {}
	_Sun("Sun", Color) = (1,1,1,1)
	_SunDirection("Sun Direction", Vector) = (0,0,1,0)
	_SunDistance("Sun Distance", Float) = 149597871000
	_Volume("Atmospheric Volume", Float) = 12000
	_ScatteringRayleigh("Rayleigh Scattering Factors", Vector) = (0.03537,0.09418,0.34869,0)
	_ScatteringMie("Mie Scattering Factors", Vector) = (0,0,0,0)
	_SkyboxExpression("Skybox Expression", Float) = 0.5
	_SunScatterFactor("Sun Atmospheric Scattering", Float) = 0.5
	_HorizonIntensity("Horizon Scattering Intensity", Float) = 0.2
	_ZenithIntensity("Zenith Scattering Intensity", Float) = 2.0
}

CGINCLUDE
	#include "UnityCG.cginc"

	uniform sampler2D _MainTex;
	uniform sampler2D_float _CameraDepthTexture;

	float _Volume;
	half4 _ScatteringRayleigh;
	half4 _ScatteringMie;
	float _SkyboxExpression;
	float _HorizonIntensity;
	float _ZenithIntensity;

	half4 _Sun;
	float _SunScatterFactor;
	float4 _SunDirection;
	float _SunDistance;
	
	// x = fog height
	// y = FdotC (CameraY-FogHeight)
	// z = k (FdotC > 0.0)
	// w = a/2
	uniform float4 _HeightParams;
	
	// x = start distance
	uniform float4 _DistanceParams;
	
	int4 _SceneFogMode; // x = fog mode, y = use radial flag
	float4 _SceneFogParams;
	#ifndef UNITY_APPLY_FOG
	half4 unity_FogColor;
	half4 unity_FogDensity;
	#endif	

	uniform float4 _MainTex_TexelSize;
	
	// for fast world space reconstruction
	uniform float4x4 _FrustumCornersWS;
	uniform float4 _CameraWS;

	struct appdata_fog
	{
		float4 vertex : POSITION;
		half2 texcoord : TEXCOORD0;
	};

	struct v2f {
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 uv_depth : TEXCOORD1;
		float4 interpolatedRay : TEXCOORD2;
	};
	
	v2f vert (appdata_fog v)
	{
		v2f o;
		v.vertex.z = 0.1;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = v.texcoord.xy;
		o.uv_depth = v.texcoord.xy;
		
		#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.uv.y = 1-o.uv.y;
		#endif				
		
		int frustumIndex = v.texcoord.x + (2 * o.uv.y);
		o.interpolatedRay = _FrustumCornersWS[frustumIndex];
		o.interpolatedRay.w = frustumIndex;
		
		return o;
	}
	
	// Applies one of standard fog formulas, given fog coordinate (i.e. distance)
	half ComputeFogFactor (float coord)
	{
		float fogFac = 0.0;
		if (_SceneFogMode.x == 1) // linear
		{
			// factor = (end-z)/(end-start) = z * (-1/(end-start)) + (end/(end-start))
			fogFac = coord * _SceneFogParams.z + _SceneFogParams.w;
		}
		if (_SceneFogMode.x == 2) // exp
		{
			// factor = exp(-density*z)
			fogFac = _SceneFogParams.y * coord; fogFac = exp2(-fogFac);
		}
		if (_SceneFogMode.x == 3) // exp2
		{
			// factor = exp(-(density*z)^2)
			fogFac = _SceneFogParams.x * coord; fogFac = exp2(-fogFac*fogFac);
		}
		return saturate(fogFac);
	}

	// Distance-based fog
	float ComputeDistance (float3 camDir, float zdepth)
	{
		float dist; 
		if (_SceneFogMode.y == 1)
			dist = length(camDir);
		else
			dist = zdepth * _ProjectionParams.z;
		// Built-in fog starts at near plane, so match that by
		// subtracting the near value. Not a perfect approximation
		// if near plane is very large, but good enough.
		dist -= _ProjectionParams.y;
		return dist;
	}

	// Linear half-space fog, from https://www.terathon.com/lengyel/Lengyel-UnifiedFog.pdf
	float ComputeHalfSpace (float3 wsDir)
	{
		float3 wpos = _CameraWS + wsDir;
		float FH = _HeightParams.x;
		float3 C = _CameraWS;
		float3 V = wsDir;
		float3 P = wpos;
		float3 aV = _HeightParams.w * V;
		float FdotC = _HeightParams.y;
		float k = _HeightParams.z;
		float FdotP = P.y-FH;
		float FdotV = wsDir.y;
		float c1 = k * (FdotP + FdotC);
		float c2 = (1-2*k) * FdotP;
		float g = min(c2, 0.0);
		g = -length(aV) * (c1 - g * g / abs(FdotV+1.0e-5f));
		return g;
	}

	half4 ComputeFog (v2f IN, bool distance, bool height) : SV_Target
	{
		half4 sceneColor = tex2D(_MainTex, UnityStereoTransformScreenSpaceTex(IN.uv));
		half4 colour;
		
		// Reconstruct world space position & direction
		// towards this screen pixel.
		float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(IN.uv_depth));
		float dpth = Linear01Depth(rawDepth);
		float4 wsDir = dpth * IN.interpolatedRay;
		float4 wsPos = _CameraWS + wsDir;

		// HEIGHT FACTOR
		// Height factor is a representation of how high in the air the current pixel is in world coordinates
		half heightFactor = max(0, min(wsPos.y / _HeightParams.x, 1));
		//half heightFactor = 1;

		// POSITION FACTOR
		// Position factor is a representation of how deep into the atmospheric volume the current pixel is in world coordinates
		//half posFactor = 1 - min(0, max(dpth / _Volume, 1));
		half posFactor = 1;

		// HALO FACTOR
		// Halo factor is a representation of how directionally aligned the ray from camera to the pixel is with the sun's inverted direction, modified by Sun scatter factor
		// Where 0 is no sun scatter present, and 1 is full sun scatter present (looking directly at the sun)
		float sunDir = dot(normalize(wsDir), -_SunDirection);
		half haloFactor = pow((sunDir + 1) / 2, _SunScatterFactor);

		// SCATTER FACTOR
		// Scatter factor is a representation of how far through the atmosphere light had to travel to get to any point of fog
		// Where 0 is no distance at all, and 1 is traveling as far as the radius of the atmosphere
		float4 originCore = _CameraWS + float4(0, _Volume * -0.50, 0, 0);
		float4 deltaFromCore = wsPos - originCore;
		float deltaLength = length(deltaFromCore);
		deltaLength = (deltaLength > _Volume) ? _Volume : deltaLength;
		float theta = dot(normalize(deltaFromCore), normalize(originCore - (_SunDirection * _SunDistance)));
		float scatterFactor = pow(_Volume * _Volume + deltaLength * deltaLength - 2 * _Volume * deltaLength * cos(theta), 0.5) / _Volume;// , 1 / _SunScatterFactor);
		float incident = dot(normalize(wsDir), float3(0, 1, 0));

		float d = length(wsPos - _CameraWS);

		float f = haloFactor * incident;

		// Scattering Function
		// Implemented with much consideration of this document in particular
		// http://www.geo.mtu.edu/~scarn/teaching/GE4250/scattering_lecture_slides.pdf
		// 8 * pi^4 ~= 779.27009538
		// N = 3*10^19cm^-3 in metres = 300000000000000000
		// 779.27009538 * 300000000000000000 = 233781028614000000000
		// polarizability of air (p) = 1.000271375 = 1.000271375 * 1.000271375 = 1.000542823644390625
		// 8 * pi^4 * N * p^2 = 233781028614000000000 * 1.000542823644390625 = 233907930483941640463.71834375
		// wavelength (w) = 0.0007 = 0.0007 * 0.0007 * 0.0007 * 0.0007 = 0.0000000000002401
		//colour.r = _Sun.r * (233907930483941640463.71834375 * (0.0000000000002401 * (d * d))) * (1 + pow(cos(incident), 2));
		//_Sun * (1 / _ScatteringRayleigh) * pow(1 - saturate(incident), _SunScatterFactor) * _HorizonIntensity;
		float4 horizon = _Sun * (1 / _ScatteringRayleigh) * f;
		//_Sun * _ScatteringRayleigh * (1 - (incident + 1) / 2) * _ZenithIntensity;
		float4 zenith = _Sun * _ScatteringRayleigh * f;
		colour = zenith;// lerp(horizon, zenith, f);
		// 0.00055 = 0.00000000000009150625
		//colour.g = _Sun.g * (233907930483941640463.71834375 * (0.00000000000009150625 * (d * d))) * (1 + pow(cos(incident), 2));
		//colour.g = _Sun.g * (1 + pow(cos(incident), 2));
		// 0.00046 = 0.00000000000004477456
		//colour.b = _Sun.b * (233907930483941640463.71834375 * (0.00000000000004477456 * (d * d))) * (1 + pow(cos(incident), 2));
		//colour.b = _Sun.b * (1 + pow(cos(incident), 2));
		//colour.a = 1;

		//haloFactor = pow(1 - scatterFactor, _SunScatterFactor);
		//haloFactor = saturate(pow(scatterFactor, _SunScatterFactor));

		// Compute fog distance
		float g = _DistanceParams.x;
		if (distance)
		{
			posFactor = ComputeDistance(wsDir, dpth);
			g += posFactor;
		}
		if (height)
		{
			//heightFactor = ;
			g += ComputeHalfSpace(wsDir);
		}

		// FOG FACTOR
		// Unity's default fog factor
		// Compute fog amount
		half fogFactor = ComputeFogFactor(max(0.0, g));

		// Skybox Fog
		if (dpth == _DistanceParams.y)
		{
			// SUN SCATTERING (Bloom around sun)
			fogFactor = 0;// min(1, (1 - haloFactor * _SkyboxExpression));
		}
		else
		{
			fogFactor = fogFactor;// max(fogFactor, heightFactor);
			//haloFactor = pow(scatterFactor, _SunScatterFactor);
		}

		//half factor = max(haloFactor, heightFactor);
		//half3 scatteringFactor;
		//scatteringFactor.r = pow(1.0 - pow(1.0 - factor, 2.0), 0.25);
		//scatteringFactor.g = pow(factor, 0.5) * 0.5 + pow(factor, 2.0) * 0.5;
		//scatteringFactor.b = max(pow(1.0 - (factor * factor), 0.5) * 0.25, 1.0 - pow(1.0 - (factor * factor), 0.5));

		//half4 sun;
		half scatterFactorPow = pow(scatterFactor, _SunScatterFactor);

		//colour.r = _Sun.r * scatterFactor * _ScatteringRayleigh.r + (_Sun.r * (1 - scatterFactorPow) / _ScatteringRayleigh.r);
		//colour.g = _Sun.g * scatterFactor * _ScatteringRayleigh.g + (_Sun.g * (1 - scatterFactorPow) / _ScatteringRayleigh.g);
		//colour.b = _Sun.b * scatterFactor * _ScatteringRayleigh.b + (_Sun.b * (1 - scatterFactorPow) / _ScatteringRayleigh.b);
		//colour.a = 1;

		//colour = sun;// lerp(unity_FogColor, sun, haloFactor);

		// Lerp between fog color & original scene color
		// by fog amount
		return lerp (colour, sceneColor, fogFactor);
	}
ENDCG

SubShader
{
	ZTest Always Cull Off ZWrite Off Fog { Mode Off }

	// 0: distance + height
	Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		half4 frag (v2f i) : SV_Target { return ComputeFog (i, true, true); }
		ENDCG
	}
	// 1: distance
	Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		half4 frag (v2f i) : SV_Target { return ComputeFog (i, true, false); }
		ENDCG
	}
	// 2: height
	Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		half4 frag (v2f i) : SV_Target { return ComputeFog (i, false, true); }
		ENDCG
	}
}

Fallback off

}
