#ifndef MASSIVE_CLOUDS_SHAPE_INCLUDED
#define MASSIVE_CLOUDS_SHAPE_INCLUDED

#include "MassiveCloudsScreenSpace.hlsl"
#include "MassiveCloudsInput.hlsl"

struct HorizontalRegion
{
    float height;
    float thickness;
    float2 softness;
};

struct Ray
{
    float from;
    float to;
    float max;
    float length;
};

struct HorizontalShapeView
{
    float CameraY;
    float CameraToBottomVDistance;
    float CameraToTopVDistance;
    float NearDistance;
    float FarDistance;
    float MaxDistance;
    float Length;
};

HorizontalShapeView CalculateHorizontalShapeView(
          ScreenSpace      ss,
          HorizontalRegion region)
{
    HorizontalShapeView hsv;

    _MaxDistance = max(0.1, _MaxDistance);
    const float3 up = float3(0, 1, 0);
    float  maxDist          = min(_MaxDistance, max(ss.maxDist, ss.isMaxPlane * _MaxDistance));
    float  cameraY          = (1 - _RelativeHeight) * ss.cameraPos.y;
    float  dbottom          = (region.height                       - cameraY);
    float  dtop             = ((region.height  + max(1, region.thickness)) - cameraY);
    float  horizontalFactor = dot(ss.rayDir, up);
    float  bottomDist       = max(0, dbottom / horizontalFactor);
    float  topDist          = max(0, dtop / horizontalFactor);
    
    float  fromDist         = min(bottomDist, topDist);
    float  toDist           = max(bottomDist, topDist);
    
    hsv.CameraY = cameraY;
    hsv.CameraToBottomVDistance = dbottom;
    hsv.CameraToTopVDistance = dtop;
    hsv.NearDistance = fromDist;
    hsv.FarDistance = toDist;
    hsv.MaxDistance = maxDist;
    hsv.Length = toDist - fromDist;
    
    return hsv;
}

inline Ray CalculateHorizontalRayRange(
          ScreenSpace      ss,
          HorizontalRegion region)
{
    HorizontalShapeView hsv = CalculateHorizontalShapeView(ss, region);

    Ray ray;

    ray.max    = hsv.MaxDistance;
#ifdef MASSIVE_CLOUDS_MATERIAL_ON
    ray.from   = min(hsv.MaxDistance, hsv.NearDistance);
    ray.to     = min(hsv.MaxDistance, hsv.FarDistance);
#else
    if (hsv.MaxDistance > hsv.NearDistance && hsv.FarDistance > hsv.MaxDistance)
    {
        ray.from   = min(hsv.MaxDistance, hsv.NearDistance);
        ray.to     = min(hsv.MaxDistance, hsv.FarDistance);
        ray.from   = hsv.NearDistance;
        ray.to     = hsv.FarDistance;
    }
    else
    {
        ray.from   = hsv.NearDistance;
        ray.to     = hsv.FarDistance;
    }
#endif

    ray.length = max(0, hsv.Length);
    ray.length = min(_MaxDistance / 4, ray.length);
    return ray;
}


inline Ray CalculateHorizontalRayRangeForStylized(
          ScreenSpace      ss,
          HorizontalRegion region)
{
    const float3 up = float3(0, 1, 0);
    float maxDist = min(_MaxDistance, max(ss.maxDist, ss.isMaxPlane * _MaxDistance));

    float  dbottom          = (region.height                      - (1 - _RelativeHeight) * ss.cameraPos.y);
    float  dtop             = ((region.height  + region.thickness) - (1 - _RelativeHeight) * ss.cameraPos.y);
    float  horizontalFactor = dot(ss.rayDir, up);
    float  bottomDist       = max(0, dbottom / horizontalFactor);
    float  topDist          = max(0, dtop / horizontalFactor);
    
    float  fromDist         = min(bottomDist, topDist);
    float  toDist           = max(bottomDist, topDist);

    Ray ray;
#ifdef MASSIVE_CLOUDS_MATERIAL_ON
    ray.from   = min(maxDist, fromDist);
    ray.to     = min(maxDist, toDist);
#else
    if (maxDist > fromDist && toDist > fromDist)
        //    if (toDist > fromDist)
        {
        ray.from   = min(maxDist, fromDist);
        ray.to     = min(maxDist, toDist);
        }
    else
    {
        ray.from   = fromDist;
        ray.to     = toDist;
    }
#endif
    ray.max    = maxDist;

    ray.length = max(0, ray.to - ray.from);
    ray.length = min(_MaxDistance / 4, ray.length);
    return ray;
}

inline Ray CalculateSphericalRayRange(ScreenSpace ss)
{
    Ray ray;
    ray.from   = _FromDistance;
//    ray.to     = lerp(min(_MaxDistance, ss.maxDist), _MaxDistance, ss.isMaxPlane);
    ray.to     = min(min(_MaxDistance, ss.maxDist), _FromDistance + _Thickness);
    ray.length = _Thickness;
    return ray;
}

HorizontalRegion CreateRegion()
{
    HorizontalRegion horizontalRegion;
    horizontalRegion.height = _FromHeight;
    horizontalRegion.thickness = _Thickness;
    horizontalRegion.softness = float2(_HorizontalSoftnessBottom, _HorizontalSoftnessTop);
    return horizontalRegion;
}


#endif