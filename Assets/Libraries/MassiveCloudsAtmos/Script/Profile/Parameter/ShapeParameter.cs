using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public class HorizontalShapeParameter
    {
        [Range(0f, 5000f)] public float FromHeight;
        [Range(0f, 10000f)] public float ToHeight;
        [Range(0f, 1f)] public float SoftnessTop;
        [Range(0f, 1f)] public float SoftnessBottom;
        [Range(0f, 1f)] public float Figure;
        [Range(0f, 200000f)] public float MaxDistance;
    }
}