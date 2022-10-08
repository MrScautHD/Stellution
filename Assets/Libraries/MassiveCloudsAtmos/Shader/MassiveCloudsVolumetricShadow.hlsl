#ifndef MASSIVE_CLOUDS_VOLUMETRIC_SHADOW_INCLUDED
#define MASSIVE_CLOUDS_VOLUMETRIC_SHADOW_INCLUDED

#include "MassiveCloudsRaymarch.hlsl"

sampler2D _ScreenTexture;
half4     _ScreenTexture_ST;
sampler2D _CloudTexture;

Ray CalculateVolumetricShadowRayRange(
          ScreenSpace      ss,
          HorizontalRegion region)
{
    HorizontalShapeView hsv = CalculateHorizontalShapeView(ss, region);

    Ray ray;

    ray.max    = hsv.NearDistance;
#ifdef MASSIVE_CLOUDS_MATERIAL_ON
    ray.from   = min(hsv.MaxDistance, hsv.NearDistance);
    ray.to     = min(hsv.MaxDistance, hsv.FarDistance);
#else
    if (hsv.CameraY >= region.height)
    {
        ray.from   = hsv.CameraToBottomVDistance;
        if (ss.rayDir.y >= 0) ray.to = 0;
        else                  ray.to = hsv.MaxDistance;
    }
    else
    {
        ray.from   = 0;
        if (hsv.FarDistance <= 0) ray.to = hsv.MaxDistance;
        else                      ray.to = min(hsv.MaxDistance, hsv.NearDistance);
    }
#endif

    ray.length = max(0, hsv.FarDistance - hsv.NearDistance);
    ray.length = min(_MaxDistance / 4, hsv.Length);
    return ray;
}

float VolumetricShadowAttn(float3 worldPos, ScreenSpace ss)
{
    HorizontalRegion horizontalRegion = CreateRegion();
    float3 cameraPos = _WorldSpaceCameraPos;
    float3 lightDir = normalize(MassiveCloudsLightDirection);
    float3 forward = worldPos - _WorldSpaceCameraPos;
    if (worldPos.y > (horizontalRegion.height + horizontalRegion.thickness/ 2)) return 0;
    float3 dCameraPos = float3(0, -(_RelativeHeight) * cameraPos.y, 0);
    float upDotLight = saturate(dot(float3(0,1,0), lightDir));
    float bottomDist = max(0, horizontalRegion.height - worldPos.y - dCameraPos.y) / upDotLight;
    float topDist = max(0, horizontalRegion.height + horizontalRegion.thickness - worldPos.y - dCameraPos.y) / upDotLight;
    float thickness = topDist - bottomDist;
    float mid = bottomDist + (0.05 + 0.2 * horizontalRegion.softness[0]) * thickness;
    bottomDist = mid - 0.2 * thickness * 0;
    topDist = mid + 0.8 * thickness * 1 + 1;
    float shadowIter = 1;
    float shadow = ShadowRaymarch(float4(0,0,0,0),
                                    dCameraPos + worldPos + bottomDist * lightDir,
                                    lightDir,
                                    topDist,
                                    (topDist - bottomDist) / shadowIter,
                                    shadowIter);
    float att = shadow;
    return att;
}

void VolumetricShadow(inout float4 col, float4 cloudLayer, float4 screeLayer, Ray ray, ScreenSpace ss, HorizontalRegion region)
{
    if (col.r > 0 && abs(pow(col.r, 0.5) - pow(ss.depth, 0.5)) < 0.01)
        return;

    if (ss.cameraPos.y > region.height && cloudLayer.a >= 1)
    {
        col.g = 1;
        col.r = ss.depth;
        return;
    }

    if (col.r > 0)
        col.b = 1;
    else
        col.r = ss.depth;

    float3 lightDir = normalize(MassiveCloudsLightDirection);
    float upDotLight = abs(dot(float3(0,1,0), lightDir));
    float progression = ray.from;
    float dim = 0;
    float step = 10 + 5 * sin(_Time.w * 1);
    float3 pos = ss.cameraPos + ss.rayDir * ray.from;
    float maxTo = min(_MaxDistance / 2, ray.to);

    float mie = pow((1 + dot(ss.rayDir, MassiveCloudsLightDirection)) / 2, 1);
    float l = max(luminance(cloudLayer.rgb), 0);
    if (!ss.isMaxPlane) l = luminance(screeLayer.rgb);
    
    for (float i = 0; ; ++i)
    {
        progression += step;
        if (progression >= maxTo) break;
        pos += step * ss.rayDir;
        float heightFactor = saturate( (region.height - pos.y) / 100 );
        float attn = VolumetricShadowAttn(pos, ss);
        float shadow = heightFactor * attn;
        float stepfactor = step;
        
        float dust = _VolumetricShadowDensity;
        float threshold = 0.01;
        float light = attn - threshold;
        float t = progression / maxTo;
        if (light >= 0)
        {
            dim += (1 - l) * (0.1 + 0.9 * pow(1 - t, 2)) * light * pow(1 - dim, 2) * dust * stepfactor;
        }
        else
        {
            dim = dim - pow(1 - t, 2) * pow(saturate(dim), 2) * _VolumetricShadowStrength;
            dim = saturate(dim);
        }
        step += 6 * saturate(progression / 1000 - 1);
    }
    if (ss.cameraPos.y > region.height)
        col.a = 1 * saturate(pow( dim, 1 ) - cloudLayer.a);
    else
        col.a = 1 * saturate(pow( dim, 1 ));
}

half4 MassiveCloudsVolumetricShadowFragment(v2f i) : SV_Target
{
    PrepareSampler();
    PrepareLighting();

    ScreenSpace ss = CreateScreenSpace(i.uv);

#ifdef _HORIZONTAL_ON
    HorizontalRegion region = CreateRegion();
#endif

#if defined(USING_STEREO_MATRICES)
    float4 screenCol = tex2Dproj(_ScreenTexture, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
    float4 texCol = tex2Dproj(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
    float4 cloudCol = tex2Dproj(_CloudTexture, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
#else 
    float4 screenCol = tex2Dproj(_ScreenTexture, i.uv);
    float4 texCol = tex2Dproj(_MainTex, i.uv);
    float4 cloudCol = tex2Dproj(_CloudTexture, i.uv);
#endif

#ifdef _HORIZONTAL_ON
    Ray shadowRay = CalculateVolumetricShadowRayRange(ss, region);
    VolumetricShadow(texCol, cloudCol, screenCol, shadowRay, ss, region);
#endif

    return texCol;
}

#endif 