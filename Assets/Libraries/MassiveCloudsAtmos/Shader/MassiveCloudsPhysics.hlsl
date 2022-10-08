#ifndef MASSIVE_CLOUDS_PHYSICS_INCLUDED
#define MASSIVE_CLOUDS_PHYSICS_INCLUDED

#define MAX_DIST 5000.0

float     _Atmosphere;

struct AtmosphereFactor
{
    float scattering;
    float shadow;
    float depth;
    float shaft;
};

AtmosphereFactor UnpackAtmosphere(float4 col)
{
    AtmosphereFactor atmosphereFactor;
    atmosphereFactor.scattering = col.r;
    atmosphereFactor.shadow     = col.g;
    atmosphereFactor.depth      = col.b;
    atmosphereFactor.shaft      = col.a;
    return atmosphereFactor;
}

float4 PackAtmosphere(AtmosphereFactor atmosphereFactor)
{
    return float4(
        atmosphereFactor.scattering,
        atmosphereFactor.shadow,
        atmosphereFactor.depth,
        atmosphereFactor.shaft);
}

float FakeMie(float3 inputDir, float3 viewDir)
{
    float d = dot(inputDir, viewDir);
    return 0.5 + 0.5 * pow(max(0, d), 4);
}

float AroundScattering(float3 inputDir, float3 viewDir)
{
    float d = dot(inputDir, viewDir);
    return (d + 1) / 2;
}

float Atmosphere()
{
    return _Atmosphere;
}

float AtmosphericScattering(ScreenSpace ss, float rayLength)
{
    float fakeMie = FakeMie(MassiveCloudsLightDirection, ss.rayDir);
    return Atmosphere() * fakeMie * saturate(rayLength / MAX_DIST);
}

float SafeLerp(float from, float to)
{
    if (to - from > 0) return lerp(from, to, 0.5);

    const float max_t = 0.9;
    const float min_t = 0.1;
    const float threshold_t = 0.3;
    float at = min(max_t, max(0.1, max_t - (from - threshold_t)));
    return lerp(from, to, at);
}
#endif