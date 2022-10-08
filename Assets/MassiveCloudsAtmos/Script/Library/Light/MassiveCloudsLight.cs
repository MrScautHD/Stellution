using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public abstract class MassiveCloudsLight
    {
        public enum LightSourceMode
        {
            Auto,
            Manual,
        }

        [SerializeField] private LightSourceMode mode = LightSourceMode.Auto;
        [SerializeField] private Vector3 rotation = Vector3.zero;
        [SerializeField] protected float intensity = 2f;
        [SerializeField] protected Color color = Color.white;

        public Vector3 LightDirection
        {
            get { return Quaternion.Euler(Rotation) * Vector3.forward; }
        }

        public LightSourceMode Mode { get { return mode; } }
        public Vector3 Rotation { get { return rotation; } }
        public float Intensity { get { return intensity; } }
        public Color Color { get { return color; } }

        public void Synchronize(Light light)
        {
            if (!light) return;
            rotation = light.transform.rotation.eulerAngles;
            intensity = light.intensity;
#if UNITY_2019_3_OR_NEWER
            if (light.useColorTemperature)
                color = light.color * Mathf.CorrelatedColorTemperatureToRGB(light.colorTemperature);
            else
                color = light.color;
#else
            if (GraphicsSettings.lightsUseColorTemperature)
                color = light.color * Mathf.CorrelatedColorTemperatureToRGB(light.colorTemperature);
            else
                color = light.color;
#endif
        }

        public void Synchronize(Transform light)
        {
            if (!light) return;
            rotation = light.rotation.eulerAngles;
        }

        public void ApplySunParameters(Material material, float scale)
        {
            material.SetVector("_MassiveCloudsSunLightDirection", LightDirection);
            material.SetVector("_MassiveCloudsSunLightColor", Color * Intensity * scale);
            material.SetFloat("_MassiveCloudsSunLightIntensity", Intensity);
        }

        public void ApplyMoonParameters(Material material, float scale)
        {
            material.SetVector("_MassiveCloudsMoonLightDirection", LightDirection);
            material.SetVector("_MassiveCloudsMoonLightColor", Color * Intensity * scale);
            material.SetFloat("_MassiveCloudsMoonLightIntensity", Intensity);
        }
    }
}