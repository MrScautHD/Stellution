Shader "MassiveCloudsPhysicsCloud"
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

			#include "MassiveCloudsPhysicsCloud.hlsl"
            
            float     _CaptureMix;
            float2    _SampleRotation;

			float4 Frag(v2f i) : SV_Target
            {
                PrepareSampler();
                PrepareLighting();
                ScreenSpace ss = CreateScreenSpace(i.uv);
                float4 uv = i.uv;
                float4 cloudTexCol = MC_TEX2DPROJ(_MainTex, uv);
                CloudFactor cloudFactor = UnpackCloud(cloudTexCol);
 
            #ifdef _HORIZONTAL_ON
                HorizontalRegion region = CreateRegion();
                Ray ray = CalculateHorizontalRayRange(ss, region);
            #else
                Ray ray = CalculateSphericalRayRange(ss);
            #endif
                if (ray.to <= 0) return 0;
                CloudFactor next = PhysicsCloud(ray, ss);
                return PackCloud(next);
            }
			ENDHLSL
		}

		Pass
		{
			HLSLPROGRAM
			#pragma vertex MassiveCloudsVert
			#pragma fragment Frag
            #pragma shader_feature _HORIZONTAL_ON
            #define _MASSIVE_CLOUDS_PROGRESSIVE_PASS

            #include "MassiveCloudsPhysicsCloud.hlsl"

            sampler2D _CaptureTexture;
            float     _CaptureMix;

            float4 Frag(v2f i) : SV_Target
            {
                PrepareSampler();
                PrepareLighting();
                ScreenSpace ss = CreateScreenSpace(i.uv);
                float4 cloudTexCol = MC_TEX2DPROJ(_MainTex, i.uv);
                CloudFactor cloudFactor = UnpackCloud(cloudTexCol);
                float4 captureTexCol = MC_TEX2DPROJ_ST(_CaptureTexture, i.uv, _MainTex_ST);
                CloudFactor captureFactor = UnpackCloud(captureTexCol);
        
            #ifdef _HORIZONTAL_ON
                HorizontalRegion region = CreateRegion();
                Ray ray = CalculateHorizontalRayRange(ss, region);
            #else
                Ray ray = CalculateSphericalRayRange(ss);
            #endif

                if (ray.to <= 0) return float4(0, 0, cloudFactor.occlusion, cloudFactor.depth);
                float far = lerp(ss.maxDist, _MaxDistance, ss.isMaxPlane);
                float occlusion = cloudFactor.occlusion;
                float threshold = log(ss.maxDist) / (10000 * _MassiveCloudsAdaptiveSampling);
                if (ray.from > far)
                    return float4(0, 0, cloudFactor.occlusion, cloudFactor.depth);

                float density = cloudFactor.density;
                float scattering = cloudFactor.scattering;
                if (ss.isMaxPlane > 0)
                {
                    scattering = cloudFactor.depth;
                    density = occlusion;
                }
/*
                float depthDiff = abs(cloudFactor.depth - ss.depth);
                if (cloudFactor.density > 0.2)
                {
                    if (ss.isMaxPlane < 0.5 && cloudFactor.occlusion - cloudFactor.density > 0 && cloudFactor.density > 0.01)
                    {
                        if (depthDiff > threshold)
                        {
                            ray.to = min(far, ray.to);  // make correct density
                            cloudFactor = PhysicsCloud(ray, ss);
                        }
                    }
                    else if (cloudFactor.density * depthDiff > threshold)
                    {
                        ray.to = min(far, ray.to);  // make correct density
                        cloudFactor = PhysicsCloud(ray, ss);
                    }
                }
*/
                cloudFactor.scattering = scattering;
                cloudFactor.occlusion = occlusion;
                cloudFactor.density = density;
                cloudFactor.depth = scattering;
                return lerp(PackCloud(cloudFactor), captureTexCol, _CaptureMix);
            }
			ENDHLSL
		}

		Pass
		{
			HLSLPROGRAM
			#pragma vertex MassiveCloudsVert
			#pragma fragment Frag
            #pragma shader_feature _HORIZONTAL_ON

            #include "MassiveCloudsPhysicsCloud.hlsl"

            sampler2D _CaptureTexture;
            float     _CaptureMix;

            float4 Frag(v2f i) : SV_Target
            {
                PrepareSampler();
                PrepareLighting();
                ScreenSpace ss = CreateScreenSpace(i.uv);
                float4 cloudTexCol = MC_TEX2DPROJ(_MainTex, i.uv);
                CloudFactor cloudFactor = UnpackCloud(cloudTexCol);
                float4 captureTexCol = MC_TEX2DPROJ_ST(_CaptureTexture, i.uv, _MainTex_ST);
                CloudFactor captureFactor = UnpackCloud(captureTexCol);
        
            #ifdef _HORIZONTAL_ON
                HorizontalRegion region = CreateRegion();
                Ray ray = CalculateHorizontalRayRange(ss, region);
            #else
                Ray ray = CalculateSphericalRayRange(ss);
            #endif
                if (ray.to <= 0) return float4(0, 0, 0, 0);
                float far = lerp(ss.maxDist, _MaxDistance, ss.isMaxPlane);
                float density = cloudFactor.density;
                float scattering = cloudFactor.scattering;
                if (ray.from > far)
                    return float4(0, 0, cloudFactor.occlusion, ss.depth);
                if (ss.isMaxPlane > 0)
                {
                    scattering = cloudFactor.depth;
                    density = cloudFactor.occlusion;
                }

                cloudFactor.scattering = lerp(scattering, captureFactor.scattering, _CaptureMix);
                cloudFactor.density = lerp(density, captureFactor.density, _CaptureMix);
                cloudFactor.occlusion = lerp(cloudFactor.occlusion, captureFactor.occlusion, _CaptureMix);
                cloudFactor.depth = lerp(cloudFactor.depth, captureFactor.depth, _CaptureMix);
                return PackCloud(cloudFactor);
            }
			ENDHLSL
		}
	}
}
