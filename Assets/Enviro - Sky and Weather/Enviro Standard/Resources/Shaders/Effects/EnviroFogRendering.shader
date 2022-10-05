Shader "Enviro/Standard/EnviroFogRendering" 
{
	Properties
	{ 
		_EnviroVolumeLightingTex("Volume Lighting Tex",  Any) = ""{}
	//	_MainTex("Source",  Any) = "black"{}
	}
	SubShader
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off Fog { Mode Off }

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma target 3.0
	#pragma multi_compile __ ENVIROVOLUMELIGHT
	#pragma multi_compile __ ENVIRO_DEPTHBLENDING
	#pragma multi_compile __ ENVIROURP
	#pragma exclude_renderers gles 
		 
	//  Start: LuxWater
#pragma multi_compile __ LUXWATER_DEFERREDFOG

#if defined(LUXWATER_DEFERREDFOG)
		sampler2D _UnderWaterMask;
		float4 _LuxUnderWaterDeferredFogParams; // x: IsInsideWatervolume?, y: BelowWaterSurface shift, z: EdgeBlend
#endif
	//  End: LuxWater

	#include "UnityCG.cginc" 
	#include "../Core/EnviroVolumeLightCore.cginc"
	#include "../../../../Core/Resources/Shaders/Core/EnviroFogCore.cginc"

	//uniform sampler2D _MainTex;
	UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
	uniform float4 _MainTex_TexelSize;
	uniform sampler3D _FogNoiseTexture;
	uniform float _DitheringIntensity;

#ifdef ENVIRO_DEPTHBLENDING	
	uniform sampler2D _EnviroCloudsTex;
#endif
	struct appdata_t 
	{
		float4 vertex : POSITION;
		float3 texcoord : TEXCOORD0;

		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f  
	{
		float4 pos : SV_POSITION;
		float3 texcoord : TEXCOORD0;
		float3 sky : TEXCOORD1;
		float4 uv : TEXCOORD2;

		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f vert(appdata_img v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v); //Insert
		UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert
#if defined(ENVIROURP)
		o.pos = float4(v.vertex.xyz,1.0);
		#if UNITY_UV_STARTS_AT_TOP
                 o.pos.y *= -1;
         #endif
#else
		o.pos = UnityObjectToClipPos(v.vertex);
#endif
		o.uv.xy = v.texcoord.xy;

#if !ENVIROURP && UNITY_UV_STARTS_AT_TOP
		//if (_MainTex_TexelSize.y > 0)
		//	o.uv.y = 1 - o.uv.y;
#endif  
		o.sky.x = saturate(_SunDir.y + 0.25);
		o.sky.y = saturate(clamp(1.0 - _SunDir.y, 0.0, 0.5));
		return o;
	}

	float3 ScreenSpaceDither(float2 vScreenPos, float3 clr)
	{
		float d = dot(float2(131.0, 312.0), vScreenPos.xy + _Time.y);
		float3 vDither = float3(d, d, d);
		vDither.rgb = frac(vDither.rgb / float3(103.0, 71.0, 97.0)) - float3(0.5, 0.5, 0.5);
		return (vDither.rgb / 15.0) * _DitheringIntensity;
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
		float noise = tex3D(_FogNoiseTexture, frac(wpos * _FogNoiseData.x + float3(_Time.y * _FogNoiseVelocity.x, 0, _Time.y * _FogNoiseVelocity.y)));
	//	float noise = tex3D(_FogNoiseTexture, wpos * 0.01);
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

	float Remap(float org_val, float org_min, float org_max, float new_min, float new_max)
	{
		return new_min + saturate(((org_val - org_min) / (org_max - org_min))*(new_max - new_min));
	}

	/// Main Fragment Shader
	fixed4 frag(v2f i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

		float4 uv = i.uv;

		float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(i.uv));
		float dpth = Linear01Depth(rawDepth);

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
		float2 uvClip = i.uv * 2.0 - 1.0;
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
		if (dpth < 0.99)  
		{
			// Calculate Distance Fog
			if (_EnviroParams.y > 0)
			{   
				g += ComputeDistance(wsDir, dpth);
				g *= _distanceFogIntensity;
			}  
			 
			if (_EnviroParams.z > 0)
			{
				gHeight = ComputeHalfSpaceWithNoise(wsDir) * (1 - dpth);
			}    
			//gHeight = lerp(0.75, 1.0, dpth);
			 

			// Add Height Fog 
			g += gHeight;
			 
			// Compute fog amount
			fogFac = ComputeFogFactor(max(0.0, g));


#ifdef ENVIRO_DEPTHBLENDING	
			float4 clouds = tex2D(_EnviroCloudsTex, UnityStereoTransformScreenSpaceTex(i.uv));

			if (clouds.a > 0.9)
			{

				if (_EnviroParams.z > 0 && _SceneFogMode.w == 1)
				{
					gHeight = ComputeHalfSpace(wsDir);
				}

				half fogFacSky = ComputeFogFactor(max(0.0, gHeight));
				float f = saturate(_EnviroSkyFog.x * (viewDir.y + _EnviroSkyFog.z));
				f = pow(f, _EnviroSkyFog.y);

				fogFac = (clamp(f, 0, 1));

				float skyLerp = Remap(_WorldSpaceCameraPos.y, 0, _SunParameters.w, 0, 1);
				fogFac = lerp(fogFac, 1, skyLerp);
			}
				//fogFac = 1;
#endif


			fogFac = lerp(_maximumFogDensity, 1.0f, fogFac);
			finalFog = ComputeScatteringScene(viewDir, i.sky.xy);
	
		}
		else //SKY
		{
			if (_EnviroParams.z > 0 && _SceneFogMode.w == 1)
			{ 
				gHeight = ComputeHalfSpace(wsDir);
			}

			half fogFacSky = ComputeFogFactor(max(0.0, gHeight));
			float f = saturate(_EnviroSkyFog.x * (viewDir.y + _EnviroSkyFog.z));
			f = pow(f, _EnviroSkyFog.y);
			 
			fogFac = (clamp(f, 0, 1));

			if (fogFac > fogFacSky) 
				fogFac = fogFacSky;

			float skyLerp = Remap(_WorldSpaceCameraPos.y, 0, _SunParameters.w, 0, 1);
			fogFac = lerp(fogFac, 1, skyLerp);
			float4 skyFog = ComputeScatteringScene(viewDir, i.sky.xy);

			finalFog = skyFog;
		}

		//Dithering
		finalFog.rgb = finalFog.rgb + ScreenSpaceDither(i.pos.xy, finalFog.rgb);

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Composing

//  Start: LuxWater
#if defined(LUXWATER_DEFERREDFOG)
		half4 fogMask = tex2D(_UnderWaterMask, UnityStereoTransformScreenSpaceTex(i.uv));
		float watersurfacefrombelow = DecodeFloatRG(fogMask.ba);

		//	Get distance and lower it a bit in order to handle edge blending artifacts (edge blended parts would not get ANY fog)
		float dist = (watersurfacefrombelow - dpth) + _LuxUnderWaterDeferredFogParams.y * _ProjectionParams.w;
		//	Fade fog from above water to below water
		float fogFactor = saturate(1.0 + _ProjectionParams.z * _LuxUnderWaterDeferredFogParams.z * dist);
		//	Clamp above result to where water is actually rendered
		fogFactor = (fogMask.r == 1) ? fogFactor : 1.0;
		//  Mask fog on underwarter parts - only if we are inside a volume (bool... :( )
		if (_LuxUnderWaterDeferredFogParams.x) {
			fogFactor *= saturate(1.0 - fogMask.g * 8.0);
			if (dist < -_ProjectionParams.w * 4 && fogMask.r == 0 && fogMask.g < 1.0) {
				fogFactor = 1.0;
			}
		}
		//	Tweak fog factor
		fogFac = lerp(1.0, fogFac, fogFactor);
#endif
//  End: LuxWater 

		float4 final;
		float4 source = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, UnityStereoTransformScreenSpaceTex(i.uv));
		
		#if defined (ENVIROVOLUMELIGHT)
			float4 volumeLighting = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_EnviroVolumeLightingTex, i.uv);
			volumeLighting *= _EnviroParams.x;

			if (_EnviroParams.w == 1) 
			{
				volumeLighting.rgb = tonemapACES(volumeLighting.rgb, 1.0);
			}

			//  Start: LuxWater
#if defined(LUXWATER_DEFERREDFOG)
			volumeLighting *= fogFactor;
#endif
			//  End: LuxWater 
			final *= volumeLighting.w;

			final = lerp (lerp(finalFog, finalFog + volumeLighting, _EnviroVolumeDensity), lerp(source, source + volumeLighting, _EnviroVolumeDensity), fogFac);
		#else
			final = lerp (finalFog, source, fogFac);
		#endif

		return final;


		}
		ENDCG
		}
	}

	Fallback Off
}
