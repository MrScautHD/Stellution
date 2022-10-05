// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Enviro/Effects/ClearLightShafts" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Pass {
		ZTest Always Cull Off ZWrite Off

CGPROGRAM
#pragma vertex vert 
#pragma fragment frag
#include "UnityCG.cginc"

UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
uniform float4 _MainTex_TexelSize;
 
struct v2f {
	float4 pos : SV_POSITION;
	UNITY_VERTEX_OUTPUT_STEREO
};

v2f vert( appdata_img v )
{
	v2f o;
	UNITY_SETUP_INSTANCE_ID(v); //Insert
	UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
	UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Ins
	o.pos = UnityObjectToClipPos(v.vertex);
	return o;
}

half4 frag (v2f i) : SV_Target
{
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
	return half4(0,0,0,0);
}
ENDCG
	}
}

Fallback off

}
