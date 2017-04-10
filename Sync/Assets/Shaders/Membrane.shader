Shader "Membrane/Simple"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_Colour("Colour", Color) = (1,1,1,1)
		_ColourEdge("Edge Colour", Color) = (1,1,1,1)
		_EdgeSharpness("Edge Sharpness", Float) = 1.0
		_Distortion("Distortion", Float) = 1.0
		_CutoffExpression("Cutoff Expression", Float) = 0.0
		_Cutoff("Cutoff", Float) = 0.0
		_CutoffRange("Cutoff Range", Float) = 0.0
		_POctaves("Perlin Octaves", Float) = 8.0
		_PSpeed("Perlin Speed", Float) = 1.0
	}
		SubShader
	{
		// Draw ourselves after all opaque geometry
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" "LightMode" = "ForwardBase" }

		// Grab the screen behind the object into _BackgroundTexture
		GrabPass
	{
		"_BackgroundTexture"
	}

		// Render the object with the texture generated above, and invert the colors
		Pass
	{
		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "UnityLightingCommon.cginc"

	struct v2f
	{
		float4 pos : SV_POSITION;
		// .w is being used as dot product between cameraDirection and normal
		float4 normal : NORMAL;
		fixed4 colour : COLOR0;
		float2 uv : TEXCOORD0;
		float4 posGrab : TEXCOORD1;
		float4 posWorld : TEXCOORD2;
		float3 cameraDirection : TEXCOORD3;
	};

	///////////////////////////////////////////////////////////////////////////////
	// 
	//		Dan's Perlin Implementation
	// 
	///////////////////////////////////////////////////////////////////////////////

	float Hash(float n)
	{
		//random number generation with no scaling
		return frac(sin(n)*43758.5453);
	}

	float Noise(float3 x)
	{
		//The noise function returns a value in the range -1.0 - 1.0f

		float3 p = floor(x);
		float3 f = frac(x);

		f = f*f*(3.0 - 2.0*f);
		float n = p.x + p.y * 57.0 + 113.0 * p.z;

		/*Randomizing a value between 2 random points. figuring out the point between the two points to
		smooth out the curve*/
		return
			lerp(
				lerp(
					lerp(Hash(n + 0.0), Hash(n + 1.0), f.x),
					lerp(Hash(n + 57.0), Hash(n + 58.0), f.x), f.y),
				lerp(
					lerp(Hash(n + 113.0), Hash(n + 114.0), f.x),
					lerp(Hash(n + 170.0), Hash(n + 171.0), f.x), f.y), f.z
				);
	}

	///
	///	My Addition
	///

	float _POctaves;
	float _PSpeed;

	float3 perlin3D(float3 pos)
	{
		float noiseVal = 0.0f;

		for (float i = 0.0f; i < _POctaves; i++)
		{
			noiseVal += Noise(float3((pos.x * i) + _Time[1] * _PSpeed, (pos.y * i) + _Time[1] * _PSpeed, (pos.z * i) + _Time[1] * _PSpeed));
		}

		noiseVal /= _POctaves;

		return noiseVal;
	}

	///////////////////////////////////////////////////////////////////////////////
	// 
	//		Scott and Dan's Reflection and Refraction
	// 
	///////////////////////////////////////////////////////////////////////////////

	float4 Refract(uniform float4 incidentVec, uniform float4 normal, float eta)
	{
		float4 Out = float4(0,0,0,1);

		float N_dot_I = dot(normal, incidentVec);
		float k = 1.0f - eta * eta * (1.0f - N_dot_I * N_dot_I);
		if (k < 0.0f)
		{
			Out = float4(0.0f, 0.0f, 0.0f, 1.0f);
		}
		else
		{
			Out = eta * incidentVec - (eta * N_dot_I + sqrt(k)) * normal;
		}

		return Out;
	}

	///////////////////////////////////////////////////////////////////////////////
	// 
	//		Vert Frag
	// 
	///////////////////////////////////////////////////////////////////////////////

	float _EdgeSharpness;

	v2f vert(appdata_base v)
	{
		v2f o;
		o.posWorld = mul(unity_ObjectToWorld, v.vertex);
		// use UnityObjectToClipPos from UnityCG.cginc to calculate 
		// the clip-space of the vertex
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord;

		half3 worldNormal = UnityObjectToWorldNormal(v.normal.xyz);
		half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
		o.colour = nl * _LightColor0;

		o.colour.rgb += ShadeSH9(half4(worldNormal, 1));

		//float scale = 0.1f;
		float speed = 1.0f;
		float octaves = 8.0f;
		float noiseVal = 0.0f;

		float4 normal = float4(0, 0, 0, 0);
		for (float i = 0.0f; i < octaves; i++)
		{
			noiseVal += Noise(float3((o.posWorld.x * i) + _Time[1] * speed, (o.posWorld.y * i) + _Time[1] * speed, (o.posWorld.z * i) + _Time[1] * speed));
		}

		noiseVal /= octaves;

		normal = noiseVal;
		normalize(normal);

		o.cameraDirection = normalize(_WorldSpaceCameraPos - o.posWorld.xyz);

		// use ComputeGrabScreenPos function from UnityCG.cginc
		// to get the correct texture coordinate
		//o.posGrab = ComputeGrabScreenPos(o.pos + float4(v.normal.xyz, 1));// +normal * scale);

		o.posGrab = ComputeGrabScreenPos(o.pos);
		o.normal = float4(v.normal.xyz, pow(1 - abs(dot(normalize(o.cameraDirection), normalize(worldNormal))), _EdgeSharpness));

		v.vertex += float4(normalize(worldNormal) * 10, 1);

		return o;
	}

	sampler2D _MainTex;
	sampler2D _BackgroundTexture;

	half4 _Colour;
	half4 _ColourEdge;

	float _Cutoff;
	float _CutoffExpression;
	float _CutoffRange;

	float _Distortion;

	half4 frag(v2f IN) : SV_Target
	{
		float3 noise = perlin3D(IN.posWorld);

		/// CUTOFF FACTOR
		// Cutoff Factor decides when, based on perlin noise, the entire distortion and freznel effect should be cut away and the raw grab pass texture should be drawn, rendering this object effectively invisible
		// Based on _Cutoff and _CutoffRange
		// _Cutoff establishes the luminance threshold of a noise value, that when passed is set to 0 (invisible)
		// _CutoffRange establishes the effective range around the threshold that will be trimmed gently
		float cutoffFactor = 1;

		// A secondary special pass, for glowing cutoff
		bool isCutoffTransitioning = false;
		
		if (_CutoffExpression > 0)
		{
			cutoffFactor = dot(noise, unity_ColorSpaceLuminance);

			if (cutoffFactor > _Cutoff)
			{
				cutoffFactor = 0;
			}
			else
			{
				cutoffFactor = saturate((_Cutoff - cutoffFactor) / _CutoffRange);
				isCutoffTransitioning = true;
			}

			cutoffFactor = lerp(IN.normal.w, cutoffFactor, _CutoffExpression);
		}

		half4 colourBG = tex2Dproj(_BackgroundTexture, IN.posGrab + float4(noise, 1) * _Distortion * (1 - cutoffFactor));
		half4 colourMT = tex2D(_MainTex, IN.uv);
		float alphaSharpened = pow(colourMT.a, _EdgeSharpness);
		float edgeFactor = max(IN.normal.w, alphaSharpened);
		cutoffFactor = max(cutoffFactor, alphaSharpened);
		half4 colour = lerp(colourBG, lerp(_Colour, _ColourEdge, min(edgeFactor, cutoffFactor)), min(edgeFactor, cutoffFactor));
		//colour = lerp(colourBG, colour, _Colour.a * length(noise));

		if (isCutoffTransitioning)
			colour = lerp(colourBG, lerp(_Colour, _ColourEdge, saturate(_CutoffExpression - cutoffFactor)), saturate(_CutoffExpression - cutoffFactor));

		return colour;
	}
		ENDCG
	}
	}
}