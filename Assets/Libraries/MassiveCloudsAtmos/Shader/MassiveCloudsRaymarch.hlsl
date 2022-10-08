#ifndef MASSIVE_CLOUDS_RAYMARCH_INCLUDED
#define MASSIVE_CLOUDS_RAYMARCH_INCLUDED

#include "Includes/MassiveCloudsLight.hlsl"
#include "Includes/MassiveCloudsSampler.hlsl"
#include "Includes/MassiveCloudsShape.hlsl"
#include "Includes/MassiveCloudsPostProcess.hlsl"
#include "MassiveCloudsShadow.hlsl"

half4 _BaseColor;
float  _Dissolve; 

// Ray Marching
float  _Iteration;

#if SHADEROPTIONS_PRE_EXPOSITION
float4 CloudColor() {
    return _BaseColor * ExposureMultiplier();
}
#else
float4 CloudColor() {
    return _BaseColor;
}
#endif

inline half4 WorldSpaceRaymarch(
    half3 screnCol,
    float3 cameraPos,
    float3 forward,
    Ray    ray,
    int    iteration)
{
    HorizontalRegion horizontalRegion = CreateRegion();
    screnCol = saturate(screnCol);
    float3 rayPos;
    float4 col          = half4(screnCol.rgb, 0);
    float3 base         = lerp(CloudColor().rgb, screnCol.rgb, _Dissolve);
    float3 from         = cameraPos + ray.from * forward;
    float  far          = ray.to;
    float  rayLength    = ray.length;

    float  totalLight   = 0;
    float contactHeight = 1000000;
    float contactProgression = 1000000;

    // pre fade
    float cameraDist = length(from - _WorldSpaceCameraPos);
    float fade = 1 - smoothstep(_FromDistance, _MaxDistance, _Fade * cameraDist);
    
    float shaft = pow(1 - abs(dot(float3(0,1,0), MassiveCloudsLightDirection)), 2);

    float prevdRay = rayLength;

#if defined(UNITY_COMPILER_HLSL)
    [loop]
#endif
    for (int i = iteration; i >= 0; --i)
    {
        // iteration method
        float x = (float)i / iteration;

        // calculation ray position
        float dRay = rayLength * x;
        rayPos = dRay * forward + from;

        
#if defined(_HORIZONTAL_ON)
        float isClip =
            step(horizontalRegion.height - 0.001, rayPos.y) *
            step(rayPos.y, horizontalRegion.height + horizontalRegion.thickness + 0.001);
#else
        float isClip = 1;
#endif
        if (fade == 0) break;
        if (isClip == 0)
        {
            prevdRay = dRay;
            continue;
        }

        float maxFactor = smoothstep(dRay, prevdRay, far - cameraDist);
        float stepLength = prevdRay - dRay;

        float density = maxFactor * isClip * SampleDetailedDensity(rayPos);
        float a    = density;
        if (density > 0)
        {
            contactHeight = min(contactHeight, rayPos.y);
            contactProgression = min(contactProgression, cameraDist + dRay);
        }

        // Lighting
        float  slope        = 0;
        half   lightFactor  = 0;
        if (a > 0)
        {
#ifdef _RENDERER_SOLID
            float _ShadingDist = _Shading * _Shading * _Thickness;
#else
            float _ShadingDist = (1 + 0.5 * shaft) * _Shading * _Shading * _Thickness;
#endif
            slope = CalculateSlope(_Scale, rayPos, density, MassiveCloudsLightDirection, _ShadingDist);
            lightFactor  = saturate(density * 10) * CalculateLightFactor(slope);
        }

#ifdef _RENDERER_SOLID
        // Solid
        lightFactor += (1-density) * (_Lighting + _CloudIntensityAdjustment) + _LightScattering ;
        float3 c = MassiveCloudsLightColor.rgb * lightFactor / 2 + base;
        col.rgb = lerp(col.rgb, c * ExposureMultiplier2(), a);

        totalLight   =  lerp(totalLight, lightFactor, a);
#else
        // Scattered
        lightFactor += (1 - density) * (_Lighting + _CloudIntensityAdjustment) + _LightScattering;
        float iterationFactor = pow(1 - 0.6 * ((float)iteration / 400), 2);
        lightFactor *= 0.01 * iterationFactor * ExposureMultiplier();
        col.rgb += lightFactor * MassiveCloudsLightColor.rgb * ExposureMultiplier();
        col.rgb = lerp(col.rgb, base, a);

        totalLight   =  lerp(totalLight + lightFactor, 0, a);
#endif
        col.a        =  (a - a * col.a) + col.a;
        prevdRay = dRay;
    }

    float contactRate = saturate((contactHeight - horizontalRegion.height) / horizontalRegion.thickness);
#if defined(_HORIZONTAL_ON)
    // ambient
    float3 ambient = lerp(
        _AmbientBottomColor,
        _AmbientTopColor,
        saturate(pow(contactRate, 1)));
    ambient = lerp(
        ambient,
        _AmbientMidColor,
        saturate(totalLight + 0.5 * pow(1 - 2 * abs(contactRate - 0.5), 2)));
    ambient *= ExposureMultiplier();
    col.rgb = lerp(col.rgb, 0.9 * (1 - shaft) * col.rgb + (1 + shaft) * ambient, _Ambient);
#else
    float3 ambient = lerp(
        _AmbientBottomColor,
        _AmbientTopColor,
        saturate(totalLight));
    ambient = lerp(
        ambient,
        _AmbientMidColor,
        (1 - saturate(totalLight * 2) * saturate(ray.from / 1000)));
    ambient *= ExposureMultiplier();
    col.rgb = lerp(col.rgb, col.rgb + ambient, _Ambient);
#endif

    // direct
    float3 dFactor = 20 * _DirectLight * (max(0, totalLight - 1 + 1 * _DirectLight)) *
        saturate(pow(totalLight, 2 + _DirectLight)) * pow(col.a, 0.1) * saturate(luminance(MassiveCloudsLightColor.rgb)) * normalize(MassiveCloudsLightColor.rgb);
    col.rgb = max(col.rgb, dFactor);

    col.a = saturate(col.a);

    // Ramp
    #if _RAMP_ON
    col.rgb = Ramp(lerp(col.rgb, screnCol, _RampStrength), luminance(totalLight));
    #endif

    // Global Lighting
    col = GlobalLighting(col, forward);

    col = PostProcess(col);

    // Far Dissolve
    float farDissolveInv = 1. - _FarDissolve;
    float farDissolveFrom = _MaxDistance * farDissolveInv;
    float farDissolveFactor = saturate((contactProgression - farDissolveFrom) / (0.001 + 0.9 * _FarDissolve * _MaxDistance));
    col.rgb = lerp(col.rgb, screnCol.rgb, farDissolveFactor);

    // Fade    
    col.a *= fade * fade;

    return col;
}

inline half4 WorldSpaceSurfaceRaymarch(
    half3 screnCol,
    float3 cameraPos,
    float3 forward,
    Ray    ray,
    int    iteration)
{
    HorizontalRegion horizontalRegion = CreateRegion();
    screnCol = saturate(screnCol);
    float4 col          = half4(screnCol.rgb, 0);
    float3 from         = cameraPos + ray.from * forward;
    float3 rayPos       = from;
    float  far          = ray.to;
    float  rayLength    = ray.length;

    float  totalDensity = 0;
    float  totalLight   = 0;
    float  finalDim   = 0;
    float contactHeight = 0;
    float contactProgression = 1000000;

    // pre fade
    float cameraDist = length(from - _WorldSpaceCameraPos);
    float fade = 1 - smoothstep(_FromDistance, _MaxDistance, _Fade * cameraDist);
    
    float shaft = pow(1 - abs(dot(float3(0,1,0), MassiveCloudsLightDirection)), 2);
    float prevdRay = 0;

    float  dist         = length(cameraPos + ray.from * forward - _WorldSpaceCameraPos);
    float  distFactor   = dist / 10000;
    float lightRayLength = max(1, (1 + 2 * distFactor + 3 * shaft) * 0.5 * _Shading * _Shading * _Thickness);

#if defined(UNITY_COMPILER_HLSL)
    [loop]
#endif
    for (float i = 0; i <= iteration; ++i)
    {
        if (totalDensity >= 1) break;
        if (fade == 0) break;

        // iteration method
        float x = i/iteration;

        // calculation ray position
        float dRay = rayLength * x;
        rayPos = dRay * forward + from;

#if defined(_HORIZONTAL_ON)
        float isClip =
            step(horizontalRegion.height - 0.001, rayPos.y) *
            step(rayPos.y, horizontalRegion.height + horizontalRegion.thickness + 0.001);
        // first return
        if (isClip == 0) break;
#endif

        float maxFactor = smoothstep(prevdRay, dRay, far - cameraDist);
        if (maxFactor <= 0) break;

        // Sample
        float  density = maxFactor * SampleDetailedDensity(rayPos);

        // Lighting
        float dim = 0;
        if (density > 0)
            dim = CalculateLightFactor(rayPos, MassiveCloudsLightDirection, density, lightRayLength, shaft);

        float throughDim  = 0.0044 * dim * (1 - totalDensity);
        float surfaceDim = 0.019 * dim * max(0, 0.5 - totalDensity);
        finalDim = finalDim + lerp(surfaceDim, throughDim, shaft) / (3 + 10 * finalDim);
        totalDensity = saturate(totalDensity + density);

        prevdRay = dRay;

        if (totalDensity > 0)
        {
            contactHeight = max(contactHeight, rayPos.y);
            contactProgression = min(contactProgression, dist + dRay);
        }
    }
    
    finalDim = finalDim * saturate(1 - 0.5 * saturate(2 * distFactor));

    float lightFactor;
    float3 baseColor = CloudColor();
    float smoothness = 0.1 + 0.9 * _LightSmoothness;
    lightFactor = saturate(1 - pow(finalDim, smoothness));
    lightFactor /= 5 * smoothness + -3.5 * (smoothness * smoothness) + 1.43 * pow(smoothness, 3);
    float3 base = lerp(saturate(1 - (_Lighting + _CloudIntensityAdjustment)) * baseColor, screnCol, _Dissolve);
    float3 light = ((1.5) * (_Lighting + _CloudIntensityAdjustment)) * MassiveCloudsLightColor.rgb;
    col.rgb = lerp(base, base + light * ExposureMultiplier2(), lightFactor);
    col.a = totalDensity;

    float contactRate = saturate((contactHeight - horizontalRegion.height) / horizontalRegion.thickness);
#if defined(_HORIZONTAL_ON)
    // ambient
    float3 ambient = lerp(
        _AmbientBottomColor,
        _AmbientTopColor,
        saturate(pow(contactRate, 1)));
    ambient = lerp(
        ambient,
        _AmbientMidColor,
        saturate(lightFactor + 0.5 * pow(1 - 2 * abs(contactRate - 0.5), 2)));
    ambient *= ExposureMultiplier();
    col.rgb = lerp(col.rgb, 0.9 * (1 - shaft) * col.rgb + (1 + shaft) * ambient, _Ambient);
#else
    float3 ambient = lerp(
        _AmbientBottomColor,
        _AmbientTopColor,
        saturate(lightFactor));
    ambient = lerp(
        ambient,
        _AmbientMidColor,
        (1 - saturate(lightFactor * 2) * saturate(ray.from / 1000)));
    ambient *= ExposureMultiplier();
    col.rgb = lerp(col.rgb, col.rgb + ambient, _Ambient);
#endif

    // direct
    float3 dFactor = 20 * _DirectLight * (max(0, lightFactor - 1 + 1 * _DirectLight)) *
        saturate(pow(lightFactor, 2 + _DirectLight)) * pow(totalDensity, 0.5) * saturate(luminance(MassiveCloudsLightColor.rgb)) * normalize(MassiveCloudsLightColor.rgb);
    col.rgb = max(col.rgb, pow(contactRate, 0.5) * dFactor);

    // Ramp
    #if _RAMP_ON
    col.rgb = Ramp(lerp(col.rgb, screnCol, _RampStrength), (_Lighting + _CloudIntensityAdjustment) * lightFactor);
    #endif

    // Global Lighting
    col = GlobalLighting(col, forward);

    col = PostProcess(col);

    // Far Dissolve
    float farDissolveInv = 1. - _FarDissolve;
    float farDissolveFrom = _MaxDistance * farDissolveInv;
    float farDissolveFactor = saturate((contactProgression - farDissolveFrom) / (0.001 + 0.9 * _FarDissolve * _MaxDistance));
    col.rgb = lerp(col.rgb, screnCol.rgb, farDissolveFactor);

    // Fade    
    col.a *= fade * fade;
    
    return col;
}

inline half4 WorldSpaceAuthenticRaymarch(
    half4 screnCol,
    float3 cameraPos,
    float3 forward,
    Ray    ray,
    int    iteration,
    ScreenSpace ss)
{
    HorizontalRegion horizontalRegion = CreateRegion();
//    screnCol = saturate(screnCol);
    float4 col          = float4(screnCol.rgb, 1);
    float3 from         = cameraPos + ray.from * forward;
    float3 rayPos       = from;
    float  far          = ray.to;
    float  rayLength    = ray.length;

    float  totalDensity = 0;
    float  totalLight   = 0;
    float  finalDim   = 0;

    // pre fade
    float cameraDist = length(from - _WorldSpaceCameraPos);
    float fade = 1 - smoothstep(_FromDistance, _MaxDistance, _Fade * cameraDist);
    
    float shaft = pow(1 - abs(dot(float3(0,1,0), MassiveCloudsLightDirection)), 2);
    float prevdRay = 0;

    float  dist         = length(cameraPos + ray.from * forward - _WorldSpaceCameraPos);
    float  distFactor   = dist / 10000;
    float lightRayLength = max(1, (1 + 3 * shaft) * 0.5 * _Shading * _Shading * _Thickness);

    float rayStep = 1;
    float progression = ray.from;
    float dRay = 0;
    float contactHeight = 0;
    float contactProgression = 1000000;
    float emptyStep = 0;
    float dynOpt = _Optimize;

#if defined(UNITY_COMPILER_HLSL)
    [loop]
#endif
    for (float i = 0; ; ++i)
    {
        if (progression >= ray.to || progression >= _MaxDistance) break;
        float nearFactor = saturate(pow(progression / 300, 2));
        rayStep = (1 + nearFactor * emptyStep * _Optimize) + nearFactor * max(0, progression / 1500);
        
        // calculation ray position
        float densityBias = 1 + (nearFactor) * 5 * totalDensity;
        float actualStep = lerp(rayStep, (1 + dynOpt) * densityBias * rayStep, nearFactor);
        progression += actualStep;
        rayPos = cameraPos + progression * forward;

#if defined(_HORIZONTAL_ON)
        float isClip =
            step(horizontalRegion.height - 0.001, rayPos.y) *
            step(rayPos.y, horizontalRegion.height + horizontalRegion.thickness + 0.001);
        // first return
        if (isClip == 0) break;
#endif
        // Sample
        float  density = SampleDetailedDensity(rayPos);
        density *= 1 + 0.5 * pow(1-nearFactor, 2);

        if (density <= 0)
        {
            progression += actualStep;
            emptyStep = max(10, emptyStep + actualStep/250);
            dynOpt = max(_Optimize, dynOpt);
            dynOpt += (1 - dynOpt) * 0.001 + pow(1 - totalDensity, 4) * (1 - nearFactor) * 0.01;
            continue;
        }
        
        if (emptyStep > 0)
        {
            progression -= actualStep * 50 / 100;
            emptyStep = 0;
            dynOpt = nearFactor * _Optimize * min(5, progression / 2000);
            continue;
        }
        
        // Lighting
        float dim = 0;
        if (density > 0)
        {
            dim = CalculateAuthenticLightFactor(rayPos, MassiveCloudsLightDirection, density, lightRayLength, shaft);
        }

        float throughDim  = 0.044 * dim;
        float surfaceDim = 0.19 * dim * max(0, 0.5);
        float totalDensityPrev = totalDensity;
        totalDensity = totalDensity + rayStep * density;
        finalDim = finalDim +
            rayStep * saturate(1 - totalDensity) * density * lerp(surfaceDim, throughDim, shaft);

        if (totalDensity > 0)
        {
            contactHeight = max(contactHeight, rayPos.y);
            contactProgression = min(contactProgression, progression);
        }

        if (totalDensity >= 1) break;
    }

    totalDensity = saturate(totalDensity);
    finalDim = saturate(finalDim);
    float lightFactor;
    float3 baseColor = CloudColor();
    lightFactor = saturate(1 - finalDim);
    float originallightFactor = lightFactor;
    lightFactor = pow(lightFactor + 0.01 * (_Lighting + _CloudIntensityAdjustment), 10 - 10 * (_Lighting + _CloudIntensityAdjustment));

    float3 base = lerp(baseColor, screnCol.rgb, _Dissolve);
    float3 light = MassiveCloudsLightColor.rgb;
    col.rgb = base * light * lightFactor;
    col.a = totalDensity;
    float contactRate = saturate((contactHeight - horizontalRegion.height) / horizontalRegion.thickness);
#if defined(_HORIZONTAL_ON)
    // ambient
    float3 ambient = lerp(
        _AmbientBottomColor,
        _AmbientTopColor,
        saturate(pow(contactRate, 1)));
    ambient = lerp(
        ambient,
        _AmbientMidColor,
        saturate(lightFactor + 0.5 * pow(1 - 2 * abs(contactRate - 0.5), 2)));
    ambient *= ExposureMultiplier();
    col.rgb = lerp(col.rgb, 0.9 * col.rgb + (1 + shaft) * ambient, _Ambient);
#else
    float3 ambient = lerp(
        _AmbientBottomColor,
        _AmbientTopColor,
        saturate(lightFactor));
    ambient = lerp(
        ambient,
        _AmbientMidColor,
        (1 - saturate(lightFactor * 2) * saturate(ray.from / 1000)));
    ambient *= ExposureMultiplier();
    col.rgb = lerp(col.rgb, col.rgb + ambient, _Ambient);
#endif

    // direct
    float dFactor = pow(contactRate, 0.3) * originallightFactor
                  * pow(totalDensity, 0.5);
    col.rgb = max(col.rgb, 5 * (1 + shaft) * col.rgb * _DirectLight * normalize(MassiveCloudsLightColor.rgb) * dFactor);

    // Ramp
    #if _RAMP_ON
    col.rgb = Ramp(lerp(col.rgb, screnCol.rgb, _RampStrength), (_Lighting + _CloudIntensityAdjustment) * lightFactor);
    #endif

    // Global Lighting
    col = GlobalLighting(col, forward);

    col = PostProcess(col);

    // Far Dissolve
    float farDissolveInv = 1. - _FarDissolve;
    float farDissolveFrom = _MaxDistance * farDissolveInv;
    float farDissolveFactor = saturate((contactProgression - farDissolveFrom) / (0.001 + 0.9 * _FarDissolve * _MaxDistance));
    col.rgb = lerp(col.rgb, screnCol.rgb, farDissolveFactor);

    // Fade    
    col.a *= fade;
    return col;
}


#endif