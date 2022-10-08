using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public class AtmosphereParameter
    {
        [Range(0f, 1f)] 
        [SerializeField] private float atmosphere;
        [Range(0f, 1f)]
        [SerializeField] private float atmosphereColoring = 0f;
        [ColorUsage(false, true)]
        [SerializeField] private Color atmosphereColor;
        [Range(0f, 1f)]
        [SerializeField] private float atmosphereHighLightColoring = 0f;
        [ColorUsage(false, true)]
        [SerializeField] private Color atmosphereHighLightColor;
        [Range(0f, 1f)] 
        [SerializeField] private float cloudOcclusion;
        [Range(0f, 1f)] 
        [SerializeField] private float cloudAtmospheric = 1f;
        [Range(0f, 1f)] 
        [SerializeField] private float godRay;
        [Range(100f, 1000f)] 
        [SerializeField] private float godRayStartDistance = 1000f;
        [Range(0f, 1f)] 
        [SerializeField] private float shadow;
        [Range(0f, 1f)] 
        [SerializeField] private float sunShaft;

        public float Atmosphere { get { return atmosphere; } }
        public float AtmosphereColoring { get { return atmosphereColoring; } }
        public Color AtmosphereColor { get { return atmosphereColor; } }
        public float AtmosphereHighLightColoring { get { return atmosphereHighLightColoring; } }
        public Color AtmosphereHighLightColor { get { return atmosphereHighLightColor; } }
        public float CloudOcclusion { get { return cloudOcclusion; } }
        public float CloudAtmospheric { get { return cloudAtmospheric; } }
        public float GodRay { get { return godRay; } }
        public float GodRayStartDistance { get { return godRayStartDistance; } }
        public float Shadow { get { return shadow; } }
        public float SunShaft { get { return sunShaft; } }

        public static AtmosphereParameter Lerp(AtmosphereParameter l, AtmosphereParameter r, float t)
        {
            return new AtmosphereParameter()
            {
                atmosphere = Mathf.Lerp(l.atmosphere, r.atmosphere, t),
                atmosphereColoring = Mathf.Lerp(l.atmosphereColoring, r.atmosphereColoring, t),
                atmosphereColor = Color.Lerp(l.atmosphereColor, r.atmosphereColor, t),
                shadow = Mathf.Lerp(l.shadow, r.shadow, t),
                cloudAtmospheric = Mathf.Lerp(l.cloudAtmospheric, r.cloudAtmospheric, t),
                cloudOcclusion = Mathf.Lerp(l.cloudOcclusion, r.cloudOcclusion, t),
                godRay = Mathf.Lerp(l.godRay, r.godRay, t),
                sunShaft = Mathf.Lerp(l.sunShaft, r.sunShaft, t),
                atmosphereHighLightColor = Color.Lerp(l.atmosphereHighLightColor, r.atmosphereHighLightColor, t),
                atmosphereHighLightColoring = Mathf.Lerp(l.atmosphereHighLightColoring, r.atmosphereHighLightColoring, t),
                godRayStartDistance = Mathf.Lerp(l.godRayStartDistance, r.godRayStartDistance, t),
            };
        }

        public AtmosphereParameter ShallowCopy()
        {
            return (AtmosphereParameter)this.MemberwiseClone();
        }
    }
}