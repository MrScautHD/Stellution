using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public class FogParameter
    {
        [Range(-10000f, 10000f)]
        [SerializeField] private float groundHeight;
        [Range(0.001f, 2000)]
        [SerializeField] private float range;
        [Range(0f, 1f)]
        [SerializeField] private float density;
        [Range(0f, 1f)]
        [SerializeField] private float coloring = 0f;
        [SerializeField] private Color color;
        [Range(0f, 1f)]
        [SerializeField] private float scattering;

        public float GroundHeight { get { return groundHeight; } }
        public float Range { get { return range; } }
        public float Density { get { return density; } }
        public float Coloring { get { return coloring; } }
        public Color Color { get { return color; } }
        public float Scattering { get { return scattering; } }

        public static FogParameter Lerp(FogParameter l, FogParameter r, float t)
        {
            return new FogParameter()
            {
                groundHeight = Mathf.Lerp(l.groundHeight, r.groundHeight, t),
                range = Mathf.Lerp(l.range, r.range, t),
                density = Mathf.Lerp(l.density, r.density, t),
                coloring = Mathf.Lerp(l.coloring, r.coloring, t),
                color = Color.Lerp(l.color, r.color, t),
                scattering = Mathf.Lerp(l.scattering, r.scattering, t),
            };
        }

        public FogParameter ShallowCopy()
        {
            return (FogParameter)this.MemberwiseClone();
        }
    }
}