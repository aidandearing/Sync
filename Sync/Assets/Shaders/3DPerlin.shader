Shader "Custom/3DPerlin" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader 
	{
		Tags { "Queue" = "Transparent"}

		// Grab the screen behind the object into _BackgroundTexture
		//GrabPass
		//{
		//	"_BackgroundTexture"
		//}
		
		//Pass
		//{
			Blend SrcAlpha OneMinusSrcAlpha
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard alpha
			#pragma vertex vert

			struct Input 
			{
				float2 uv_MainTex;
				//float2 uv_GrabPos;
			};

			//sampler2D _BackgroundTexture;
			sampler2D _MainTex;
			

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;

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

			void Reflect(float3 Out, uniform float3 incidentVec, uniform float3 normal)
			{
				Out = incidentVec - 2.0f * dot(incidentVec, normal) * normal;
			}

			void Refract(float3 Out, uniform float3 incidentVec, uniform float3 normal, float eta)
			{
				float N_dot_I = dot(normal, incidentVec);
				float k = 1.0f - eta * eta * (1.0f - N_dot_I * N_dot_I);
				if (k < 0.0f)
				{
					Out = float3(0.0f, 0.0f, 0.0f);
				}
				else
				{
					Out = eta * incidentVec - (eta * N_dot_I + sqrt(k)) * normal;
				}
			}

			void vert(inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);

				float scale = 1.1f;
				float speed = 1.5f;
				float octaves = 10.0f;
				float noiseVal = 0.0f;
	
				for (float i = 0.0f; i < octaves; i++)
				{
					noiseVal += Noise(float3((v.vertex.x * i) + _Time[1] * speed, v.vertex.y, (v.vertex.z * i) + _Time[1] * speed));
					//float n = noise(float3((v.vertex.x * scale) + _Time[1] * speed, v.vertex.y, (v.vertex.z * scale) + _Time[1] * speed));
				}

				noiseVal /= octaves;
				v.vertex.y += noiseVal / 5.0f;

				v.normal += Noise(float3((v.vertex.x * scale) + _Time[1] * speed, v.vertex.y, (v.vertex.z * scale) + _Time[1] * speed));
				normalize(v.normal);
			}

			void surf(Input IN, inout SurfaceOutputStandard o)
			{
				// Albedo comes from a texture tinted by color
				//fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = _Color.rgb; //c.rgb
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = _Color.a;
			}
			ENDCG
		//}
	}
}
