using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [HelpURL("http://massive-clouds-atmos.mewli.st/mca_stylized_cloud_profile_ja.html")]
    [CreateAssetMenu(fileName = "MassiveCloudsStylizedCloudProfile",
        menuName = "Mewlist/MassiveClouds/StylizedCloud Profile", order = 31)]
    public class MassiveCloudsStylizedCloudProfile : ScriptableObject
    {
        [SerializeField] public MassiveCloudsParameter Parameter;

        public void UpdateParameter(MassiveCloudsParameter other)
        {
            Parameter = new MassiveCloudsParameter(other);
        }

        public MassiveCloudsStylizedCloudProfile()
        {
            Parameter = new MassiveCloudsParameter();
        }

        public Material CreateMaterial()
        {
            var mat = new Material(Shader.Find("MassiveClouds"));
            SetMaterial(mat, Parameter);
            return mat;
        }

        public Material CreateShadowMaterial()
        {
            var mat = new Material(Shader.Find("MassiveCloudsScreenSpaceShadow"));
            SetMaterial(mat, Parameter);
            return mat;
        }

        public Material CreateHeightFogMaterial()
        {
            var mat = new Material(Shader.Find("MassiveCloudsHeightFog"));
            SetMaterial(mat, Parameter);
            return mat;
        }

        public Material CreateMaterialForExport()
        {
            var mat = new Material(Shader.Find("MassiveCloudsMaterial"));
            SetMaterial(mat, Parameter);
            return mat;
        }

        public MassiveCloudsParameter Lerp(MassiveCloudsStylizedCloudProfile from, float t)
        {
            if (from == null) return Parameter;
            return Parameter.Lerp(from.Parameter, t);
        }

        public void SetMaterial(Material mat, MassiveCloudsParameter param)
        {
            mat.SetTexture("_VolumeTex", param.VolumeTexture);
            mat.SetTextureScale("_VolumeTex", param.Tiling);

            mat.SetFloat("_Octave", param.Octave);
            mat.SetFloat("_Sculpture", param.Sculpture);
            mat.SetFloat("_Sculpture2", param.Sculpture2);
            mat.SetFloat("_Phase", param.Phase);
            mat.SetFloat("_DetailDistance", param.DetailDistance);

            mat.SetFloat("_RENDERER", ((float) param.Renderer) / 3f);
            switch (param.Renderer)
            {
                case MassiveCloudsParameter.RendererType.Authentic:
                    mat.EnableKeyword("_RENDERER_AUTHENTIC");
                    mat.DisableKeyword("_RENDERER_SURFACE");
                    mat.DisableKeyword("_RENDERER_LUCID");
                    mat.DisableKeyword("_RENDERER_SOLID");
                    break;
                case MassiveCloudsParameter.RendererType.Lucid:
                    mat.DisableKeyword("_RENDERER_AUTHENTIC");
                    mat.DisableKeyword("_RENDERER_SURFACE");
                    mat.EnableKeyword("_RENDERER_LUCID");
                    mat.DisableKeyword("_RENDERER_SOLID");
                    break;
                case MassiveCloudsParameter.RendererType.Solid:
                    mat.DisableKeyword("_RENDERER_AUTHENTIC");
                    mat.DisableKeyword("_RENDERER_SURFACE");
                    mat.DisableKeyword("_RENDERER_LUCID");
                    mat.EnableKeyword("_RENDERER_SOLID");
                    break;
                case MassiveCloudsParameter.RendererType.Surface:
                    mat.DisableKeyword("_RENDERER_AUTHENTIC");
                    mat.EnableKeyword("_RENDERER_SURFACE");
                    mat.DisableKeyword("_RENDERER_LUCID");
                    mat.DisableKeyword("_RENDERER_SOLID");
                    break;
            }

            if (param.Ramp) mat.EnableKeyword("_RAMP_ON");
            else mat.DisableKeyword("_RAMP_ON");
            mat.SetFloat("_RAMP", param.Ramp ? 1f : 0f);
            mat.SetTexture("_RampTex", param.RampTexture);
            mat.SetFloat("_RampScale", param.RampScale);
            mat.SetFloat("_RampOffset", param.RampOffset);
            mat.SetFloat("_RampStrength", param.RampStrength);

            mat.SetFloat("_Softness", param.Softness);
            mat.SetFloat("_Density", param.Density);
            mat.SetFloat("_Dissolve", param.Dissolve);
            mat.SetFloat("_FarDissolve", param.FarDissolve);
            mat.SetFloat("_Transparency", param.Transparency);
            mat.SetFloat("_Scale", param.Scale);

            mat.SetVector("_ScrollVelocity", param.ScrollVelocity);

            mat.SetFloat("_Lighting", param.Lighting);
            mat.SetFloat("_DirectLight", param.DirectLight);
            mat.SetFloat("_Ambient", param.Ambient);
            mat.SetFloat("_LightingQuality", param.LightingQuality);
            mat.SetFloat("_LightSmoothness", param.LightSmoothness);
            mat.SetFloat("_LightScattering", param.LightScattering);
            mat.SetFloat("_Shading", param.Shading);
            mat.SetFloat("_EdgeLighting", param.EdgeLighting);
            mat.SetFloat("_GlobalLighting", param.GlobalLighting);
            mat.SetFloat("_GlobalLightingRange", param.GlobalLightingRange);

            if (param.Shadow) mat.EnableKeyword("_SHADOW_ON");
            else mat.DisableKeyword("_SHADOW_ON");
            mat.SetFloat("_SHADOW", param.Shadow ? 1f : 0f);
            mat.SetFloat("_ShadowSoftness", param.ShadowSoftness);
            mat.SetFloat("_ShadowQuality", param.ShadowQuality);
            mat.SetFloat("_ShadowStrength", param.ShadowStrength);
            mat.SetFloat("_ShadowThreshold", param.ShadowThreshold);
            mat.SetFloat("_VolumetricShadow", param.VolumetricShadow ? 1f : 0f);
            mat.SetFloat("_VolumetricShadowDensity", param.VolumetricShadowDensity);
            mat.SetFloat("_VolumetricShadowStrength", param.VolumetricShadowStrength);

            mat.SetFloat("_Brightness", param.Brightness);
            mat.SetFloat("_Contrast", param.Contrast);

            mat.SetFloat("_RelativeHeight", param.RelativeHeight ? 1f : 0f);
            if (param.Horizontal) mat.EnableKeyword("_HORIZONTAL_ON");
            else mat.DisableKeyword("_HORIZONTAL_ON");
            mat.SetFloat("_HORIZONTAL", param.Horizontal ? 1f : 0f);

            mat.SetFloat("_FromHeight", param.FromHeight);
            mat.SetFloat("_FromDistance", param.FromDistance);
            mat.SetFloat("_MaxDistance", param.MaxDistance);
            if (param.Horizontal)
                mat.SetFloat("_Thickness", param.ToHeight - param.FromHeight);
            else
                mat.SetFloat("_Thickness", param.Thickness);
            mat.SetFloat("_Iteration", param.Iteration);
            mat.SetFloat("_HorizontalSoftnessTop", param.HorizontalSoftnessTop);
            mat.SetFloat("_HorizontalSoftnessBottom", param.HorizontalSoftnessBottom);
            mat.SetFloat("_HorizontalSoftnessFigure", param.HorizontalSoftnessFigure);
            mat.SetFloat("_Optimize", param.Optimize);

            mat.SetFloat("_Fade", param.Fade);

            // ColorSpace
            mat.SetFloat("_IsLinear", QualitySettings.activeColorSpace == ColorSpace.Linear ? 1f : 0f);
        }
    }
}