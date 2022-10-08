#ifndef MASSIVE_CLOUDS_ATMOSPHERE_INCLUDED
#define MASSIVE_CLOUDS_ATMOSPHERE_INCLUDED

#include "MassiveCloudsCommon.hlsl"
#include "Includes/MassiveCloudsScreenSpace.hlsl"
#include "Includes/MassiveCloudsLight.hlsl"
#include "MassiveCloudsPhysics.hlsl"
#include "MassiveCloudsProgressive.hlsl"
#include "MassiveCloudsRaymarch.hlsl"
#include "MassiveCloudsPhysicsCloud.hlsl"

float     _SunShaft;
float     _GodRay;
float     _GodRayStartDistance;
float     _ShaftQuality;
float     _GodRayQuality;
sampler2D _CloudsTexture;
half4     _CloudsTexture_ST;
float4    _CloudsTexture_TexelSize;

sampler2D _CameraGBufferTexture2;

inline float Raymarch(
    half3 screnCol,
    float3 from,
    float3 forward,
    float  far,
    float  rayLength,
    int    iteration)
{
    float3 ray;
    float4 col          = half4(screnCol.rgb, 0);

    float  totalDensity = 0;
    float  totalLight   = 0;

    // pre fade
    float cameraDist = length(from - _WorldSpaceCameraPos);
    float fade = 1 - smoothstep(_FromDistance, _MaxDistance, _Fade * cameraDist);
    HorizontalRegion horizontalRegion = CreateRegion();
    #if defined(UNITY_COMPILER_HLSL)
    [loop]
    #endif
    for (int i = 0; i < iteration; ++i)
    {
        ray = rayLength * i * forward + from;
        
        #if defined(_HORIZONTAL_OsN)
        float isClip = step(horizontalRegion.height - 0.001, ray.y);
        #else
        float isClip = 1;
        #endif

    
        if (isClip == 0) continue;

        float  density = isClip * Sample(ray);
        totalDensity = totalDensity + 0.001 * density * rayLength;
    }
//    totalDensity = saturate(totalDensity / iteration;

    return saturate(totalDensity);
}

float VolumetricShadowAttn(float3 worldPos, ScreenSpace ss, HorizontalRegion horizontalRegion, int iter)
{
    float3 lightDir = normalize(MassiveCloudsLightDirection);
    if (worldPos.y > (horizontalRegion.height + horizontalRegion.thickness/ 2)) return 0;
    float3 dCameraPos = float3(0, -(_RelativeHeight) * ss.cameraPos.y, 0);
    float upDotLight = saturate(dot(float3(0,1,0), lightDir));
    float bottomDist = max(0, horizontalRegion.height - worldPos.y - dCameraPos.y) / upDotLight;
    float topDist = max(0, horizontalRegion.height + horizontalRegion.thickness - worldPos.y - dCameraPos.y) / upDotLight;
    float thickness = topDist - bottomDist;
    float mid = bottomDist + (0.05 + 0.2 * horizontalRegion.softness[0]) * thickness;
    bottomDist = mid - 0.2 * thickness * 0;
    topDist = mid + 0.8 * thickness * 1 + 1;
    float shadowIter = iter;
    float shadow = Raymarch(float4(0,0,0,0),
                                    dCameraPos + worldPos + bottomDist * lightDir,
                                    lightDir,
                                    topDist,
                                    (topDist - bottomDist) / shadowIter,
                                    shadowIter);
    float att = shadow;
    return att;
}

Ray CalculateVolumetricRayRange(
          ScreenSpace      ss,
          HorizontalRegion region)
{
    HorizontalShapeView hsv = CalculateHorizontalShapeView(ss, region);

    Ray ray;

    ray.max    = hsv.FarDistance;

    if (hsv.CameraY >= region.height)
    {
        ray.from   = hsv.CameraToBottomVDistance;
        ray.to     = hsv.MaxDistance;
    }
    else
    {
        ray.from   = 0;
        if (hsv.FarDistance <= 0) ray.to = hsv.MaxDistance;
        else                      ray.to = min(hsv.MaxDistance, hsv.NearDistance);
    }

    ray.length = max(0, hsv.FarDistance - hsv.NearDistance);
    ray.length = min(_MaxDistance / 4, hsv.Length);
    return ray;
}

inline float CloudOcclusion(ScreenSpace ss, float2 uv)
{
    float4 col = MC_TEX2DPROJ_ST(_CloudsTexture, float4(uv, 0, 1), _MainTex_ST);
    return pow(col.b, 2);
}

inline float SceneLightOcclusion(ScreenSpace ss)
{
#if _DEFERRED_SHADING
    float3 normal = MC_TEX2DPROJ_ST(_CameraGBufferTexture2, ss.uv, _MainTex_ST).xyz;
    normal = 2 * (normal - (0.5, 0.5, 0.5));
    return saturate(dot(normalize(MassiveCloudsLightDirection), normalize(normal)));
#else
    return 1;
#endif
}

float SunSet()
{
    return smoothstep(0.2, 0.3, MassiveCloudsLightDirection.y);
}

float Shadow(ScreenSpace ss, float noise, CloudFactor cloud)
{
    if (_Shadow <= 0) return 0;
    HorizontalRegion region = CreateRegion();
    return SceneLightOcclusion(ss) * SunSet() * VolumetricShadowAttn(ss.worldPos, ss, region, 3);
}

inline float VolumetricLightStep(float noise, float progression)
{
    float qualityFactor = 150.0 - 140.0 * _GodRayQuality;
//    float nf = 1 + lerp(ProgressiveShuffler(noise), 0, saturate(progression/1000));
    float nf = 1 + ProgressiveShuffler(noise);
    return max(1, lerp(100, 10, _GodRayQuality) * log10(nf * progression));
    return qualityFactor + nf * progression;
}

float VolumetricLight(ScreenSpace ss, float noise, CloudFactor cloud)
{
    if (_GodRay <= 0) return 0;
    
    HorizontalRegion region = CreateRegion();
    Ray ray = CalculateVolumetricRayRange(ss, region);

    float progression = max(min(_GodRayStartDistance, ss.maxDist), 0);
    float maxTo       = min(MAX_DIST, ray.to);
    float fakeMie     = FakeMie(MassiveCloudsLightDirection, ss.rayDir);
    float scattering  = AtmosphericScattering(ss, progression);

    float step = VolumetricLightStep(noise, progression);
    float totalOcclusionFactor;
    float sunSet = SunSet();
    float rayScattering = 0;
    for (float i = 0; ; ++i)
    { 
        if (progression >= maxTo)
        {
            progression = maxTo;
            break;
        }
        progression += step;
        float3 worldPos       = ss.cameraPos + progression * ss.rayDir;
        float  occlusion      = VolumetricShadowAttn(worldPos, ss, region, 2);
        float  occlusionFactor = saturate(1 - 10000 * occlusion);

        if (ss.cameraPos.y > region.height)
        {
            if (ss.rayDir.y < 0)
                totalOcclusionFactor = lerp(occlusionFactor, 1, cloud.density);
            else
                break;
        }
        else
            totalOcclusionFactor = occlusionFactor;

        rayScattering += lerp(1, totalOcclusionFactor, sunSet) * AtmosphericScattering(ss, step);
        step = VolumetricLightStep(noise, progression);
//        step += max(1, d);
//        step *= 1.1;
    }
    
    scattering += rayScattering;

    float rayDist     = lerp(min(ss.maxDist, MAX_DIST), MAX_DIST, ss.isMaxPlane);
    rayDist = lerp(rayDist, MAX_DIST, cloud.density);
    scattering += AtmosphericScattering(ss, max(0.0, rayDist - progression));
    
    scattering = pow(scattering, 2);

    return _GodRay * scattering;
}

float Shaft(ScreenSpace ss, float currentShaft, float noise)
{
    if (_SunShaft <= 0) return 0;

    float3 posCS     = WorldPosToCameraFarClipPlanePos(ss.cameraPos + ss.rayDir * 100000.);
    float3 posSunCS  = WorldPosToCameraFarClipPlanePos(ss.cameraPos + MassiveCloudsLightDirection * 100000.);
    float2 sunUV     = CalculateForwardUV(MassiveCloudsLightDirection);
    float  sunDistUV = length(sunUV - ss.uv);
    float2 sunUVDir  = normalize(posSunCS.xy - posCS.xy);
    float  baseStep  = lerp(0.1, 0.005, _ShaftQuality);
    float  step      = 0;

    sunUVDir.y /= _ScreenParams.y / _ScreenParams.x;;
    sunUVDir = normalize(sunUVDir);

    float shaft = 0;
    #if defined(UNITY_COMPILER_HLSL)
    [loop]
    #endif
    for (float i = 1; i > 0; i -= step)
    {
        step = lerp(0.005, baseStep, pow(shaft, 0.5));
        step = step + step * ProgressiveShuffler(noise);

        float  p  = sunDistUV * i;

        if (p >= 0.5)
            continue;

        float2 uv = ss.uv + sunUVDir * p;

        if (uv.x <= 0 || uv.x >= 1.0 || uv.y <= 0 || uv.y >= 1.0)
            continue;

        uv = saturate(uv);

        ScreenSpace oclusionSS     = CreateScreenSpace(float4(uv, 0, 1));
        float4      cloudColor     = MC_TEX2DLOD_ST(_CloudsTexture, float4(uv, 0, 1), _MainTex_ST);
        CloudFactor cloudFactor    = UnpackCloud(cloudColor);
        float dotRayDir            = dot(MassiveCloudsLightDirection, oclusionSS.rayDir);
        float shaftRayDist         = lerp(oclusionSS.maxDist, MAX_DIST, oclusionSS.isMaxPlane);
        float shaftFactor          = shaftRayDist / 1000 * step;
        float shaftStrength        = _SunShaft * pow(saturate(dotRayDir), 4) * shaftFactor;
        float cloudOcclusionFactor = (oclusionSS.isMaxPlane < 0.98) ? 0 : 1 - pow(cloudFactor.occlusion, 4);

        shaft += (1 - shaft) * pow(saturate(1 - p), 5) * saturate(cloudOcclusionFactor) * shaftStrength;

        if (shaft >= 1)
            break;
    }

    shaft = saturate(pow(shaft, 0.5));
    return SafeLerp(currentShaft, shaft);
}
            
float4 AtmosphereFragment(AtmosphereFactor atmosphereFactor, ScreenSpace ss, float noise, CloudFactor cloudFactor)
{
    float  maxDist               = lerp(ss.maxDist, MAX_DIST, ss.isMaxPlane);
    float  rayDist               = lerp(maxDist, MAX_DIST, cloudFactor.density);
    float  atmosphericScattering = AtmosphericScattering(ss, rayDist);
    float  scattering            = lerp(atmosphericScattering, 0, _GodRay);

    #ifdef _HORIZONTAL_ON 
        scattering += VolumetricLight(ss, noise, cloudFactor);
    #endif

    float cloudOcclusionFactor = saturate(1 - _CloudOcclusion * cloudFactor.density * cloudFactor.density);
    scattering *= cloudOcclusionFactor;
    atmosphereFactor.scattering = pow(scattering, 0.5);
    float shadow = Shadow(ss, noise, cloudFactor);
    shadow = pow(shadow, 0.8);
    atmosphereFactor.depth  = ss.depth;
    atmosphereFactor.shadow = shadow;
    #if defined(MASSIVE_CLOUDS_ADAPTIVE)
        atmosphereFactor.shaft  = atmosphereFactor.shaft;
    #else
        atmosphereFactor.shaft  = Shaft(ss, atmosphereFactor.shaft, noise);
    #endif

    return PackAtmosphere(atmosphereFactor);
}


#endif