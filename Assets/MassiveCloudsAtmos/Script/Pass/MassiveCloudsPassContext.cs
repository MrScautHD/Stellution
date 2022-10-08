using UnityEngine;
using UnityEngine.Rendering;

namespace Mewlist.MassiveClouds
{
    public struct MassiveCloudsPassContext
    {
        public readonly CommandBuffer cmd;
        public readonly Camera targetCamera;
        public readonly RenderTextureDesc colorBufferDesc;
        public readonly RenderTargetIdentifier source;

        public MassiveCloudsPassContext(
            CommandBuffer cmd,
            Camera targetCamera,
            RenderTextureDesc colorBufferDesc,
            RenderTargetIdentifier source)
        {
            this.cmd = cmd;
            this.targetCamera = targetCamera;
            this.colorBufferDesc = colorBufferDesc;
            this.source = source;
        }
    }
}