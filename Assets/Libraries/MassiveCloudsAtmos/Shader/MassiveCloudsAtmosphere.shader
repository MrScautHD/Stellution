Shader "MassiveCloudsAtmosphere"
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
            #pragma shader_feature _DEFERRED_SHADING            

            #include "MassiveCloudsAtmosphere.hlsl"

            float4 Frag(v2f i) : SV_Target
            {
                PrepareSampler();
                PrepareLighting();
                float4 atmosphereCol = MC_TEX2DPROJ(_MainTex, i.uv);
                float4 cloudColor    = MC_TEX2DPROJ_ST(_CloudsTexture, i.uv, _MainTex_ST);
                ScreenSpace      ss               = CreateScreenSpace(i.uv);
                CloudFactor      cloudFactor      = UnpackCloud(cloudColor);
                AtmosphereFactor atmosphereFactor = UnpackAtmosphere(atmosphereCol);
                float            noise            = ProgressiveNoise(ss);

                if (!ProgressiveFrameActive(noise)) return atmosphereCol;
                return AtmosphereFragment(atmosphereFactor, ss, noise, cloudFactor);
            }
			ENDHLSL
		}

		Pass
		{
			HLSLPROGRAM
			#pragma vertex MassiveCloudsVert
			#pragma fragment Frag
            #pragma shader_feature _HORIZONTAL_ON
            #pragma shader_feature _DEFERRED_SHADING            

            #define MASSIVE_CLOUDS_ADAPTIVE

            #include "MassiveCloudsAtmosphere.hlsl"

            float4 Frag(v2f i) : SV_Target
            {
                PrepareSampler();
                PrepareLighting();
                float4 atmosphereCol = MC_TEX2DPROJ(_MainTex, i.uv);
                float4 cloudColor    = MC_TEX2DPROJ_ST(_CloudsTexture, i.uv, _MainTex_ST);
                ScreenSpace      ss               = CreateScreenSpace(i.uv);
                CloudFactor      cloudFactor      = UnpackCloud(cloudColor);
                AtmosphereFactor atmosphereFactor = UnpackAtmosphere(atmosphereCol);
                float            noise            = ProgressiveNoise(ss);

            #ifdef _HORIZONTAL_ON
                HorizontalRegion region = CreateRegion();
                Ray ray = CalculateHorizontalRayRange(ss, region);
            #else
                Ray ray = CalculateSphericalRayRange(ss);
            #endif

                float far = lerp(ss.maxDist, _MaxDistance, ss.isMaxPlane);
                float depthDiff = abs(atmosphereFactor.depth - ss.depth);
                float threshold = log(ss.maxDist) / (10000 * _MassiveCloudsAdaptiveSampling);
                if (far <= 0) return atmosphereCol;
                if (depthDiff > threshold)
                {
                    return AtmosphereFragment(atmosphereFactor, ss, noise, cloudFactor);
                }
                return atmosphereCol;
            }
			ENDHLSL
		}
	}
}
