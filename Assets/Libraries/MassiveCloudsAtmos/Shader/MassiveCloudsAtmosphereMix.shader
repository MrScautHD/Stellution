Shader "MassiveCloudsAtmosphereMix"
{
	Properties
	{
	    [HideInInspector]
		_MainTex ("Texture", 2D) = "white" {}
    }

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM
			#pragma vertex MassiveCloudsVert
			#pragma fragment FragAll
            #pragma shader_feature _HORIZONTAL_ON
            #pragma shader_feature _SkyEnabled

            #include "MassiveCloudsAtmosphereMix.hlsl"

			ENDHLSL
		}
	}
}
