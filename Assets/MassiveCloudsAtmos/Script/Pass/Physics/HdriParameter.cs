using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public class HdriParameter
    {
        [SerializeField] protected Cubemap texture = null;
        [SerializeField] protected float exposure = 0f;
        [Range(0f, 1f), SerializeField] protected float mix = 0f;

        public Cubemap Texture { get { return texture; } }
        public float Exposure { get { return exposure; } }

        public float Intensity
        {
            get
            {
                var ev100 = -Exposure;
                var maxLuminance = 1.2f * Mathf.Pow(2f, ev100);
                return 1f / maxLuminance;
            }
        }
        public float Mix { get { return Texture == null ? 0 : mix; } }

        public HdriParameter(Cubemap hdriCubemap, float exposure, float mix)
        {
            this.texture = hdriCubemap;
            this.exposure = exposure;
            this.mix = mix;
        }

        public HdriParameter(HdriParameter other)
        {
            texture = other.Texture;
            exposure = other.Exposure;
            mix = other.Mix;
        }

        public virtual void ApplyTo(Material mat)
        {
            if (Texture) mat.SetTexture("_MassiveCloudsHdri", Texture);
            mat.SetFloat("_MassiveCloudsHdriIntensity", Intensity);
            mat.SetFloat("_MassiveCloudsHdriMix", Mix);
            mat.SetFloat("_MassiveCloudsSecondaryWeight", 0);
        }

        public virtual void ApplyTo(MaterialPropertyBlock mat)
        {
            if (Texture) mat.SetTexture("_MassiveCloudsHdri", Texture);
            mat.SetFloat("_MassiveCloudsHdriIntensity", Intensity);
            mat.SetFloat("_MassiveCloudsHdriMix", Mix);
            mat.SetFloat("_MassiveCloudsSecondaryWeight", 0);
        }

    }
}