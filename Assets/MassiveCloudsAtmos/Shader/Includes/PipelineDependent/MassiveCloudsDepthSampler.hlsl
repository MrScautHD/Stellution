#ifndef MASSIVE_CLOUDS_DEPTH_SAMPLER_INCLUDED
#define MASSIVE_CLOUDS_DEPTH_SAMPLER_INCLUDED

#ifndef MASSIVE_CLOUDS_HLSL

// Depth
sampler2D _CameraDepthTexture;
half4     _CameraDepthTexture_ST;
float4    _CameraDepthTexture_TexelSize;

#endif

float SampleCameraDepth(float4 uv)
{
#if defined(USING_STEREO_MATRICES)
    return Linear01Depth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UnityStereoScreenSpaceUVAdjust(uv, _CameraDepthTexture_ST))));
#else
    return Linear01Depth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, uv)));
#endif
}

#endif