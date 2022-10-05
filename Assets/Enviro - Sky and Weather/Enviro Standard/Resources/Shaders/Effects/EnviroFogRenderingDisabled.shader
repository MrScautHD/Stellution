
Shader "Enviro/Standard/EnviroFogRenderingDisabled" 
{
	Properties
	{ 
		_EnviroVolumeLightingTex("Volume Lighting Tex",  Any) = ""{}
		_MainTex("Source",  Any) = "black"{}
	}
	SubShader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off Fog { Mode Off }

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma target 3.0
	#pragma multi_compile ENVIROVOLUMELIGHT
	#pragma exclude_renderers gles

	#include "UnityCG.cginc" 

	UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_EnviroVolumeLightingTex);
	uniform float4 _MainTex_TexelSize;
	uniform float4 _EnviroParams;
	uniform float _EnviroVolumeDensity;

	struct appdata_t 
	{
		float4 vertex : POSITION;
		float3 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f 
	{
		float4 pos : SV_POSITION;
		float3 texcoord : TEXCOORD0;
		float3 sky : TEXCOORD1;
		float2 uv : TEXCOORD2;
		UNITY_VERTEX_OUTPUT_STEREO
	};


	v2f vert(appdata_img v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v); //Insert
		UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Ins
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy = v.texcoord.xy;
#if UNITY_UV_STARTS_AT_TOP 
		//if (_MainTex_TexelSize.y > 0)
		//	o.uv.y = 1 - o.uv.y;
#endif 
		return o;
	}

	half3 tonemapACES(half3 color, float Exposure)
	{
		color *= Exposure;

		// See https://knarkowicz.wordpress.com/2016/01/06/aces-filmic-tone-mapping-curve/
		const half a = 2.51;
		const half b = 0.03;
		const half c = 2.43;
		const half d = 0.59;
		const half e = 0.14;
		return saturate((color * (a * color + b)) / (color * (c * color + d) + e));
	}

	fixed4 frag(v2f i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		float4 source = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, UnityStereoTransformScreenSpaceTex(i.uv));
		
		#if defined (ENVIROVOLUMELIGHT)
			float4 volumeLighting = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_EnviroVolumeLightingTex, UnityStereoTransformScreenSpaceTex(i.uv));
			volumeLighting *= _EnviroParams.x; 
			if (_EnviroParams.w == 1)
			{
				volumeLighting.rgb = tonemapACES(volumeLighting.rgb, 1.0);
			}
			return lerp(source, source + volumeLighting, _EnviroVolumeDensity);
		#else
			return source;
		#endif
		}
		ENDCG
		}
	}
	Fallback Off
}
