Shader "Unlit/ChromaKey"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_ChromaColor("ChromaColor", Color) = (1.0, 1.0, 1.0, 1.0)
		_ChromaSimilarity("ChromaSimilarity", Range(0.0, 1.0)) = 0.2
	}
		SubShader
		{
			Tags { "RenderType" = "Transparent" }

			// No culling or depth
			Cull Off ZWrite Off ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
				};
				
				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;

					return o;
				}

				sampler2D _MainTex;
				float4 _ChromaColor;
				float _ChromaSimilarity;

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 texCol = tex2D(_MainTex, i.uv);

					fixed4 checkCol = fixed4(abs(_ChromaColor.r - texCol.r), 
											abs(_ChromaColor.g - texCol.g), 
											abs(_ChromaColor.b - texCol.b), 
											abs(_ChromaColor.a - texCol.a));					

					clip((checkCol.r > _ChromaSimilarity || checkCol.g > _ChromaSimilarity || checkCol.b > _ChromaSimilarity) ? 1 : -1);					

					return texCol;
				}

			ENDCG
			}
		}
}