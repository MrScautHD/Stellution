#ifndef MASSIVE_CLOUDS_PROCEDURAL_SKY_INCLUDED
#define MASSIVE_CLOUDS_PROCEDURAL_SKY_INCLUDED

#include "Includes/MassiveCloudsLight.hlsl"
#include "Includes/MassiveCloudsScreenSpace.hlsl"
#include "MassiveCloudsPhysics.hlsl"

half  _SkyExposure;
half3 _GroundColor;
float _GroundScattering;
float _SunSize;
float _SunSizeConvergence;
float _Saturation;
float _AtmosphereThickness;
float _Gradation;
float _SkyIntensity;

samplerCUBE _MassiveCloudsHdri;
float _MassiveCloudsHdriIntensity;
float _MassiveCloudsHdriMix;

samplerCUBE _MassiveCloudsSecondaryHdri;
float _MassiveCloudsSecondaryIntensity;
float _MassiveCloudsSecondaryMix;
float _MassiveCloudsSecondaryWeight;

static const float3 WaveLength       = float3(.65, .53, .45);
static const float  ScatteringFactor = 0.000002;
static const float3  AttenuateFactor  = 0.0000004 * float3(0.5, 1.0, 0.8);
static const float  AttenuateScale   = 0.10;
static const float  BaseThickness    = 100000.0;
static const float  Radius           = 6300000.0;

float3 RayleighFactor(float3 lightDir, float3 viewDir, float3 waveLength)
{
    float k = 1;
    float outCos = saturate(dot(lightDir, viewDir));
    float lf = pow(saturate(1 - dot(viewDir, float3(0,1,0))), 4);
    return k * (1 + lerp(outCos * outCos, 1, lf)) / pow(waveLength, 4);
}

float3 fix(float3 v)
{
    #if UNITY_COLORSPACE_GAMMA
        return pow(v, .454545);
    #else
        return v;
    #endif
}

float4 SunDisk(float3 rayDir)
{
    #if _REMOVE_SUN
        return 0;
    #else
        float sunFactor = saturate(dot(-_MassiveCloudsSunLightDirection, rayDir));
        sunFactor = abs(1.0 + 0.9801 - 2.0 * 0.99 * sunFactor);
        sunFactor = pow(sunFactor, pow(_SunSize, 0.65));
        sunFactor = (0.006677) * (1.0 + sunFactor) / max(sunFactor, 0.0000001);
        return pow(min(sunFactor, 1.0), _SunSizeConvergence) * float4(_MassiveCloudsSunLightColor, 1);
    #endif
}

float SurfaceLength(float3 rayDir, float eye)
{
    float th = 1.5 * _AtmosphereThickness * BaseThickness;
    float r = th + Radius;
    float e = eye + Radius;
    float t = e / r;
    float dh = r * cos(t * 3.141592 / 2.0);
    float upDot = dot(rayDir, float3(0.0, 1.0, 0.0));
    float l = lerp(dh, th, abs(upDot));
    return l * (1 - 0.5 *  abs(upDot));
}


float3 SaturateColor(float3 col, float v)
{
    float maxCol = max(col.r, max(col.g, col.b));
    float minCol = min(col.r, min(col.g, col.b));
    float avgCol = (maxCol + minCol) * 0.5;
    float3 d = (col - float3(avgCol, avgCol, avgCol)) * v;
    return max(float3(0,0,0), (col + d));
}


float4 ProceduralSkyFrag(float3 rayDir, float exposureMultiplier)
{
    PrepareLighting();

    float  grad        = 0.5 + 0.5 * dot(-_MassiveCloudsSunLightDirection, rayDir);
    grad = pow(grad, _Gradation);
    float  eye         = _WorldSpaceCameraPos.y;

    float  lightDot    = dot(-_MassiveCloudsSunLightDirection, float3(0, 1, 0));
    float3 rayleigh    = RayleighFactor(-_MassiveCloudsSunLightDirection, rayDir, WaveLength);

    float  rayDot      = dot(rayDir, float3(0, 1, 0));
    float  rayLength   = max(0, SurfaceLength(rayDir, eye));
    rayleigh *= 1 + (1 - rayDot);

    float  lightLength = abs(SurfaceLength(-_MassiveCloudsSunLightDirection, eye));
    float3 scattering  = lerp(1, rayleigh, grad) * (1 - exp(- ScatteringFactor * rayLength));
    float3 attenuate   = AttenuateScale * exp(-AttenuateFactor * scattering * lightLength);

    float3 day   = scattering * attenuate;
    float3 night = lerp(0.01, 0.01, saturate(-lightDot*100.0)) * rayleigh;
    float  nightFactor = saturate(-lightDot*50.0);
    float3 sky = pow(lerp(day, night, nightFactor), 2.2);

    float  groundStrength = 1 - 0.9 * saturate(-rayDot);
    float  groundNightFactor = smoothstep(-0.2, 0.05, -lightDot);
    float  groundGrad        = pow(FakeMie(MassiveCloudsLightDirection, rayDir), 2);
    float3 ground = lerp(0.5, 0.01, groundNightFactor) * _GroundColor * groundStrength + 0.005 * MassiveCloudsLightColor * groundGrad * _GroundScattering;

    float3 hdri = texCUBE(_MassiveCloudsHdri, rayDir).rgba * _MassiveCloudsHdriIntensity;
    float3 secondaryHdri = texCUBE(_MassiveCloudsSecondaryHdri, rayDir).rgba * _MassiveCloudsSecondaryIntensity;
    float3 hdriColor = lerp(sky * max(_SkyExposure, 0.0000001), hdri, _MassiveCloudsHdriMix);
    float3 secondaryHdriColor = lerp(sky * max(_SkyExposure, 0.0000001), secondaryHdri, _MassiveCloudsSecondaryMix);

    float3 final = lerp(
        lerp(hdriColor, secondaryHdriColor, _MassiveCloudsSecondaryWeight),
        ground * max(_SkyExposure, 0.0000001),
        pow(saturate(1 - rayDir.y), 20));
    final += SunDisk(rayDir);
    final = fix(final * exposureMultiplier * _SkyIntensity);

    return float4(SaturateColor(final, _Saturation), 1.0);
}
            

#endif