#ifndef MASSIVE_CLOUDS_HEIGHT_FOG_INCLUDED
#define MASSIVE_CLOUDS_HEIGHT_FOG_INCLUDED

#include "Includes/MassiveCloudsScreenSpace.hlsl"
#include "Includes/MassiveCloudsLight.hlsl"

// Height Fog
half4 _MCFogColor;
half4 _FogColorTop;
float  _GroundHeight;
float  _HeightFogFromDistance;
float  _HeightFogRange;
float  _FarHeightFogRange;
float  _HeightFogDensity;

float HeightFog(float3 pos, float from, float to)
{
    float factor = smoothstep(_GroundHeight + _HeightFogRange, _GroundHeight, pos.y);
    factor *= smoothstep(0, from, to);
    return factor;
}

void HeightFogFragment(inout half4 col, ScreenSpace ss, float multiplier)
{
    const float farDist = 60000;
    float  fogDist = lerp(ss.maxDist, farDist, ss.isMaxPlane);
    float3 fromPos = ss.cameraPos;
    float3 ray     = fogDist * ss.rayDir + fromPos;
    float3 farPos = ss.cameraPos + ss.isMaxPlane * ss.rayDir * 100000000;
    float factor = HeightFog(ray, _HeightFogFromDistance, fogDist);
    col = lerp(col, lerp(_MCFogColor * multiplier, _MCFogColor * multiplier, factor), _HeightFogDensity * factor);

    float maxRay = smoothstep(0.0001 * _FarHeightFogRange, 0, ss.rayDir.y);
    col = lerp(col, lerp(col, lerp(_MCFogColor * multiplier, _MCFogColor * multiplier, maxRay), ss.isMaxPlane), _HeightFogDensity * maxRay);
}

#endif