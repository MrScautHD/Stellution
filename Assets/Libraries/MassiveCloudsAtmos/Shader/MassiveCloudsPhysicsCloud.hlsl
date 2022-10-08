#ifndef MASSIVE_CLOUDS_PHYSICS_CLOUD_INCLUDED
#define MASSIVE_CLOUDS_PHYSICS_CLOUD_INCLUDED

#include "MassiveCloudsCommon.hlsl"
#include "Includes/MassiveCloudsScreenSpace.hlsl"
#include "Includes/MassiveCloudsLight.hlsl"
#include "MassiveCloudsRaymarch.hlsl"
#include "MassiveCloudsPhysics.hlsl"
#include "MassiveCloudsProgressive.hlsl"

float     _CloudOcclusion;
float     _Mie;
float     _NearSoftnessScale;
float _Resolution;            
float _ShadingDistance;            
float _Warp;            

struct CloudFactor
{
    float scattering;
    float density;
    float occlusion;
    float depth;
};

CloudFactor UnpackCloud(float4 col)
{
    CloudFactor cloudFactor;
    cloudFactor.scattering = col.r;
    cloudFactor.density    = col.g;
    cloudFactor.occlusion  = col.b;
    cloudFactor.depth  = col.a;
    return cloudFactor;
}

float4 PackCloud(CloudFactor cloudFactor)
{
    return float4(
        cloudFactor.scattering,
        cloudFactor.density,
        cloudFactor.occlusion,
        cloudFactor.depth);
}

float Sample(float3 pos)
{
    float dist = length(pos - _WorldSpaceCameraPos);
    float distFactor = (1 + (_NearSoftnessScale - 1.0) * saturate((250. - dist) / 250.));

    float3 texPos =  pos * 0.00006 * _Octave + _OctaveTexOffset;
//    float4 dtex = tex3Dlod(_VolumeTex, float4(texPos, 0)) - float4(0.5,0.5,0.5,0.5);
    float4 dtex = SampleVolumeTexture(texPos) - float4(0.5,0.5,0.5,0.5);

    float3 wpos = pos + 1000 * _Warp * dtex.aaa;
    
    float base = ClipHorizontalDensity(pos, SampleBaseDensity(wpos));
    float base2 = base * base;

    float4 s = base.xxxx * _Sculptures * dtex;
    base += dot(s, float4(1, 1, 1, 1));

    // Softness
    float softness        = saturate(_Softness * distFactor);
    float softnessnFactor = 1 - 0.9 * softness;
    float densityFactor   = 0.9 * softness;
    float densityScale    = 1. - ClipHorizontalDensity(pos, _Density);

    float density = pow(base, softnessnFactor) * softnessnFactor;
    density = saturate(density - (1 - densityFactor) * densityScale);
    density = pow(density, 0.1 + densityFactor);

    base = density * softnessnFactor;
    return base;
}

inline float CalculateLightOcclusion(
    float     baseDensity,
    float3    pos,
    float3    forward,
    ScreenSpace ss,
    float     qualityFactor)
{
    float sunset  = 2 - abs(dot(forward.y, float3(0, 1, 0)));
    float bottom  = _FromHeight;
    float top     = bottom + _Thickness;
    float dist    = length(pos - ss.cameraPos);
    float distFactor = floor(min(floor(qualityFactor * _LightingQuality), max(0, log10(dist) / 4)));
    
    float fromStep = distFactor;
    float maxStep = qualityFactor * _LightingQuality;
    
    float d = dot(ss.rayDir, MassiveCloudsLightDirection);

    float progression = _ShadingDistance * 100;
    float step        = max(1, progression) * 2;
    step /= (1 + 2 * baseDensity);
    float density = baseDensity;
    
    float i = 0;

#if defined(UNITY_COMPILER_HLSL)
    [loop]
#endif
    for (; i < maxStep; ++i)
    {
        float3 rayPos = pos + progression * forward;
        if (rayPos.y >= top || rayPos.y <= bottom) break;
        float scattering = pow(1 - _LightScattering, 4);
        density += saturate(Sample(rayPos) * scattering) * step;
        if (density >= 1) break;
        step *= 2 / (1 + 2 * density);
        progression += step;
    }
    return pow(saturate(density), 2);
}

inline float PhysicsCloudStep(float progression)
{
    float baseStep = 5;
    return baseStep + max(0, 0.01 * progression);
}

CloudFactor PhysicsCloud(
    Ray    ray,
    ScreenSpace ss)
{

#if defined(_MASSIVE_CLOUDS_PROGRESSIVE_PASS)
    float skipQuality = 3.0;
#else
    float skipQuality = 1.0;
#endif

    float noise           = rand(ss.uv);
    float progression     = ray.from;
    float nf              = ProgressiveShuffler(noise);
    float step            = PhysicsCloudStep(progression);
    float skip            = 0;
    float skipProgression = 50;
    
    float  scattering   = 0;
    float  farScattering= 0;
    float  occludedTo   = min(ray.to, lerp(ss.maxDist, _MaxDistance, ss.isMaxPlane));
    float  dim          = 0;
    float  farDim          = 0;
    float  occludedDim  = 0;
    float  mie          = saturate(dot(MassiveCloudsLightDirection, ss.rayDir));
    float3 lightd       = lerp(MassiveCloudsLightDirection, float3(0, 1, 0), 0.05);
    float  maxTo        = min(occludedTo, _MaxDistance);
    float  totalDensity = 0;
    bool   done = (progression > ray.to || progression > _MaxDistance);

#if defined(UNITY_COMPILER_HLSL)
    [loop]
#endif
    for (float i = 0; !done; ++i)
    {
        done = (progression > ray.to || progression > _MaxDistance);
            
        float3 rayPos  = ss.cameraPos + progression * ss.rayDir;
        float  density = Sample(rayPos);

        if (density <= 0.00001)
        {
            skip = 1;
            float nscale = 1 + lerp(0.5 * nf, 0, saturate(progression / 5000));
            step = nscale * min(ray.length / 2, PhysicsCloudStep(skipQuality * progression));
            progression += step;
        }
        else
        {
            if (skip >= 1)
            {
                progression -= step;
                density = 0;
                rayPos  = ss.cameraPos + progression * ss.rayDir;
                skipProgression = 50;
                skip = 0;
            }
    
            if (density > 0)
            {
                float lightQualityFactor = 3 - farScattering;
//                if (progression >= occludedTo) lightQualityFactor = 0;
                float lo = CalculateLightOcclusion(totalDensity, rayPos, lightd, ss, lightQualityFactor);
    
                float shadingInv = max(0.0001, 1 - _Shading);
                float lighting   = (1 - pow(lo, shadingInv)) * (exp(-density))
                                 * lerp((1 - pow(lo, shadingInv)) * (1. - exp(-2 * density)), 0.1, _Shading)
                                 ;
                if (progression < occludedTo)
                {
                    scattering += (1 - scattering) * saturate(lighting * density);
                    farScattering = scattering;
                    dim += saturate((1.01 - dim) * saturate(density));
                    farDim = dim;
                    occludedDim += saturate((1.01 - occludedDim) * saturate(density));
                }
                else
                {
                    farScattering += (1 - farScattering) * saturate(lighting * density);
                    farDim += saturate((1.01 - farDim) * saturate(density));
                    occludedDim += saturate((1.01 - occludedDim) * saturate(4 * density));
                }
                totalDensity += density;
            }

            #if defined(_MASSIVE_CLOUDS_PROGRESSIVE_PASS)
            if (progression >= occludedTo) break;
            #endif

            if (occludedDim >= 0.9999) break;
            step = min(PhysicsCloudStep(skipProgression), 200);
            progression += step;
            skipProgression += step;
        }
    }

    scattering = saturate(2 * (_Lighting + _CloudIntensityAdjustment) * scattering);
    scattering = scattering + _Mie * pow(mie, 2) * dim / 10;
    farScattering = saturate(2 * (_Lighting + _CloudIntensityAdjustment) * farScattering);
    farScattering = farScattering + _Mie * pow(mie, 2) * farDim / 10;

    float fadeFrom = _MaxDistance / 4;
    float fadeTo = _MaxDistance;
    float fade = 1 - saturate(max(0, progression - fadeFrom) / (fadeTo - fadeFrom));
    fade = lerp(fade, 1, length(_MaxDistance - ray.from) / _MaxDistance);
    float scatteringFade = lerp(0, 1, saturate(max(0, (_MaxDistance - ray.from)) / _MaxDistance));

    CloudFactor cloudFactor;
    cloudFactor.scattering = pow(scatteringFade, 2) * scattering;
    cloudFactor.density = fade * dim;
    cloudFactor.occlusion = fade * occludedDim;
    cloudFactor.depth = pow(scatteringFade, 2) * farScattering;
    return cloudFactor;
}


#endif