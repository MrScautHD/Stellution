#if ENVIRO_LWRP

namespace UnityEngine.Rendering.LWRP
{
    /// <summary>
    /// Copy the given color buffer to the given destination color buffer.
    ///
    /// You can use this pass to copy a color buffer to the destination,
    /// so you can use it later in rendering. For example, you can copy
    /// the opaque texture to use it for distortion effects.
    /// </summary>
    internal class EnviroBlitPass : UnityEngine.Rendering.Universal.ScriptableRenderPass
    {
        public enum RenderTarget
        {
            Color,
            RenderTexture,
        }

        public Material blitMaterial = null;
        public int blitShaderPassIndex = 0;
        public FilterMode filterMode { get; set; }

        private UnityEngine.Rendering.Universal.ScriptableRenderer renderer { get; set; }
        private UnityEngine.Rendering.Universal.RenderTargetHandle destination { get; set; }
        private RenderTexture setMainTexTo { get; set; }
        private Material blitThrough;
        UnityEngine.Rendering.Universal.RenderTargetHandle m_TemporaryColorTexture;
        string m_ProfilerTag;

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureTarget(renderer.cameraColorTarget);
            ConfigureInput(UnityEngine.Rendering.Universal.ScriptableRenderPassInput.Depth);
        }

        /// <summary>
        /// Create the CopyColorPass
        /// </summary>
        public EnviroBlitPass(UnityEngine.Rendering.Universal.RenderPassEvent renderPassEvent, Material blitMaterial, int blitShaderPassIndex, string tag)
        {
            this.renderPassEvent = renderPassEvent;
            this.blitMaterial = blitMaterial;
            this.blitShaderPassIndex = blitShaderPassIndex;
            m_ProfilerTag = tag;
            m_TemporaryColorTexture.Init("_TemporaryColorTexture");
        }

        /// <summary>
        /// Configure the pass with the source and destination to execute on.
        /// </summary>
        /// <param name="source">Source Render Target</param>
        /// <param name="destination">Destination Render Target</param>
        public void Setup(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, UnityEngine.Rendering.Universal.RenderTargetHandle destination, Material mat)
        {
            this.renderer = renderer;
            this.destination = destination;
            this.blitMaterial = mat; 
        }

        /// <summary>
        /// Configure the pass with the source and destination to execute on.
        /// </summary>
        /// <param name="source">Source Render Target</param>
        /// <param name="destination">Destination Render Target</param>
        public void Setup(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, UnityEngine.Rendering.Universal.RenderTargetHandle destination, RenderTexture setTexTo)
        {
            this.renderer = renderer;
            this.destination = destination;
            this.setMainTexTo = setTexTo;      
        }

        public void CustomBlit(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, Material mat)
        {
            cmd.SetGlobalTexture("_MainTex", source);
            cmd.SetRenderTarget(target, 0, CubemapFace.Unknown, -1);
            cmd.DrawMesh(UnityEngine.Rendering.Universal.RenderingUtils.fullscreenMesh, Matrix4x4.identity, mat);
        }

        public void CustomBlit(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target, Material mat, int pass)
        {
            cmd.SetGlobalTexture("_MainTex", source);
            cmd.SetRenderTarget(target, 0, CubemapFace.Unknown, -1);
            cmd.DrawMesh(UnityEngine.Rendering.Universal.RenderingUtils.fullscreenMesh, Matrix4x4.identity, mat,0,pass);
        }

        public void CustomBlit(CommandBuffer cmd, RenderTargetIdentifier source, RenderTargetIdentifier target)
        {
            if(blitThrough == null)
                blitThrough = new Material(Shader.Find("Hidden/EnviroBlitThrough"));
                
            cmd.SetGlobalTexture("_MainTex", source);
            cmd.SetRenderTarget(target, 0, CubemapFace.Unknown, -1);
            cmd.DrawMesh(UnityEngine.Rendering.Universal.RenderingUtils.fullscreenMesh, Matrix4x4.identity, blitThrough);
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

            RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
            opaqueDesc.depthBufferBits = 0;

            // Can't read and write to same color target, create a temp render target to blit. 
            if (destination == UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget)
            {
                cmd.GetTemporaryRT(m_TemporaryColorTexture.id, opaqueDesc, filterMode);

                RenderTargetIdentifier mainID;

                if (setMainTexTo != null)
                {
                    mainID = new RenderTargetIdentifier(setMainTexTo);
                    CustomBlit(cmd,mainID,m_TemporaryColorTexture.Identifier(),blitMaterial,blitShaderPassIndex);
                }
                else
                { 
                  CustomBlit(cmd,renderer.cameraColorTarget,m_TemporaryColorTexture.Identifier(),blitMaterial,blitShaderPassIndex);
                }  

                CustomBlit(cmd,m_TemporaryColorTexture.Identifier(),renderer.cameraColorTarget);
            }
            else
            {      
                CustomBlit(cmd,renderer.cameraColorTarget, destination.Identifier(),blitMaterial,blitShaderPassIndex);
            }
      
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// <inheritdoc/>
        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (destination == UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget)
                cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
        }
    }
}
#endif