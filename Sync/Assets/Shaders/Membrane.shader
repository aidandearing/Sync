Shader "Membrane/Distortion/Edge+Map"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_NormTex("Normal Texture", 2D) = "white" {}
		_NormStrength("Normal Strength", Range(0,100)) = 1.0
		_Colour("Colour", Color) = (1,1,1,1)
		_ColourEdge("Edge Colour", Color) = (1,1,1,1)
		_EdgeSharpness("Edge Sharpness", Float) = 1.0
		_POctaves("Perlin Octaves", Float) = 8.0
		_PSpeed("Perlin Speed", Float) = 1.0
		_PStrength("Perlin Strength", Range(0, 100)) = 1.0
		_PSharpness("Perlin Sharpness", Float) = 2.0
		_Distortion("Distortion", Range(0,1)) = 1.0
		_Cutoff("Cutoff", Range(0,1)) = 0.0
		_CutoffRange("Cutoff Range", Range(0,1)) = 0.0
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(.002, 0.03)) = .005
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
				float4 posLocal : TEXCOORD7;
				float3 cameraDirection : TEXCOORD3;
				// these three vectors will hold a 3x3 rotation matrix
				// that transforms from tangent to world space
				half3 tspace0 : TEXCOORD4; // tangent.x, bitangent.x, normal.x
				half3 tspace1 : TEXCOORD5; // tangent.y, bitangent.y, normal.y
				half3 tspace2 : TEXCOORD6; // tangent.z, bitangent.z, normal.z
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

			v2f vert(appdata_full IN)
			{
				//float scale = 0.1f;
				/*float speed = 0.1f;
				float octaves = 8.0f;
				float noiseVal = 0.0f;

				for (float i = 0.0f; i < octaves; i++)
				{
					noiseVal += Noise(float3((IN.vertex.x * i) + _Time[1] * speed, (IN.vertex.y * i) + _Time[1] * speed, (IN.vertex.z * i) + _Time[1] * speed));
				}

				noiseVal /= octaves;

				IN.vertex += float4(IN.normal * (noiseVal - 0.3f) * -1.0f, 0);*/

				v2f o;
				o.posLocal = IN.vertex;
				o.posWorld = mul(unity_ObjectToWorld, IN.vertex);
				// use UnityObjectToClipPos from UnityCG.cginc to calculate 
				// the clip-space of the vertex
				o.pos = UnityObjectToClipPos(IN.vertex);
				o.uv = IN.texcoord;

				half3 wNormal = UnityObjectToWorldNormal(IN.normal);
				half3 wTangent = UnityObjectToWorldDir(IN.tangent.xyz);
				// compute bitangent from cross product of normal and tangent
				half tangentSign = IN.tangent.w * unity_WorldTransformParams.w;
				half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
				// output the tangent space matrix
				o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
				o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
				o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);

				half nl = max(0, dot(wNormal, _WorldSpaceLightPos0.xyz));
				o.colour = nl * _LightColor0;

				o.colour.rgb += ShadeSH9(half4(wNormal, 1));

				o.cameraDirection = normalize(_WorldSpaceCameraPos - o.posWorld.xyz);

				// use ComputeGrabScreenPos function from UnityCG.cginc
				// to get the correct texture coordinate
				//o.posGrab = ComputeGrabScreenPos(o.pos + float4(v.normal.xyz, 1));// +normal * scale);

				o.posGrab = ComputeGrabScreenPos(o.pos);
				o.normal = float4(IN.normal.xyz, pow(1 - abs(dot(normalize(o.cameraDirection), normalize(wNormal))), _EdgeSharpness));

				return o;
			}

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _NormTex;
			float4 _NormTex_ST;
			sampler2D _BackgroundTexture;

			float _NormStrength;
			float _PStrength;
			float _PSharpness;

			half4 _Colour;
			half4 _ColourEdge;

			float _Cutoff;
			float _CutoffRange;

			float _Distortion;

			half4 frag(v2f IN) : SV_Target
			{
				float3 noise = pow((perlin3D(IN.posLocal) - float3(-0.25,-0.25,-0.25)), _PSharpness);

				/// CUTOFF FACTOR
				// Cutoff Factor decides when, based on perlin noise, the entire distortion and freznel effect should be cut away and the raw grab pass texture should be drawn, rendering this object effectively invisible
				// Based on _Cutoff and _CutoffRange
				// _Cutoff establishes the luminance threshold of a noise value, that when passed is set to 0 (invisible)
				// _CutoffRange establishes the effective range around the threshold that will be lerped from 1 to 0, this occurs outward
				float cutoffFactor = 1;

				// During the transition a special logic needs to be applied to generate those edges around the cutoff zones
				bool isCutoffTransitioning = false;
		
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

				// sample the normal map, and decode from the Unity encoding
				half3 tnormal = UnpackNormal(tex2D(_NormTex, IN.uv * _NormTex_ST.xy + _NormTex_ST.zw));
				// transform normal from tangent to world space
				half3 worldNormal;
				worldNormal.x = dot(IN.tspace0, tnormal);
				worldNormal.y = dot(IN.tspace1, tnormal);
				worldNormal.z = dot(IN.tspace2, tnormal);

				half3 worldViewDir = normalize(UnityWorldSpaceViewDir(IN.posWorld));
				half3 worldRefl = reflect(-worldViewDir, worldNormal);
				half worldIncident = abs(pow(dot(normalize(worldRefl), normalize(worldViewDir)), 2) + noise * _PStrength) * dot(IN.colour, unity_ColorSpaceLuminance) * _NormStrength;

				float invCutoffFactor = (1 - cutoffFactor);
				half4 colourBG = tex2Dproj(_BackgroundTexture, IN.posGrab + float4(noise, 1) * _Distortion * invCutoffFactor + float4(worldNormal,0) * _NormStrength * invCutoffFactor);
				half4 colourMT = tex2D(_MainTex, IN.uv);
				float edgeFactor = max(IN.normal.w, colourMT.a);
				cutoffFactor = max(max(IN.normal.w, cutoffFactor), colourMT.a);
				half4 colour = lerp(colourBG, lerp(_Colour, _ColourEdge, min(edgeFactor, cutoffFactor)), min(edgeFactor, cutoffFactor));
				colour = lerp(colour, lerp(_Colour, _ColourEdge, worldIncident), worldIncident);

				if (isCutoffTransitioning)
					colour = lerp(colourBG, lerp(_Colour, _ColourEdge, saturate(_Cutoff - cutoffFactor) / _Cutoff), saturate(_Cutoff - cutoffFactor) / _Cutoff);

				return colour;
			}
			ENDCG
		}

		//UsePass "Toon/Basic Outline/OUTLINE"
	}
}