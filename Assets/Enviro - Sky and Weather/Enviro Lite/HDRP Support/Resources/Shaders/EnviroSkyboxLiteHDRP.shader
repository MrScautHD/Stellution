Shader "Enviro/HDRP/Skybox Lite"
{

	//Properties
	//{
	//_Stars("Stars Cubemap", Cube) = "black" {}
	//_Galaxy("Galaxy Cubemap", Cube) = "black" {}
	//_SatTex("Satellites Tex", 2D) = "black" {}
	//_MoonTex("Moon Tex", 2D) = "black" {}
	//_MoonNormal("Moon Normal", 2D) = "black" {}
	//}
	HLSLINCLUDE


	//#pragma target 4.5
	#pragma editor_sync_compilation
	#pragma multi_compile __ ENVIROHDRP
	#if defined (ENVIROHDRP)
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonLighting.hlsl"
	#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
	#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Sky/SkyUtils.hlsl"

	uniform half4 _SkyColor;
	uniform half4 _HorizonColor;
	uniform half4 _SunColor;	
	uniform float4x4 _StarsMatrix;
	uniform half _StarsIntensity;
	uniform half _SunDiskSizeSimple;
	uniform float4 _weatherSkyMod;
	uniform half _BlackGround;
	uniform float3 _SunDir;
	uniform sampler2D _MoonTex;
	uniform float3 _MoonDir;
	uniform float4 _MoonColor;
	uniform float4 _moonParams;


	uniform float _EnviroSkyIntensity;
	float _DitheringIntensity;

	TEXTURECUBE(_Stars);
	SAMPLER(sampler_Stars);
	TEXTURECUBE(_StarsTwinklingNoise);
	SAMPLER(sampler_StarsTwinklingNoise);


	struct Attributes
	{
		uint vertexID : SV_VertexID;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct Varyings
	{
		float4 positionCS : SV_POSITION;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	Varyings Vert(Attributes input)
	{
		Varyings output;
		UNITY_SETUP_INSTANCE_ID(input);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
		output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID, UNITY_RAW_FAR_CLIP_VALUE);
		return output;
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

	half getMiePhase(half eyeCos, half eyeCos2, half y)
	{
		half temp = 1.0 + 0.9801 - 2.0 * (-0.990) * eyeCos;
		temp = pow(temp, pow(_SunDiskSizeSimple, 0.65) * 10);
		temp = max(temp, 1.0e-4); // prevent division by zero, esp. in half precision
		temp = 1.5 * ((1.0 - 0.9801) / (2.0 + 0.9801)) * (1.0 + eyeCos2) / temp;
//#if defined(UNITY_COLORSPACE_GAMMA) && SKYBOX_COLOR_IN_TARGET_COLOR_SPACE
//				temp = pow(temp, .454545);
//#endif
		return temp;
	}

	float3 ScreenSpaceDither(float2 vScreenPos, float3 clr)
	{
		float d = dot(float2(131.0, 312.0), vScreenPos.xy + _Time.y);
		float3 vDither = float3(d, d, d);
		vDither.rgb = frac(vDither.rgb / float3(103.0, 71.0, 97.0)) - float3(0.5, 0.5, 0.5);
		return (vDither.rgb / 15.0) * _DitheringIntensity * Luminance(clr);
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

	float4 Frag(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

		float3 viewDirWS = GetSkyViewDirWS(input.positionCS.xy);
		float3 dir = -viewDirWS;

		float3 wpos = normalize(mul((float4x4)UNITY_MATRIX_M, dir)).xyz;
		float3 viewDir = normalize(wpos + float3(0,0.2,0));

		//float3 viewDir = normalize(-viewDirWS);
		float night = pow(max(0.0, viewDir.y), 1.25);
		float3 sky = float3(saturate(_SunDir.y + 0.25), saturate(clamp(1.0 - _SunDir.y, 0.0, 0.5)), saturate(dot(-_MoonDir.xyz, viewDir)));

		float4 skyColor = float4(0, 0, 0, 1);

		if(_BlackGround == 1.0 && viewDir.y < 0.0)
			skyColor = float4(0, 0, 0, 1);
		else
		{		
			//float3 viewDir = normalize(i.WorldPosition + float3(0,0.2,0));

			// Moon UV
			float3 r = normalize(cross(_MoonDir.xyz, float3(0, 0, 1)));
			float3 u = cross(_MoonDir.xyz, r);
			float2 moonUV = float2(dot(r, dir), dot(u, dir)) * (21.0 - _moonParams.x) + 0.5;
			float4 moonSampler = tex2D(_MoonTex, moonUV);
			float alpha = MoonPhaseFactor(moonUV, 1-_moonParams.w);

			float3 moonArea = clamp(moonSampler * 10, 0, 1) * sky.z;
			moonSampler = lerp(float4(0, 0, 0, 0), moonSampler, alpha);
			moonSampler = (moonSampler * _MoonColor) * 2;
			float starsBehindMoon = 1 - clamp((moonArea * 5), 0, 1);

			float3 starsUV = mul((float3x3)_StarsMatrix, dir);
			float3 starsMap = SAMPLE_TEXTURECUBE_LOD(_Stars, sampler_Stars, starsUV, 0).rgb;

			float4 nightSky = float4(((_StarsIntensity * 50) * starsMap.rgb),1) * starsBehindMoon;
			skyColor = lerp(_HorizonColor,_SkyColor,smoothstep(dot(viewDir.y, float3(0,2,0)),0,0.3));

			if (viewDir.y < 0.0)
				skyColor = _HorizonColor;

			skyColor = skyColor + (1 - skyColor.a) * nightSky;
	
			half eyeCos = dot(_SunDir, -viewDir);
			half eyeCos2 = eyeCos * eyeCos;
			half mie = getMiePhase(eyeCos, eyeCos2,viewDir.y);
			skyColor += mie * _SunColor;

			skyColor.rgb += (moonSampler.rgb * sky.z);

			skyColor = lerp(skyColor, (lerp(skyColor, _weatherSkyMod, _weatherSkyMod.a)), _weatherSkyMod.a);
		}

		return float4(pow(skyColor.rgb,2) * _EnviroSkyIntensity * GetCurrentExposureMultiplier(), 1);
	}

	float4 FragBaking(Varyings input) : SV_Target
	{
		float3 viewDirWS = GetSkyViewDirWS(input.positionCS.xy);
		float3 dir = -viewDirWS;	 
		
		float3 wpos = normalize(mul((float4x4)UNITY_MATRIX_M, dir)).xyz;
		float3 viewDir = normalize(wpos + float3(0,0.2,0));

		float night = pow(max(0.0, viewDir.y), 1.25);
		float3 sky = float3(saturate(_SunDir.y + 0.25), saturate(clamp(1.0 - _SunDir.y, 0.0, 0.5)), saturate(dot(-_MoonDir.xyz, viewDir)));
			
		float4 skyColor = float4(0, 0, 0, 1);

		if(_BlackGround == 1.0 && viewDir.y < 0.0)
			skyColor = float4(0, 0, 0, 1);
		else
		{		
			//float3 viewDir = normalize(i.WorldPosition + float3(0,0.2,0));

			// Moon UV
			float3 r = normalize(cross(_MoonDir.xyz, float3(0, 0, 1)));
			float3 u = cross(_MoonDir.xyz, r);
			float2 moonUV = float2(dot(r, dir), dot(u, dir)) * (21.0 - _moonParams.x) + 0.5;
			float4 moonSampler = tex2D(_MoonTex, moonUV);
			float alpha = MoonPhaseFactor(moonUV, 1-_moonParams.w);

			float3 moonArea = clamp(moonSampler * 10, 0, 1) * sky.z;
			moonSampler = lerp(float4(0, 0, 0, 0), moonSampler, alpha);
			moonSampler = (moonSampler * _MoonColor) * 2;
			float starsBehindMoon = 1 - clamp((moonArea * 5), 0, 1);

			float3 starsUV = mul((float3x3)_StarsMatrix, dir);
			float3 starsMap = SAMPLE_TEXTURECUBE_LOD(_Stars, sampler_Stars, starsUV, 0).rgb;

			float4 nightSky = float4(((_StarsIntensity * 50) * starsMap.rgb),1) * starsBehindMoon;
			skyColor = lerp(_HorizonColor,_SkyColor,smoothstep(dot(viewDir.y, float3(0,2,0)),0,0.3));

			if (viewDir.y < 0.0)
				skyColor = _HorizonColor;

			skyColor = skyColor + (1 - skyColor.a) * nightSky;
	
			half eyeCos = dot(_SunDir, viewDir);
			half eyeCos2 = eyeCos * eyeCos;
			half mie = getMiePhase(eyeCos, eyeCos2,viewDir.y);
			skyColor += mie * _SunColor;

			skyColor.rgb += (moonSampler.rgb * sky.z);

			skyColor = lerp(pow(skyColor,2), (lerp(skyColor, _weatherSkyMod, _weatherSkyMod.a)), _weatherSkyMod.a);
		}

		return float4(skyColor.rgb * _EnviroSkyIntensity, 1);
	}


	uniform sampler2D _CloudMap;
	uniform float _CloudAlpha;
	uniform float _CloudCoverage;
	uniform float _CloudAltitude;
	uniform float4 _CloudColor;
	uniform float _CloudColorPower;
	uniform float2 _CloudCirrusAnimation;

	Varyings VertCirrus(Attributes input)
	{
		Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
		output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID, UNITY_RAW_FAR_CLIP_VALUE);
		return output;
	}

	float4 FragCirrus(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float3 viewDirWS = GetSkyViewDirWS(input.positionCS.xy);
		// Reverse it to point into the scene
		float3 dir = -viewDirWS;

		float3 wpos = normalize(mul((float4x4)UNITY_MATRIX_M, dir)).xyz;
		float3 viewDir = normalize(wpos + float3(0,1,0));
		wpos.y *= 1 - dot(viewDir.y + _CloudAltitude, float3(0,-0.15,0));

		float3 uvs = normalize(wpos);

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

		return float4(finalClouds.rgb * _EnviroSkyIntensity * GetCurrentExposureMultiplier(), finalClouds.a);
	}

	float4 FragCirrusBaking(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float3 viewDirWS = GetSkyViewDirWS(input.positionCS.xy);
		// Reverse it to point into the scene
		float3 dir = -viewDirWS;

		float3 wpos = normalize(mul((float4x4)UNITY_MATRIX_M, dir)).xyz;
		float3 viewDir = normalize(wpos + float3(0,1,0));
		wpos.y *= 1 - dot(viewDir.y + _CloudAltitude, float3(0,-0.15,0));

		float3 uvs = normalize(wpos);

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

		return float4(finalClouds.rgb * _EnviroSkyIntensity, finalClouds.a);
	}

	uniform sampler2D _FlatCloudsBaseTexture;
	uniform sampler2D _FlatCloudsDetailTexture;
	uniform float4 _FlatCloudsAnimation;
	uniform float3 _FlatCloudsLightDirection;
	uniform float3 _FlatCloudsLightColor;
	uniform float3 _FlatCloudsAmbientColor;
	uniform float4 _FlatCloudsLightingParams; // x = LightIntensity, y = AmbientIntensity, z = Absorbtion, w = HgPhase
	uniform float4 _FlatCloudsParams; // x = Coverage, y = Density, z = Altitude, w = tonemapping
	uniform float4 _FlatCloudsTiling; // x = Base, y = Detail
	uniform float _CloudsExposure;

	Varyings VertFlat(Attributes input)
	{
		Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
		output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID, UNITY_RAW_FAR_CLIP_VALUE);
		return output;
	}

	float Remap(float org_val, float org_min, float org_max, float new_min, float new_max)
	{
		return new_min + saturate(((org_val - org_min) / (org_max - org_min))*(new_max - new_min));
	}

	float HenryGreenstein(float cosTheta, float g)
	{
		float k = 3.0 / (8.0 * 3.1415926f) * (1.0 - g * g) / (2.0 + g * g);
		return k * (1.0 + cosTheta * cosTheta) / pow(abs(1.0 + g * g - 2.0 * g * cosTheta), 1.5);
	}

	float CalculateCloudDensity(float2 posBase, float2 posDetail, float coverage)
	{
		float4 baseNoise = tex2D(_FlatCloudsBaseTexture, posBase);
		float low_freq_fBm = (baseNoise.g * 0.625) + (baseNoise.b * 0.25) + (baseNoise.a * 0.125);
		float base_cloud = Remap(baseNoise.r, -(1.0 - low_freq_fBm), 1.0, 0.0, 1.0) * coverage;
	
		float4 detailNoise = tex2D(_FlatCloudsDetailTexture, posDetail * 2);
		float high_freq_fBm = (detailNoise.r * 0.625) + (detailNoise.g * 0.25) + (detailNoise.b * 0.125);
		float density = Remap(base_cloud, 1.0 - high_freq_fBm * 0.5, 1.0, 0.0, 1.0);
		
		density *= pow(high_freq_fBm, 0.4);
		density *= _FlatCloudsParams.y;
		
		return density;
	}

	float4 FragFlat(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float4 col;
		float3 viewDirWS = GetSkyViewDirWS(input.positionCS.xy);
		// Reverse it to point into the scene
		float3 dir = -viewDirWS;

		float3 wpos = normalize(mul((float4x4)UNITY_MATRIX_M, dir)).xyz;
		float3 viewDir = normalize(wpos + float3(0,1,0));
		wpos.y *= 1 - dot(viewDir.y + _FlatCloudsParams.z, float3(0,-0.2,0));

		float3 uvs = normalize(wpos);
		float4 uv1;
		uv1.xy = (uvs.xz * _FlatCloudsTiling.x) + _FlatCloudsAnimation.xy;
		uv1.zw = (uvs.xz * _FlatCloudsTiling.y) + _FlatCloudsAnimation.zw;

		float cloudExtinction = pow(uvs.y, 2);		
		float density = CalculateCloudDensity(uv1.xy, uv1.zw, _FlatCloudsParams.x);

		//Lighting	
		float absorbtion = exp2(-1 * (density * _FlatCloudsLightingParams.z));
		//float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos);
		float inscatterAngle = dot(normalize(_FlatCloudsLightDirection), -viewDir);
		float hg = HenryGreenstein(inscatterAngle, _FlatCloudsLightingParams.w) * 2 * absorbtion;
		float lighting = density * (absorbtion + hg);
		float3 lightColor = pow(_FlatCloudsLightColor, 2) * (_FlatCloudsLightingParams.x );
		col.rgb = lightColor * lighting;
		col.rgb = col.rgb + (_FlatCloudsAmbientColor * _FlatCloudsLightingParams.y);
		
		col.a = saturate(density * cloudExtinction);

		if (uvs.y < 0)
			col.a = 0;

		return float4(col.rgb * _EnviroSkyIntensity * GetCurrentExposureMultiplier(), col.a);
	}

	float4 FragFlatBaking(Varyings input) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
		float4 col;
		float3 viewDirWS = GetSkyViewDirWS(input.positionCS.xy);
		// Reverse it to point into the scene
		float3 dir = -viewDirWS;

		float3 wpos = normalize(mul((float4x4)UNITY_MATRIX_M, dir)).xyz;
		float3 viewDir = normalize(wpos + float3(0,1,0));
		wpos.y *= 1 - dot(viewDir.y + _FlatCloudsParams.z, float3(0,-0.2,0));

		float3 uvs = normalize(wpos);
		float4 uv1;
		uv1.xy = (uvs.xz * _FlatCloudsTiling.x) + _FlatCloudsAnimation.xy;
		uv1.zw = (uvs.xz * _FlatCloudsTiling.y) + _FlatCloudsAnimation.zw;

		float cloudExtinction = pow(uvs.y, 2);		
		float density = CalculateCloudDensity(uv1.xy, uv1.zw, _FlatCloudsParams.x);

		//Lighting	
		float absorbtion = exp2(-1 * (density * _FlatCloudsLightingParams.z));
		//float3 viewDir = normalize(i.worldPos - _WorldSpaceCameraPos);
		float inscatterAngle = dot(normalize(_FlatCloudsLightDirection), -viewDir);
		float hg = HenryGreenstein(inscatterAngle, _FlatCloudsLightingParams.w) * 2 * absorbtion;
		float lighting = density * (absorbtion + hg);
		float3 lightColor = pow(_FlatCloudsLightColor, 2) * (_FlatCloudsLightingParams.x );
		col.rgb = lightColor * lighting;
		col.rgb = col.rgb + (_FlatCloudsAmbientColor * _FlatCloudsLightingParams.y);
		
		col.a = saturate(density * cloudExtinction);

		if (uvs.y < 0)
			col.a = 0;

		return float4(col.rgb * _EnviroSkyIntensity, col.a);
	}

	#else
			sampler2D _MainTex;

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

			v2f VertCirrus (appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }

			v2f VertFlat (appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                o.uv = v.uv;
                return o;
            }

			float4 FragBaking (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }

			float4 Frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }

			float4 FragCirrus (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }

			float4 FragCirrusBaking (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
            }

			float4 FragFlat (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                col.rgb = 1 - col.rgb;
                return col;
			}

			float4 FragFlatBaking (v2f i) : SV_Target
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
            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

			HLSLPROGRAM
			#pragma vertex Vert
			#pragma fragment FragBaking
			ENDHLSL
		}

		// For fullscreen Sky
		Pass
		{
			ZWrite Off
			ZTest LEqual
			Blend Off
			Cull Off

			HLSLPROGRAM	
			#pragma vertex Vert
			#pragma fragment Frag
			ENDHLSL
		}

		//Cirrus Clouds
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Cull Off

			HLSLPROGRAM
			#pragma vertex VertCirrus
			#pragma fragment FragCirrus
			#pragma target 3.0
			ENDHLSL
		}

		//Cirrus Clouds Baking
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest Always
			Cull Off

			HLSLPROGRAM
			#pragma vertex VertCirrus
			#pragma fragment FragCirrusBaking
			#pragma target 3.0
			ENDHLSL
		}

		//Flat Clouds
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest LEqual
			Cull Off

			HLSLPROGRAM
			#pragma vertex VertFlat
			#pragma fragment FragFlat 
			#pragma target 3.0
			ENDHLSL
		}	

		//Flat Clouds Baking
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			ZTest Always
			Cull Off

			HLSLPROGRAM
			#pragma vertex VertFlat
			#pragma fragment FragFlatBaking
			#pragma target 3.0
			ENDHLSL
		}	
	}
}