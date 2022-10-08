using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Mewlist.MassiveClouds
{
    public class MassiveCloudsMaterial : IDisposable
    {
        public readonly Material CloudMaterial;
        public readonly Material ShadowMaterial;
        public readonly Material MixMaterial;
        public readonly Material VolumetricShadowMaterial;
        public readonly Material VolumetricShadowMixMaterial;

        private MassiveCloudsStylizedCloudProfile StylizedCloudProfile { get; set; }

        public MassiveCloudsMaterial()
        {
            CloudMaterial = new Material(Shader.Find("MassiveClouds"));
            ShadowMaterial =  new Material(Shader.Find("MassiveCloudsScreenSpaceShadow"));
            VolumetricShadowMaterial = new Material(Shader.Find("MassiveCloudsVolumetricShadow"));
            MixMaterial = new Material(Shader.Find("MassiveCloudsMix"));
            VolumetricShadowMixMaterial = new Material(Shader.Find("MassiveCloudsVolumetricShadowMix"));
        }

        public void SetProfile(MassiveCloudsStylizedCloudProfile stylizedCloudProfile)
        {
            StylizedCloudProfile = stylizedCloudProfile;
            if (StylizedCloudProfile == null) return;

            stylizedCloudProfile.SetMaterial(CloudMaterial, this.StylizedCloudProfile.Parameter);
            CloudMaterial.DisableKeyword("_SHADOW_ON");
            CloudMaterial.DisableKeyword("_HEIGHTFOG_ON");
            stylizedCloudProfile.SetMaterial(ShadowMaterial, this.StylizedCloudProfile.Parameter);
            stylizedCloudProfile.SetMaterial(VolumetricShadowMaterial, this.StylizedCloudProfile.Parameter);
            stylizedCloudProfile.SetMaterial(MixMaterial, this.StylizedCloudProfile.Parameter);
            stylizedCloudProfile.SetMaterial(VolumetricShadowMixMaterial, this.StylizedCloudProfile.Parameter);
        }

        public void SetLight(MassiveCloudsLight sun, MassiveCloudsLight moon, float scale)
        {
            sun.ApplySunParameters(CloudMaterial, scale);
            sun.ApplySunParameters(ShadowMaterial, scale);
            sun.ApplySunParameters(VolumetricShadowMaterial, scale);
            moon.ApplyMoonParameters(CloudMaterial, scale);
            moon.ApplyMoonParameters(ShadowMaterial, scale);
            moon.ApplyMoonParameters(VolumetricShadowMaterial, scale);
        }

        public void SetScrollOffset(Vector3 offset)
        {
            CloudMaterial.SetVector("_ScrollOffset", offset);
            ShadowMaterial.SetVector("_ScrollOffset", offset);
            VolumetricShadowMaterial.SetVector("_ScrollOffset", offset);
        }

        public void SetBaseColor(Color color)
        {
            CloudMaterial.SetColor("_BaseColor", color);
        }

        public void SetFade(float v)
        {
            if (StylizedCloudProfile == null) return;
            CloudMaterial.SetFloat("_Density", StylizedCloudProfile.Parameter.Density * v);
            ShadowMaterial.SetFloat("_Density", StylizedCloudProfile.Parameter.Density * v);
            VolumetricShadowMaterial.SetFloat("_Density", StylizedCloudProfile.Parameter.Density * v);
        }

        public void SetShaodwColor(Color color)
        {
            if (StylizedCloudProfile == null) return;
            ShadowMaterial.SetColor("_ShadowColor", color);
            VolumetricShadowMixMaterial.SetColor("_ShadowColor", color);
        }

        public void SetParameter(MassiveCloudsParameter parameter)
        {
            if (StylizedCloudProfile == null) return;
            StylizedCloudProfile.SetMaterial(CloudMaterial, parameter);
            StylizedCloudProfile.SetMaterial(ShadowMaterial, parameter);
            StylizedCloudProfile.SetMaterial(VolumetricShadowMaterial, parameter);
            StylizedCloudProfile.SetMaterial(MixMaterial, parameter);
            StylizedCloudProfile.SetMaterial(VolumetricShadowMaterial, parameter);
            StylizedCloudProfile.SetMaterial(VolumetricShadowMixMaterial, parameter);
        }

        public void SetAmbientColor(MassiveCloudsAmbient ambient)
        {
            ambient.ApplyShaderParameters(CloudMaterial);
            ambient.ApplyShaderParameters(VolumetricShadowMixMaterial);
        }

        public void Dispose()
        {
            SafeDestroy(CloudMaterial);
            SafeDestroy(ShadowMaterial);
            SafeDestroy(MixMaterial);
            SafeDestroy(VolumetricShadowMaterial);
            SafeDestroy(VolumetricShadowMixMaterial);
            StylizedCloudProfile = null;
        }

        private void SafeDestroy(Object obj)
        {
            if (Application.isPlaying)
                Object.Destroy(obj);
            else
                Object.DestroyImmediate(obj);
        }
    }
}