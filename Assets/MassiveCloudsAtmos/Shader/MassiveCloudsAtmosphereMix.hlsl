#ifndef MASSIVE_CLOUDS_ATMOSPHERE_MIX
#define MASSIVE_CLOUDS_ATMOSPHERE_MIX

#include "MassiveCloudsCommon.hlsl"
#include "Includes/PipelineDependent/MassiveCloudsExposure.hlsl"
#include "Includes/MassiveCloudsShape.hlsl"
#include "MassiveCloudsPhysicsCloud.hlsl" 
#include "MassiveCloudsProceduralSky.hlsl" 

float _ScreenBlending;
float _AmbientPivot;
float _AtmosphereColoring;
float3 _AtmosphereColor;
float _AtmosphereHighLightColoring;
float3 _AtmosphereHighLightColor;
float _CloudAtmospheric;

// Height Fog
float  _HeightFogColoring;
half4 _HeightFogColor;
float  _HeightFogGroundHeight;
float  _HeightFogRange;
float  _HeightFogDensity;
float  _HeightFogScattering;

sampler2D _ScreenTexture;
half4     _ScreenTexture_ST;
sampler2D _CloudsTexture;
half4     _CloudsTexture_ST;
float4    _CloudsTexture_TexelSize;
sampler2D _CloudsTexture1;
half4     _CloudsTexture1_ST;
float4    _CloudsTexture1_TexelSize;
sampler2D _CloudsTexture2;
half4     _CloudsTexture2_ST;
float4    _CloudsTexture2_TexelSize;

float3 ScreenBlend(float3 src, float3 dst)
{
    float3 one = float3(1, 1, 1);
    return one - ((one - src) * (one - saturate(dst)));
}

float3 AdditiveBlend(float3 src, float3 dst)
{
    return src + dst;
}

float _MassiveCloudsProbeScale;

float3 Mix(CloudFactor cloudFactor, float3 col, ScreenSpace ss, Ray ray)
{
    #if UNITY_COLORSPACE_GAMMA
        float3 atmosphereCol = fixColorSpaceToLinear(_AtmosphereColor.rgb);
        float3 highLightAtmosphereCol = fixColorSpaceToLinear(_AmbientMidColor.rgb);
        float3 ambientTopColor = fixColorSpaceToLinear(_AmbientTopColor.rgb);
        float3 ambientMidColor = fixColorSpaceToLinear(_AmbientMidColor.rgb);
        float3 ambientBottomColor = fixColorSpaceToLinear(_AmbientBottomColor.rgb);
    #else
        float3 atmosphereCol = _AtmosphereColor.rgb;
        float3 highLightAtmosphereCol = _AtmosphereHighLightColor.rgb;
        float3 ambientTopColor = _AmbientTopColor.rgb;
        float3 ambientMidColor = _AmbientMidColor.rgb;
        float3 ambientBottomColor = _AmbientBottomColor.rgb;
//return ExposureMultiplier() * ambientMidColor;
    #endif
    float  fakeMie = FakeMie(MassiveCloudsLightDirection, ss.rayDir);
    float3 lightCol = MassiveCloudsLightColor.rgb;
    atmosphereCol = lerp(0.5 * lightCol, atmosphereCol * MassiveCloudsLightIntensity * ExposureMultiplier(), _AtmosphereColoring);
    highLightAtmosphereCol = lerp(0.5 * lightCol, highLightAtmosphereCol * MassiveCloudsLightIntensity * ExposureMultiplier(), _AtmosphereHighLightColoring);
    float  d = pow(saturate(0.5 + 0.5 * dot(MassiveCloudsLightDirection, ss.rayDir)), 2);
    float3 atmosphereDstCol = lerp(atmosphereCol, highLightAtmosphereCol, d * fakeMie);

    float treshold = _AmbientPivot;
    float3 ambient  = ExposureMultiplier() * lerp(
        lerp(ambientBottomColor, ambientMidColor, saturate(cloudFactor.scattering / treshold)),
        ambientTopColor,
        cloudFactor.scattering);

    float3 l = lerp(1, atmosphereDstCol, _CloudAtmospheric);
    float3 dstCol = cloudFactor.density * l * _MassiveCloudsProbeScale * MassiveCloudsLightColor.rgb * cloudFactor.scattering;
    float3 cloudCol = lerp(col, ambient, cloudFactor.density);
    float  maxDist  = lerp(ss.maxDist, _MaxDistance, ss.isMaxPlane);
    if (ss.isMaxPlane > 0 || (ray.to > ray.from && maxDist > ray.from))
    {
        col = lerp(col, cloudCol, lerp(cloudFactor.density, 0, _Transparency));
        col = lerp(AdditiveBlend(col, dstCol), ScreenBlend(col, dstCol), _ScreenBlending);
    }
    return col;
}

Ray CalculateFogRay(ScreenSpace ss, HorizontalRegion region)
{
    HorizontalShapeView hsv = CalculateHorizontalShapeView(ss, region);
    Ray ray;
    ray.max    = hsv.MaxDistance;
    ray.from   = hsv.NearDistance;
    ray.to     = min(hsv.MaxDistance, hsv.FarDistance);
    ray.length = max(0, ray.to - ray.from);
    return ray;
}

float HeightFogDensity(ScreenSpace ss, HorizontalRegion region, Ray ray)
{
    if (ray.length <= 0) return 0;
    float3 from = ss.cameraPos + ray.from * ss.rayDir;
    float3 to = ss.cameraPos + ray.to * ss.rayDir;
    float dFrom = 1 - saturate((from.y - region.height) / region.thickness);
    float dTo   = 1 - saturate((to.y - region.height) / region.thickness);
    return ray.length * ((dTo - dFrom) / 2 + dFrom);
}

float HeightFog(ScreenSpace ss, float3 col, float fakeMie)
{
    HorizontalRegion region;
    region.height = _HeightFogGroundHeight;
    region.thickness = _HeightFogRange;
    region.softness = 0;

    HorizontalShapeView hsv = CalculateHorizontalShapeView(ss, region);

    float ceilFactor = saturate((ss.cameraPos.y - (region.height + region.thickness))/ 10000);
    float limitDist = min(hsv.MaxDistance, lerp(5000, 200000, ceilFactor));

    Ray ray;
    ray.max    = hsv.MaxDistance;
    ray.from   = hsv.NearDistance;
    ray.to     = min(limitDist, hsv.FarDistance);
    ray.length = ray.to - ray.from;
    float density = HeightFogDensity(ss, region, ray);
    
    if (ss.cameraPos.y < region.height)
    {
        if (ray.to > 0)
            density += min(ray.from, limitDist);
        else
            density += limitDist;
    }
    else if (ss.rayDir.y < 0)
        density += max(0, limitDist - ray.to);
    
    if (density <= 0) return 0;
    float f = 1 - 1.0 / exp(density * pow(_HeightFogDensity, 4));
    float over = ss.cameraPos.y - _HeightFogGroundHeight - _HeightFogRange;
    return f * (1 - 0.75 * saturate(over / 1000));
}

float4 MassiveCloudsFragment(v2f i, float4 screenCol, ScreenSpace ss)
{
    float fakeMie = FakeMie(MassiveCloudsLightDirection, ss.rayDir);
    float fogScattering = AroundScattering(MassiveCloudsLightDirection, ss.rayDir);

#ifdef _HORIZONTAL_ON
    HorizontalRegion region = CreateRegion();
    Ray ray = CalculateHorizontalRayRange(ss, region);
#else
    Ray ray = CalculateSphericalRayRange(ss);
#endif

#ifdef _SkyEnabled
    if (ss.isMaxPlane > 0.9) screenCol = ProceduralSkyFrag(ss.rayDir, ExposureMultiplier());
#endif

    CloudFactor      cloudFactor1       = UnpackCloud(MC_TEX2DPROJ_ST(_CloudsTexture1, i.uv, _MainTex_ST));
    CloudFactor      cloudFactor2       = UnpackCloud(MC_TEX2DPROJ_ST(_CloudsTexture2, i.uv, _MainTex_ST));
    CloudFactor      cloudFactor = cloudFactor1;
    float3 col      = screenCol.rgb;
    float heightFog = HeightFog(ss, col, fakeMie);
    float fogDim    = 1 - heightFog;
    float3 heightFogCol = lerp(ExposureMultiplier() * _AmbientBottomColor, _HeightFogColor.rgb, _HeightFogColoring);

    cloudFactor.density = max(cloudFactor1.density, cloudFactor2.density);
    cloudFactor.scattering = lerp(cloudFactor2.scattering, cloudFactor1.scattering, cloudFactor1.density);

    if (ss.cameraPos.y >= _HeightFogGroundHeight + _HeightFogRange)
        col = lerp(col, lerp(heightFogCol, heightFogCol + MassiveCloudsLightColor * fogScattering, _HeightFogScattering), heightFog);

    if (cloudFactor1.density < 1) col = Mix(cloudFactor2, col, ss, ray);
    col = Mix(cloudFactor1, col, ss, ray);

    if (ss.cameraPos.y < _HeightFogGroundHeight + _HeightFogRange)
        col = lerp(col, lerp(heightFogCol, heightFogCol + MassiveCloudsLightColor * fogScattering, _HeightFogScattering), heightFog);

    float  eye           = _WorldSpaceCameraPos.y;
    float  atmosphereDim = saturate((11000 - eye) / 10000);
    AtmosphereFactor atmosphereFactor  = UnpackAtmosphere(MC_TEX2DPROJ(_MainTex, i.uv));
    atmosphereFactor.scattering = fixColorSpaceToGamma(atmosphereFactor.scattering) * atmosphereDim;
    atmosphereFactor.shaft = fixColorSpaceToGamma(atmosphereFactor.shaft);

    float3 lightCol = MassiveCloudsLightColor.rgb;
    float3 atmosphereCol = 0.5 * lerp(lightCol, _AtmosphereColor.rgb * MassiveCloudsLightIntensity * ExposureMultiplier(), _AtmosphereColoring);
    float3 highLightAtmosphereCol = 0.5 * lerp(lightCol, _AtmosphereHighLightColor * MassiveCloudsLightIntensity * ExposureMultiplier(), _AtmosphereHighLightColoring);
    float  d = pow(saturate(0.5 + 0.5 * dot(MassiveCloudsLightDirection, ss.rayDir)), 2);
    float3 atmosphereDstCol = lerp(atmosphereCol, highLightAtmosphereCol, d * fakeMie);

    float shadowFactor = _Shadow * pow(1 - atmosphereFactor.scattering, 4) *
         (1 - cloudFactor.density) *
         (1 - ss.isMaxPlane) *
         fogDim *
         saturate(30 * atmosphereFactor.shadow);
    float dstScattering = atmosphereFactor.scattering *
        lerp(1, 1 - _CloudOcclusion, pow(cloudFactor.density, 4) * cloudFactor.scattering);
    dstScattering *= saturate(1 - 0.5 * shadowFactor);
    float3 dstCol = atmosphereDstCol * dstScattering;
    col *= (1 - 0.5 * shadowFactor);
    col = lerp(AdditiveBlend(col, dstCol), ScreenBlend(col, dstCol), _ScreenBlending);
    float3 shaftCol = MassiveCloudsLightColor.rgb * atmosphereFactor.shaft;
    col = lerp(AdditiveBlend(col, shaftCol), ScreenBlend(col, shaftCol), _ScreenBlending);
    return float4(col, 1);
}

float4 FragAll(v2f i) : SV_Target
{
    PrepareSampler();
    PrepareLighting();

    ScreenSpace ss = CreateScreenSpace(i.uv);
     
    float3 screenCol = MC_TEX2DPROJ_ST(_ScreenTexture, i.uv, _MainTex_ST);
    return MassiveCloudsFragment(i, float4(screenCol, 1), ss);
}

#endif