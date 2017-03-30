Shader "Hidden/GlobalFog" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "black" {}
	_Sun("Sun", Color) = (1,1,1,1)
	_SunDirection("Sun Direction", Vector) = (0,0,1,0)
	_SunDistance("Sun Distance", Float) = 149597871000
	_Volume("Atmospheric Volume", Float) = 100000
	_Scattering("Atmospheric Scatter (RGBA)", Vector) = (0.5,0.75,1.0,0)
	_SkyboxExpression("Skybox Expression", Float) = 0.5
	_SunScatterFactor("Sun Atmospheric Scattering", Float) = 0.5
}

CGINCLUDE

	#include "UnityCG.cginc"

	uniform sampler2D _MainTex;
	uniform sampler2D_float _CameraDepthTexture;

	float _Volume;
	half4 _Scattering;
	float _SkyboxExpression;

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

	/*float4 permute(float4 x)
	{
		return fmod(34.0 * pow(x, 2) + x, 289.0);
	}

	float2 fade(float2 t) 
	{
		return  t * t * t * (t * (t * 6 - 15) + 10);
	}

	float4 taylorInvSqrt(float4 r) 
	{
		return 1.79284291400159 - 0.85373472095314 * r;
	}

	#define DIV_289 0.00346020761245674740484429065744f

	float mod289(float x) 
	{
		return x - floor(x * DIV_289) * 289.0;
	}

	float PerlinNoise2D(float2 P)
	{
		float4 Pi = floor(P.xyxy) + float4(0.0, 0.0, 1.0, 1.0);
		float4 Pf = frac(P.xyxy) - float4(0.0, 0.0, 1.0, 1.0);

		float4 ix = Pi.xzxz;
		float4 iy = Pi.yyww;
		float4 fx = Pf.xzxz;
		float4 fy = Pf.yyww;

		float4 i = permute(permute(ix) + iy);

		float4 gx = frac(i / 41.0) * 2.0 - 1.0;
		float4 gy = abs(gx) - 0.5;
		float4 tx = floor(gx + 0.5);
		gx = gx - tx;

		float2 g00 = float2(gx.x, gy.x);
		float2 g10 = float2(gx.y, gy.y);
		float2 g01 = float2(gx.z, gy.z);
		float2 g11 = float2(gx.w, gy.w);

		float4 norm = taylorInvSqrt(float4(dot(g00, g00), dot(g01, g01), dot(g10, g10), dot(g11, g11)));
		g00 *= norm.x;
		g01 *= norm.y;
		g10 *= norm.z;
		g11 *= norm.w;

		float n00 = dot(g00, float2(fx.x, fy.x));
		float n10 = dot(g10, float2(fx.y, fy.y));
		float n01 = dot(g01, float2(fx.z, fy.z));
		float n11 = dot(g11, float2(fx.w, fy.w));

		float2 fade_xy = fade(Pf.xy);
		float2 n_x = lerp(float2(n00, n01), float2(n10, n11), fade_xy.x);
		float n_xy = lerp(n_x.x, n_x.y, fade_xy.y);
		return 2.3 * n_xy;
	}*/

	half4 ComputeFog (v2f IN, bool distance, bool height) : SV_Target
	{
		half4 sceneColor = tex2D(_MainTex, UnityStereoTransformScreenSpaceTex(IN.uv));
		
		// Reconstruct world space position & direction
		// towards this screen pixel.
		float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(IN.uv_depth));
		float dpth = Linear01Depth(rawDepth);
		float4 wsDir = dpth * IN.interpolatedRay;
		float4 wsPos = _CameraWS + wsDir;

		float noise = 1;

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
		half haloFactor = pow((dot(normalize(wsDir), -_SunDirection) + 1) / 2, _SunScatterFactor);

		// DELTA
		// Delta tells us how far through the atmosphere a given pixel of fog's light had to travel to get there, which determines how much scattering the light should recieve
		half delta = length(-_SunDirection * _SunDistance - wsPos);

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

			/*noise = 0;

			for (int i = 1; i <= 8; i++)
			{
				noise += PerlinNoise2D(IN.interpolatedRay.xy * pow(2,i) * 0.000002);
			}

			noise /= 8;

			noise = noise * 0.5 + 0.5;
			noise = min(1, max(noise, 0));*/
		}

		//return fogFac; // for debugging
		
		//float intensity = dot(_Sun, half4(0.3, 0.6, 0.1, 1));

		half4 colour;

		// FOG FACTOR
		// Unity's default fog factor
		// Compute fog amount
		half fogFactor = ComputeFogFactor(max(0.0, g));

		// Fog wants to be placed such that it is the minimum of all factors
		//fogFactor = min(heightFactor, min(posFactor, min(haloFactor, fogFactor)));

		//float pos = length(_SunDirection * _SunDistance - wsPos) / _Volume;
		//pos = min(1, max(pos, 0));

		//float posFactor = 1 - pos;

		//if (!height)
		//	fogFac = max(fogFactor, 1 - haloFactor);
		//else
		//	fogFac = max(fogFactor, posFactor);
		//colour.rgb *= intensity;

		// Skybox Fog
		if (dpth == _DistanceParams.y)
			fogFactor = min(1, (1 - haloFactor * _SkyboxExpression));
		else
			fogFactor = max(fogFactor, heightFactor);
		//{
		//	if (height)
		//		fogFactor = heightFactor;
		//}
		half factor = max(haloFactor, heightFactor);
		half3 scatteringFactor;
		scatteringFactor.r = pow(1.0 - pow(1.0 - factor, 2.0), 0.25);// pow(1.0 - pow(1.0 - factor, 2.0), 0.25);
		scatteringFactor.g = pow(factor, 0.5) * 0.5 + pow(factor, 2.0) * 0.5;// factor;// pow(1.0 - pow(1.0 - factor, 0.5), 0.25);
		scatteringFactor.b = max(pow(1.0 - (factor * factor), 0.5) * 0.25, 1.0 - pow(1.0 - (factor * factor), 0.5));// max(pow(1 - (factor * factor), 0.9) * 0.5, 1 - pow(1 - (factor * factor), 0.5));

		half3 rayleighFactor;
		rayleighFactor.r = 0.05;
		rayleighFactor.g = 0.1;
		rayleighFactor.b = 0.2;

		half4 sun;
		sun.r = _Sun.r * scatteringFactor.r;
		sun.g = _Sun.g * scatteringFactor.g;
		sun.b = _Sun.b * scatteringFactor.b;
		//sun.r = _Sun.r * pow(max(haloFactor, heightFactor), _Scattering.x);
		//sun.g = _Sun.g * pow(max(haloFactor, heightFactor), _Scattering.y);
		//sun.b = _Sun.b * pow(max(haloFactor, heightFactor), _Scattering.z);
		sun.a = 1;// _Sun.a * pow(max(haloFactor, heightFactor), _Scattering.w);
		/*if (!height)
		{
			sun.r = _Sun.r * max(pow(haloFactor, _Scattering.x), pow(posFactor, _Scattering.x));
			sun.g = _Sun.g * max(pow(haloFactor, _Scattering.y), pow(posFactor, _Scattering.y));
			sun.b = _Sun.b * max(pow(haloFactor, _Scattering.z), pow(posFactor, _Scattering.z));
			sun.a = _Sun.a * max(pow(sunFactor, _Scattering.w), pow(posFactor, _Scattering.w));
		}
		else
		{
			sun.r = _Sun.r * pow(posFactor, _Scattering.x);
			sun.g = _Sun.g * pow(posFactor, _Scattering.y);
			sun.b = _Sun.b * pow(posFactor, _Scattering.z);
			sun.a = _Sun.a * pow(posFactor, _Scattering.w);
		}*/
		//sun.r = _Sun.r * pow(pos, _Scattering.x);
		//sun.g = _Sun.g * pow(pos, _Scattering.y);
		//sun.b = _Sun.b * pow(pos, _Scattering.z);
		//sun.a = _Sun.a * pow(pos, _Scattering.w);
		//sun.rgb *= fogFactor;

		colour = lerp(unity_FogColor, sun, haloFactor);
		//colour = lerp(unity_FogColor, sun, 1-fogFactor);
		//colour = lerp(sun, unity_FogColor, 1 - dot(IN.interpolatedRay, -_SunDirection));
		//colour = lerp(sun, unity_FogColor, (1 - pos);
		//colour = lerp(unity_FogColor, sun, lerp(posFactor, haloFactor, haloFactor));
		//colour = lerp(sun, unity_FogColor, (dot(normalize(wsDir), -_SunDirection) + 1) / 2);
		//colour = lerp(colour, unity_FogColor, dot(colour, fixed4(0.3, 0.6, 0.1, 1)));
		//colour.rgba = dot(normalize(wsDir), -_SunDirection);

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
