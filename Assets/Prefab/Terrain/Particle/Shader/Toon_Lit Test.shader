Shader "Toon/Lit Test" {
	Properties{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_ToonTint("Toon Ramp Tint", Color) = (0.5,0.5,0.5,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_ToonOffset("Toon Offset", Range(0,1)) = 0.5
		_ToonBlur("Toon Blur", Range(0,0.5)) = 0.05
	}

		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

	CGPROGRAM
	#pragma surface surf ToonRamp

			// custom lighting function that uses a texture ramp based
			// on angle between light direction and normal
			#pragma lighting ToonRamp exclude_path:prepass

				float4 _ToonTint;
				float _ToonOffset, _ToonBlur;
				inline half4 LightingToonRamp(SurfaceOutput s, half3 lightDir, half atten)
				{
			#ifndef USING_DIRECTIONAL_LIGHT
					lightDir = normalize(lightDir);
			#endif
					float d = dot(s.Normal, lightDir);
					float3 ramp = smoothstep(_ToonOffset, _ToonOffset + _ToonBlur, d) + _ToonTint;

					half4 c;
					c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
					c.a = 0;
					return c;
				}


			sampler2D _MainTex;
			float4 _Color;

			struct Input {
				float2 uv_MainTex : TEXCOORD0;
			};

			void surf(Input IN, inout SurfaceOutput o) {
				half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
			ENDCG

		}

			Fallback "Diffuse"
}