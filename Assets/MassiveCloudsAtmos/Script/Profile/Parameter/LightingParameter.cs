using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public class LightingParameter
    {
        [Range(0f, 1f)] public float Intensity;
        [Range(0f, 1f)] public float Quality;
        [Range(0f, 1f)] public float Scattering;
        [Range(0f, 1f)] public float Shading;
        [Range(0f, 1f)] public float ShadingDistance;
        [Range(0f, 1f)] public float Transparency;
        [Range(0f, 1f)] public float Mie;
    }
}