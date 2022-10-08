using UnityEngine;

namespace Mewlist.MassiveClouds
{
    public class MixedHdriParameter : HdriParameter
    {
        private HdriParameter secondaryHdri = null;
        private float weight = 0f;

        public float Weight { get { return weight; } }

        public MixedHdriParameter(HdriParameter lhs, HdriParameter rhs, float weight) : base(lhs)
        {
            this.secondaryHdri = rhs;
            this.weight = weight;
        }

        public override void ApplyTo(Material mat)
        {
            base.ApplyTo(mat);
            if (secondaryHdri.Texture) mat.SetTexture("_MassiveCloudsSecondaryHdri", secondaryHdri.Texture);
            mat.SetFloat("_MassiveCloudsSecondaryIntensity", secondaryHdri.Intensity);
            mat.SetFloat("_MassiveCloudsSecondaryMix", secondaryHdri.Mix);
            mat.SetFloat("_MassiveCloudsSecondaryWeight", weight);
        }

        public override void ApplyTo(MaterialPropertyBlock mat)
        {
            base.ApplyTo(mat);
            if (secondaryHdri.Texture) mat.SetTexture("_MassiveCloudsSecondaryHdri", secondaryHdri.Texture);
            mat.SetFloat("_MassiveCloudsSecondaryIntensity", secondaryHdri.Intensity);
            mat.SetFloat("_MassiveCloudsSecondaryMix", secondaryHdri.Mix);
            mat.SetFloat("_MassiveCloudsSecondaryWeight", weight);
        }
    }
}