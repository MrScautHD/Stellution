using UnityEngine;

namespace Mewlist.MassiveClouds
{
    public struct RenderTextureDesc
    {
        public readonly int Width;
        public readonly int Height;
        public readonly VRTextureUsage VRTextureUsage;

        public RenderTextureDesc(int width, int height)
        {
            Width = width;
            Height = height;
            VRTextureUsage = VRTextureUsage.None;
        }

        public RenderTextureDesc(int width, int height, VRTextureUsage vrTextureUsage)
        {
            Width = width;
            Height = height;
            VRTextureUsage = vrTextureUsage;
        }

        public RenderTextureDesc(int width, int height, Vector2 scale, VRTextureUsage vrTextureUsage)
        {
            Width = Mathf.RoundToInt(width * scale.x);
            Height = Mathf.RoundToInt(height * scale.y);
            VRTextureUsage = vrTextureUsage;
        }

        public static RenderTextureDesc operator *(RenderTextureDesc a, float b)
        {
            return new RenderTextureDesc(a.Width, a.Height, Vector2.one * b, a.VRTextureUsage);
        }
    }
}