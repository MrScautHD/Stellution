using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public struct EnvironmentKey
    {
        [SerializeField] public Color Fog;
        [SerializeField] public Color Shadow;
        [SerializeField] public Color AmbientSky;
        [SerializeField] public Color AmbientEquator;
        [SerializeField] public Color AmbientGround;
        [SerializeField] public Color Light;
        [SerializeField] public float AmbientMultiplier;
        [SerializeField] public float SunIntensity;
        [SerializeField] public float CloudSunIntensity;
        [SerializeField] public float MoonIntensity;
        [SerializeField] public float CloudMoonIntensity;
        [SerializeField] public float HeightFog;

        public static EnvironmentKey Lerp(EnvironmentKey l, EnvironmentKey r, float t)
        {
            return new EnvironmentKey()
            {
                Fog = Color.Lerp(l.Fog, r.Fog, t),
                Shadow = Color.Lerp(l.Shadow, r.Shadow, t),
                AmbientSky = Color.Lerp(l.AmbientSky, r.AmbientSky, t),
                AmbientEquator = Color.Lerp(l.AmbientEquator, r.AmbientEquator, t),
                AmbientGround = Color.Lerp(l.AmbientGround, r.AmbientGround, t),
                AmbientMultiplier = Mathf.Lerp(l.AmbientMultiplier, r.AmbientMultiplier, t),
                Light = Color.Lerp(l.Light, r.Light, t),
                SunIntensity = Mathf.Lerp(l.SunIntensity, r.SunIntensity, t),
                CloudSunIntensity = Mathf.Lerp(l.CloudSunIntensity, r.CloudSunIntensity, t),
                MoonIntensity = Mathf.Lerp(l.MoonIntensity, r.MoonIntensity, t),
                CloudMoonIntensity = Mathf.Lerp(l.CloudMoonIntensity, r.CloudMoonIntensity, t),
                HeightFog = Mathf.Lerp(l.HeightFog, r.HeightFog, t),
            };
        }
    }
}