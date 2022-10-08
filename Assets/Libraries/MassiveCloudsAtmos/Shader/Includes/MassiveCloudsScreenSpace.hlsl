#ifndef MASSIVE_CLOUDS_SCREEN_SPACE_INCLUDED
#define MASSIVE_CLOUDS_SCREEN_SPACE_INCLUDED

#include "MassiveCloudsInput.hlsl"
#include "PipelineDependent/MassiveCloudsDepthSampler.hlsl"

struct ScreenSpace
{
    float3           cameraPos;
    float3           worldPos;
    float            maxDist;
    float            isMaxPlane;
    float3           rayDir;
    float            depth;
    float4           uv;
}; 


#if defined(MASSIVE_CLOUDS_MATERIAL_ON)

float4 CalculateWorldPos(float4 uv, float depth)
{
#if defined(USING_STEREO_MATRICES)
    float4 posProjection = float4(2 * (2 * (uv.x - 0.5 * unity_StereoEyeIndex)) - 1, 2 * uv.y - 1, 1, 1);
    float3 view          = mul(unity_CameraInvProjection, posProjection) * _ProjectionParams.z;
    view = view * depth;
    view.z *= -1; // revert z on all platform
#else
    float4 posProjection = float4(2 * uv.x - 1, 2 * uv.y - 1, 1, 1);
    float3 view          = mul(unity_CameraInvProjection, posProjection) * _ProjectionParams.z;
    view = view * depth;
    view.z *= -1; // revert z on all platform
#endif
    return mul(unity_CameraToWorld, float4(view, 1));
}

#else

float4 CalculateWorldPos(float4 uv, float depth)
{
    float4 posProjection = float4(- 1 + 2 * uv.xy, 1, 1);
    float3 view          = mul(unity_CameraInvProjection, posProjection).xyz * _ProjectionParams.z;
    view = view * depth;
    view.z *= -1; // revert z on all platform
    return mul(unity_CameraToWorld, float4(view, 1));
}

#endif

float2 CalculateUV(float3 worldPos, float depth)
{
    float3 cs = mul(unity_WorldToCamera, float4(worldPos, 1));
    cs.z *= -1;
    cs /= depth;
    float4 posProjection  = mul(unity_CameraProjection, float4(cs, 1) / _ProjectionParams.z);
    return (posProjection.xy + 1) / 2;
}

float3 PositionOnFarClipPlane(float3 pos)
{
    float3 cameraForward = mul((float3x3)unity_CameraToWorld, float3(0,0,1));
    float3 cameraToPos = pos - _WorldSpaceCameraPos;
    float3 dir = normalize(cameraToPos);
    float  dotCameraForward = dot(dir, cameraForward);
    return _WorldSpaceCameraPos + dir * _ProjectionParams.z / dotCameraForward;
}

float3 WorldPosToCameraFarClipPlanePos(float3 pos)
{
    return mul(unity_WorldToCamera, float4(PositionOnFarClipPlane(pos), 1));
} 

float2 CalculateForwardUV(float3 forward)
{
    float3 cs = WorldPosToCameraFarClipPlanePos(_WorldSpaceCameraPos + forward);
    cs.z *= -1;
    float4 posProjection  = mul(unity_CameraProjection, float4(cs, 1) / _ProjectionParams.z);
    return (posProjection.xy + 1) / 2;
}

float CalculateDepth(float4 uv)
{
    return SampleCameraDepth(uv);
}

ScreenSpace CreateScreenSpace(float4 uv)
{
    ScreenSpace ss;
    float       depth          = CalculateDepth(uv);
    float3      cameraPos      = _WorldSpaceCameraPos;
    float3      world          = CalculateWorldPos(uv, depth);
    float3      rayDir         = normalize(world.xyz - cameraPos);
    float       isMaxPlane     = smoothstep(0.9, 0.999, depth);
    float       maxDist        = length(world.xyz - cameraPos);
    
    ss.cameraPos  = cameraPos;
    ss.worldPos   = world;
    ss.isMaxPlane = isMaxPlane;
    ss.maxDist    = maxDist;
    ss.rayDir     = rayDir;
    ss.depth      = depth;
    ss.uv         = uv;

    return ss;
}

float3 Ramp(float3 screenCol, float factor)
{
    return screenCol +
        _RampStrength *
        tex2D( _RampTex, float2(1 - factor / _RampScale + _RampOffset * _RampScale, 0.5));
}

#endif