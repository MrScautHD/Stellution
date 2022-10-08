using UnityEngine;

namespace Mewlist.MassiveClouds
{
    internal static class AtmosPadExtension
    {
        internal static string ToTimeString(this float x)
        {
            return Mathf.Floor(x * 24f).ToString("00") + ":" +
                   Mathf.Floor(60f * Mathf.Repeat(x * 24f, 1f)).ToString("00");
        }
    }
}