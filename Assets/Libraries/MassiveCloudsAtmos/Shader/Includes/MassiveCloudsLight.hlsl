#ifndef MASSIVE_CLOUDS_LIGHT_INCLUDED
#define MASSIVE_CLOUDS_LIGHT_INCLUDED

#include "MassiveCloudsSampler.hlsl"
#include "MassiveCloudsInput.hlsl"
#include "PipelineDependent/MassiveCloudsExposure.hlsl"
            
float3 MassiveCloudsLightColor;
float3 MassiveCloudsNightLightColor;
float3 MassiveCloudsLightDirection;
float3 MassiveCloudsNightLightDirection;
float MassiveCloudsLightIntensity;
float DayOrNight;

void PrepareLighting()
{
    PrepareExposure();
    MassiveCloudsLightDirection = -_MassiveCloudsSunLightDirection;
    MassiveCloudsNightLightDirection = -_MassiveCloudsMoonLightDirection;

    MassiveCloudsLightColor = _MassiveCloudsSunLightColor * ExposureMultiplier();
    MassiveCloudsNightLightColor = _MassiveCloudsMoonLightColor * ExposureMultiplier();

    MassiveCloudsLightIntensity = _MassiveCloudsSunLightIntensity;
 
    DayOrNight = smoothstep(0, 0.1, -MassiveCloudsLightDirection.y);
    MassiveCloudsLightDirection = lerp(
        MassiveCloudsLightDirection,
        MassiveCloudsNightLightDirection,
        DayOrNight);
    MassiveCloudsLightColor = lerp(
        MassiveCloudsLightColor,
        MassiveCloudsNightLightColor,
        DayOrNight);
    MassiveCloudsLightIntensity = lerp(
        _MassiveCloudsSunLightIntensity,
        _MassiveCloudsMoonLightIntensity,
        DayOrNight);

    _ShadingDist =  (_Shading * _Shading * _Thickness);
    _LightingIteration = floor(1 + 10 * _LightingQuality);
}

inline float CalculateSlope(
    float     scale,
    float3    pos,
    float     baseDesnsity,
    float3    forward,
    float     shadingDist)
{
    float  density     = SampleDetailedDensity(pos + shadingDist * forward);
    return saturate((baseDesnsity - density) / 10 * (1.01 - _LightSmoothness));
}

inline half CalculateLightFactor(float slope)
{
    return slope * 30 + slope * 30 * _ShadingDist;
}

inline float CalculateLightFactor(
    float3    pos,
    float3    forward,
    float     baseDesnsity,
    float     totalRayLength,
    float     shaft)
{
    float iteration = _LightingIteration;
    float totalDensity = baseDesnsity;
    float scatteringFactor = (5 - 4.999 * _LightScattering);
    
#if defined(UNITY_COMPILER_HLSL)
    [loop]
#endif
    float3 rayPos = pos;
    for (int i = 1; i <= iteration; i+=1)
    {
        float rayLength = totalRayLength / iteration;
        rayPos += rayLength * forward;
        float  density;
        density = SampleDetailedDensity(rayPos);
        density = density;
        totalDensity += pow(density, 1 + 0.5 * shaft) * scatteringFactor * rayLength;
    }
    return saturate(baseDesnsity * 10) * totalDensity * (1 + 5 * _Softness);
}

inline float CalculateAuthenticLightFactor(
    float3    pos,
    float3    forward,
    float     baseDesnsity,
    float     totalRayLength,
    float     shaft)
{
    float iteration = _LightingIteration;
    float totalDensity = baseDesnsity;
    
#if defined(UNITY_COMPILER_HLSL)
    [loop]
#endif
    float3 rayPos = pos;
    for (int i = 1; i <= iteration; i+=1)
    {
        float rayLength = totalRayLength / iteration;
        rayPos += rayLength * forward;
        float  density     = SampleDetailedDensity(rayPos);
        density     = density * rayLength;
        totalDensity += pow(density, 1 + 0.5 * shaft) / (1 + 1000 * pow(_LightScattering, 2));
    }
    return totalDensity * (1 + 5 * _Softness) * 50;
}

inline float4 GlobalLighting(float4 col, float3 forward)
{
    // GlobalLighting
    float directionalFactor = saturate((1 + (dot(MassiveCloudsLightDirection, forward))) * 0.5);
    float rangeFactor = pow(directionalFactor, 40 * (1 - _GlobalLightingRange));
    float globalLightingFactor = _GlobalLighting * rangeFactor;

    // EdgeLighting
    float densityFactor = pow(smoothstep(0.0, col.a, 0.6), 4);
    float edgeLightingFactor = (rangeFactor * rangeFactor) * max(0, _EdgeLighting * (1 - _GlobalLighting));
    col.rgb = col.rgb + 
        col.rgb * globalLightingFactor + 
        edgeLightingFactor * densityFactor * normalize(MassiveCloudsLightColor);

    return col;
}

#endif