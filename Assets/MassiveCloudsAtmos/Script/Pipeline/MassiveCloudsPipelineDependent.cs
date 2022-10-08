using UnityEngine;

namespace Mewlist.MassiveClouds
{
    public class MassiveCloudsPipelineDependent
    {
        public static void SetLightIntensity(Light light, float intensity, Color lightColor, float temperature)
        {
            light.intensity = intensity;
            light.colorTemperature = temperature;
            light.color = lightColor;
        }
    }
}