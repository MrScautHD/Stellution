using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mewlist.MassiveClouds
{
    
    public class FlippingRenderTextures
    {
        private static readonly int firstId = Shader.PropertyToID("_MassiveCloudsBufferFirst");
        private static readonly int secondId = Shader.PropertyToID("_MassiveCloudsBufferSecond");
        public static readonly int LowResolutionTmpId = Shader.PropertyToID("_MassiveCloudsBufferLow");
        public readonly int ScreenWidth;
        public readonly int ScreenHeight;

        private bool flipped;

        public readonly RenderTextureFormat FormatAlpha;

        public int From { get { return flipped ? firstId : secondId; } }
        public int To { get { return flipped ? secondId : firstId; } }

        public FlippingRenderTextures(
            RenderTextureDesc colorBufferDesc,
            RenderTextureFormat formatAlpha,
            CommandBuffer commandBuffer,
            float resolution)
        {
            flipped = false;

            ScreenWidth = colorBufferDesc.Width;
            ScreenHeight = colorBufferDesc.Height;

            FormatAlpha = formatAlpha;

            CreateRenderTextures(colorBufferDesc, commandBuffer, resolution);
        } 

        private void CreateRenderTextures(
            RenderTextureDesc colorBufferDesc,
            CommandBuffer commandBuffer,
            float resolution)
        {
            commandBuffer.GetTemporaryRT(firstId, colorBufferDesc.Width, colorBufferDesc.Height, 0,
                FilterMode.Point, FormatAlpha);
            commandBuffer.GetTemporaryRT(secondId, colorBufferDesc.Width, colorBufferDesc.Height, 0,
                FilterMode.Point, FormatAlpha);

            commandBuffer.GetTemporaryRT(LowResolutionTmpId,
                Mathf.RoundToInt(colorBufferDesc.Width * resolution),
                Mathf.RoundToInt(colorBufferDesc.Height * resolution),
                0, FilterMode.Trilinear, FormatAlpha);
        }

        public RenderTexture CreateRenderTexture(RenderTextureDesc desc)
        {
            RenderTexture rt;
            var rtDesc = new RenderTextureDescriptor(desc.Width, desc.Height, FormatAlpha, 0)
            {
                useMipMap = false,
                vrUsage = desc.VRTextureUsage
            };
            rt = new RenderTexture(rtDesc);
            rt.filterMode = FilterMode.Trilinear;
            rt.wrapMode = TextureWrapMode.Mirror;

            rt.name = "MassiveCloudsRT" + DateTime.Now.Millisecond;
            return rt;
        }

        public void Release(CommandBuffer commandBuffer)
        {
            commandBuffer.ReleaseTemporaryRT(firstId);
            commandBuffer.ReleaseTemporaryRT(secondId);
            commandBuffer.ReleaseTemporaryRT(LowResolutionTmpId);
        }

        public void Flip()
        {
            flipped = !flipped;
        }
    }
}