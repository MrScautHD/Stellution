using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;

namespace Mewlist.MassiveClouds
{
    public class CommandBufferUtility
    {
        private static Material blitMat;
        private static Material BlitMat
        {
            get
            {
                if (blitMat == null) blitMat = new Material(Shader.Find("MassiveCloudsBlit"));
                return blitMat;
            }
        }

        public static void BlitProcedural(CommandBuffer commandBuffer, RenderTargetIdentifier source, RenderTargetIdentifier destination)
        {
            if (XRSettings.enabled && XRSettings.eyeTextureDesc.vrUsage == VRTextureUsage.TwoEyes)
            {
                commandBuffer.Blit(source, destination);
            }
            else
            {
                commandBuffer.SetGlobalTexture("_MassiveCloudsResultTexture", source);
                commandBuffer.SetRenderTarget(destination);
                commandBuffer.DrawProcedural(Matrix4x4.identity, BlitMat, 0, MeshTopology.Triangles, 3, 1, null);
            }
        }

        public static void BlitProcedural(CommandBuffer commandBuffer, RenderTargetIdentifier source, RenderTargetIdentifier destination, RenderTargetIdentifier destinationDepth)
        {
            if (XRSettings.enabled && XRSettings.eyeTextureDesc.vrUsage == VRTextureUsage.TwoEyes)
            {
                commandBuffer.Blit(source, destination);
            }
            else
            {
                commandBuffer.SetGlobalTexture("_MassiveCloudsResultTexture", source);
                commandBuffer.SetRenderTarget(destination, destinationDepth);
                commandBuffer.DrawProcedural(Matrix4x4.identity, BlitMat, 0, MeshTopology.Triangles, 3, 1, null);
            }
        }

        public static void Blit(CommandBuffer commandBuffer, RenderTargetIdentifier source, RenderTargetIdentifier destination)
        {
            commandBuffer.Blit(source, destination);
        }

        public static void EnableKeyword(string keyword)
        {
            BlitMat.EnableKeyword(keyword);
        }

    }
}