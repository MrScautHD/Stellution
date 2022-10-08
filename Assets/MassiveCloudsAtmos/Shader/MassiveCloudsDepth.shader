Shader "MassiveCloudsDepth"
{
	Properties
	{
	    [HideInInspector]
		_MainTex              ("Texture", 2D)                       = "white" {}
    }

	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM
			#pragma vertex MassiveCloudsVert
			#pragma fragment Frag
            #pragma shader_feature _HORIZONTAL_ON
            #pragma shader_feature _ADAPTIVE_PASS

            #include "MassiveCloudsPhysicsCloud.hlsl"
            
            float4 Frag(v2f i) : SV_Target
            {
                PrepareSampler();
                ScreenSpace ss = CreateScreenSpace(i.uv);
                return ss.depth;
            }
			ENDHLSL
		}
	}
}
