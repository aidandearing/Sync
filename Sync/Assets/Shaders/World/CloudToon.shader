Shader "World/Cloud/Toon"
{
	Properties
	{
		_Ramp("Toon Ramp (RGB)", 2D) = "gray" {}
	_CloudDark("Cloud Dark", Color) = (1,1,1,1)
		_CloudBright("Cloud Bright", Color) = (1,1,1,1)
		_CloudLightFactor("Cloud Light Scatter Factor", Float) = 1.0
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_Outline("Outline width", Range(.002, 0.03)) = .005
	}

		SubShader
	{
		// Draw ourselves after all opaque geometry
		Tags{ "RenderType" = "Opaque" "LightMode" = "ForwardBase" }

		// Grab the screen behind the object into _BackgroundTexture
		//GrabPass
		//{
		//	"_BackgroundTexture"
		//}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"

			//ZWrite Off

			struct v2f
			{
				float4 pos : SV_POSITION;
				// .w is being used as dot product between cameraDirection and normal
				float4 normal : NORMAL;
				fixed4 colour : COLOR0;
				float2 uv : TEXCOORD0;
				float4 posWorld : TEXCOORD1;
				float4 posLocal : TEXCOORD2;
				float3 cameraDirection : TEXCOORD3;
			};

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

			sampler2D _Ramp;

			v2f vert(float4 vertex : POSITION, float3 normal : NORMAL, float4 tangent : TANGENT, float2 uv : TEXCOORD0)
			{
				v2f o;
				o.posLocal = vertex;
				o.posWorld = mul(unity_ObjectToWorld, vertex);
				// use UnityObjectToClipPos from UnityCG.cginc to calculate 
				// the clip-space of the vertex
				o.pos = UnityObjectToClipPos(vertex);
				o.uv = uv;

				half3 worldNormal = UnityObjectToWorldNormal(normal.xyz);
				half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				o.colour = nl * _LightColor0;
				o.colour.rgb += ShadeSH9(half4(worldNormal, 1));

				o.cameraDirection = normalize(_WorldSpaceCameraPos - o.posWorld.xyz);
				o.normal = float4(normal.xyz, pow(1 - abs(dot(normalize(o.cameraDirection), normalize(worldNormal))), 1));

				return o;
			}

			half4 _CloudBright;
			half4 _CloudDark;
			float _CloudLightFactor;

			half4 frag(v2f IN) : SV_Target
			{
				half d = max(dot(IN.colour.rgb, unity_ColorSpaceLuminance), IN.normal.w);
				half4 inColour = half4(IN.colour.rgb * tex2D(_Ramp, half2(d, d)).rgb, 1);
				half4 colour = lerp(_CloudDark, _CloudBright, pow(saturate(dot(inColour, unity_ColorSpaceLuminance)), _CloudLightFactor));

				return colour;
			}
			ENDCG
		}
		UsePass "Toon/Basic Outline/OUTLINE"
	}
}