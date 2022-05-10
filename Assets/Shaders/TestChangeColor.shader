Shader "Test/TestChangeColor"
{
	Properties
	{
		_MainTex("_Texture", 2D) = "white" {}
		_IsAdditive("_IsAdditive", Int) = 1
		_Color("_Color", Color) = (1,1,1,1)	
	}
		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

			Pass
			{
				CGPROGRAM
#pragma target 3.0
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
					float4 vertex : SV_POSITION;
					float3 worldPos : TEXCOORD2;
					float2 screenPos : TEXCOORD1;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.screenPos = ComputeScreenPos(o.vertex);
					o.worldPos = mul(unity_ObjectToWorld, o.vertex);
					return o;
				}

				sampler2D _MainTex;
				fixed4 _Color;
				int _IsAdditive;

				fixed4 tempColor;

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);

					if (_IsAdditive)
					{
						col += _Color * _Color.a * i.screenPos.y;
					}
					else
					{
						col -= _Color * _Color.a * i.screenPos.y;
					}

					return col;
				}
				ENDCG
			}
		}
}
