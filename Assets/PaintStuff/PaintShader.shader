Shader "Jared/PaintShader"
{
	Properties
	{
		_BrushColor("Brush Color", Color) = (0, 0, 0, 0)
	}
		SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
			"RenderPipeline" = "UniversalPipeline"
			"LightMode" = "UniversalForward"
		}
		//LOD 100
		ZTest Always
		ZWrite Off
		//Cull Off

		//Blend One OneMinusDstColor
		//Blend SrcAlpha OneMinusSrcAlpha, One One
		Pass
		{		
			HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

				struct appdata
				{
					float4 vertexOS : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertexUV : SV_POSITION;
					float3 vertexWS : TEXCOORD0;
					float2 uv : TEXCOORD1;
				};

				CBUFFER_START(UnityPerMaterial)
					sampler2D _MainTex;
					float4 _MainTex_ST;

					float3 _ObjPos;
					float4 _BrushColor;
					float _BrushSize;
					float _BrushStrength;
					float4 _BrushHardness;
				CBUFFER_END

				v2f vert(appdata v)
				{
					v2f o;

					float4 uv = float4(0.0f, 0.0f, 0.0f, 1.0f);
					uv.xy = (v.uv.xy * 2.0f - 1.0f) * float2(1.0f, _ProjectionParams.x);
					o.vertexUV = uv;
					o.vertexWS = mul(unity_ObjectToWorld, v.vertexOS);
					o.uv = v.uv;

					return o;
				}

				float mask(float3 position, float3 center, float radius, float hardness)
				{
					float dist = distance(center, position);
					return 1 - smoothstep(radius * hardness, radius, dist);
				}

				float4 frag(v2f i) : SV_TARGET
				{
					float4 col = tex2D(_MainTex, i.uv);
					float m = mask(i.vertexWS, _ObjPos, _BrushSize, _BrushHardness);
					float edge = m * _BrushStrength;

					float4 result = lerp(col, _BrushColor, edge);

					if (result.r <= 0.51f && result.b <= 0.51f && result.a <= 0.51f)
					{
						result.a = 0.0f;
					}

					return result;
				}

			ENDHLSL
		}
	}
}
