#if ENVIRO_LWRP && ENVIRO_HD

namespace UnityEngine.Rendering.LWRP
{
    internal class EnviroDistanceBlurPass : UnityEngine.Rendering.Universal.ScriptableRenderPass
    {
        private Camera myCam;
        public Material blitMaterial = null;
        private Material blitThrough;
         private UnityEngine.Rendering.Universal.ScriptableRenderer renderer { get; set; }
        private UnityEngine.Rendering.Universal.RenderTargetHandle destination { get; set; }

        #region Blur Var
        /////////////////// Blur //////////////////////
        private const int kMaxIterations = 16;
        private RenderTexture[] _blurBuffer1 = new RenderTexture[kMaxIterations];
        private RenderTexture[] _blurBuffer2 = new RenderTexture[kMaxIterations];
        private Texture2D distributionTexture;
        public float thresholdGamma
        {
            get { return Mathf.Max(0f, 0); }
        }
        public float thresholdLinear
        {
            get { return Mathf.GammaToLinearSpace(thresholdGamma); }
        }
        #endregion

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

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            ConfigureTarget(renderer.cameraColorTarget);
        }

        /// <summary>
        /// Create the CopyColorPass
        /// </summary>
        public EnviroDistanceBlurPass(UnityEngine.Rendering.Universal.RenderPassEvent renderPassEvent, Material blitMaterial, Texture2D distributionTexture)
        {
            this.renderPassEvent = renderPassEvent;
            this.blitMaterial = blitMaterial;
            this.distributionTexture = distributionTexture;         
        }

        public void Setup(Camera myCam, UnityEngine.Rendering.Universal.ScriptableRenderer renderer, UnityEngine.Rendering.Universal.RenderTargetHandle destination)
        {
            this.myCam = myCam;
            this.renderer = renderer;
            this.destination = destination;
        }
        private void UpdateMatrix(UnityEngine.Rendering.Universal.RenderingData renderingData, Material mat)
        {
            if (UnityEngine.XR.XRSettings.enabled)
            {
                // Both stereo eye inverse view matrices
                Matrix4x4 left_world_from_view = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left).inverse;
                Matrix4x4 right_world_from_view = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right).inverse;

                // Both stereo eye inverse projection matrices, plumbed through GetGPUProjectionMatrix to compensate for render texture
                Matrix4x4 left_screen_from_view = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                Matrix4x4 right_screen_from_view = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                Matrix4x4 left_view_from_screen = GL.GetGPUProjectionMatrix(left_screen_from_view, true).inverse;
                Matrix4x4 right_view_from_screen = GL.GetGPUProjectionMatrix(right_screen_from_view, true).inverse;
 
                // Negate [1,1] to reflect Unity's CBuffer state
                if (SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3)
                {
                    left_view_from_screen[1, 1] *= -1;
                    right_view_from_screen[1, 1] *= -1;
                }

                mat.SetMatrix("_LeftWorldFromView", left_world_from_view);
                mat.SetMatrix("_RightWorldFromView", right_world_from_view);
                mat.SetMatrix("_LeftViewFromScreen", left_view_from_screen);
                mat.SetMatrix("_RightViewFromScreen", right_view_from_screen);
            }
            else
            {
                Matrix4x4 left_world_from_view = renderingData.cameraData.GetViewMatrix().inverse;

                // Inverse projection matrices, plumbed through GetGPUProjectionMatrix to compensate for render texture
                Matrix4x4 screen_from_view = renderingData.cameraData.GetProjectionMatrix();
                Matrix4x4 left_view_from_screen = GL.GetGPUProjectionMatrix(screen_from_view, true).inverse;

                // Negate [1,1] to reflect Unity's CBuffer state
                if (SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3)
                    left_view_from_screen[1, 1] *= -1;

                // Store matrices
                mat.SetMatrix("_LeftWorldFromView", left_world_from_view);
                mat.SetMatrix("_LeftViewFromScreen", left_view_from_screen);
            }
        }
        /// 
        public override void Execute(ScriptableRenderContext context, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Distance Blur");

  
            // source texture size
            var tw = renderingData.cameraData.cameraTargetDescriptor.width;
            var th = renderingData.cameraData.cameraTargetDescriptor.height;

            // halve the texture size for the low quality mode
            if (!EnviroSky.instance.distanceBlurSettings.highQuality)
            {
                tw /= 2;
                th /= 2;
            }

            if(blitMaterial == null)
               blitMaterial = new Material(Shader.Find("Hidden/EnviroDistanceBlur"));

            UpdateMatrix(renderingData,blitMaterial);

            blitMaterial.EnableKeyword("ENVIROURP");
            blitMaterial.SetTexture("_DistTex", distributionTexture);
            blitMaterial.SetFloat("_Distance", EnviroSky.instance.blurDistance);
            blitMaterial.SetFloat("_Radius", EnviroSky.instance.distanceBlurSettings.radius);

            // determine the iteration count
            var logh = Mathf.Log(th, 2) + EnviroSky.instance.distanceBlurSettings.radius - 8;
            var logh_i = (int)logh;
            var iterations = Mathf.Clamp(logh_i, 1, kMaxIterations);

            // update the shader properties
            var lthresh = thresholdLinear;
            blitMaterial.SetFloat("_Threshold", lthresh);

            var knee = lthresh * 0.5f + 1e-5f;
            var curve = new Vector3(lthresh - knee, knee * 2, 0.25f / knee);
            blitMaterial.SetVector("_Curve", curve);

            var pfo = !EnviroSky.instance.distanceBlurSettings.highQuality && EnviroSky.instance.distanceBlurSettings.antiFlicker;
            blitMaterial.SetFloat("_PrefilterOffs", pfo ? -0.5f : 0.0f);

            blitMaterial.SetFloat("_SampleScale", 0.5f + logh - logh_i);
            blitMaterial.SetFloat("_Intensity", EnviroSky.instance.blurIntensity);
            blitMaterial.SetFloat("_SkyBlurring", EnviroSky.instance.blurSkyIntensity);
            float width = 1 / renderingData.cameraData.cameraTargetDescriptor.width;
            float height = 1 / renderingData.cameraData.cameraTargetDescriptor.height;
            blitMaterial.SetVector("_MainTex_TexelSize",new Vector4(width, height, renderingData.cameraData.cameraTargetDescriptor.width, renderingData.cameraData.cameraTargetDescriptor.height));
           

            // prefilter pass
            RenderTextureDescriptor renderDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            renderDescriptor.width = tw;
            renderDescriptor.height = th;
            //renderDescriptor.graphicsFormat = Experimental.Rendering.GraphicsFormat.R32G32B32A32_SFloat;
            renderDescriptor.depthBufferBits = 0;

            var prefiltered = RenderTexture.GetTemporary(renderDescriptor);

            var pass = EnviroSky.instance.distanceBlurSettings.antiFlicker ? 1 : 0;
            CustomBlit(cmd, renderer.cameraColorTarget, prefiltered, blitMaterial, pass);

            // construct a mip pyramid
            var last = prefiltered;
            for (var level = 0; level < iterations; level++)
            {
                RenderTextureDescriptor lastDescriptor = last.descriptor;
                lastDescriptor.width = lastDescriptor.width / 2;
                lastDescriptor.height = lastDescriptor.height / 2;
                _blurBuffer1[level] = RenderTexture.GetTemporary(lastDescriptor);

                pass = (level == 0) ? (EnviroSky.instance.distanceBlurSettings.antiFlicker ? 3 : 2) : 4;
                CustomBlit(cmd, last, _blurBuffer1[level], blitMaterial, pass);
                last = _blurBuffer1[level];
            }

            // upsample and combine loop
            for (var level = iterations - 2; level >= 0; level--)
            {
                var basetex = _blurBuffer1[level];
                blitMaterial.SetTexture("_BaseTex", basetex);
                RenderTextureDescriptor baseDescriptor = basetex.descriptor;
                _blurBuffer2[level] = RenderTexture.GetTemporary(baseDescriptor);

                pass = EnviroSky.instance.distanceBlurSettings.highQuality ? 6 : 5;
                CustomBlit(cmd, last, _blurBuffer2[level], blitMaterial, pass);
                last = _blurBuffer2[level];
            }

            // finish process
            RenderTexture sourceRT = RenderTexture.GetTemporary(renderDescriptor);

            CustomBlit(cmd, renderer.cameraColorTarget, sourceRT);
            blitMaterial.SetTexture("_BaseTex", sourceRT);
            cmd.SetGlobalTexture("_BaseTex", sourceRT);

            pass = EnviroSky.instance.distanceBlurSettings.highQuality ? 8 : 7;
            CustomBlit(cmd, last, renderer.cameraColorTarget, blitMaterial, pass);

            // release the temporary buffers
            for (var i = 0; i < kMaxIterations; i++)
            {
                if (_blurBuffer1[i] != null)
                    RenderTexture.ReleaseTemporary(_blurBuffer1[i]);

                if (_blurBuffer2[i] != null)
                    RenderTexture.ReleaseTemporary(_blurBuffer2[i]);

                _blurBuffer1[i] = null;
                _blurBuffer2[i] = null;
            }

            RenderTexture.ReleaseTemporary(prefiltered);
            RenderTexture.ReleaseTemporary(sourceRT);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// <inheritdoc/>
        public override void FrameCleanup(CommandBuffer cmd)
        {
            //if (destination == RenderTargetHandle.CameraTarget)
            //    cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
        }
    }
}
#endif