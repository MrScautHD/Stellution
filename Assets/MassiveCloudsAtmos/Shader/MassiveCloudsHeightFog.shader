Shader "MassiveCloudsHeightFog"
{
	Properties
	{
	    [HideInInspector]
		_MainTex            ("Texture", 2D)                     = "white" {}
		_MCFogColor           ("FogColor", Color)                 = (1, 1, 1, 1)
		_GroundHeight       ("GroundHeight", Range(-1000, 1000)) = 0
		_HeightFogFromDistance  ("HeightFogFromDistance", Range(0, 10000)) = 0
		_HeightFogRange     ("HeightFogRange", Range(0.001, 1000)) = 0
		_FarHeightFogRange     ("FarHeightFogRange", Range(0.001, 1000)) = 0
		_HeightFogDensity     ("HeightFogDensity", Range(0, 1)) = 1
		_HeightFogScattering     ("HeightFogScattering", Range(0, 1)) = 1
    }

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			HLSLPROGRAM
			#pragma vertex MassiveCloudsVert
			#pragma fragment MassiveCloudsFragment
            #pragma shader_feature _HEIGHTFOG_ON
 
			#include "MassiveCloudsCommon.hlsl"
			#include "MassiveCloudsHeightFog.hlsl"

            half4 MassiveCloudsFragment(v2f i) : SV_Target
            {
                PrepareLighting();
                ScreenSpace ss = CreateScreenSpace(i.uv);
                float exposureMultiplier = ExposureMultiplier();
            
            #if defined(USING_STEREO_MATRICES)
                half4 screenCol = tex2Dproj(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
            #else 
                half4 screenCol = tex2Dproj(_MainTex, i.uv);
            #endif
            
                #if defined(_HEIGHTFOG_ON)
                HeightFogFragment(screenCol, ss, exposureMultiplier);
                #endif
                screenCol.a = 0;
            
                return screenCol;
            }
            
			ENDHLSL
		}
	}
}
