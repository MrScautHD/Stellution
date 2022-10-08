#ifndef MASSIVE_CLOUDS_PROGRESSIVE_INCLUDED
#define MASSIVE_CLOUDS_PROGRESSIVE_INCLUDED

#include "Includes/MassiveCloudsScreenSpace.hlsl"

float     _Cycle;
float     _Progressive;

sampler2D _DitheringTexture;
half4     _DitheringTexture_ST;
float4    _DitheringTexture_TexelSize;

float ProgressiveNoise(ScreenSpace ss)
{
    float4 uv = float4(ss.uv.x * 10, ss.uv.y * 10, 0, 1);
    float4 col = tex2Dproj(_DitheringTexture, uv);
    return saturate(col.r);
}

bool ProgressiveFrameActive(float value)
{
    value *= 0.999999;
    return _Cycle <= value && value < _Cycle + _Progressive;
}

float ProgressiveShuffler(float noise)
{
    return fmod((0.5 + 0.5 * noise) * _Time.w * 55, 1.0);
}

#endif