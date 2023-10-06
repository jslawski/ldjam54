Shader "Jared/PaintableShader"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		_MaskTexture("Mask Texture", 2D) = "white" {}
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

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha	
		
			HLSLPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

				struct appdata
				{
					float4 vertexOS : POSITION;
					float2 uvMain : TEXCOORD0;
					float2 uvMask : TEXCOORD1;
				};

				struct v2f
				{					
					float4 vertex : SV_POSITION;
					float2 uvMain : TEXCOORD0;
					float2 uvMask : TEXCOORD1;
				};

				CBUFFER_START(UnityPerMaterial)
					sampler2D _MainTex;
					float4 _MainTex_ST;

					sampler2D _MaskTexture;
					float4 _MaskTexture_ST;
				CBUFFER_END

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = TransformObjectToHClip(v.vertexOS);
					o.uvMain = v.uvMain;
					o.uvMask = v.uvMask;

					return o;
				}

				float4 frag(v2f i) : SV_TARGET
				{
					float4 col = tex2D(_MainTex, i.uvMain);
					float4 colMask = tex2D(_MaskTexture, i.uvMask);										

					return lerp(col, colMask, colMask.w);
				}

			ENDHLSL
		}
	}
}
