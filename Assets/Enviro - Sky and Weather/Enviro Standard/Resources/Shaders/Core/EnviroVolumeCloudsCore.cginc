UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
uniform float4x4 _InverseProjection;
uniform float4x4 _InverseRotation;
uniform float4x4 _InverseProjection_SP;
uniform float4x4 _InverseRotation_SP;
uniform sampler2D _MainTex;
uniform float4 _MainTex_TexelSize;
uniform sampler3D _Noise;
uniform sampler3D _DetailNoise;
uniform sampler2D _WeatherMap;
uniform sampler2D _CurlNoise;
uniform float4 _CloudsParameter;
uniform float4 _Steps;
uniform float4 _CloudsLighting; //x = ExtinctionCoef, y = HgPhaseFactor, z = Silver_intensity, w = Silver_spread
uniform float4 _CloudsLightingExtended; // x = EdgeDarkness, y = AmbientSkyColorIntensity, z = _Tonemapping, w = _CloudsExposure
uniform float4 _CloudsErosionIntensity; //x = Base, y = Detail
uniform float _BaseNoiseUV;
uniform float _DetailNoiseUV;
uniform float4 _CloudDensityScale;
uniform float _LightIntensity;
uniform float _AmbientSkyColorIntensity;
uniform float4 _CloudsCoverageSettings; //x = _GlobalCoverage, y = Bottom Coverage Mod, z = Top coverage mod, w = Clouds Up Morph Intensity
uniform float _GlobalCoverage;
uniform float4 _LightColor;
uniform float4 _MoonLightColor;
uniform float4 _AmbientLightColor;
uniform float4 _CloudsAnimation;
uniform float3 _LightDir;
uniform float _stepsInDepth;
uniform float _LODDistance;
uniform float _gameTime;
uniform float3 _CameraPosition;
////
uniform float4 _Randomness;
////

const float env_inf = 1e10;

uint intersectRaySphere(
	float3 rayOrigin,
	float3 rayDir, // must be normalized
	float3 sphereCenter,
	float  sphereRadius,
	out float2 t)
{
	float3 l = rayOrigin - sphereCenter;
	float a = 1.0f; // dot(rayDir, rayDir) where rayDir is normalized
	float b = 2.0f * dot(rayDir, l);
	float c = dot(l, l) - sphereRadius * sphereRadius;
	float discriminate = b * b - 4.0f * a * c;
	if (discriminate < 0.0f)
	{
		t.x = t.y = 0.0f;
		return 0u;
	}
	else if (abs(discriminate) - 0.00005f <= 0.0f)
	{
		t.x = t.y = -0.5f * b / a;
		return 1u;
	}
	else 
	{
		float q = b > 0.0f ? -0.5f * (b + sqrt(discriminate)) : -0.5f * (b - sqrt(discriminate));
		float h1 = q / a;
		float h2 = c / q;
		t.x = min(h1, h2);
		t.y = max(h1, h2);
		if (t.x < 0.0f)
		{
			t.x = t.y;
			if (t.x < 0.0f)
			{
				return 0u;
			}
			return 1u;
		}
		return 2u;
	}
}
 
float rand(float2 co) {
	float a = 12.9898;
	float b = 78.233;
	float c = 43758.5453;
	float dt = dot(co.xy, float2(a, b));
	float sn = fmod(dt, 3.14);

	return 2.0 * frac(sin(sn) * c) - 1.0;
}

float2 ResolveInside(float3 cameraPos, float3 cameraDir, float maxDistance)
{
	const float3 up = float3(0, 1, 0);

	maxDistance = min(_CloudsParameter.w, maxDistance);

	float bottom = (_CloudsParameter.x - cameraPos.y);
	float top = ((_CloudsParameter.x + _CloudsParameter.y) - cameraPos.y);

	float horizon = dot(cameraDir, up);
	float bottomDist = max(0, bottom / horizon);
	float topDist = max(0, top / horizon);

	float startDist = min(bottomDist, topDist);
	float endDist = max(bottomDist, topDist);

	startDist = min(maxDistance, startDist);
	endDist = min(maxDistance, endDist);

	return float2(startDist, endDist);
} 

// Realtime Volumetric Rendering Course Notes by Patapom (page 15)
float exponential_integral(float z) {
return 0.5772156649015328606065 + log(1e-4 + abs(z)) + z * (1.0 + z * (0.25 + z * ((1.0 / 18.0) + z * ((1.0 / 96.0) + z * (1.0 / 600.0))))); // For x!=0
}
 
// Realtime Volumetric Rendering Course Notes by Patapom (page 15)
float3 CalculateAmbientLighting(float altitude, float extinction_coeff, float3 skyColor)
{
float ambient_term = 0.6 * saturate(1.0 - altitude);
float3 isotropic_scattering_top = (skyColor.rgb * 0.25) * max(0.0, exp(ambient_term) - ambient_term * exponential_integral(ambient_term));

ambient_term = -extinction_coeff * altitude;
float3 isotropic_scattering_bottom = skyColor.rgb * 1.0 * max(0.0, exp(ambient_term) - ambient_term * exponential_integral(ambient_term)) * 1.5;

isotropic_scattering_top *= saturate(altitude);

return (isotropic_scattering_top)+(isotropic_scattering_bottom);
}

float HenryGreenstein(float cosTheta, float g) {

	float k = 3.0 / (8.0 * 3.1415926f) * (1.0 - g * g) / (2.0 + g * g);
	return k * (1.0 + cosTheta * cosTheta) / pow(abs(1.0 + g * g - 2.0 * g * cosTheta), 1.5);
}

float Remap(float org_val, float org_min, float org_max, float new_min, float new_max)
{
	return new_min + saturate(((org_val - org_min) / (org_max - org_min))*(new_max - new_min));
}

float4 GetHeightGradient(float cloudType)
{
	const float4 CloudGradient1 = float4(0.0, 0.05, 0.1, 0.25);
	const float4 CloudGradient2 = float4(0.0, 0.05, 0.4, 0.8);
	const float4 CloudGradient3 = float4(0.0, 0.05, 0.6, 1.0);

	float a = 1.0 - saturate(cloudType * 2.0);
	float b = 1.0 - abs(cloudType - 0.5) * 2.0;
	float c = saturate(cloudType - 0.5) * 2.0;

	return CloudGradient1 * a + CloudGradient2 * b + CloudGradient3 * c;
}

float GradientStep(float a, float4 gradient)
{
	return smoothstep(gradient.x, gradient.y, a) - smoothstep(gradient.z, gradient.w, a);
}

float3 GetWeather(float3 pos)
{
	float2 uv = pos.xz * 0.00001 + 0.5;
	return tex2Dlod(_WeatherMap, float4(uv, 0.0, 0.0));
}

float GetSamplingHeight(float3 pos, float3 center)
{
	return (length(pos - center) - (_CloudsParameter.w + _CloudsParameter.x)) * _CloudsParameter.z;
}
 
float3 ScreenSpaceDither(float2 vScreenPos, float lum)
{
	float d = dot(float2(131.0, 312.0), vScreenPos.xy + _Time.y);
	float3 vDither = float3(d, d, d);
	vDither.rgb = frac(vDither.rgb / float3(103.0, 71.0, 97.0)) - float3(0.5, 0.5, 0.5);
	return (vDither.rgb / 15.0) * 1.0 * lum;
}

float GetRaymarchEnd(float sceneDepth, float3 dir) 
{
	float raymarchEnd = 0.0f;	
#if ENVIRO_DEPTHBLENDING
	if (sceneDepth == 1.0f) 
	{
		raymarchEnd = 1e7;
	}
	else 
	{
		raymarchEnd = length(dir);
	//	raymarchEnd -= _ProjectionParams.y;
	}
#else
	raymarchEnd = 1e8;	
#endif
	return raymarchEnd;
}

float set_range_clamped(float value, float low, float high) {
	float ranged_value = clamp(value, low, high);
	ranged_value = (ranged_value - low) / (high - low);
	return saturate(ranged_value);
}

float get_fade_term(float3 sample_pos) {
	float distance = length(sample_pos.xy);
	return saturate((distance - 5000) / 20000.0);
}

float get_altitude_scalar(float cloud_type) {
	return lerp(8.0, 2.0, cloud_type);
}

float HeightAlter(float percent_height, float weather) {
	float cloud_anvil_amount = 0.5;
	float global_coverage = 0.5;
	// Round bottom a bit
	float ret_val = saturate(Remap(percent_height, 0.0, 0.07, 0.0, 1.0));
	// Round top a lot
	float stop_height = saturate(weather + 0.12);
	ret_val *= saturate(Remap(percent_height, stop_height *	0.2, stop_height, 1.0, 0.0));
	// Apply anvil ( cumulonimbus /" giant storm" clouds)
	ret_val = pow(ret_val, saturate(Remap(percent_height, 0.65, 0.95, 1.0, (1 - cloud_anvil_amount * global_coverage))));
	return ret_val;
}

 float DensityAlter(float percent_height) {
	float cloud_anvil_amount = 1.0;	 
	// Have density be generally increasing over height
	float ret_val = percent_height;	
	// Reduce density at base
	ret_val *= saturate(Remap(percent_height, 0.0, 0.2, 0.0, 1.0));
	ret_val *= 2;	
	// Reduce density for the anvil ( cumulonimbus clouds)
	ret_val *= lerp(1, saturate(Remap(pow(percent_height, 0.5) , 0.4, 0.95, 1.0, 0.2)), cloud_anvil_amount);
	// Reduce density at top to make better transition
	ret_val *= saturate(Remap(percent_height, 0.9, 1.0, 1.0, 0.0));	
	return ret_val;	
}

// Sample Cloud Density
float CalculateCloudDensity(float3 pos, float3 PlanetCenter, float3 weather, float mip, float dist, bool details)
{
	const float baseFreq = 1e-5;

	// Get Height fraction
	float height = GetSamplingHeight(pos, PlanetCenter);

	// wind settings
	float cloud_top_offset = 20.0;
	float3 wind_direction = float3(_CloudsAnimation.z, 0.0, _CloudsAnimation.w);

	// skew in wind direction
	pos += height * wind_direction * cloud_top_offset;

	float mip1 = mip + (1-dist) * (3.5 * _LODDistance);

	float4 coord = float4(pos * baseFreq * _BaseNoiseUV, mip1);
	// Animate Wind
	coord.xyz += float3(_CloudsAnimation.x, _CloudsErosionIntensity.w, _CloudsAnimation.y);
	 
	float4 baseNoise = 0;

	baseNoise = tex3Dlod(_Noise, coord);

	float low_freq_fBm = (baseNoise.g * 0.625) + (baseNoise.b * 0.25) + (baseNoise.a * 0.125);
	float base_cloud = Remap(baseNoise.r, -(1.0 - low_freq_fBm) * _CloudsErosionIntensity.x, 1.0, 0.0, 1.0);

	float heightGradient = GradientStep(height, GetHeightGradient(weather.b));

	base_cloud *= heightGradient;

	float cloud_coverage = saturate(1 - weather.r);
	    
	float densAlter = DensityAlter(1-height);
	cloud_coverage = pow(cloud_coverage, densAlter);

	//cloud_coverage = pow(cloud_coverage, Remap(height, 0.7, 0.8, 1.0, lerp(1.0, 0.5, 1.0)));

	float cloudDensity = Remap(base_cloud, cloud_coverage, 1.0, 0.0, 1.0);

	cloudDensity *= saturate(1-cloud_coverage);

	//DETAIL
	[branch]  
	if (details)
	{ 		 
		float mip2 = mip + (1-dist) * (_LODDistance);	
		coord = float4(pos * baseFreq * _DetailNoiseUV, mip2);
#ifdef ENVIRO_CURLNOISE
		float2 curl_noise = tex2D(_CurlNoise, coord.xy * 0.75).rg; 
		coord.xy += curl_noise.rg * height; 
#endif
		coord.xyz += float3(_CloudsAnimation.x, _CloudsErosionIntensity.w, _CloudsAnimation.y);		 
		float3 detailNoise = tex3Dlod(_DetailNoise, coord).rgb;		
		float high_freq_fBm = (detailNoise.r * 0.625) + (detailNoise.g * 0.25) + (detailNoise.b * 0.125);
		float high_freq_noise_modifier = lerp(high_freq_fBm, 1.0f - high_freq_fBm, saturate((height) * 20));		
		cloudDensity = Remap(cloudDensity, high_freq_noise_modifier * _CloudsErosionIntensity.y, 1.0, 0.0, 1.0);
	}
	 
	return cloudDensity;
}
  

// Lighting Energy Function
float GetLightEnergy(float3 p, float height_fraction, float dl, float ds_loded, float phase_probability, float cos_angle, float step_size, float brightness, float3 weather)
{ 
	brightness *= 300 *  weather.g;
	float sc = lerp(dl * 0.25, _CloudsCoverageSettings.y, 0.75);
	float s1 = lerp(_CloudsErosionIntensity.z * 0.75, _CloudsErosionIntensity.z, cos_angle);
	float prim_att = exp(-s1 * dl);
	float sec_att = exp(-s1 * sc) * 0.7;
	float attenuation_probability = max(Remap(cos_angle, 0.5, 1.0, sec_att, sec_att * 0.25), prim_att) ;	 
	float vertical_probability = pow(Remap(height_fraction, 0.07, 0.3, 0.1, 1.0), 0.8);
	float depth = _CloudsLightingExtended.x * pow(lerp(0.25, ds_loded * 1.5, Remap(height_fraction,0.0,1.0,0.5,1.0)), Remap(height_fraction, 0.1, 1.0, 0.5, 0.75));
	float in_scatter = depth;
	in_scatter = 0.05 + saturate(in_scatter);
	in_scatter = saturate(lerp(in_scatter, 0.45, cos_angle - 0.1));

	float light_energy = attenuation_probability * in_scatter * vertical_probability * phase_probability * brightness;
	return light_energy;
}      


static const float shadowSampleDistance[6] = {0.0, 0.5, 1.0, 1.5, 2.0, 6.0};
static const float LightingInfluence[6] = { { 1.0f },{ 2.0f },{ 3.0f },{ 4.0f },{ 6.0f },{ 8.0f } };

// Lighting Sample Function
float GetDensityAlongRay(float3 pos, float3 PlanetCenter, float3 LightDirection, float3 weather, float dist)
{
	float opticalDepth = 0.0;

	[loop]
	for (int i = 0; i < 6; i++)
	{

		float3 samplePoint = pos + LightDirection * shadowSampleDistance[i]  * (512 * _CloudDensityScale.y);

		float mip_offset = i * 0.5;

		//if (opticalDepth < 0.3)
			opticalDepth += CalculateCloudDensity(samplePoint, PlanetCenter, weather, mip_offset, dist, true) * LightingInfluence[i];
		//else
		//	opticalDepth += CalculateCloudDensity(samplePoint, PlanetCenter, weather, mip_offset, dist, false) * LightingInfluence[i];
		 
	}

	return opticalDepth;
}

