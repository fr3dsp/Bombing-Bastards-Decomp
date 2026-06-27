Shader "Custom/SimpleMix"
{
	Properties
	{
		_TintColor("Tint Color",  Color) = (1, 1, 1, 1)
		_MainTex("Main Tex (RGBA)", 2D) = "white" {}
		_BlendTex("Blend Tex (RGBA)", 2D) = "white" {}
		_BlendAlpha("Blend Alpha", Float) = 0
	}

		SubShader
		{
			Tags
			{
				"Queue" = "Transparent"
				"IgnoreProjector" = "True"
				"RenderType" = "Transparent"
			}

			LOD 200

			Pass
			{
				Name "FORWARD"
				Tags { "LightMode" = "ForwardBase" }

				ZWrite Off
				Blend SrcAlpha OneMinusSrcAlpha

				CGPROGRAM
				#pragma vertex   vert
				#pragma fragment frag
				#pragma multi_compile_fwdbase

				#include "UnityCG.cginc"
				#include "UnityLightingCommon.cginc"

			// ------------------------------------------------------------------
			// Properties
			// ------------------------------------------------------------------
			sampler2D _MainTex;
			float4    _MainTex_ST;

			sampler2D _BlendTex;
			float4    _BlendTex_ST;

			fixed4    _TintColor;
			float     _BlendAlpha;

			// ------------------------------------------------------------------
			// Structs
			// ------------------------------------------------------------------
			struct appdata
			{
				float4 vertex   : POSITION;
				float3 normal   : NORMAL;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos      : SV_POSITION;
				float4 uv       : TEXCOORD0;   // .xy = MainTex UV,  .zw = BlendTex UV
				float3 sh       : TEXCOORD1;   // spherical harmonics ambient (unused in frag but kept for parity)
			};

			// ------------------------------------------------------------------
			// Vertex shader
			// ------------------------------------------------------------------
			v2f vert(appdata v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);

				// Pack both UVs into a single interpolator (matches original)
				o.uv.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.uv.zw = TRANSFORM_TEX(v.texcoord, _BlendTex);

				// Spherical harmonics ambient (matches original vertex program)
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				o.sh = ShadeSH9(float4(worldNormal, 1.0));

				return o;
			}

			// ------------------------------------------------------------------
			// Fragment shader
			//
			// Logic (reverse-engineered from the ARB/ps_2_0 programs):
			//
			//   blendSample  = tex2D(_BlendTex, uv.zw)
			//   blendPremul  = blendSample.rgb * blendSample.a       // premultiplied blend colour
			//   mainWeight   = 1.0 - blendSample.a * _BlendAlpha     // how much of main shows through
			//   mainSample   = tex2D(_MainTex,  uv.xy)
			//   mixed.rgb    = mainSample.rgb * mainWeight + blendPremul.rgb * _BlendAlpha
			//   mixed.a      = mainSample.a   * mainWeight + blendPremul.a   * _BlendAlpha
			//   out.rgba     = mixed * _TintColor
			// ------------------------------------------------------------------
			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 mainSample = tex2D(_MainTex,  i.uv.xy);
				fixed4 blendSample = tex2D(_BlendTex, i.uv.zw);

				// Pre-multiply blend texture by its own alpha
				fixed4 blendPremul = blendSample * blendSample.a;

				// Weight for the main texture (how much blend "covers" it)
				fixed mainWeight = 1.0 - blendSample.a * _BlendAlpha;

				// Composite
				fixed4 result;
				result.rgb = mainSample.rgb * mainWeight + blendPremul.rgb * _BlendAlpha;
				result.a = mainSample.a   * mainWeight + blendPremul.a   * _BlendAlpha;

				// Apply tint
				result *= _TintColor;

				return result;
			}
			ENDCG
		}
		}

			Fallback "Diffuse"
}
