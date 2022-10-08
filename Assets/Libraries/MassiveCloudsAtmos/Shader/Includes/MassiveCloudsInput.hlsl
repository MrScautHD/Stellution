#ifndef MASSIVE_CLOUDS_INPUT_INCLUDED
#define MASSIVE_CLOUDS_INPUT_INCLUDED

#include "PipelineDependent/MassiveCloudsExposure.hlsl"

// Lighting
float  _Lighting;
float  _DirectLight;
float  _Ambient;
half   _LightingQuality;
float  _LightSmoothness;
float  _LightScattering;
float  _Shading;
float  _EdgeLighting;
float  _GlobalLighting;
float  _GlobalLightingRange;

float _CloudIntensityAdjustment;

float3 _MassiveCloudsSunLightColor;
float3 _MassiveCloudsMoonLightColor;

float3 _MassiveCloudsSunLightDirection;
float3 _MassiveCloudsMoonLightDirection;

float _MassiveCloudsSunLightIntensity;
float _MassiveCloudsMoonLightIntensity;

half3  _AmbientTopColor;
half3  _AmbientMidColor;
half3  _AmbientBottomColor;

float _ShadingDist;
float _LightingIteration;

// PostProcess
float     _IsLinear;
float     _Brightness;
float     _Contrast;
float     _Transparency;

// Texture
sampler3D _VolumeTex;
float4 _VolumeTex_ST;
float _Octave;
float _Sculpture;
float _Sculpture2;
float _Sculpture3;
float _Sculpture4;
float _Phase;
float _DetailDistance;

float  _Softness;
float  _Density;

// Animation
float4 _ScrollVelocity;
float3 _ScrollOffset;

float  _Scale;

float _Optimize;

// Ramp
sampler2D _RampTex;
float     _RampScale;
float     _RampOffset;
float     _RampStrength;

// Shape
float  _HorizontalSoftnessTop;
float  _HorizontalSoftnessBottom;
float  _HorizontalSoftnessFigure;
float  _Thickness;
float  _FromHeight;
float  _FromDistance;
float  _MaxDistance;
float  _RelativeHeight;
float  _Fade;
float  _FarDissolve;

float _Shadow;
// Shadow
float _ShadowSoftness;
float _ShadowQuality;
float _ShadowStrength;
float _ShadowThreshold;
half4 _ShadowColor;

// Volumetric Shadow
float _VolumetricShadowDensity;
float _VolumetricShadowStrength;

#ifndef MASSIVE_CLOUDS_HLSL
float4 _ScreenSize; // { w, h, 1 / w, 1 / h }
#endif

float _MassiveCloudsAdaptiveSampling;

#endif