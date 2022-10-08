using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [HelpURL("http://massive-clouds-atmos.mewli.st/mca_physics_cloud_profile_ja.html")]
    [CreateAssetMenu(fileName = "MassiveCloudsPhysicsCloudProfile", menuName = "Mewlist/MassiveClouds/PhysicsCloud Profile", order = 11)]
    public class MassiveCloudsPhysicsCloudProfile : ScriptableObject
    {
        [SerializeField] public SamplerParameter Sampler;
        [SerializeField] public HorizontalShapeParameter HorizontalShape;
        [SerializeField] public LightingParameter Lighting;
    }

    public class MassiveCloudsPhysicsCloudParameter
    {
        public Texture3D VolumeTexture;
        public Vector2 Tiling;
        public float Octave;
        public float Sculpture;
        public float Sculpture2;
        public float Sculpture3;
        public float Sculpture4;
        public float Warp;
        public float Softness;
        public float NearSoftnessScale;
        public float Density;
        public float Scale;

        public float FromHeight;
        public float ToHeight;
        public float SoftnessTop;
        public float SoftnessBottom;
        public float Figure;
        public float MaxDistance;

        public float Intensity;
        public float Quality;
        public float Scattering;
        public float Shading;
        public float ShadingDistance;
        public float Transparency;
        public float Mie;

        public void Set(MassiveCloudsPhysicsCloudProfile profile)
        {
            var sampler = profile.Sampler;
            var shape = profile.HorizontalShape;
            var lighting = profile.Lighting;

            VolumeTexture = sampler.VolumeTexture;
            Tiling = sampler.Tiling;
            Octave = sampler.Octave;
            Sculpture = sampler.Sculpture;
            Sculpture2 = sampler.Sculpture2;
            Sculpture3 = sampler.Sculpture3;
            Sculpture4 = sampler.Sculpture4;
            Warp = sampler.Warp;
            Softness = sampler.Softness;
            NearSoftnessScale = sampler.NearSoftnessScale;
            Density = sampler.Density;
            Scale = sampler.Scale;
            
            FromHeight = shape.FromHeight;
            ToHeight = shape.ToHeight;
            SoftnessTop = shape.SoftnessTop;
            SoftnessBottom = shape.SoftnessBottom;
            Figure = shape.Figure;
            MaxDistance = shape.MaxDistance;

            Intensity = lighting.Intensity;
            Quality = lighting.Quality;
            Scattering = lighting.Scattering;
            Shading = lighting.Shading;
            ShadingDistance = lighting.ShadingDistance;
            Transparency = lighting.Transparency;
            Mie = lighting.Mie;
        }

        public void ApplyDensity(Material mat, float density)
        {
            mat.SetFloat("_Density", density);
        }

        public void ApplyScattering(Material mat, float scattering)
        {
            mat.SetFloat("_LightScattering", scattering);
        }

        public void ApplyShading(Material mat, float shading)
        {
            mat.SetFloat("_Shading", shading);
        }

        public void ApplyShadingDistance(Material mat, float shadingDistance)
        {
            mat.SetFloat("_ShadingDistance", shadingDistance);
        }


        public void ApplyTo(Material mat)
        {
            mat.SetTexture("_VolumeTex", VolumeTexture);
            mat.SetTextureScale("_VolumeTex", Tiling);
            mat.SetFloat("_Octave", Octave);
            mat.SetFloat("_Sculpture", Sculpture);
            mat.SetFloat("_Sculpture2", Sculpture2);
            mat.SetFloat("_Sculpture3", Sculpture3);
            mat.SetFloat("_Sculpture4", Sculpture4);
            mat.SetFloat("_Warp", Warp);
            mat.SetFloat("_Softness", Softness);
            mat.SetFloat("_NearSoftnessScale", NearSoftnessScale);
            mat.SetFloat("_Density", Density);
            mat.SetFloat("_Scale", Scale);

            mat.SetFloat("_FromHeight", FromHeight);
            mat.SetFloat("_Thickness", ToHeight - FromHeight);
            mat.SetFloat("_HorizontalSoftnessTop", SoftnessTop);
            mat.SetFloat("_HorizontalSoftnessBottom", SoftnessBottom);
            mat.SetFloat("_HorizontalSoftnessFigure", Figure);
            mat.SetFloat("_MaxDistance", MaxDistance);
            
            mat.SetFloat("_Lighting", Intensity);
            mat.SetFloat("_LightingQuality", Quality);
            mat.SetFloat("_LightScattering", Scattering);
            mat.SetFloat("_Shading", Shading);
            mat.SetFloat("_ShadingDistance", ShadingDistance);
            mat.SetFloat("_Transparency", Transparency);
            mat.SetFloat("_Mie", Mie);
        }
    }
}