
Shader "Enviro/Lite/EnviroFogRenderingSimple" 
{
	Properties
	{
	//	_MainTex("Base (RGB)", Any) = "white" {}
	}
	SubShader
	{ 
		Pass
		{
			ZTest Always Cull Off ZWrite Off Fog { Mode Off }

	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
	#pragma target 2.0
	#pragma multi_compile __ ENVIROURP
	#include "UnityCG.cginc" 

	UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
	uniform float4 _MainTex_TexelSize;
	UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
	
	uniform float4x4 _LeftWorldFromView;
	uniform float4x4 _RightWorldFromView;
	uniform float4x4 _LeftViewFromScreen;
	uniform float4x4 _RightViewFromScreen;
	uniform float4 _EnviroParams;
	uniform float4 _DistanceParams;
	uniform int4 _SceneFogMode;
	uniform float4 _SceneFogParams;
	uniform half _distanceFogIntensity;
	uniform float4 _EnviroSkyFog; // x = _SkyFogHeight, y = _SkyFogIntensity, z = _SkyFogStart, w = _HeightFogIntensity
	uniform float _maximumFogDensity;
	uniform float _lightning;

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
		float2 uv : TEXCOORD1;
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
		o.pos = v.vertex * float4(2, 2, 1, 1) + float4(-1, -1, 0, 0);
#endif
		o.uv.xy = v.texcoord.xy;

#if !ENVIROURP && UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y > 0)
			o.uv.y = 1 - o.uv.y;
#endif  
		return o;
	}

	half ComputeFogFactor(float coord)
	{
		float fogFac = 0.0;

		if (_SceneFogMode.x == 1) // linear
		{
			fogFac = coord * _SceneFogParams.z + _SceneFogParams.w;
		}

		if (_SceneFogMode.x >= 2) // exp
		{
			fogFac = _SceneFogParams.y * coord; fogFac = exp2(-fogFac);
		}

		return saturate(fogFac);
	}

	// Distance fog
	float ComputeDistance(float3 camDir, float zdepth)
	{
		float dist;
		dist = length(camDir);
		dist -= _ProjectionParams.y;
		return dist;
	}

	fixed4 frag(v2f i) : SV_Target
	{
	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
	
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

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		half fogFac = 0;
		float4 finalFog = unity_FogColor;
		float g = _DistanceParams.x;
/////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//Scene
		if (dpth < 0.99999) 
		{
			// Calculate Distance Fog
			if (_EnviroParams.y > 0)
			{
				g += ComputeDistance(wsDir,dpth);
				g *= _distanceFogIntensity;
			}

			// Compute fog amount
			fogFac = ComputeFogFactor(max(0.0, g));
			fogFac = lerp(_maximumFogDensity, 1.0f, fogFac);
		}
		else //SKY
		{
			float3 viewDir = normalize(wsDir);
			float f = saturate(_EnviroSkyFog.x * (viewDir.y + _EnviroSkyFog.z));
			f = pow(f, _EnviroSkyFog.y);
			fogFac = clamp(f, 0, 1);
		}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		float4 source = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, UnityStereoTransformScreenSpaceTex(i.uv));		
		return lerp (finalFog, source, fogFac);
		}
		ENDCG
		}
	}
	Fallback Off
}
