Shader "Jared/DeltaPaintShader"
{
	Properties
	{
		_DeltaTex("DeltaTexture", 2D) = "white" {}
		_MainTex("MainTexture", 2D) = "white" {}
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
		//ZTest Always
		//ZWrite Off
		//Cull Off

		Pass
		{
			HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

				struct appdata
				{
					float4 vertexOS : POSITION;
					float2 uvMain : TEXCOORD0;
					float2 uvDelta : TEXCOORD1;
				};

				struct v2f
				{
					float4 vertexCS : SV_POSITION;
					float2 uvMain : TEXCOORD0;
					float2 uvDelta : TEXCOORD1;
				};

				CBUFFER_START(UnityPerMaterial)
					sampler2D _DeltaTex;				
					float4 _DeltaTex_ST;
					
					sampler2D _MainTex;
					float4 _MainTex_ST;

					
				CBUFFER_END

				v2f vert(appdata v)
				{
					v2f o;

					o.vertexCS = TransformObjectToHClip(v.vertexOS);
					o.uvMain = v.uvMain;
					o.uvDelta = v.uvDelta;


					return o;
				}

				float4 frag(v2f i) : SV_TARGET
				{
					float4 col = tex2D(_MainTex, i.uvMain);
					float4 mask = tex2D(_DeltaTex, i.uvDelta);
						
					if (col.a < 0.5f)
					{
						clip(-1);
					}								
					
					return lerp(col, mask, mask.a);			
				}

			ENDHLSL
		}
	}
}
