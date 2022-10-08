Shader "MassiveCloudsMix"
{
	Properties
	{
	    [HideInInspector]
		_MainTex   ("Texture", 2D) = "white" {}
        [Toggle]
        _HORIZONTAL         ("Horizontal?", Float)              = 0
        [Toggle]
        _RelativeHeight     ("RelativeHeight?", Float)          = 0
		_Thickness          ("Thickness", Range(0, 10000))      = 50
		_FromHeight         ("FromHeight", Range(0, 5000))      = 1
		_MaxDistance        ("MaxDistance", Range(0, 60000))    = 5000
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
            #pragma shader_feature _HORIZONTAL_ON

            #include "MassiveCloudsCommon.hlsl"
            #include "Includes/MassiveCloudsScreenSpace.hlsl"
            #include "Includes/MassiveCloudsLight.hlsl"
            #include "Includes/MassiveCloudsShape.hlsl"
            #include "Includes/PipelineDependent/MassiveCloudsExposure.hlsl"

            sampler2D _ScreenTexture;
            half4     _ScreenTexture_ST;
            
            half4 MassiveCloudsFragment(v2f i) : SV_Target
            {
                float d = CalculateDepth(i.uv);
                ScreenSpace ss = CreateScreenSpace(i.uv);

#ifdef _HORIZONTAL_ON
                HorizontalRegion region = CreateRegion();
                Ray ray = CalculateHorizontalRayRange(ss, region);
#else
                Ray ray = CalculateSphericalRayRange(ss);
#endif

#if defined(USING_STEREO_MATRICES)
                half4 screenCol = tex2Dproj(_ScreenTexture, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
                half4 texCol = tex2Dproj(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
#else 
                half4 screenCol = tex2Dproj(_ScreenTexture, i.uv);
                half4 texCol = tex2Dproj(_MainTex, i.uv);
#endif

                // RelativeHeight
                float3 fixedCameraPos = ss.cameraPos;
                fixedCameraPos.y *= (1 - _RelativeHeight);
                float3 rayPos = fixedCameraPos + ray.from * ss.rayDir;

                if (ss.isMaxPlane) return lerp(screenCol, texCol, texCol.a);
                if (ray.from >= ss.maxDist) return screenCol;
#if defined(_HORIZONTAL_ON)
                HorizontalRegion horizontalRegion = CreateRegion();
                float isClip =
                    step(horizontalRegion.height - 0.001, rayPos.y) *
                    step(rayPos.y, horizontalRegion.height + horizontalRegion.thickness + 0.001);

                float yDiff = abs(dot(ss.rayDir, float3(0, -1, 0))) * (ss.maxDist - ray.from);

                if (isClip == 0 || ray.from > 0 && yDiff < ray.from / 30)
                    return screenCol;
#endif
                return lerp(screenCol, texCol, texCol.a);
            }
			ENDHLSL
		}
	}
}
