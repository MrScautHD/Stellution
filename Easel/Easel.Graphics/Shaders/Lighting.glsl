#ifndef LIGHTING_GLSL
#define LIGHTING_GLSL

#include "Easel.Graphics.Shaders.Types.glsl"

const float PI = 3.141592653589793;

// TODO: Replace this with proper normal mapping.
vec3 TempNormal(sampler2D normalTex, vec3 fragPosition, vec2 texCoords, vec3 normal)
{
    vec3 tangentNormal = texture(normalTex, texCoords).rgb * 2.0 - 1.0;
    
    vec3 Q1 = dFdx(fragPosition);
    vec3 Q2 = dFdy(fragPosition);
    vec2 st1 = dFdx(texCoords);
    vec2 st2 = dFdy(texCoords);

    vec3 N = normalize(normal);
    vec3 T = normalize(Q1 * st2.t - Q2 * st1.t);
    vec3 B = -normalize(cross(N, T));
    mat3 TBN = mat3(T, B, N);

    return normalize(TBN * tangentNormal);
}

vec3 FresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}

float DistributionGGX(vec3 N, vec3 H, float roughness)
{
    float a = roughness * roughness;
    float a2 = a * a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;

    float num = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return num / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r * r) / 8.0;

    float num = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return num / denom;
}

float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);

    return ggx1 * ggx2;
}

vec3 ProcessLight(vec3 albedo, vec3 normal, float metallic, float roughness, vec3 L, vec3 N, vec3 V, vec3 radiance)
{
    vec3 H = normalize(V + L);

    // 0.04 looks correct for dialetric surfaces
    vec3 F0 = vec3(0.04);
    F0 = mix(F0, albedo, metallic);
    vec3 F = FresnelSchlick(max(dot(H, V), 0.0), F0);

    float NDF = DistributionGGX(N, H, roughness);
    float G = GeometrySmith(N, V, L, roughness);

    vec3 numerator = NDF * G * F;
    float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.0001;
    vec3 specular = numerator / denominator;

    vec3 kS = F;
    vec3 kD = vec3(1.0) - kS;
    kD *= 1.0 - metallic;

    float NdotL = max(dot(N, L), 0.0);
    return (kD * albedo.rgb / PI + specular) * radiance * NdotL;
}

vec3 ProcessDirLight(DirectionalLight dirLight, vec3 viewDir, vec3 albedo, vec3 normal, float metallic, float roughness)
{
    // Do I even need this?
    vec3 N = normalize(normal);
    
    //vec3 L = normalize(lightPos - in_data.fragPosition);
    vec3 L = normalize(-dirLight.direction.xyz);
    //float distance = length(lightPos - in_data.fragPosition);
    //float attenuation = 1.0 / (distance * distance);
    //vec3 radiance = uSun.color.rgb * attenuation;
    vec3 radiance = dirLight.color.rgb;
    
    return ProcessLight(albedo, normal, metallic, roughness, L, N, viewDir, radiance);
}

#endif