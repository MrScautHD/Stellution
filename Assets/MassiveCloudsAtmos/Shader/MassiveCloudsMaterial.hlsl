#ifndef MASSIVE_CLOUDS_MATERIAL_INCLUDED
#define MASSIVE_CLOUDS_MATERIAL_INCLUDED

#include "Includes/MassiveCloudsLight.hlsl"
#include "Includes/MassiveCloudsSampler.hlsl"
#include "Includes/MassiveCloudsPostProcess.hlsl"
#include "MassiveCloudsRaymarch.hlsl"
#include "MassiveCloudsHeightFog.hlsl"

sampler2D _BackgroundTexture;
half4     _BackgroundTexture_ST;

struct appdata
{
    float4 vertex : POSITION;
    float4 uv : TEXCOORD0;
};

struct v2f
{
    float4 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
    float3 worldpos : TEXCOORD1;
};

#ifdef MASSIVE_CLOUDS_HLSL

    inline float4 ComputeGrabScreenPos (float4 pos) {
        #if UNITY_UV_STARTS_AT_TOP
        float scale = -1.0;
        #else
        float scale = 1.0;
        #endif
        float4 o = pos * 0.5f;
        o.xy = float2(o.x, o.y*scale) + o.w;
        #ifdef UNITY_SINGLE_PASS_STEREO
        o.xy = TransformStereoScreenSpaceTex(o.xy, pos.w);
        #endif
        o.zw = pos.zw;
        return o;
    }

#endif

v2f MassiveCloudsVert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.worldpos = mul (UNITY_MATRIX_M, v.vertex).xyz;
    o.uv = ComputeGrabScreenPos(o.vertex);
    return o;
}

half4 MassiveCloudsFragment(v2f i) : SV_Target
{
    PrepareSampler();
    PrepareLighting();

    ScreenSpace ss = CreateScreenSpace(i.uv / i.uv.w);

#ifdef _HORIZONTAL_ON
    HorizontalRegion region = CreateRegion();
    Ray ray = CalculateHorizontalRayRange(ss, region);
#else
    Ray ray = CalculateSphericalRayRange(ss);
#endif

#if defined(USING_STEREO_MATRICES)
    half4 screenCol = tex2Dproj(_BackgroundTexture, UnityStereoScreenSpaceUVAdjust(i.uv, _BackgroundTexture_ST));
#else 
    half4 screenCol = tex2Dproj(_BackgroundTexture, i.uv);
#endif

    #if defined(_SHADOW_ON) && defined(_HORIZONTAL_ON)
    ScreenSpaceShadow(screenCol, ss.worldPos, ss);
    #endif

    #if defined(_HEIGHTFOG_ON)
    HeightFogFragment(screenCol, ss, 1);
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

    col = PostProcess(col);
    return lerp(float4(screenCol.rgb, 1), float4(col.rgb, 1), col.a);
}

#endif