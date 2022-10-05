Shader "Enviro/Pro/EnviroFogHDRP"
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
	#include "../../../../Core/Resources/Shaders/Core/EnviroFogCore.hlsl"
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
		float3 sky : TEXCOORD1;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
		output.sky.x = saturate(_SunDir.y + 0.25);
		output.sky.y = saturate(clamp(1.0 - _SunDir.y, 0.0, 0.5));
        return output;
    }

    TEXTURE2D_X(_MainTex);
	TEXTURE3D(_FogNoiseTexture);
	float4x4 _LeftWorldFromView;
	float4x4 _RightWorldFromView;
	float4x4 _LeftViewFromScreen;
	float4x4 _RightViewFromScreen;
	float _EnviroSkyIntensity;
	float _DitheringIntensity;


	float Remap(float org_val, float org_min, float org_max, float new_min, float new_max)
	{
		return new_min + saturate(((org_val - org_min) / (org_max - org_min))*(new_max - new_min));
	}

	float3 ScreenSpaceDither(float2 vScreenPos, float3 clr)
	{
		float d = dot(float2(131.0, 312.0), vScreenPos.xy + _Time.y);
		float3 vDither = float3(d, d, d);
		vDither.rgb = frac(vDither.rgb / float3(103.0, 71.0, 97.0)) - float3(0.5, 0.5, 0.5);
		return (vDither.rgb / 15.0) * _DitheringIntensity * Luminance(clr);
	}

	// Linear height fog function
	float ComputeHalfSpaceWithNoise(float3 wsDir)
	{
		float3 wpos = _WorldSpaceCameraPos + wsDir;
		float FH = _HeightParams.x;
		float3 C = _WorldSpaceCameraPos;
		float3 V = wsDir;
		float3 P = wpos;
		float3 aV = (_HeightParams.w * _EnviroSkyFog.w) * V;
		float noise = LOAD_TEXTURE3D(_FogNoiseTexture, frac(wpos * _FogNoiseData.x + float3(_Time.y * _FogNoiseVelocity.x, 0, _Time.y * _FogNoiseVelocity.y)));	
		noise = saturate(noise - _FogNoiseData.z) * _FogNoiseData.y;
		aV *= noise;
		float FdotC = _HeightParams.y;
		float k = _HeightParams.z;
		float FdotP = P.y - FH;
		float FdotV = wsDir.y;
		float c1 = k * (FdotP + FdotC);
		float c2 = (1 - 2 * k) * FdotP;
		float g = min(c2, 0.0);
		g = -length(aV) * (c1 - g * g / abs(FdotV + 1.0e-5f));
		return g;
	}

    float4 FullFogPass(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        uint2 positionSS = input.texcoord * _ScreenSize.xy;
		float rawDepth = LoadCameraDepth(input.positionCS.xy);
		float dpth = Linear01Depth(rawDepth, _ZBufferParams);

		float4x4 proj, eyeToWorld;
		if (unity_StereoEyeIndex == 0)
		{
			proj = _LeftViewFromScreen;
			eyeToWorld = _LeftWorldFromView;
		}
		else
		{
			proj = _RightViewFromScreen;
			eyeToWorld = _RightWorldFromView;
		}

		//bit of matrix math to take the screen space coord (u,v,depth) and transform to world space
		float2 uvClip = input.texcoord * 2.0 - 1.0;
		float clipDepth = rawDepth; // Fix for OpenGl Core thanks to Lars Bertram
		clipDepth = (UNITY_NEAR_CLIP_VALUE < 0) ? clipDepth * 2 - 1 : clipDepth;
		float4 clipPos = float4(uvClip, clipDepth, 1.0);
		float4 viewPos = mul(proj, clipPos); // inverse projection by clip position
		viewPos /= viewPos.w; // perspective division
		float4 wsPos = float4(mul(eyeToWorld, viewPos).xyz, 1);
		float4 wsDir = wsPos - float4(_WorldSpaceCameraPos, 0);
		float3 viewDir = normalize(wsDir);

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		half fogFac = 0;
		float4 finalFog = 0;
		float g = _DistanceParams.x;
		half gHeight = 0;
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//Scene 
		if (dpth < 0.99999)
		{
			// Calculate Distance Fog
			if (_EnviroParams.y > 0)
			{
				g += ComputeDistance(wsDir, dpth);
				g *= _distanceFogIntensity;
			}

			if (_EnviroParams.z > 0)
			{
				gHeight = ComputeHalfSpaceWithNoise(wsDir)* (1 - dpth);
			}

			// Add Height Fog 
			g += gHeight;

			// Compute fog amount
			fogFac = ComputeFogFactorEnviro(max(0.0, g));
			fogFac = lerp(_maximumFogDensity, 1.0f, fogFac); 

			finalFog = ComputeScatteringScene(viewDir, input.sky.xy) * _EnviroSkyIntensity * GetCurrentExposureMultiplier();
		}
		else //SKY
		{
			if (_EnviroParams.z > 0 && _SceneFogMode.w == 1)
			{
				gHeight = ComputeHalfSpace(wsDir);
			}

			half fogFacSky = ComputeFogFactorEnviro(max(0.0, gHeight));
			float f = saturate(_EnviroSkyFog.x * (viewDir.y + _EnviroSkyFog.z));
			f = pow(f, _EnviroSkyFog.y);

			fogFac = (clamp(f, 0, 1));

			if (fogFac > fogFacSky)
				fogFac = fogFacSky;


			float skyLerp = Remap(_WorldSpaceCameraPos.y, 0, 11000, 0, 1);
			fogFac = lerp(fogFac, 1, skyLerp);

			float4 skyFog = ComputeScatteringScene(viewDir, input.sky.xy);
			finalFog = skyFog * _EnviroSkyIntensity * GetCurrentExposureMultiplier();
		}	

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		/// Composing
		float4 source = LOAD_TEXTURE2D_X(_MainTex, positionSS);

		//HDRP Fog
		float3 V = GetSkyViewDirWS(positionSS);
		PositionInputs posInput = GetPositionInput(input.positionCS.xy, _ScreenSize.zw, rawDepth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
		posInput.positionWS = GetCurrentViewPosition() - V * _MaxFogDistance;
		float3 color, opacity;
		EvaluateAtmosphericScattering(posInput, V, color, opacity);
		finalFog.rgb = color + (1 - opacity) * finalFog.rgb;
		// 

		//Dithering
		finalFog.rgb = finalFog.rgb + ScreenSpaceDither(input.positionCS.xy, finalFog.rgb);

		float4 final = lerp(finalFog, source, fogFac);

		return final;
    }


	float4 SkyOnlyPass(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		uint2 positionSS = input.texcoord * _ScreenSize.xy;
		float rawDepth = LoadCameraDepth(input.positionCS.xy);
		float dpth = Linear01Depth(rawDepth, _ZBufferParams);

		float4x4 proj, eyeToWorld;
		if (unity_StereoEyeIndex == 0)
		{
			proj = _LeftViewFromScreen;
			eyeToWorld = _LeftWorldFromView;
		}
		else
		{
			proj = _RightViewFromScreen;
			eyeToWorld = _RightWorldFromView;
		}

		//bit of matrix math to take the screen space coord (u,v,depth) and transform to world space
		float2 uvClip = input.texcoord * 2.0 - 1.0;
		float clipDepth = rawDepth; // Fix for OpenGl Core thanks to Lars Bertram
		clipDepth = (UNITY_NEAR_CLIP_VALUE < 0) ? clipDepth * 2 - 1 : clipDepth;
		float4 clipPos = float4(uvClip, clipDepth, 1.0);
		float4 viewPos = mul(proj, clipPos); // inverse projection by clip position
		viewPos /= viewPos.w; // perspective division
		float4 wsPos = float4(mul(eyeToWorld, viewPos).xyz, 1);
		float4 wsDir = wsPos - float4(_WorldSpaceCameraPos, 0);
		float3 viewDir = normalize(wsDir);

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		half fogFac = 1;
		float4 finalFog = 0;
		float g = _DistanceParams.x;
		half gHeight = 0;
		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//Scene 
		if (dpth > 0.99999)
		{
			if (_EnviroParams.z > 0 && _SceneFogMode.w == 1)
			{
				gHeight = ComputeHalfSpace(wsDir);
			}

			half fogFacSky = ComputeFogFactorEnviro(max(0.0, gHeight));
			float f = saturate(_EnviroSkyFog.x * (viewDir.y + _EnviroSkyFog.z));
			f = pow(f, _EnviroSkyFog.y);

			fogFac = (clamp(f, 0, 1));

			if (fogFac > fogFacSky)
				fogFac = fogFacSky;


			float skyLerp = Remap(_WorldSpaceCameraPos.y, 0, 11000, 0, 1);
			fogFac = lerp(fogFac, 1, skyLerp);

			float4 skyFog = ComputeScatteringScene(viewDir, input.sky.xy);
			finalFog = skyFog * _EnviroSkyIntensity * GetCurrentExposureMultiplier();

			//HDRP Fog
			float3 V = GetSkyViewDirWS(positionSS);
			PositionInputs posInput = GetPositionInput(input.positionCS.xy, _ScreenSize.zw, rawDepth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
			posInput.positionWS = GetCurrentViewPosition() - V * _MaxFogDistance;
			float3 color, opacity;
			EvaluateAtmosphericScattering(posInput, V, color, opacity);
			finalFog.rgb = color + (1 - opacity) * finalFog.rgb;
			// 
		}

		/////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		//Dithering
		finalFog.rgb = finalFog.rgb + ScreenSpaceDither(input.positionCS.xy, finalFog.rgb);

		/// Composing
		float4 source = LOAD_TEXTURE2D_X(_MainTex, positionSS);
	
		return lerp(finalFog, source, fogFac);
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

            float4 FullFogPass (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }

			float4 SkyOnlyPass (v2f i) : SV_Target
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
            Name "EnviroFog Full Pass"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
            #pragma fragment FullFogPass
            #pragma vertex Vert
            ENDHLSL
        }

		Pass
	{
		Name "EnviroFog SkyOnly Pass"

		ZWrite Off
		ZTest Always
		Blend Off
		Cull Off

		HLSLPROGRAM
		#pragma fragment SkyOnlyPass
		#pragma vertex Vert
		ENDHLSL
	}
    }
    Fallback Off
}
