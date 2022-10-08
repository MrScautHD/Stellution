#ifndef MASSIVE_CLOUDS_SCREEN_SPACE_SHADOW_INCLUDED
#define MASSIVE_CLOUDS_SCREEN_SPACE_SHADOW_INCLUDED

#include "Includes/MassiveCloudsLight.hlsl"
#include "Includes/MassiveCloudsSampler.hlsl"
#include "MassiveCloudsShadow.hlsl"
#include "MassiveCloudsRaymarch.hlsl"


half4 MassiveCloudsFragment(v2f i) : SV_Target
{
    PrepareSampler();
    PrepareLighting();
    float exposureMultiplier = ExposureMultiplier();

    ScreenSpace ss = CreateScreenSpace(i.uv);

#if defined(USING_STEREO_MATRICES)
    half4 screenCol = tex2Dproj(_MainTex, UnityStereoScreenSpaceUVAdjust(i.uv, _MainTex_ST));
#else 
    half4 screenCol = tex2Dproj(_MainTex, i.uv);
#endif
    #if defined(_SHADOW_ON) && defined(_HORIZONTAL_ON)
    HorizontalRegion region = CreateRegion();
    ScreenSpaceShadow(screenCol, ss.worldPos, ss);
    #endif

    return screenCol;
}

#endif