// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Camera/BlurScreenZone"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_TransitionTex("Transition Texture", 2D) = "white" {}
		_Blur("Blur", Range(0, 0.01)) = 0.0005
		_Definition("Definition", Range(0, 100)) = 10.0
	}

		SubShader
		{
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
					float2 uv : TEXCOORD0;
					float2 uv1 : TEXCOORD1;
					float4 vertex : SV_POSITION;
				};

				float4 _MainTex_TexelSize;

				v2f simplevert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.uv1 = v.uv;

					#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0)
						o.uv1.y = 1 - o.uv1.y;
					#endif

					return o;
				}

				sampler2D _MainTex;
				sampler2D _TransitionTex;
				float _Blur;
				float _Definition;

				fixed4 frag(v2f i) : SV_Target
				{

					//Image filmé par la camera
					fixed4 col = tex2D(_MainTex, i.uv);

					if (_Blur == 0 || _Definition == 0)
						return col;

					//Texture d'animation
					fixed4 transit = tex2D(_TransitionTex, i.uv1);

					float blur = _Blur - transit.b * _Blur;

					float amont = 6.283 / _Definition + 1;

					float j = -3.1415;
					for (int h = _Definition; h > 0; h--)
					{
						col += tex2D(_MainTex, float2(i.uv.x + (blur * sin(j)), i.uv.y + (blur * cos(j))));
						j += amont;
					}

					return col / (_Definition + 1);
				}
				ENDCG
			}
		}
}
