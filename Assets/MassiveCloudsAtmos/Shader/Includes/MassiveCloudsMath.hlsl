#ifndef MASSIVE_CLOUDS_MATH_INCLUDED
#define MASSIVE_CLOUDS_MATH_INCLUDED

float4 mulQuaternion(float4 q1, float4 q2) {
    return float4(
        q1.w * q2.x + q1.x * q2.w + q1.z * q2.y - q1.y * q2.z,
        q1.w * q2.y + q1.y * q2.w + q1.x * q2.z - q1.z * q2.x,
        q1.w * q2.z + q1.z * q2.w + q1.y * q2.x - q1.x * q2.y,
        q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z
    );
}

float luminance(half3 col)
{
    const half3 w = half3(0.2125, 0.7154, 0.0721);
    return dot(col.rgb, w);
}

float rand(float2 st)
{
    return fmod(sin(dot(st.xy, float2(12.9898,78.233))) * 43758.5453123, 1.0);
}

float3 rotate(float4 q, float3 v) {
    float4 qv = mulQuaternion(float4(-q.x, -q.y, -q.z, q.w), float4(v, 0.0));
    return mulQuaternion(qv, q).xyz;
}

float lamp(float v, float f)
{
    float a = step(v, 0.5) * v;
    float b = step(0.5, v) * v;
    
    a = a * 2;
    b = 1 - (b - 0.5) * 2;
    
    f = 1 + f;
    a = pow(a, f);
    b = pow(b, f);

    a = a / 2;
    b = (1 - b) / 2 + 0.5;

    return a * step(a, 0.5)
        + b * step(0.5, b);
}

float _ColorSpaceIsLinear;

float fixColorSpace(float v)
{
    #if UNITY_COLORSPACE_GAMMA
        return pow(v, .454545);
    #else
        return v;
    #endif
}

#if UNITY_COLORSPACE_GAMMA
    #define fixColorSpaceToLinear(v) pow(v, .454545)
    #define fixColorSpaceToGamma(v) v
#else
    #define fixColorSpaceToLinear(v) v
    #define fixColorSpaceToGamma(v) pow(v, 2.2)
#endif

#endif