Shader "Hidden/Enviro/Upsample"
{
	SubShader
	{
		Pass
	{
		ZTest Always Cull Off ZWrite Off Fog{ Mode Off }

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile __ ENVIROURP
		#include "UnityCG.cginc"

		UNITY_DECLARE_SCREENSPACE_TEXTURE(_LowResTexture);
		UNITY_DECLARE_SCREENSPACE_TEXTURE(_CameraDepthLowRes);
		UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
		UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
		float2 _LowResPixelSize;
		float2 _LowResTextureSize;
		float _DepthMult;
		float _Threshold;
		float4 _MainTex_TexelSize;


	struct appdata
	{
		float4 vertex 	: POSITION;
		float4 uv 		: TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID	
	};

	struct v2f
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		float2 uv00 : TEXCOORD1;
		float2 uv10 : TEXCOORD2;
		float2 uv01 : TEXCOORD3;
		float2 uv11 : TEXCOORD4;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f vert(appdata v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v); 
		UNITY_INITIALIZE_OUTPUT(v2f, o); 
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
#if defined(ENVIROURP)
	o.pos = float4(v.vertex.xyz,1.0);
#if UNITY_UV_STARTS_AT_TOP
    o.pos.y *= -1;
#endif
#else
	o.pos = UnityObjectToClipPos(v.vertex);
#endif
		o.uv = UnityStereoTransformScreenSpaceTex(v.uv);
		o.uv00 = v.uv - 0.5 * _LowResPixelSize;
		o.uv10 = o.uv00 + float2(_LowResPixelSize.x, 0.0);
		o.uv01 = o.uv00 + float2(0.0, _LowResPixelSize.y);
		o.uv11 = o.uv00 + _LowResPixelSize;

		return o;
	}

	fixed4 ClosestDepthFast(v2f i)
	{
		float z00 = DecodeFloatRGBA(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthLowRes, i.uv00));
		float z10 = DecodeFloatRGBA(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthLowRes, i.uv10));
		float z01 = DecodeFloatRGBA(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthLowRes, i.uv01));
		float z11 = DecodeFloatRGBA(UNITY_SAMPLE_SCREENSPACE_TEXTURE(_CameraDepthLowRes, i.uv11));

		float zfull = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv));

		float dist00 = abs(z00 - zfull);
		float dist10 = abs(z10 - zfull);
		float dist01 = abs(z01 - zfull);
		float dist11 = abs(z11 - zfull);

		float3 uvd00 = float3(i.uv00, dist00);
		float3 uvd10 = float3(i.uv10, dist10);
		float3 uvd01 = float3(i.uv01, dist01);
		float3 uvd11 = float3(i.uv11, dist11);

		float3 finalUV = lerp(uvd10, uvd00, saturate(99999 * (uvd10.z - uvd00.z)));
		finalUV = lerp(uvd01, finalUV, saturate(99999 * (uvd01.z - finalUV.z)));
		finalUV = lerp(uvd11, finalUV, saturate(99999 * (uvd11.z - finalUV.z)));

		float maxDist = max(max(max(dist00, dist10), dist01), dist11) - _Threshold;

		fixed r = saturate(maxDist * 99999);
		float2 uv = lerp(i.uv, finalUV.xy, r);
		return UNITY_SAMPLE_SCREENSPACE_TEXTURE(_LowResTexture, uv);
	}

	fixed4 frag(v2f i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		return ClosestDepthFast(i);
	}
		ENDCG
	}
	}
		Fallback Off
}