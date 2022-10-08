using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [Serializable]    
    public class SkyPass
    {
        [SerializeField] public bool Enabled = true;
        [SerializeField] public SkyParameter SkyParameter;

        public SkyPass() {}

        public SkyPass(bool enabled)
        {
            Enabled = enabled;
        }

        public void ApplyTo(Material mat)
        {
            if (Enabled) mat.EnableKeyword("_SkyEnabled");
            else mat.DisableKeyword("_SkyEnabled");
            mat.SetFloat("_SkyExposure", SkyParameter.SkyExposure);
            mat.SetColor("_GroundColor", SkyParameter.GroundColor);
            mat.SetFloat("_GroundScattering", SkyParameter.GroundScattering);
            mat.SetFloat("_SunSize", SkyParameter.SunSize);
            mat.SetFloat("_SunSizeConvergence", SkyParameter.SunSizeConvergence);
            mat.SetFloat("_Saturation", SkyParameter.Saturation);
            mat.SetFloat("_AtmosphereThickness", SkyParameter.AtmosphereThickness);
            mat.SetFloat("_Gradation", SkyParameter.Gradation);
            SkyParameter.Hdri.ApplyTo(mat);
        }

        public void ApplyTo(MaterialPropertyBlock mat)
        {
            mat.SetFloat("_SkyExposure", SkyParameter.SkyExposure);
            mat.SetColor("_GroundColor", SkyParameter.GroundColor);
            mat.SetFloat("_SunSize", SkyParameter.SunSize);
            mat.SetFloat("_SunSizeConvergence", SkyParameter.SunSizeConvergence);
            mat.SetFloat("_Saturation", SkyParameter.Saturation);
            mat.SetFloat("_AtmosphereThickness", SkyParameter.AtmosphereThickness);
            mat.SetFloat("_Gradation", SkyParameter.Gradation);
            SkyParameter.Hdri.ApplyTo(mat);
        }

        public void SetSky(SkyParameter other)
        {
            SkyParameter.Set(other);
        }
    }
}