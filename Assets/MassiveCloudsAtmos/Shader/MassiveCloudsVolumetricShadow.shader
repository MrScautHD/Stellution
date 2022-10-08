Shader "MassiveCloudsVolumetricShadow"
{
	Properties
	{
	    [HideInInspector]
		_MainTex            ("Texture", 2D)                     = "white" {}


		_BaseColor          ("BaseColor", Color)                = (1,1,1,1)
        [Toggle]
        _IsLinear           ("IsLinearColorSpace?", Float)      = 0

        // VolumeTexture
		_VolumeTex          ("VolumeTexture (RGB)", 3D)         = "" {}
        [KeywordEnum(Authentic, Surface, Lucid, Solid)]
        _RENDERER           ("Renderer Type", Float)            = 0

        // Shape
        [Toggle]
        _HORIZONTAL         ("Horizontal?", Float)              = 0
        [Toggle]
        _RelativeHeight     ("RelativeHeight?", Float)          = 0
		_HorizontalSoftnessTop ("HorizontalSoftnessTop", Range(0, 1)) = 0.1
		_HorizontalSoftnessBottom ("HorizontalSoftnessBottom", Range(0, 1)) = 0.1
		_HorizontalSoftnessFigure ("HorizontalSoftnessFigure", Range(0, 1)) = 0.5
		_Thickness          ("Thickness", Range(0, 10000))      = 50
		_FromHeight         ("FromHeight", Range(0, 5000))      = 1
		_FromDistance       ("FromDistance", Range(0, 5000))    = 1
		_Octave             ("Octave", Range(1, 32))            = 1
		_Sculpture          ("Sculpture", Range(0, 1))          = 0
		_Softness           ("Softness", Range(0.001, 5))       = 0.001
        _DetailDistance     ("DetailDistance", Range(1, 5000))  = 500

        [Toggle]
        _RAMP               ("Ramp?", Float)                    = 0
		_RampTex            ("Ramp", 2D)                        = "white" {}
		_RampScale          ("RampScale", Range(0.1, 1))        = 0.5
		_RampOffset         ("RampOffset", Range(-10, 10))      = 0
		_RampStrength       ("RampStrength", Range(0, 1))       = 0.0

        // Texture
		_Density            ("Density", Range(0, 1))            = 0
		_Dissolve           ("Dissolve", Range(0, 1))           = 1
		_Transparency       ("Transparency", Range(0, 1))       = 1
		_Scale              ("Scale", Range(1, 10))             = 1
 
        // Animation
		_ScrollVelocity     ("ScrollVelocity", Vector)          = (0,0,0,0)
		_Phase              ("Phase", Range(-1, 1))             = 0

        // Lighting
        _MassiveCloudsSunLightColor         ("SunLightColor", Color)        = (1,1,1,1)
        _MassiveCloudsMoonLightColor        ("MoonLightColor", Color)       = (1,1,1,1)
		_MassiveCloudsSunLightDirection     ("SunLightDirection", Vector)   = (0,1,0,0)
		_MassiveCloudsMoonLightDirection    ("MoonLightDirection", Vector)  = (0,1,0,0)
        _AmbientTopColor    ("AmbientTopColor", Color)          = (1,1,1,1)
        _AmbientMidColor    ("AmbientMidColor", Color)          = (1,1,1,1)
        _AmbientBottomColor ("AmbientBottomColor", Color)       = (1,1,1,1)

		_Lighting           ("Lighting", Range(0, 1))           = 1
		_DirectLight        ("DirectLight", Range(0, 1))        = 1
		_Ambient            ("Ambient", Range(0, 1))            = 1
		_LightingQuality    ("LightingQuality", Range(0, 1))    = 1
		_LightSmoothness    ("LightSmoothness", Range(0, 1))    = 0.0
		_LightScattering    ("LightScattering", Range(0, 1))    = 0.0
		_Shading            ("Shading", Range(0, 1))            = 1.0

		_EdgeLighting        ("EdgeLighting", Range(0, 1))      = 0
		_GlobalLighting      ("GlobalLighting", Range(-1, 1))   = 0
		_GlobalLightingRange ("GlobalLightingRange", Range(0, 1)) = 1
		
		// Screen Space Shadow
        [Toggle]
        _SHADOW              ("Shadow?", Float)                 = 0
		_ShadowSoftness      ("ShadowSoftness", Range(0, 1))    = 0
		_ShadowQuality       ("ShadowQuality", Range(0, 1))     = 0
		_ShadowStrength      ("ShadowStrength", Range(0, 1))    = 0
		_ShadowThreshold     ("ShadowThreshold", Range(0, 1))   = 0
		_ShadowColor         ("ShadowColor", Color)             = (0,0,0,0)

		// Finishing
		_Brightness         ("Brightness", Range(-1,1))         = 0
		_Contrast           ("Contrast", Range(-1,1))           = 0

        // Ray Marching        
		_Iteration          ("Iteration", Range(1, 200))        = 16
		_Optimize           ("Optimize", Range(0, 1))           = 1
		_MaxDistance        ("MaxDistance", Range(0, 60000))    = 5000
		_Fade               ("Fade", Range(1,10))               = 1

		// Height Fog
        [Toggle]
        _HEIGHTFOG          ("HeightFog?", Float)               = 0
		_MCFogColor           ("FogColor", Color)                 = (1, 1, 1, 1)
		_GroundHeight       ("GroundHeight", Range(-1000, 1000)) = 0
		_HeightFogFromDistance  ("HeightFogFromDistance", Range(0, 10000)) = 0
		_HeightFogRange     ("HeightFogRange", Range(0.001, 1000)) = 0

		_VolumetricShadowDensity  ("VolumetricShadowDensity",  Range(0, 1)) = 0
		_VolumetricShadowStrength ("VolumetricShadowStrength", Range(0, 1)) = 0
	}

	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
 			HLSLPROGRAM
			#pragma vertex MassiveCloudsVert
			#pragma fragment MassiveCloudsVolumetricShadowFragment
            #pragma shader_feature _HORIZONTAL_ON
            #pragma shader_feature _RAMP_ON
            #pragma multi_compile _RENDERER_AUTHENTIC _RENDERER_SURFACE _RENDERER_LUCID _RENDERER_SOLID
            
            #define MASSIVE_CLOUDS_VOLUMETRIC_SHADOW
 
            #include "MassiveCloudsCommon.hlsl"
			#include "MassiveCloudsShadow.hlsl"
			#include "MassiveCloudsVolumetricShadow.hlsl"
			ENDHLSL
		}
	}
}
