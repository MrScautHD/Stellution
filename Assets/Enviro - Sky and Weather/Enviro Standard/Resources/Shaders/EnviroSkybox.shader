
Shader "Enviro/Standard/Skybox"
{ 
	Properties
	{
	_Stars("Stars Cubemap", Cube) = "black" {}
	_StarsTwinklingNoise("Stars Noise", Cube) = "black" {}
	_Galaxy("Galaxy Cubemap", Cube) = "black" {}
	_SatTex("Satellites Tex", 2D) = "black" {}
	_MoonTex("Moon Tex", 2D) = "black" {}
	_GlowTex("Glow Tex", 2D) = "black" {}
	_Aurora_Layer_1("Aurora Layer 1", 2D) = "black" {}
	_Aurora_Layer_2("Aurora Layer 2", 2D) = "black" {}
	_Aurora_Colorshift("Aurora Color Shift", 2D) = "black" {}
	//_Background("Background Cubemap", Cube) = "black" {}
	}

		SubShader
	{
		Tags{ "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" }
		Cull Off
		Fog{ Mode Off }
		ZWrite Off

		Pass
	{

	CGPROGRAM
	#pragma target 3.0
	#pragma vertex vert
	#pragma fragment frag
	#pragma multi_compile __ UNITY_COLORSPACE_GAMMA
	#include "UnityCG.cginc"

	uniform float3 _Br;
	uniform float3 _Bm;
	uniform float3 _mieG;
	uniform float  _SunIntensity;
	uniform float _Tonemapping;
	uniform float _SkyExposure;
	uniform float _SkyLuminance;
	uniform float _scatteringPower;
	uniform float _SunDiskSize;
	uniform float _SunDiskIntensity;
	uniform float _StarsIntensity;
	uniform float4 _scatteringColor;
	uniform float4 _sunDiskColor;
	uniform samplerCUBE _Stars;
	uniform samplerCUBE _StarsTwinklingNoise;
	uniform float4x4 _StarsMatrix;
	uniform float4x4 _StarsTwinklingMatrix;
	uniform float _SkyColorPower;
	uniform float3 _SunDir;
	uniform float3 _MoonDir;
	uniform float4 _weatherSkyMod;
	uniform float4 _moonGlowColor;
	uniform sampler2D _MoonTex;
	uniform sampler2D _GlowTex;
	uniform sampler2D _SatTex;
	uniform float4 _MoonColor;
	uniform float4 _moonParams;
	uniform float _GalaxyIntensity;
	uniform samplerCUBE _Galaxy;
	uniform int _blackGround;
	uniform float _StarsTwinkling;
	uniform float _DitheringIntensity;

	//uniform samplerCUBE _Background;

	struct appdata
	{
		float4 vertex : POSITION;
		float3 texcoord : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		float4 Position : SV_POSITION;
		float4 moonPos : TEXCOORD0;
		float4 sky : TEXCOORD1;
		float night : TEXCOORD2;
		float3 texcoord : TEXCOORD3;
		float3 starPos : TEXCOORD4;
		float4 screenUV : TEXCOORD5;
		float3 starsTwinklingPos : TEXCOORD6;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f vert(appdata v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v); 
		UNITY_INITIALIZE_OUTPUT(v2f, o); 
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); 
		o.Position = UnityObjectToClipPos(v.vertex);
		float3 viewDir = normalize(v.texcoord + float3(0.0,0.1,0.0));
		
		o.sky.x = saturate(_SunDir.y + 0.25);
		o.sky.y = saturate(clamp(1.0 - _SunDir.y, 0.0, 0.5));
		o.sky.z = saturate(dot(-_MoonDir.xyz,viewDir));

		float3 r = normalize(cross(_MoonDir.xyz, float3(0, -1, 0)));
		float3 u = cross(_MoonDir.xyz,r);
		o.moonPos.xy = float2(dot(r, v.vertex.xyz), dot(u, v.vertex.xyz)) * (21.0 - _moonParams.x) + 0.5;
		o.moonPos.zw = float2(dot(r, v.vertex.xyz), dot(u, v.vertex.xyz)) * (21.0 - _moonParams.y) + 0.5;

		o.starPos = mul((float3x3)_StarsMatrix,v.vertex.xyz);
		o.starsTwinklingPos = mul((float3x3)_StarsTwinklingMatrix, v.vertex.xyz);
		o.night = pow(max(0.0,viewDir.y),1.25);
		o.texcoord = v.texcoord;
		o.screenUV = ComputeScreenPos(o.Position);

		return o;
	}

	float MoonPhaseFactor(float2 uv, float phase)
	{
		float alpha = 1.0;


		float srefx = uv.x - 0.5;
		float refx = abs(uv.x - 0.5);

		if (phase > 0)
		{
			srefx = (1 - uv.x) - 0.5;
			refx = abs((1 - uv.x) - 0.5);
		}

		phase = abs(_moonParams.w);
		float refy = abs(uv.y - 0.5);
		float refxfory = sqrt(0.25 - refy * refy);
		float xmin = -refxfory;
		float xmax = refxfory;
		float xmin1 = (xmax - xmin) * (phase / 2) + xmin;
		float xmin2 = (xmax - xmin) * phase + xmin;

		if (srefx < xmin1)
		{
			alpha = 0;
		}
		else if (srefx < xmin2 && xmin1 != xmin2)
		{
			alpha = (srefx - xmin1) / (xmin2 - xmin1);
		}

		return alpha;
	}

	float3 ScreenSpaceDither(float2 vScreenPos, float3 clr)
	{
		float d = dot(float2(131.0, 312.0), vScreenPos.xy + _Time.y);
		float3 vDither = float3(d, d, d);
		vDither.rgb = frac(vDither.rgb / float3(103.0, 71.0, 97.0)) - float3(0.5, 0.5, 0.5);
		return (vDither.rgb / 15.0) * _DitheringIntensity;
	}

	half3 tonemapACES(half3 color, float Exposure)
	{
		color *= Exposure;

		// See https://knarkowicz.wordpress.com/2016/01/06/aces-filmic-tone-mapping-curve/
		const half a = 2.51;
		const half b = 0.03;
		const half c = 2.43;
		const half d = 0.59;
		const half e = 0.14;
		return saturate((color * (a * color + b)) / (color * (c * color + d) + e));
	}

	float4 frag(v2f i) : SV_Target
	{
		float2 screenPosition = (i.screenUV.xy / i.screenUV.w);
		float3 viewDir = normalize(i.texcoord);
		float cosTheta = dot(viewDir,_SunDir);
		viewDir = normalize(i.texcoord + float3(0.0,0.1,0.0));
		float zen = acos(saturate(viewDir.y));
		float alb = (cos(zen) + 0.5 * pow(93.885 - ((zen * 180.0) / 3.141592), -0.253));
		float3 fex = exp(-(_Br * (4 / alb) + _Bm * (1.25 / alb)));
		float rayPhase = 2.5 + pow(cosTheta,1);
		float miePhase = _mieG.x / pow(_mieG.y - _mieG.z * cosTheta, 1);
		float3 BrTheta = 0.059683 * _Br * rayPhase;
		float3 BmTheta = 0.079577  * _Bm * miePhase;
		float3 BrmTheta = (BrTheta + BmTheta * 2.0) / ((_Bm + _Br) * 0.75);
		float3 scattering = BrmTheta * _SunIntensity * (1.0 - fex);
		float3 sunClr = lerp(fex, _sunDiskColor.rgb, 0.75) * _SunDiskIntensity;
		float3 sunDisk = (min(2, pow((1 - cosTheta) * (_SunDiskSize * 100), -2)) * sunClr) * (_sunDiskColor * 10);

		float4 moonSampler = tex2D(_MoonTex, i.moonPos.xy);
		float alpha = MoonPhaseFactor(i.moonPos.xy, _moonParams.w);
		float3 moonArea = clamp(moonSampler * 10, 0, 1)* i.sky.z;
		moonSampler = lerp(float4(0,0,0,0),moonSampler,alpha);
		moonSampler = (moonSampler * _MoonColor) * 2;

		float4 moonGlow = tex2D(_GlowTex, i.moonPos.zw) * i.sky.z;

		float3 skyFinalize = saturate((pow(1.0 - fex, 2.0) * 0.234) * (1 - i.sky.x)) * _SkyLuminance;
		skyFinalize = saturate(lerp(float3(0.01,0.01,0.01), skyFinalize, saturate(dot(viewDir.y + 0.3, float3(0,1,0)))) * (1 - fex));


		float fadeStar = i.night * _StarsIntensity * 75;
		float3 starsMap = texCUBE(_Stars, i.starPos.xyz);

		if (_StarsTwinkling > 0)
		{
			float3 starsTwinklingMap = texCUBE(_StarsTwinklingNoise, i.starsTwinklingPos.xyz);
			starsMap = starsMap * starsTwinklingMap;
		} 

		float starsBehindMoon = 1 - clamp((moonArea * 5), 0, 1);
		float3 stars = pow(clamp((starsMap * fadeStar) * starsBehindMoon,0,4),2);
		//float3 stars = pow(clamp((starsMap * 10) * starsBehindMoon,0,4),2);
		
		float3 galaxyMap = texCUBE(_Galaxy, i.starPos.xyz);
		float3 galaxy = galaxyMap * starsBehindMoon * (i.night * _GalaxyIntensity);
		//float3 galaxy = galaxyMap * starsBehindMoon * (_GalaxyIntensity);

		scattering *= saturate((lerp(float3(_scatteringPower, _scatteringPower, _scatteringPower), pow(2000.0f * BrmTheta * fex, 0.75f), i.sky.y) * 0.05));
		scattering *= (_SkyLuminance * _scatteringColor.rgb) * pow((1 - fex), 2) * i.sky.x;

		if (viewDir.y - 0.08 < 0)
			sunDisk = float3(0,0,0);

		float3 skyScattering = (scattering + sunDisk) + (skyFinalize + galaxy + stars);
		float4 satSampler = tex2D(_SatTex, screenPosition);

		skyScattering = satSampler.rgb + skyScattering * (1 - satSampler.a);
		skyScattering += (moonSampler.rgb * i.sky.z) + ((moonGlow.xyz * _moonGlowColor) * _moonParams.z) * (1 - moonSampler.a);

		//Tonemapping
		if (_Tonemapping == 1)
		{
			skyScattering.rgb = tonemapACES(skyScattering.rgb, _SkyExposure);
		}
			
		skyScattering = pow(skyScattering,_SkyColorPower);

		//half4 background = texCUBE(_Background, i.texcoord);
		//skyScattering = skyScattering * (1 - background.a) + background.rgb * background.a;

		skyScattering = lerp(skyScattering, (lerp(skyScattering,_weatherSkyMod.rgb,_weatherSkyMod.a)),_weatherSkyMod.a);

#if defined(UNITY_COLORSPACE_GAMMA)
		skyScattering = LinearToGammaSpace(skyScattering);
#endif

		if (viewDir.y + 0.1 < 0 && _blackGround > 0)
			skyScattering = 0;

		float3 final = float3(0, 0, 0);

		//Dithering
		final = skyScattering + ScreenSpaceDither(i.Position.xy, final.rgb);

		return float4(final , 1);
	}
		ENDCG
	}



		//AURORA
		Pass
	{
		Blend One One

		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile __ ENVIRO_AURORA
		#include "UnityCG.cginc"
		#pragma exclude_renderers gles


	sampler2D _Aurora_Layer_1;
	sampler2D _Aurora_Layer_2;
	sampler2D _Aurora_Colorshift;

	float4 _AuroraColor;
	float _AuroraIntensity;
	float _AuroraBrightness;
	float _AuroraContrast;
	float _AuroraHeight;
	float _AuroraScale;
	float _AuroraSpeed;
	float _AuroraSteps;

	float4 _Aurora_Tiling_Layer1;
	float4 _Aurora_Tiling_Layer2;
	float4 _Aurora_Tiling_ColorShift;

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float3 worldPos : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f vert(appdata_full v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v); //Insert
		UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

		o.vertex = UnityObjectToClipPos(v.vertex);
		o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		return o;
	}

	float randomNoise(float3 co) {
		return frac(sin(dot(co.xyz ,float3(17.2486,32.76149, 368.71564))) * 32168.47512);
	}

	half4 SampleAurora(float3 uv) {

		float2 uv_1 = uv.xy * _Aurora_Tiling_Layer1.xy + (_Aurora_Tiling_Layer1.zw * _AuroraSpeed * _Time.y);

		half4 aurora = tex2Dlod(_Aurora_Layer_1, float4(uv_1.xy,0,0));

		float2 uv_2 = uv_1 * _Aurora_Tiling_Layer2.xy + (_Aurora_Tiling_Layer2.zw * _AuroraSpeed * _Time.y);
		half4 aurora2 = tex2Dlod(_Aurora_Layer_2, float4(uv_2.xy,0,0));
		aurora += (aurora2 - 0.5) * 0.5;

		aurora.w = aurora.w * 0.8 + 0.05;

		float3 uv_3 = float3(uv.xy * _Aurora_Tiling_ColorShift.xy + (_Aurora_Tiling_ColorShift.zw * _AuroraSpeed * _Time.y), 0.0);
		half4 cloudColor = tex2Dlod(_Aurora_Colorshift, float4(uv_3.xy,0,0));

		half contrastMask = 1.0 - saturate(aurora.a);
		contrastMask = pow(contrastMask, _AuroraContrast);
		aurora.rgb *= lerp(half3(0,0,0), _AuroraColor.rgb * cloudColor.rgb * _AuroraBrightness, contrastMask);

		half cloudSub = 1.0 - uv.z;
		aurora.a = aurora.a - cloudSub * cloudSub;
		aurora.a = saturate(aurora.a * _AuroraIntensity);
		aurora.rgb *= aurora.a;

		return aurora;
	}

	fixed4 frag(v2f i) : SV_Target
	{
#if defined(ENVIRO_AURORA)

		if (_AuroraIntensity < 0.05)
		return float4(0,0,0,0);

	float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos);

	float viewFalloff = 1.0 - saturate(dot(viewDir, float3(0,1,0)));

	if (viewDir.y < 0 || viewDir.y > 1)
		return half4(0, 0, 0, 0);

	float3 traceDir = normalize(viewDir + float3(0, viewFalloff * 0.2 ,0));

	float3 worldPos = _WorldSpaceCameraPos + traceDir * ((_AuroraHeight - _WorldSpaceCameraPos.y) / max(traceDir.y, 0.01));
	float3 uv = float3(worldPos.xz * 0.01 * _AuroraScale, 0);

	half3 uvStep = half3(traceDir.xz * -1.0 * (1.0 / traceDir.y), 1.0) * (1.0 / _AuroraSteps);
	uv += uvStep * randomNoise(i.worldPos + _SinTime.w);

	half4 finalColor = half4(0,0,0,0);

	[loop]
	for (int iCount = 0; iCount < _AuroraSteps; iCount++)
	{
		if (finalColor.a > 1)
			break;

		uv += uvStep;
		finalColor += SampleAurora(uv) * (1.0 - finalColor.a);
	}

	finalColor *= viewDir.y;

	return finalColor;
#else
		return half4(0, 0, 0, 0);
#endif
	}
		ENDCG
	}

		//Cirrus Clouds
		Pass
	{
		Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM
		#pragma target 3.0
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		uniform sampler2D _CloudMap;
		uniform float _CloudAlpha;
		uniform float _CloudCoverage;
		uniform float _CloudAltitude;
		uniform float4 _CloudColor;
		uniform float _CloudColorPower;
		uniform float2 _CloudCirrusAnimation;

	struct appdata 
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f
	{
		float4 Position : SV_POSITION;
		float4 uv : TEXCOORD0;
		float3 worldPos : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	v2f vert(appdata v)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(v); //Insert
		UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert

		o.Position = UnityObjectToClipPos(v.vertex);
		o.worldPos = normalize(v.vertex).xyz;
		float3 viewDir = normalize(o.worldPos + float3(0,1,0));
		o.worldPos.y *= 1 - dot(viewDir.y + _CloudAltitude, float3(0,-0.15,0));
		return o;
	}

	float4 frag(v2f i) : SV_Target
	{
		float3 uvs = normalize(i.worldPos);

		float4 uv1;
		float4 uv2;

		uv1.xy = (uvs.xz * 0.2) + _CloudCirrusAnimation;
		uv2.xy = (uvs.xz * 0.4) + _CloudCirrusAnimation;

		float4 clouds1 = tex2D(_CloudMap, uv1.xy);
		float4 clouds2 = tex2D(_CloudMap, uv2.xy);

		float color1 = pow(clouds1.g + clouds2.g, 0.1);
		float color2 = pow(clouds2.b * clouds1.r, 0.2);

		float4 finalClouds = lerp(clouds1, clouds2, color1 * color2);
		float cloudExtinction = pow(uvs.y , 2);


		finalClouds.a *= _CloudAlpha;
		finalClouds.a *= cloudExtinction;

		if (uvs.y < 0)
			finalClouds.a = 0;

		finalClouds.rgb = finalClouds.a * pow(_CloudColor,_CloudColorPower);
		finalClouds.rgb = pow(finalClouds.rgb,1 - _CloudCoverage);

		return finalClouds;
	}
		ENDCG
	}
		///CIRRUS END
	}
}