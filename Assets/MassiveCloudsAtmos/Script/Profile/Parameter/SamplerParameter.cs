using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public class SamplerParameter
    {
        public Texture3D VolumeTexture;
        public Vector2 Tiling;
        [Range(1f, 32f)] public float Octave = 4f;
        [Range(-1f, 1f)] public float Sculpture = 0f;
        [Range(-1f, 1f)] public float Sculpture2 = 0f;
        [Range(-1f, 1f)] public float Sculpture3 = 0f;
        [Range(-1f, 1f)] public float Sculpture4 = 0f;
        [Range(-1f, 1f)] public float Warp = 0f;
        [Range(0f, 1f)] public float Softness = 0.5f;
        [Range(1f, 2f)] public float NearSoftnessScale = 1f;
        [Range(0f, 1f)] public float Density = 0.5f;
        [Range(0.1f, 10f)] public float Scale = 5f;
    }
}