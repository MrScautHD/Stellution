Shader "Enviro/Pro/CloudsBlit"
{
    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 ps4 xboxone vulkan metal switch
	#pragma multi_compile __ ENVIROHDRP

#if defined (ENVIROHDRP)
    
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"
	#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/AtmosphericScattering/AtmosphericScattering.hlsl"
	#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Sky/SkyUtils.hlsl"


    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord   : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

	TEXTURE2D(_SubFrame);
	TEXTURE2D(_PrevFrame);

	uniform float4x4 _Projection;
	uniform float4x4 _ProjectionSPVR;
	uniform float4x4 _InverseProjection;
	uniform float4x4 _InverseProjectionSPVR;

	uniform float4x4 _InverseRotation;
	uniform float4x4 _InverseRotationSPVR;

	uniform float4x4 _PreviousRotation;
	uniform float4x4 _PreviousRotationSPVR;

	uniform float _FrameNumber;
	uniform float _ReprojectionPixelSize;
	uniform float _UpsampleFactor;

	uniform float2 _SubFrameDimension;
	uniform float2 _FrameDimension;

	float _EnviroSkyIntensity;
	float3 color;
	float3 opacity;

    TEXTURE2D_X(_MainTex);

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        uint2 positionSS = input.texcoord * _ScreenSize.xy;

		float depthSample = LoadCameraDepth(input.positionCS.xy);
		PositionInputs posInput = GetPositionInput(input.positionCS.xy, _ScreenSize.zw, depthSample, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);

	
		
		depthSample = Linear01Depth(depthSample, _ZBufferParams);


		float4 main = LOAD_TEXTURE2D_X(_MainTex, positionSS);

		float4 final = main;

		if (depthSample > 0.9999)
		{
			float2 uv = (floor(positionSS * _FrameDimension) + 0.5) / _FrameDimension;
			float2 uv2 = (floor(positionSS * _SubFrameDimension) + 0.5) / _SubFrameDimension;

			float x = fmod(positionSS.x, _ReprojectionPixelSize);
			float y = fmod(positionSS.y, _ReprojectionPixelSize);
			float currentFrame = y * _ReprojectionPixelSize + x;

			float4 cloud;

			if (currentFrame == _FrameNumber)
			{
				cloud = LOAD_TEXTURE2D(_SubFrame, positionSS * _UpsampleFactor);
			}
			else
			{
				float4 reprojection;
				float4 pos = float4(positionSS * 2.0 - 1.0, 1.0, 1.0);

#if UNITY_SINGLE_PASS_STEREO
				if (unity_StereoEyeIndex == 0)
				{
					pos = mul(_InverseProjection, pos);
					pos = pos / pos.w;
					pos.xyz = mul((float3x3)_InverseRotation, pos.xyz);
					pos.xyz = mul((float3x3)_PreviousRotation, pos.xyz);

					reprojection = mul(_Projection, pos);
				}
				else
				{
					pos = mul(_InverseProjectionSPVR, pos);
					pos = pos / pos.w;
					pos.xyz = mul((float3x3)_InverseRotationSPVR, pos.xyz);
					pos.xyz = mul((float3x3)_PreviousRotationSPVR, pos.xyz);

					reprojection = mul(_ProjectionSPVR, pos);
				}
#else
				pos = mul(_InverseProjection, pos);
				pos = pos / pos.w;
				pos.xyz = mul((float3x3)_InverseRotation, pos.xyz);
				pos.xyz = mul((float3x3)_PreviousRotation, pos.xyz);

				reprojection = mul(_Projection, pos);
#endif

				if (reprojection.y < 0.0 || reprojection.y > 1.0 || reprojection.x < 0.0 || reprojection.x > 1.0)
				{
					cloud = LOAD_TEXTURE2D(_SubFrame, positionSS * _UpsampleFactor);
				}
				else
				{
					cloud = LOAD_TEXTURE2D(_PrevFrame, reprojection.xy * _UpsampleFactor);
				}
			}
	
			cloud.rgb = ((cloud.rgb * cloud.a) *_EnviroSkyIntensity * GetCurrentExposureMultiplier());			
			final = float4 (main * (1 - cloud.a) + cloud.rgb, 1.0);

			if (depthSample > 0.9999)
			{ 
				float3 V = GetSkyViewDirWS(positionSS);
				posInput.positionWS = GetCurrentViewPosition() - V * _MaxFogDistance;
				EvaluateAtmosphericScattering(posInput, V, color, opacity);
				final.rgb = color + (1 - opacity) * final.rgb;
			}
		}

		return final;
	
    }

#else		
			struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f Vert (appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            float4 CustomPostProcess (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }
#endif

    ENDHLSL

    SubShader
    {
		Tags{ "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "Enviro Clouds Blit"

            ZWrite Off
            ZTest Less
            Blend Off
            Cull Off

            HLSLPROGRAM
            #pragma fragment CustomPostProcess
            #pragma vertex Vert
            ENDHLSL 
        }
    }

    Fallback Off
}
