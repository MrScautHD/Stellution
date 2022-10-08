#ifndef MASSIVE_CLOUDS_SAMPLER_INCLUDED
#define MASSIVE_CLOUDS_SAMPLER_INCLUDED

#include "MassiveCloudsInput.hlsl"
#include "MassiveCloudsShape.hlsl"

float _TimeFactor;
float3 _TexOffset;
float3 _BaseTexOffset;
float3 _BaseTexScale;
float3 _OctaveTexScale;
float3 _OctaveTexOffset;
float4 _Sculptures;
HorizontalRegion _HorizontalRegion;


inline float4 HorizontalSculpt(float3 pos)
{
    HorizontalRegion horizontalRegion = _HorizontalRegion;

    float2 softnessLength = float2(
        (0.01 + horizontalRegion.softness[1]),
        (0.01 + horizontalRegion.softness[0])
    ) * horizontalRegion.thickness;

    float2 height = float2(
        horizontalRegion.height + horizontalRegion.thickness,
        horizontalRegion.height
    );

    float2 rate = saturate(float2(
        (height.x - pos.y),
        (pos.y -  height.y)
    ) / softnessLength);

    float2 sculptFactor = float2(1, 1) - rate;
    float2 fade = saturate(rate.xy * 10);

    return float4(
        fade.xy,
        sculptFactor.xy
    );
}

inline float ClipHorizontalDensity(
    float3 pos,
    float  density)
{
    HorizontalRegion horizontalRegion = _HorizontalRegion;

#if defined(_HORIZONTAL_ON)
    float4 sculpt       = HorizontalSculpt(pos);
    float  sculptFactor = pow(pow(density, 1.5), 4 * _HorizontalSoftnessFigure)
                        * max(sculpt.z, pow(sculpt.w, 2));
    return saturate(sculpt.x * sculpt.y * density - pow(sculptFactor, 1));
#else

    return density;

#endif
}

inline void PrepareSampler()
{
    _HorizontalRegion = CreateRegion();

    _Sculptures = float4(_Sculpture, _Sculpture2, _Sculpture3, _Sculpture4) / 2.;

    float bias = 10000;
    _BaseTexScale = _VolumeTex_ST.xyx / bias / _Scale;
    _OctaveTexScale = _BaseTexScale * _Octave;

    _TimeFactor = _Time.y / 3600;
    _TexOffset = _ScrollVelocity.xyz * 1000 * _TimeFactor;

    float3 offset = (_TexOffset + _ScrollOffset);
    _BaseTexOffset = (offset * _BaseTexScale);
    _OctaveTexOffset = (offset * (1 + _Phase) * _OctaveTexScale);
}

inline float SampleDensity(float3 pos, float scale, float phase)
{
    float bias = 10000;
    float3 texScale  = _VolumeTex_ST.xyx / bias / scale;
    float3 texOffset = (_TexOffset + _ScrollOffset) * phase * texScale;
    float3 texPos    =  pos * texScale + texOffset;
    
    return tex3Dlod(_VolumeTex, float4(texPos.xyz, 0)).a;
}

inline float4 SampleVolumeTexture(float3 pos)
{
    float3 texPos    =  pos;
    return tex3Dlod(_VolumeTex, float4(texPos.xyz, 0));
}

inline float SampleBaseDensity(float3 pos)
{
    float3 texPos    =  pos * _BaseTexScale + _BaseTexOffset;
    return SampleVolumeTexture(texPos).a;
}

float SampleDetailedDensity(float3 pos)
{
    float  base         = SampleBaseDensity(pos);
    float  baseInv      = 1 - base;
    float  dist         = length(pos - _WorldSpaceCameraPos);
    float  distFactor   = dist / 60000;
    float  sculpture    = _Sculpture * (1 + 2 * exp(-dist * 0.001));

    if (_Octave > 1)
    {
        float o = _Octave;
        float o2 = 2 * _Octave;
        float o4 = 4 * _Octave;

        float octave = SampleDensity(pos, _Scale / o, 1 + _Phase);
        float hardSculpture = baseInv * saturate(2 * sculpture - 1);
        float a = lerp(base, octave, base * baseInv * saturate(2 * sculpture));
        base = a * ( (1 - base * hardSculpture) + base * octave * hardSculpture);
        if (dist < _DetailDistance / 2)
        {
            float octave4 = SampleDensity(pos, _Scale / o4, 1 + _Phase);
            base = base + base * (1 - pow(dist / _DetailDistance * 2, 0.8)) * sculpture * (1 - a) * (0.5 - octave4);
        }
        else if (dist < _DetailDistance)
        {
            float octave2 = SampleDensity(pos, _Scale / o2, 1 + _Phase);
            base = base + base * (1 - pow(dist / _DetailDistance, 0.8)) * sculpture * (1 - a) * (0.5 - octave2);
        }
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
#endif