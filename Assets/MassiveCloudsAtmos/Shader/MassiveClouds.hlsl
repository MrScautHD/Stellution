#ifndef MASSIVE_CLOUDS_INCLUDED
#define MASSIVE_CLOUDS_INCLUDED

#include "Includes/MassiveCloudsLight.hlsl"
#include "Includes/MassiveCloudsSampler.hlsl"

#include "MassiveCloudsRaymarch.hlsl"

sampler2D _ScreenTexture;
half4     _ScreenTexture_ST;
sampler2D _CloudTexture;

float4 MassiveCloudsFragment(v2f i) : SV_Target
{
    PrepareSampler();
    PrepareLighting();

    ScreenSpace ss = CreateScreenSpace(i.uv);

#ifdef _HORIZONTAL_ON
    HorizontalRegion region = CreateRegion();
    Ray ray = CalculateHorizontalRayRangeForStylized(ss, region);
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

    #if defined(_SHADOW_ON) && defined(_HORIZONTAL_ON)
    ScreenSpaceShadow(screenCol, ss);
    #endif

    #if defined(_HEIGHTFOG_ON)
    HeightFogFragment(screenCol, ss);
    #endif

#if !defined(_RENDERER_AUTHENTIC)
    // Optimize
    if (ray.from >= _MaxDistance) return screenCol;
    if (ray.to <= 0) return screenCol;
    float iterationScale = 1 + saturate(-0.5 + _Optimize * (ray.from/1000));
    float iteration = (_Iteration / iterationScale);
#else
    float iteration = _Iteration;
#endif

    // RelativeHeight
    float3 fixedCameraPos = ss.cameraPos;
    fixedCameraPos.y *= (1 - _RelativeHeight);

#if defined(_RENDERER_SURFACE)
    half4 col = WorldSpaceSurfaceRaymarch(screenCol,
                                    fixedCameraPos,
                                    ss.rayDir,
                                    ray,
                                    iteration);
#elif defined(_RENDERER_AUTHENTIC)
    half4 col = WorldSpaceAuthenticRaymarch(screenCol,
                                    fixedCameraPos,
                                    ss.rayDir,
                                    ray,
                                    iteration,
                                    ss); 
#else
    half4 col = WorldSpaceRaymarch(screenCol,
                                    fixedCameraPos,
                                    ss.rayDir,
                                    ray,
                                    iteration);
#endif

    float finalA = col.a;
    col = lerp(float4(texCol.rgb, finalA), float4(col.rgb, finalA), col.a);

    return col;
}
#endif