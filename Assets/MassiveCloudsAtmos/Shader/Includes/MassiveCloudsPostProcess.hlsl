#ifndef MASSIVE_CLOUDS_POSTPROCESS_INCLUDED
#define MASSIVE_CLOUDS_POSTPROCESS_INCLUDED

#include "MassiveCloudsInput.hlsl"

float4 PostProcess(float4 col)
{
    // Contrast
    float baseContrast = lerp(1.0001, 2.2, _IsLinear);
    col.rgb = pow(col.rgb, saturate(1 + _Contrast) + baseContrast * saturate(_Contrast));

    // Brightness
    col.rgb = col.rgb * _Brightness + col.rgb;

    col.a = saturate(col.a * (1 - _Transparency) * 1.5);
    return col;
}

#endif