using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [Serializable]    
    public class SkyParameter
    {
        [SerializeField] public float SkyExposure = 1.0f;
        public Color GroundColor = new Color(1,1,1);
        [Range(0f, 1f), SerializeField] public float GroundScattering = 1;
        [Range(0f, 1f), SerializeField] public float SunSize = 0.1f;
        [Range(1f, 10f), SerializeField] public float SunSizeConvergence = 10f;
        [Range(-1f, 1f)]
        public float Saturation = 0f;
        [Range(0f, 2f), SerializeField] public float AtmosphereThickness = 1;
        [Range(0f, 1f), SerializeField] public float Gradation = 1;
        [SerializeField] public HdriParameter Hdri;

        public SkyParameter(float skyExposure, float groundScattering, Color groundColor, float sunSize, float sunSizeConvergence, float saturation, float atmosphereThickness, float gradation, HdriParameter hdriParameter)
        {
            SkyExposure = skyExposure;
            GroundColor = groundColor;
            GroundScattering = groundScattering;
            SunSize = sunSize;
            SunSizeConvergence = sunSizeConvergence;
            Saturation = saturation;
            AtmosphereThickness = atmosphereThickness;
            Gradation = gradation;
            Hdri = hdriParameter;
        }

        public void Set(SkyParameter other)
        {
            SkyExposure = other.SkyExposure;
            GroundColor = other.GroundColor;
            GroundScattering = other.GroundScattering;
            SunSize = other.SunSize;
            SunSizeConvergence = other.SunSizeConvergence;
            Saturation = other.Saturation;
            AtmosphereThickness = other.AtmosphereThickness;
            Gradation = other.Gradation;
            Hdri = other.Hdri;
        }

        public static SkyParameter Lerp(SkyParameter lhs, SkyParameter rhs, float weight)
        {
            return new SkyParameter(
                Mathf.Lerp(lhs.SkyExposure, rhs.SkyExposure, weight),
                Mathf.Lerp(lhs.GroundScattering, rhs.GroundScattering, weight),
                Color.Lerp(lhs.GroundColor, rhs.GroundColor, weight),
                Mathf.Lerp(lhs.SunSize, rhs.SunSize, weight),
                Mathf.Lerp(lhs.SunSizeConvergence, rhs.SunSizeConvergence, weight),
                Mathf.Lerp(lhs.Saturation, rhs.Saturation, weight),
                Mathf.Lerp(lhs.AtmosphereThickness, rhs.AtmosphereThickness, weight),
                Mathf.Lerp(lhs.Gradation, rhs.Gradation, weight),
                new MixedHdriParameter(lhs.Hdri, rhs.Hdri, weight));
        }
    }
}