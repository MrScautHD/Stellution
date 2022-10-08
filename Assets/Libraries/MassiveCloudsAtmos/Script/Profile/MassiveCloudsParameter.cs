using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public struct MassiveCloudsParameter
    {
        public enum RendererType
        {
            Authentic = 4,
            Surface = 0,
            Lucid = 2,
            Solid = 3
        }

        public Texture3D VolumeTexture;
        public Vector2 Tiling;
        [Range(1f, 32f)] public float Octave;
        [Range(0f, 1f)] public float Sculpture;
        [Range(0f, 1f)] public float Sculpture2;
        [Range(-1f, 1f)] public float Phase;

        [Range(1f, 5000f)] public float DetailDistance;

        public RendererType Renderer;

        public bool Ramp;
        public Texture2D RampTexture;
        [Range(0.1f, 1f)] public float RampScale;
        [Range(-10f, 10f)] public float RampOffset;
        [Range(0f, 1f)] public float RampStrength;

        // Texture
        [Range(0f, 1f)] public float Softness;
        [Range(0f, 1f)] public float Density;
        [Range(0f, 1f)] public float Dissolve;
        [Range(0f, 1f)] public float FarDissolve;
        [Range(0f, 1f)] public float Transparency;
        [Range(0.1f, 10f)] public float Scale;

        // Animation
        public Vector3 ScrollVelocity;

        // Lighting
        [Range(0f, 1f)] public float Lighting;
        [Range(0f, 1f)] public float DirectLight;
        [Range(0f, 1f)] public float Ambient;
        [Range(0f, 1f)] public float LightingQuality;
        [Range(0f, 1f)] public float LightSmoothness;
        [Range(0f, 1f)] public float LightScattering;
        [Range(0f, 1f)] public float Shading;

        [Range(0f, 1f)] public float EdgeLighting;
        [Range(-1f, 1f)] public float GlobalLighting;
        [Range(0f, 1f)] public float GlobalLightingRange;

        // Shadow
        public bool Shadow;
        [Range(0f, 1f)] public float ShadowSoftness;
        [Range(0f, 1f)] public float ShadowQuality;
        [Range(0f, 1f)] public float ShadowStrength;
        [Range(0f, 1f)] public float ShadowThreshold;
        public bool VolumetricShadow;
        [Range(0f, 1f)] public float VolumetricShadowDensity;
        [Range(0f, 1f)] public float VolumetricShadowStrength;

        // Finishing
        [Range(-1f, 1f)] public float Brightness;
        [Range(-1f, 1f)] public float Contrast;

        // Ray Marching
        public bool Horizontal;
        public bool RelativeHeight;
        [Range(0f, 1f)] public float HorizontalSoftnessTop;
        [Range(0f, 1f)] public float HorizontalSoftnessBottom;
        [Range(0f, 1f)] public float HorizontalSoftnessFigure;
        [Range(0f, 5000f)] public float FromHeight;
        [Range(0f, 10000f)] public float ToHeight;
        [Range(0f, 10000f)] public float Thickness;
        [Range(0f, 60000f)] public float FromDistance;
        [Range(0f, 60000f)] public float MaxDistance;
        [Range(1f, 400f)] public float Iteration;
        [Range(0.01f, 10f)] public float Fade;
        [Range(0f, 1f)] public float Optimize;

        public MassiveCloudsParameter(MassiveCloudsParameter other)
        {
            VolumeTexture = other.VolumeTexture;
            Tiling = other.Tiling;
            Octave = other.Octave;
            Sculpture = other.Sculpture;
            Sculpture2 = other.Sculpture2;
            Phase = other.Phase;
            DetailDistance = other.DetailDistance;

            Renderer = other.Renderer;

            Ramp = other.Ramp;
            RampTexture = other.RampTexture;
            RampScale = other.RampScale;
            RampOffset = other.RampOffset;
            RampStrength = other.RampStrength;

            Softness = other.Softness;
            Density = other.Density;
            Dissolve = other.Dissolve;
            FarDissolve = other.FarDissolve;
            Transparency = other.Transparency;
            Scale = other.Scale;

            ScrollVelocity = other.ScrollVelocity;

            Lighting = other.Lighting;
            DirectLight = other.DirectLight;
            Ambient = other.Ambient;
            LightingQuality = other.LightingQuality;

            LightSmoothness = other.LightSmoothness;
            LightScattering = other.LightScattering;
            Shading = other.Shading;
            EdgeLighting = other.EdgeLighting;
            GlobalLighting = other.GlobalLighting;
            GlobalLightingRange = other.GlobalLightingRange;

            Shadow = other.Shadow;
            ShadowSoftness = other.ShadowSoftness;
            ShadowQuality = other.ShadowQuality;
            ShadowStrength = other.ShadowStrength;
            ShadowThreshold = other.ShadowThreshold;
            VolumetricShadow = other.VolumetricShadow;
            VolumetricShadowDensity = other.VolumetricShadowDensity;
            VolumetricShadowStrength = other.VolumetricShadowStrength;

            Brightness = other.Brightness;
            Contrast = other.Contrast;

            RelativeHeight = other.RelativeHeight;
            FromHeight = other.FromHeight;
            ToHeight = other.ToHeight;
            FromDistance = other.FromDistance;
            Horizontal = other.Horizontal;
            MaxDistance = other.MaxDistance;
            Thickness = other.Thickness;
            Iteration = other.Iteration;
            HorizontalSoftnessTop = other.HorizontalSoftnessTop;
            HorizontalSoftnessBottom = other.HorizontalSoftnessBottom;
            HorizontalSoftnessFigure = other.HorizontalSoftnessFigure;
            Optimize = other.Optimize;

            Fade = other.Fade;
        }

        public bool Equals(MassiveCloudsParameter other)
        {
            return VolumeTexture == other.VolumeTexture &&
                   Tiling == other.Tiling &&
                   Octave == other.Octave &&
                   Sculpture == other.Sculpture &&
                   Sculpture2 == other.Sculpture2 &&
                   Phase == other.Phase &&
                   DetailDistance == other.DetailDistance &&
                   Renderer == other.Renderer &&
                   Ramp == other.Ramp &&
                   RampTexture == other.RampTexture &&
                   RampScale == other.RampScale &&
                   RampOffset == other.RampOffset &&
                   RampStrength == other.RampStrength &&
                   Softness == other.Softness &&
                   Density == other.Density &&
                   Dissolve == other.Dissolve &&
                   FarDissolve == other.FarDissolve &&
                   Transparency == other.Transparency &&
                   Scale == other.Scale &&
                   ScrollVelocity == other.ScrollVelocity &&
                   Lighting == other.Lighting &&
                   DirectLight == other.DirectLight &&
                   Ambient == other.Ambient &&
                   LightingQuality == other.LightingQuality &&
                   LightSmoothness == other.LightSmoothness &&
                   LightScattering == other.LightScattering &&
                   Shading == other.Shading &&
                   EdgeLighting == other.EdgeLighting &&
                   GlobalLighting == other.GlobalLighting &&
                   GlobalLightingRange == other.GlobalLightingRange &&
                   Shadow == other.Shadow &&
                   ShadowSoftness == other.ShadowSoftness &&
                   ShadowQuality == other.ShadowQuality &&
                   ShadowStrength == other.ShadowStrength &&
                   ShadowThreshold == other.ShadowThreshold &&
                   VolumetricShadow == other.VolumetricShadow &&
                   VolumetricShadowDensity == other.VolumetricShadowDensity &&
                   VolumetricShadowStrength == other.VolumetricShadowStrength &&
                   Brightness == other.Brightness &&
                   Contrast == other.Contrast &&
                   RelativeHeight == other.RelativeHeight &&
                   FromHeight == other.FromHeight &&
                   ToHeight == other.ToHeight &&
                   FromDistance == other.FromDistance &&
                   Horizontal == other.Horizontal &&
                   MaxDistance == other.MaxDistance &&
                   Thickness == other.Thickness &&
                   Iteration == other.Iteration &&
                   HorizontalSoftnessTop == other.HorizontalSoftnessTop &&
                   HorizontalSoftnessBottom == other.HorizontalSoftnessBottom &&
                   HorizontalSoftnessFigure == other.HorizontalSoftnessFigure &&
                   Optimize == other.Optimize &&
                   Fade == other.Fade;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is MassiveCloudsParameter && Equals((MassiveCloudsParameter) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (VolumeTexture != null ? VolumeTexture.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Tiling.GetHashCode();
                hashCode = (hashCode * 397) ^ Octave.GetHashCode();
                hashCode = (hashCode * 397) ^ Sculpture.GetHashCode();
                hashCode = (hashCode * 397) ^ Sculpture2.GetHashCode();
                hashCode = (hashCode * 397) ^ Phase.GetHashCode();
                hashCode = (hashCode * 397) ^ DetailDistance.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) Renderer;
                hashCode = (hashCode * 397) ^ Ramp.GetHashCode();
                hashCode = (hashCode * 397) ^ (RampTexture != null ? RampTexture.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ RampScale.GetHashCode();
                hashCode = (hashCode * 397) ^ RampOffset.GetHashCode();
                hashCode = (hashCode * 397) ^ RampStrength.GetHashCode();
                hashCode = (hashCode * 397) ^ Softness.GetHashCode();
                hashCode = (hashCode * 397) ^ Density.GetHashCode();
                hashCode = (hashCode * 397) ^ Dissolve.GetHashCode();
                hashCode = (hashCode * 397) ^ FarDissolve.GetHashCode();
                hashCode = (hashCode * 397) ^ Transparency.GetHashCode();
                hashCode = (hashCode * 397) ^ Scale.GetHashCode();
                hashCode = (hashCode * 397) ^ ScrollVelocity.GetHashCode();
                hashCode = (hashCode * 397) ^ Lighting.GetHashCode();
                hashCode = (hashCode * 397) ^ DirectLight.GetHashCode();
                hashCode = (hashCode * 397) ^ Ambient.GetHashCode();
                hashCode = (hashCode * 397) ^ LightingQuality.GetHashCode();
                hashCode = (hashCode * 397) ^ LightSmoothness.GetHashCode();
                hashCode = (hashCode * 397) ^ LightScattering.GetHashCode();
                hashCode = (hashCode * 397) ^ Shading.GetHashCode();
                hashCode = (hashCode * 397) ^ EdgeLighting.GetHashCode();
                hashCode = (hashCode * 397) ^ GlobalLighting.GetHashCode();
                hashCode = (hashCode * 397) ^ GlobalLightingRange.GetHashCode();
                hashCode = (hashCode * 397) ^ Shadow.GetHashCode();
                hashCode = (hashCode * 397) ^ ShadowSoftness.GetHashCode();
                hashCode = (hashCode * 397) ^ ShadowQuality.GetHashCode();
                hashCode = (hashCode * 397) ^ ShadowStrength.GetHashCode();
                hashCode = (hashCode * 397) ^ ShadowThreshold.GetHashCode();
                hashCode = (hashCode * 397) ^ VolumetricShadow.GetHashCode();
                hashCode = (hashCode * 397) ^ VolumetricShadowDensity.GetHashCode();
                hashCode = (hashCode * 397) ^ VolumetricShadowStrength.GetHashCode();
                hashCode = (hashCode * 397) ^ Brightness.GetHashCode();
                hashCode = (hashCode * 397) ^ Contrast.GetHashCode();
                hashCode = (hashCode * 397) ^ Horizontal.GetHashCode();
                hashCode = (hashCode * 397) ^ RelativeHeight.GetHashCode();
                hashCode = (hashCode * 397) ^ HorizontalSoftnessTop.GetHashCode();
                hashCode = (hashCode * 397) ^ HorizontalSoftnessBottom.GetHashCode();
                hashCode = (hashCode * 397) ^ HorizontalSoftnessFigure.GetHashCode();
                hashCode = (hashCode * 397) ^ FromHeight.GetHashCode();
                hashCode = (hashCode * 397) ^ ToHeight.GetHashCode();
                hashCode = (hashCode * 397) ^ Thickness.GetHashCode();
                hashCode = (hashCode * 397) ^ FromDistance.GetHashCode();
                hashCode = (hashCode * 397) ^ MaxDistance.GetHashCode();
                hashCode = (hashCode * 397) ^ Iteration.GetHashCode();
                hashCode = (hashCode * 397) ^ Fade.GetHashCode();
                hashCode = (hashCode * 397) ^ Optimize.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(MassiveCloudsParameter lhs, MassiveCloudsParameter rhs)
        {
            return lhs.Equals(rhs);
        }

        public static bool operator !=(MassiveCloudsParameter lhs, MassiveCloudsParameter rhs)
        {
            return !(lhs == rhs);
        }

        public MassiveCloudsParameter Lerp(MassiveCloudsParameter other, float t)
        {
            t = Mathf.SmoothStep(0, 1, t);
            return new MassiveCloudsParameter()
            {
                VolumeTexture = VolumeTexture,
                Tiling = Vector2.Lerp(other.Tiling, Tiling, t),
                Octave = Mathf.Lerp(other.Octave, Octave, t),
                Sculpture = Mathf.Lerp(other.Sculpture, Sculpture, t),
                Sculpture2 = Mathf.Lerp(other.Sculpture2, Sculpture2, t),
                Phase = Mathf.Lerp(other.Phase, Phase, t),
                DetailDistance = Mathf.Lerp(other.DetailDistance, DetailDistance, t),

                Renderer = Renderer,

                Ramp = Ramp,
                RampTexture = RampTexture,
                RampScale = Mathf.Lerp(other.RampScale, RampScale, t),
                RampOffset = Mathf.Lerp(other.RampOffset, RampOffset, t),
                RampStrength = Mathf.Lerp(other.RampStrength, RampStrength, t),

                Softness = Mathf.Lerp(other.Softness, Softness, t),
                Density = Mathf.Lerp(other.Density, Density, t),
                Dissolve = Mathf.Lerp(other.Dissolve, Dissolve, t),
                FarDissolve = Mathf.Lerp(other.FarDissolve, FarDissolve, t),
                Transparency = Mathf.Lerp(other.Transparency, Transparency, t),
                Scale = Mathf.Lerp(other.Scale, Scale, t),

                ScrollVelocity = Vector3.Lerp(other.ScrollVelocity, ScrollVelocity, t),

                Lighting = Mathf.Lerp(other.Lighting, Lighting, t),
                DirectLight = Mathf.Lerp(other.DirectLight, DirectLight, t),
                Ambient = Mathf.Lerp(other.Ambient, Ambient, t),
                LightingQuality = Mathf.Lerp(other.LightingQuality, LightingQuality, t),

                LightSmoothness = Mathf.Lerp(other.LightSmoothness, LightSmoothness, t),
                LightScattering = Mathf.Lerp(other.LightScattering, LightScattering, t),
                Shading = Mathf.Lerp(other.Shading, Shading, t),
                EdgeLighting = Mathf.Lerp(other.EdgeLighting, EdgeLighting, t),
                GlobalLighting = Mathf.Lerp(other.GlobalLighting, GlobalLighting, t),
                GlobalLightingRange = Mathf.Lerp(other.GlobalLightingRange, GlobalLightingRange, t),

                Shadow = Shadow,
                ShadowSoftness = Mathf.Lerp(other.ShadowSoftness, ShadowSoftness, t),
                ShadowQuality = Mathf.Lerp(other.ShadowQuality, ShadowQuality, t),
                ShadowStrength = Mathf.Lerp(other.ShadowStrength, ShadowStrength, t),
                ShadowThreshold = Mathf.Lerp(other.ShadowThreshold, ShadowThreshold, t),
                VolumetricShadow = VolumetricShadow,
                VolumetricShadowDensity = Mathf.Lerp(other.VolumetricShadowDensity, VolumetricShadowDensity, t),
                VolumetricShadowStrength = Mathf.Lerp(other.VolumetricShadowStrength, VolumetricShadowStrength, t),

                Brightness = Mathf.Lerp(other.Brightness, Brightness, t),
                Contrast = Mathf.Lerp(other.Contrast, Contrast, t),

                RelativeHeight = RelativeHeight,
                FromHeight = Mathf.Lerp(other.FromHeight, FromHeight, t),
                ToHeight = Mathf.Lerp(other.ToHeight, ToHeight, t),
                FromDistance = Mathf.Lerp(other.FromDistance, FromDistance, t),
                Horizontal = Horizontal,
                MaxDistance = Mathf.Lerp(other.MaxDistance, MaxDistance, t),
                Thickness = Mathf.Lerp(other.Thickness, Thickness, t),
                Iteration = Mathf.Lerp(other.Iteration, Iteration, t),
                HorizontalSoftnessTop = Mathf.Lerp(other.HorizontalSoftnessTop, HorizontalSoftnessTop, t),
                HorizontalSoftnessBottom = Mathf.Lerp(other.HorizontalSoftnessBottom, HorizontalSoftnessBottom, t),
                HorizontalSoftnessFigure = Mathf.Lerp(other.HorizontalSoftnessFigure, HorizontalSoftnessFigure, t),
                Optimize = Mathf.Lerp(other.Optimize, Optimize, t),

                Fade = Mathf.Lerp(other.Fade, Fade, t),
            };
        }
    }
}