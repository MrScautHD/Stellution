#ifndef MASSIVE_CLOUDS_SHADOW_RAYMARCH_INCLUDED
#define MASSIVE_CLOUDS_SHADOW_RAYMARCH_INCLUDED

#include "Includes/MassiveCloudsLight.hlsl"
#include "Includes/MassiveCloudsSampler.hlsl"
#include "Includes/MassiveCloudsShape.hlsl"

float SampleShadowDensity(float3 pos)
{
    float  base         = SampleBaseDensity(pos);
    float  baseInv      = 1 - base;
    float  dist         = length(pos - _WorldSpaceCameraPos);
    float  distFactor   = dist / 60000;
    float  sculpture    = _Sculpture * (1 + 2 * exp(-dist * 0.001));

    if (_Octave > 1)
    {
        float o = _Octave;
        float octave = SampleDensity(pos, _Scale / o, 1 + _Phase);
        float hardSculpture = baseInv * saturate(2 * sculpture - 1);
        float a = lerp(base, octave, base * baseInv * saturate(2 * sculpture));
        base = a * ( (1 - base * hardSculpture) + base * octave * hardSculpture);
    }

    // Softness
    float softnessnFactor = 1 - 0.9 * saturate(_Softness);
    float density = pow(base, softnessnFactor) * softnessnFactor;
    float densityFactor = 0.9 * _Softness;
    float densityScale = ClipHorizontalDensity(pos, _Density);
    density = saturate(density - (1 - densityFactor) * (1 - densityScale));
    density = pow(density, 0.1 + densityFactor);
    base = density * softnessnFactor;
    base = ClipHorizontalDensity(pos, base);
    return base;
}

inline float ShadowRaymarch(
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
        
#if defined(_HORIZONTAL_ON)
        float isClip = step(horizontalRegion.height - 0.001, ray.y);
#else
        float isClip = 1;
#endif
        if (isClip == 0) continue;

        float  density = isClip * SampleShadowDensity(ray);
          totalDensity = totalDensity + density;
          if (totalDensity >= 1) break;
    }
    totalDensity = saturate(totalDensity / iteration);

    return totalDensity;
}

#endif