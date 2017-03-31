Shader "UI/Masks/Soft Mask"
{
	Properties
	{
		// Main texture properties
		[PerRenderData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)

		// Stencil properties
		_StencilComp("Stencil Comparison", Float) = 8
		_Stencil("Stencil ID", Float) = 0
		_StencilOp("Stencil Operation", Float) = 0
		_StencilWriteMask("Stencil Write Mask", Float) = 255
		_StencilReadMask("Stencil Read Mask", Float) = 255

		_ColorMask("Color Mask", Float) = 15

		// Alpha Map properties
		_Min("Min", Vector) = (0,0,0,0)
		_Max("Max", Vector) = (1,1,0,0)
		_AlphaMask("Alpha Mask - Must be wrapped", 2D) = "white" {}
		_AlphaUV("Alpha UV", Vector) = (1,1,0,0)

		// Alpha Mode properties
		[KeywordEnum(Alpha, Stepped, HighPass, LowPass, Threshold, Range, RangeNormalized, Exponential)] _AlphaRangeMode("Alpha Mode", Float) = 0
		[Toggle] _AlphaBlit("Alpha Blend", Float) = 1
		[Toggle] _AlphaPre("Alpha Blend Pre-mode", Float) = 0
		_AlphaRange("Alpha Range", Float) = 1
		_AlphaThreshold("Alpha Threshold", Float) = 0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CauseSpriteAtlas" = "True"
		}

		Stencil
		{
			Ref[_Stencil]
			Comp[_StencilComp]
			Pass[_StencilOP]
			ReadMask[_StencilReadMask]
			WriteMask[_StencilWriteMask]
		}

		Cull Off
		Lighting Off
		ZWrite Off
		ZTest[unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask[_ColorMask]

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			struct appdata_t
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
			};

			fixed4 _Color;
			fixed4 _TextureSampleAdd;

			bool _UseClipRect;
			float4 _ClipRect;

			bool _UseAlphaClip;

			float4 _ProgressColor;
			float _Value;
			int _LeftToRight;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.worldPosition = IN.vertex;
				OUT.vertex = mul(UNITY_MATRIX_MVP, OUT.worldPosition);

				OUT.texcoord = IN.texcoord;

				#ifdef UNITY_HALF_TEXEL_OFFSET
					OUT.vertex.xy += (_ScreenParams.zw - 1.0) * float2(-1, 1);
				#endif

				OUT.color = IN.color * _Color;
				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaMask;

			float2 _AlphaUV;

			float _AlphaRangeMode;
			float _AlphaBlit;
			float _AlphaPre;
			float _AlphaRange;
			float _AlphaThreshold;

			float2 _Min;
			float2 _Max;

			fixed4 frag(v2f IN) : SV_Target
			{
				// Sample the main texture
				half4 color = (tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd) * IN.color;

				// Clip the texture within the mask bounds
				if (IN.texcoord.x < _Min.x || IN.texcoord.x > _Max.x || IN.texcoord.y < _Min.y || IN.texcoord.y > _Max.y)
				{
					color.a = 0;
				}
				else
				{
					// Sample the alpha mask
					float alpha = tex2D(_AlphaMask, (IN.texcoord - _Min) / _AlphaUV).a;

					// If pre mode blending is enabled the alpha mask and color alpha will be combined, then the alpha masking mode will be applied
					if (_AlphaPre == 1)
					{
						alpha = color.a * alpha;
					}

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
					
					color.a = alpha;
				}

				if (_UseClipRect)
				{
					color *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
				}
				
				if (_UseAlphaClip)
					clip(color.a - 0.001f);

				
				//	color.a = (int)(color.a * (255 * _AlphaRange));
				//else if (_AlphaRangeMode == 2)
				//	color.a = (int)(color.a * (255 * _AlphaRange)) / (255 * _AlphaRange);
				//else if (_AlphaRangeMode == 3)
				//	color.a = floor(color.a * _AlphaRange) / _AlphaRange;
				//else if (_AlphaRangeMode == 4)
				//	color.a = floor(pow(color.a, _AlphaCurve) * _AlphaRange) / _AlphaRange;

				return color;
			}
			ENDCG
		}
	}
}
