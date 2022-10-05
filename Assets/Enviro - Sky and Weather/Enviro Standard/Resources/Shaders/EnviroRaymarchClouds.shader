Shader "Enviro/Standard/RaymarchClouds"
{
	Properties
	{

	}
	SubShader
	{ 
		Cull Off ZWrite Off ZTest Always

		Tags{ "RenderType" = "Opaque" }

		Pass
	{
		CGPROGRAM 
		#pragma vertex vert    
		#pragma fragment frag 
		#pragma target 3.0  
		#pragma exclude_renderers gles d3d9
		#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
		#pragma multi_compile __ ENVIRO_DEPTHBLENDING
		#pragma multi_compile __ ENVIRO_CURLNOISE
		#pragma multi_compile __ ENVIRO_HALTONOFFSET
		#pragma multi_compile __ ENVIROURP
		#include "UnityCG.cginc" 
		#include "../../../Core/Resources/Shaders/Core/EnviroFogCore.cginc"
		#include "Core/EnviroVolumeCloudsCore.cginc"
	  
		uniform half4 _MainTex_ST;
		float4x4 _LeftWorldFromView;
		float4x4 _RightWorldFromView;
		float4x4 _LeftViewFromScreen;
		float4x4 _RightViewFromScreen;

#ifdef ENVIRO_HALTONOFFSET		           
	float _RaymarchOffset;
	float4 _TexelSize;
#else
#define BAYER_FACTOR (1.0/16.0)
	const float bayerFilter[16] =
	{
		0.0*(1.0 / 16.0) ,
		8.0*(1.0 / 16.0),
		2.0*(1.0 / 16.0),
		10.0*(1.0 / 16.0),
		12.0*(1.0 / 16.0),
		4.0*(1.0 / 16.0),
		14.0*(1.0 / 16.0),
		6.0*(1.0 / 16.0),
		3.0*(1.0 / 16.0),
		11.0*(1.0 / 16.0),
		1.0*(1.0 / 16.0),
		9.0*(1.0 / 16.0),
		15.0*(1.0 / 16.0),
		7.0*(1.0 / 16.0),
		13.0*(1.0 / 16.0),
		5.0*(1.0 / 16.0)
	};
#endif  
	float3 ScreenSpaceDither(float2 vScreenPos, float3 clr)
	{
		float d = dot(float2(131.0, 312.0), vScreenPos.xy + _Time.y);
		float3 vDither = float3(d, d, d);
		vDither.rgb = frac(vDither.rgb / float3(103.0, 71.0, 97.0)) - float3(0.5, 0.5, 0.5);
		return (vDither.rgb / 15.0) * 1.0 * Luminance(clr);
	}

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{ 
		float4 position : SV_POSITION;
		float2 uv : TEXCOORD0;
		float3 sky : TEXCOORD1;
		float4 screenPos : TEXCOORD2;

		UNITY_VERTEX_OUTPUT_STEREO
	};


	v2f vert(appdata_img v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v); 
		UNITY_INITIALIZE_OUTPUT(v2f, o); 
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
#if defined(ENVIROURP)
		o.position = float4(v.vertex.xyz,1.0);
#if UNITY_UV_STARTS_AT_TOP
   		o.position.y *= -1;
#endif
#else
	o.position = UnityObjectToClipPos(v.vertex);
#endif
		o.uv = v.texcoord;
		o.sky.x = saturate(_SunDir.y + 0.25);
		o.sky.y = saturate(clamp(1.0 - _SunDir.y, 0.0, 0.5));
		o.screenPos = ComputeScreenPos(o.position);
		return o;
	}


	float4 frag(v2f i) : SV_Target
	{
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
	
	float4 cameraRay = float4(i.uv * 2.0 - 1.0, 1.0, 1.0);
	//World Space
	float3 EyePosition = _CameraPosition;
	float3 EyePositionDepth = _WorldSpaceCameraPos;
	//Workaround for large scale games where player position will be resetted.
	//float3 EyePosition = float3(0.0,_CameraPosition.y, 0.0);

	float2 sPos = i.position.xy;

	float3 ray = 0;


	//#if UNITY_SINGLE_PASS_STEREO
	if (unity_StereoEyeIndex == 0)
	{
		cameraRay = mul(_InverseProjection, cameraRay);
		cameraRay = cameraRay / cameraRay.w;
		ray = normalize(mul((float3x3)_InverseRotation, cameraRay.xyz));
	}
	else
	{
		cameraRay = mul(_InverseProjection_SP, cameraRay);
		cameraRay = cameraRay / cameraRay.w;
		ray = normalize(mul((float3x3)_InverseRotation_SP, cameraRay.xyz));
	}
	//#else      
	//		cameraRay = mul(_InverseProjection, cameraRay);
	//		cameraRay = cameraRay / cameraRay.w;
	//		ray = normalize(mul((float3x3)_InverseRotation, cameraRay.xyz));
	//#endif                                                              


	float rawDepth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, UnityStereoTransformScreenSpaceTex(i.uv));
	bool depthPresent = rawDepth > 0.0;
	float dpth = Linear01Depth(rawDepth);

#ifdef ENVIRO_DEPTHBLENDING	    
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
	float4 wsDir = wsPos - float4(EyePosition, 0);
	float3 viewDir = normalize(wsDir);
#endif 		  
 
	float4 sky = ComputeScatteringClouds(ray, i.sky.xy, _gameTime);
	float4 color = float4(0,0,0,1);

	float3 LightDirection = _LightDir;
	float3 LightColor = _LightColor.rgb;

	//Switch to Moon Light Color  
	if (_CloudDensityScale.w < _CloudDensityScale.z)
		LightColor = _MoonLightColor.rgb;

	float pRad = _CloudsParameter.w;
	float3 pCent = float3(EyePosition.x, -pRad, EyePosition.z);

	float3 startPos;
	float3 endPos;

	// find nearest inner shell point
	float2 ih = 0.0f;
	uint innerShellHits = intersectRaySphere(
		EyePosition,
		ray,
		pCent,
		pRad + _CloudsParameter.x,
		ih);

	// find nearest outer shell point
	float2 oh = 0.0f;
	uint outerShellHits = intersectRaySphere(
		EyePosition,
		ray,
		pCent,
		pRad + _CloudsParameter.y,
		oh);

	// world space ray intersections
	float3 innerShellHit = EyePositionDepth + (ray * ih.x);
	float3 outerShellHit = EyePositionDepth + (ray * oh.x);

	float2 hitDistance;
	// eye radius from planet center
	float ch = length(EyePosition - pCent) - _CloudsParameter.w;

	if (ch < _CloudsParameter.x)
	{

#ifdef ENVIRO_DEPTHBLENDING
		// exit if there's something in front of the start of the cloud volume
		if ((depthPresent && (distance(wsPos, EyePositionDepth) < distance(innerShellHit, EyePositionDepth))) || ray.y < -0.05) // shell hits are guaranteed, but the ground may be occluding cloud layer
		{
			return float4(0.0f, 0.0f, 0.0f, 0.0f);
		}
#else   

		if (ray.y < -0.02)
			return float4(0.0f, 0.0f, 0.0f, 0.0f);
#endif 

		endPos = outerShellHit;

		hitDistance = float2(ih.x, oh.x);
	}
	else if (ch > _CloudsParameter.y)
	{
		float3 firstShellHit = outerShellHit;
		float3 secondShellHit = outerShellHits == 2u && innerShellHits == 0u ? EyePosition + (ray * oh.y) : innerShellHit;

#ifdef ENVIRO_DEPTHBLENDING
		if (outerShellHits == 0u || depthPresent && (distance(wsPos, EyePositionDepth) <= distance(firstShellHit, EyePositionDepth)))
		{
			return float4(0.0f, 0.0f, 0.0f, 0.0f);
		}
#endif                                                   
		 
		endPos = secondShellHit;

		float hit2 = outerShellHits == 2u && innerShellHits == 0u ? oh.y : ih.x;
		hitDistance = float2(oh.x, hit2);

	}
	else // between shells                                                  
	{
		float3 shellHit = innerShellHits > 0u ? innerShellHit : outerShellHit;
		float hit = innerShellHits > 0u ? ih.x : oh.x;
		float height = Remap(EyePosition.y, _CloudsParameter.x, _CloudsParameter.y, 0, 1);
		hitDistance = ResolveInside(EyePosition.xyz, ray, lerp(25000, 100000, height));
		      
#ifdef ENVIRO_DEPTHBLENDING		  
		if (depthPresent && (distance(wsPos, EyePositionDepth) < distance(shellHit, EyePositionDepth)))
		{
			shellHit = wsPos;
			hitDistance.y = (wsPos - EyePositionDepth) / viewDir;
		}
#endif   
		endPos = shellHit;

		//float reducedDistance = 500 * (1.0 + 0.0) / (1 * lerp(1.0, 0.015, smoothstep(-0.2, -0.6, 0.1)));
		//hit = min(hit, 0.0 + reducedDistance);
		//hitDistance = float2(0.0, hit);
	}

	hitDistance.x = max(0.0, hitDistance.x);
	///                    

	int steps = (int)lerp(_Steps.x, _Steps.x, ray.y);
	float rayStepLength = (1 * (hitDistance.y - hitDistance.x) / steps);
	float3 rayStep = ray * rayStepLength;

#ifdef ENVIRO_HALTONOFFSET						
	const float bayerOffsets[3][3] = {
		{ 0, 7, 3 },
		{ 6, 5, 2 },
		{ 4, 1, 8 }
	};

	float2 screenPos = i.screenPos.xy / i.screenPos.w;
	int2 texelID = int2(fmod(screenPos, 3.0));	//Calculate a texel id to index bayer matrix.

	float bayerOffset = (bayerOffsets[texelID.x][texelID.y]) / 9.0f;	//bayeroffset between[0,1)
	float offset = -fmod(_RaymarchOffset + bayerOffset, 1.0f);			//final offset combined. The value will be multiplied by sample step in GetDensity.

	float3 pos = (EyePosition + (hitDistance.x + offset * rayStepLength) * ray);
	rayStepLength = rayStepLength * offset;

#else
	float3 pos = (EyePosition + (hitDistance.x + rayStepLength) * ray);
	uint a = uint(i.uv.x) % 4;
	uint b = uint(i.uv.y) % 4;
	pos += bayerFilter[a * 4 + b] * rayStep;
#endif 

	float cloud_test = 0.0;
	int zero_density_sample_count = 0;
	float sampled_density_previous = -1.0;
	float ds = 0.0;
	float trans = 1.0;
	float intensity = 0.0;
	float alpha = 1.0;
	float eyeToEnd = distance(EyePosition, endPos);
	float lod = saturate((0.5 - Remap(eyeToEnd, 0, _CloudsParameter.w * 0.1, 0, 1.25) ) * 1.25);
	float inScatteringAngle = dot(normalize(ray), normalize(LightDirection));

#ifndef ENVIRO_DEPTHBLENDING
	// Reduce steps when rendering behind objects. 
	if (dpth < 1)
		steps *= _stepsInDepth;
#else
#endif     
	//Raymarching                                  
	[loop]
	for (int iCount = 0; iCount < steps; iCount++)
	{
		 
#ifdef ENVIRO_HALTONOFFSET
		pos += rayStep;
#endif		      
		//Calculate projection height
		float height = GetSamplingHeight(pos, pCent);

		//Get out of expensive raymarching			
		if (alpha <= 0.01 || height > 1.0 || height < 0.0 || _CloudsCoverageSettings.x <= -0.9)
			break;
		  
		// Get Weather Data                                                                                  
		float3 weather = GetWeather(pos);

		if (cloud_test > 0.0)
		{  
			float sampled_density = CalculateCloudDensity(pos, pCent, weather, 0, lod, true);

			if (sampled_density == 0.0 && sampled_density_previous == 0.0)
			{ 
				zero_density_sample_count++;
			} 

			if (zero_density_sample_count < 11 && sampled_density != 0.0)
			{
				float dl = GetDensityAlongRay(pos, pCent, LightDirection, weather, lod);
				ds += saturate(sampled_density);

				float extinction = _CloudDensityScale.x * sampled_density;
				float transmittance = exp(-extinction );

				float hg = max(HenryGreenstein(inScatteringAngle, _CloudsLighting.y) * 0.5, _CloudsLighting.z * 2 * HenryGreenstein(inScatteringAngle, 0.99 - _CloudsLighting.w));
				float luminance = GetLightEnergy(pos, height, dl, ds, hg, inScatteringAngle, rayStepLength, _CloudsLighting.x, weather);

				float integScatt = (luminance - luminance * transmittance);
				  
				intensity += trans * integScatt;
				trans *= transmittance;
				alpha *= max(trans, 0.0);

				float3 sunLight = pow(LightColor, 2) * _LightIntensity;
				sunLight.rgb = sunLight.rgb * intensity * saturate(alpha);
				color.rgb += sunLight.rgb;
				  
				if (alpha <= _CloudsCoverageSettings.z)
					alpha = 0.0;
			}
			// if not, then set cloud_test to zero so that we go back to the cheap sample case
			else
			{
				cloud_test = 0.0;
				zero_density_sample_count = 0;
			}

			sampled_density_previous = sampled_density;
		}
		else
		{
			// sample density the cheap way, only using the low frequency noise
			cloud_test = CalculateCloudDensity(pos, pCent, weather, 0, lod, false);

			if (cloud_test == 0.0)
			{
				pos += rayStep; 
			}
			else  //take a step back and capture area we skipped.
			{
				pos -= rayStep;
			}
		}
#ifndef ENVIRO_HALTONOFFSET
		pos += rayStep;
#endif
	}

	color.a = saturate(1 - alpha);

	// Ambient Lighting
	float3 ambientColor = (sky.rgb * _AmbientLightColor.rgb);
	color = color + float4(ambientColor * _AmbientSkyColorIntensity * _CloudsLightingExtended.y, 0) * saturate(1 - (color));

	//Tonemapping
	if (_CloudsLightingExtended.z == 0)
	{
		color.rgb = tonemapACES(color.rgb, _CloudsLightingExtended.w);
	}

	//Dithering
	color.rgb += ScreenSpaceDither(sPos, color.rgb);
	

#if defined(UNITY_COLORSPACE_GAMMA)
	color.rgb = LinearToGammaSpace(color.rgb);
#endif
	return color;
	}
		ENDCG
	}
	}
}
