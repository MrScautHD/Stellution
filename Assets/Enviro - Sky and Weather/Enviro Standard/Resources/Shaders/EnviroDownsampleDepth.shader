Shader "Hidden/Enviro/DepthDownsample"
{

CGINCLUDE
#pragma multi_compile __ ENVIROURP
#include "UnityCG.cginc"

	struct v2f 
	{
		float4 pos : SV_POSITION;
		float2 uv : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
	float2 _PixelSize;
	float4 _MainTex_TexelSize;

	v2f vert(appdata_img v)
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
		o.uv = v.texcoord.xy;
		return o;
	}

	half4 frag(v2f i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

		float d = Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv));
	
	if (d>0.99999)
		return half4(1,1,1,1);
	else
		return EncodeFloatRGBA(d);
	}

		ENDCG


	Subshader 
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}
	Fallback off
}