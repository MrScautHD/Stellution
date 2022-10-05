#if ENVIRO_LWRP && ENVIRO_HD

namespace UnityEngine.Rendering.LWRP
{
    /// <summary>
    /// Copy the given color buffer to the given destination color buffer.
    ///
    /// You can use this pass to copy a color buffer to the destination,
    /// so you can use it later in rendering. For example, you can copy
    /// the opaque texture to use it for distortion effects.
    /// </summary>
    internal class EnviroBlitPassVolumeClouds : UnityEngine.Rendering.Universal.ScriptableRenderPass
    {
        
        private Camera myCam;

        public Material cloudsMat = null;
        public Material blitMat = null;
        private Material compose;
        private Material downsample;
        private Material blitThrough;

        public FilterMode filterMode { get; set; }

        private UnityEngine.Rendering.Universal.ScriptableRenderer renderer { get; set; }
        private UnityEngine.Rendering.Universal.RenderTargetHandle destination { get; set; }


        #region Clouds Var
        private EnviroHaltonSequence sequence = new EnviroHaltonSequence() { radix = 3 };
        private RenderTexture subFrameTex;
        private RenderTexture prevFrameTex;
        private Matrix4x4 projection;
        private Matrix4x4 projectionSPVR;
        private Matrix4x4 inverseRotation;
        private Matrix4x4 inverseRotationSPVR;
        private Matrix4x4 rotation;
        private Matrix4x4 rotationSPVR;
        private Matrix4x4 previousRotation;
        private Matrix4x4 previousRotationSPVR;
        [HideInInspector]
        public EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize currentReprojectionPixelSize;
        private int reprojectionPixelSize;
        private bool isFirstFrame;
        private int subFrameNumber;
        private int[] frameList;
        private int renderingCounter;
        private int subFrameWidth;
        private int subFrameHeight;
        private int frameWidth;
        private int frameHeight;
        private bool textureDimensionChanged;
        #endregion


        UnityEngine.Rendering.Universal.RenderTargetHandle m_TemporaryColorTexture;
        string m_ProfilerTag;

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
            ConfigureInput(UnityEngine.Rendering.Universal.ScriptableRenderPassInput.Depth);
        }

        /// <summary>
        /// Create the CopyColorPass
        /// </summary>
        public EnviroBlitPassVolumeClouds(UnityEngine.Rendering.Universal.RenderPassEvent renderPassEvent,Material cloudsMaterial, Material blitMaterial,Material composeMaterial, Material downsampleMaterial, string tag)
        {
            this.renderPassEvent = renderPassEvent;
            this.blitMat = blitMaterial;
            this.cloudsMat = cloudsMaterial;
            this.compose = composeMaterial;
            this.downsample = downsampleMaterial;
            m_ProfilerTag = tag;
            m_TemporaryColorTexture.Init("_TemporaryColorTexture");


            if (EnviroSky.instance != null)
               SetReprojectionPixelSize(EnviroSky.instance.cloudsSettings.cloudsQualitySettings.reprojectionPixelSize);
        }

        /// <summary>
        /// Configure the pass with the source and destination to execute on.
        /// </summary>
        /// <param name="source">Source Render Target</param>
        /// <param name="destination">Destination Render Target</param>
        public void Setup(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, UnityEngine.Rendering.Universal.RenderTargetHandle destination, Camera cam)
        {
            this.renderer = renderer;
            this.destination = destination;
            this.myCam = cam;
        }

        #region Volume Clouds Functions
        ////////// Clouds Functions ///////////////
        private void SetCloudProperties(UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            if (EnviroSky.instance.cloudsSettings.cloudsQualitySettings.baseQuality == EnviroVolumeCloudsQualitySettings.CloudDetailQuality.Low)
                cloudsMat.SetTexture("_Noise", EnviroSky.instance.ressources.noiseTexture);
            else
                cloudsMat.SetTexture("_Noise", EnviroSky.instance.ressources.noiseTextureHigh);

            if (EnviroSky.instance.cloudsSettings.cloudsQualitySettings.detailQuality == EnviroVolumeCloudsQualitySettings.CloudDetailQuality.Low)
                cloudsMat.SetTexture("_DetailNoise", EnviroSky.instance.ressources.detailNoiseTexture);
            else
                cloudsMat.SetTexture("_DetailNoise", EnviroSky.instance.ressources.detailNoiseTextureHigh);

            if (EnviroSky.instance.floatingPointOriginAnchor != null)
                EnviroSky.instance.floatingPointOriginMod = EnviroSky.instance.floatingPointOriginAnchor.position;

            cloudsMat.SetVector("_CameraPosition", myCam.transform.position - EnviroSky.instance.floatingPointOriginMod);

            if(UnityEngine.XR.XRSettings.enabled && EnviroSky.instance.singlePassInstancedVR && renderingData.cameraData.cameraType == CameraType.Game && Application.isPlaying)
            {
                projection = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                cloudsMat.SetMatrix("_InverseProjection", projection.inverse);
                inverseRotation = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left).inverse;
                cloudsMat.SetMatrix("_InverseRotation", inverseRotation);

                Matrix4x4 inverseProjectionRightSP = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right).inverse;
                cloudsMat.SetMatrix("_InverseProjection_SP", inverseProjectionRightSP);
                inverseRotationSPVR = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right).inverse;
                cloudsMat.SetMatrix("_InverseRotation_SP", inverseRotationSPVR);
            }
            else
            {
                if(UnityEngine.XR.XRSettings.enabled)
                {
                    projection = renderingData.cameraData.GetProjectionMatrix();
                    Matrix4x4 inverseProjection = projection.inverse;
                    cloudsMat.SetMatrix("_InverseProjection", inverseProjection);
                    inverseRotation = myCam.cameraToWorldMatrix;   
                    cloudsMat.SetMatrix("_InverseRotation", inverseRotation);  
                }
                else
                {
                    projection = myCam.projectionMatrix;
                    Matrix4x4 inverseProjection = projection.inverse;
                    cloudsMat.SetMatrix("_InverseProjection", inverseProjection);
                    inverseRotation = myCam.cameraToWorldMatrix; 
                    cloudsMat.SetMatrix("_InverseRotation", inverseRotation);  
                }
            }

            //Weather Map
            if (EnviroSky.instance.cloudsSettings.customWeatherMap == null)
                cloudsMat.SetTexture("_WeatherMap", EnviroSky.instance.weatherMap);
            else
                cloudsMat.SetTexture("_WeatherMap", EnviroSky.instance.cloudsSettings.customWeatherMap);

            //Curl Noise
            if (EnviroSky.instance.cloudsSettings.cloudsQualitySettings.useCurlNoise)
            {
                cloudsMat.EnableKeyword("ENVIRO_CURLNOISE");
                cloudsMat.SetTexture("_CurlNoise", EnviroSky.instance.ressources.curlMap);
            }
            else
            {
                cloudsMat.DisableKeyword("ENVIRO_CURLNOISE");
            }

            //Optimizations
            if (EnviroSky.instance.cloudsSettings.useHaltonRaymarchOffset)
            {
                cloudsMat.EnableKeyword("ENVIRO_HALTONOFFSET");
                cloudsMat.SetFloat("_RaymarchOffset", sequence.Get());
                cloudsMat.SetVector("_TexelSize", subFrameTex.texelSize);
            }
            else
            {
                cloudsMat.DisableKeyword("ENVIRO_HALTONOFFSET");
            }

            //RaymarchOffset

            if (!EnviroSky.instance.cloudsSettings.useLessSteps)
                cloudsMat.SetVector("_Steps", new Vector4(EnviroSky.instance.cloudsSettings.cloudsQualitySettings.raymarchSteps * EnviroSky.instance.cloudsConfig.raymarchingScale, EnviroSky.instance.cloudsSettings.cloudsQualitySettings.raymarchSteps * EnviroSky.instance.cloudsConfig.raymarchingScale, 0.0f, 0.0f));
            else
                cloudsMat.SetVector("_Steps", new Vector4((EnviroSky.instance.cloudsSettings.cloudsQualitySettings.raymarchSteps * EnviroSky.instance.cloudsConfig.raymarchingScale) * 0.75f, (EnviroSky.instance.cloudsSettings.cloudsQualitySettings.raymarchSteps * EnviroSky.instance.cloudsConfig.raymarchingScale) * 0.75f, 0.0f, 0.0f));

            cloudsMat.SetFloat("_BaseNoiseUV", EnviroSky.instance.cloudsSettings.cloudsQualitySettings.baseNoiseUV);
            cloudsMat.SetFloat("_DetailNoiseUV", EnviroSky.instance.cloudsSettings.cloudsQualitySettings.detailNoiseUV);
            cloudsMat.SetFloat("_AmbientSkyColorIntensity", EnviroSky.instance.cloudsSettings.ambientLightIntensity.Evaluate(EnviroSky.instance.GameTime.solarTime));
            cloudsMat.SetVector("_CloudsLighting", new Vector4(EnviroSky.instance.cloudsConfig.scatteringCoef, EnviroSky.instance.cloudsSettings.hgPhase, EnviroSky.instance.cloudsSettings.silverLiningIntensity, EnviroSky.instance.cloudsSettings.silverLiningSpread.Evaluate(EnviroSky.instance.GameTime.solarTime)));
            cloudsMat.SetVector("_CloudsLightingExtended", new Vector4(EnviroSky.instance.cloudsConfig.edgeDarkness, EnviroSky.instance.cloudsConfig.ambientSkyColorIntensity, EnviroSky.instance.tonemapping ? 0f : 1f, EnviroSky.instance.cloudsSettings.cloudsExposure));
            cloudsMat.SetColor("_AmbientLightColor", EnviroSky.instance.cloudsSettings.volumeCloudsAmbientColor.Evaluate(EnviroSky.instance.GameTime.solarTime));

            float bottomH = EnviroSky.instance.cloudsSettings.cloudsQualitySettings.bottomCloudHeight + EnviroSky.instance.cloudsSettings.cloudsHeightMod;
            float topH = EnviroSky.instance.cloudsSettings.cloudsQualitySettings.topCloudHeight + EnviroSky.instance.cloudsSettings.cloudsHeightMod;

            cloudsMat.SetVector("_CloudsParameter", new Vector4(bottomH, topH,  1 / (topH - bottomH), EnviroSky.instance.cloudsSettings.cloudsWorldScale * 10));

            if (EnviroSky.instance.cloudsSettings.useLessSteps)
                cloudsMat.SetVector("_CloudDensityScale", new Vector4(EnviroSky.instance.cloudsConfig.density * 1.5f, EnviroSky.instance.cloudsConfig.lightStepModifier, EnviroSky.instance.GameTime.dayNightSwitch, EnviroSky.instance.GameTime.solarTime));
            else
                cloudsMat.SetVector("_CloudDensityScale", new Vector4(EnviroSky.instance.cloudsConfig.density, EnviroSky.instance.cloudsConfig.lightStepModifier, EnviroSky.instance.GameTime.dayNightSwitch, EnviroSky.instance.GameTime.solarTime));

            cloudsMat.SetFloat("_CloudsType", EnviroSky.instance.cloudsConfig.cloudType);
            cloudsMat.SetVector("_CloudsCoverageSettings", new Vector4(EnviroSky.instance.cloudsConfig.coverage * EnviroSky.instance.cloudsSettings.globalCloudCoverage, EnviroSky.instance.cloudsConfig.lightAbsorbtion, EnviroSky.instance.cloudsSettings.cloudsQualitySettings.transmissionToExit, 0f));
            cloudsMat.SetVector("_CloudsAnimation", new Vector4(EnviroSky.instance.cloudAnim.x, EnviroSky.instance.cloudAnim.y, EnviroSky.instance.cloudsSettings.cloudsWindDirectionX, EnviroSky.instance.cloudsSettings.cloudsWindDirectionY));
            cloudsMat.SetColor("_LightColor", EnviroSky.instance.cloudsSettings.volumeCloudsColor.Evaluate(EnviroSky.instance.GameTime.solarTime));
            cloudsMat.SetColor("_MoonLightColor", EnviroSky.instance.cloudsSettings.volumeCloudsMoonColor.Evaluate(EnviroSky.instance.GameTime.lunarTime));
            cloudsMat.SetFloat("_stepsInDepth", EnviroSky.instance.cloudsSettings.cloudsQualitySettings.stepsInDepthModificator);
            cloudsMat.SetFloat("_LODDistance", EnviroSky.instance.cloudsSettings.cloudsQualitySettings.lodDistance);


            if (EnviroSky.instance.lightSettings.directionalLightMode == EnviroLightSettings.LightingMode.Dual)
            {
                if (EnviroSky.instance.GameTime.dayNightSwitch < EnviroSky.instance.GameTime.solarTime)
                    cloudsMat.SetVector("_LightDir", -EnviroSky.instance.Components.DirectLight.transform.forward);
                else if (EnviroSky.instance.Components.AdditionalDirectLight != null)
                    cloudsMat.SetVector("_LightDir", -EnviroSky.instance.Components.AdditionalDirectLight.transform.forward);
            }
            else
                cloudsMat.SetVector("_LightDir", -EnviroSky.instance.Components.DirectLight.transform.forward);

            cloudsMat.SetFloat("_LightIntensity", EnviroSky.instance.cloudsSettings.lightIntensity.Evaluate(EnviroSky.instance.GameTime.solarTime));
            cloudsMat.SetVector("_CloudsErosionIntensity", new Vector4(1f - EnviroSky.instance.cloudsConfig.baseErosionIntensity, EnviroSky.instance.cloudsConfig.detailErosionIntensity, EnviroSky.instance.cloudsSettings.attenuationClamp.Evaluate(EnviroSky.instance.GameTime.solarTime), EnviroSky.instance.cloudAnim.z));
            // cloudsMat.SetTexture("_BlueNoise", blueNoise);        
            // cloudsMat.SetVector("_Randomness", new Vector4(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value));
        }

        private void SetBlitmaterialProperties()
        {
            Matrix4x4 inverseProjection = projection.inverse;

            blitMat.SetMatrix("_PreviousRotation", previousRotation);
            blitMat.SetMatrix("_Projection", projection);
            blitMat.SetMatrix("_InverseRotation", inverseRotation);
            blitMat.SetMatrix("_InverseProjection", inverseProjection);

            if (UnityEngine.XR.XRSettings.enabled)
            {
                Matrix4x4 inverseProjectionSPVR = projectionSPVR.inverse;
                blitMat.SetMatrix("_PreviousRotationSPVR", previousRotationSPVR);
                blitMat.SetMatrix("_ProjectionSPVR", projectionSPVR);
                blitMat.SetMatrix("_InverseRotationSPVR", inverseRotationSPVR);
                blitMat.SetMatrix("_InverseProjectionSPVR", inverseProjectionSPVR);
            }

            blitMat.SetFloat("_FrameNumber", subFrameNumber);
            blitMat.SetFloat("_ReprojectionPixelSize", reprojectionPixelSize);
            blitMat.SetVector("_SubFrameDimension", new Vector2(subFrameWidth, subFrameHeight));
            blitMat.SetVector("_FrameDimension", new Vector2(frameWidth, frameHeight));
        }
        RenderTexture DownsampleDepth(CommandBuffer cmd,RenderTextureDescriptor desc, Texture src, Material mat, int downsampleFactor)
        {
            int X = desc.width;
            int Y = desc.height;
            Vector2 offset = new Vector2(1.0f / X, 1.0f / Y);
            X /= downsampleFactor;
            Y /= downsampleFactor;
            RenderTextureDescriptor lowDepthDescr = desc;
            lowDepthDescr.width = X;
            lowDepthDescr.height = Y;
            lowDepthDescr.depthBufferBits = 0;
            RenderTexture lowDepth = RenderTexture.GetTemporary(lowDepthDescr);
            mat.SetVector("_PixelSize", offset);
            //Graphics.Blit(src, lowDepth, mat);
            RenderTargetIdentifier srcID = new RenderTargetIdentifier(src);
            RenderTargetIdentifier lowDepthID = new RenderTargetIdentifier(lowDepth);
            mat.EnableKeyword("ENVIROURP");
            CustomBlit(cmd,srcID,lowDepthID,mat);
            return lowDepth;
        }

        private void CreateCloudsRenderTextures(RenderTextureDescriptor d)
        {
            if (subFrameTex != null)
            {
                MonoBehaviour.DestroyImmediate(subFrameTex);
                subFrameTex = null;
            }

            if (prevFrameTex != null)
            {
                MonoBehaviour.DestroyImmediate(prevFrameTex);
                prevFrameTex = null;
            }

            RenderTextureFormat format = myCam.allowHDR ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
 
            if (subFrameTex == null)
            {
                RenderTextureDescriptor desc = d;
                desc.width = subFrameWidth;
                desc.height = subFrameHeight;
                desc.depthBufferBits = 0;
                desc.graphicsFormat = Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat;

                subFrameTex = new RenderTexture(desc);
                subFrameTex.filterMode = FilterMode.Bilinear;
                subFrameTex.hideFlags = HideFlags.HideAndDontSave;

                isFirstFrame = true;
            }

            if (prevFrameTex == null)
            {
                RenderTextureDescriptor desc = d;
                desc.width = frameWidth;
                desc.height = frameHeight;
                desc.depthBufferBits = 0;
                desc.graphicsFormat = Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat;

                prevFrameTex = new RenderTexture(desc);
                prevFrameTex.filterMode = FilterMode.Bilinear;
                prevFrameTex.hideFlags = HideFlags.HideAndDontSave;

                isFirstFrame = true;
            }
        }
        private void SetReprojectionPixelSize(EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize pSize)
        {
            switch (pSize)
            {
                case EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize.Off:
                    reprojectionPixelSize = 1;
                    break;

                case EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize.Low:
                    reprojectionPixelSize = 2;
                    break;

                case EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize.Medium:
                    reprojectionPixelSize = 4;
                    break;

                case EnviroVolumeCloudsQualitySettings.ReprojectionPixelSize.High:
                    reprojectionPixelSize = 8;
                    break;
            }

            frameList = CalculateFrames(reprojectionPixelSize);
        }
        private void StartFrame(UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            textureDimensionChanged = UpdateFrameDimensions(renderingData.cameraData.cameraTargetDescriptor);

            switch (myCam.stereoActiveEye)
            {
                case Camera.MonoOrStereoscopicEye.Mono: 
                
                    if(UnityEngine.XR.XRSettings.enabled && renderingData.cameraData.cameraType == CameraType.Game && Application.isPlaying)
                    {
                        projection = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                        rotation = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
                        inverseRotation = rotation.inverse;

                        projectionSPVR = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                        rotationSPVR = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
                        inverseRotationSPVR = rotationSPVR.inverse;
                    } 
                    else
                    {
                        projection = myCam.projectionMatrix;
                        rotation = myCam.worldToCameraMatrix;
                        inverseRotation = myCam.cameraToWorldMatrix;

                        projectionSPVR = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                        rotationSPVR = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
                        inverseRotationSPVR = myCam.cameraToWorldMatrix;
                    }
                break;

                case Camera.MonoOrStereoscopicEye.Left:
                    projection = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                    rotation = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
                    inverseRotation = rotation.inverse;
                    projectionSPVR = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                    rotationSPVR = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
                    inverseRotationSPVR = rotationSPVR.inverse;

                    break;

                case Camera.MonoOrStereoscopicEye.Right:
                    projection = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                    rotation = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
                    inverseRotation = rotation.inverse;
                    break;
            }

            //projection = renderingData.cameraData.GetGPUProjectionMatrix();
            //rotation = renderingData.cameraData.GetViewMatrix();
            //inverseRotation = rotation.inverse;

            //projectionSPVR = renderingData.cameraData.GetGPUProjectionMatrix();
            //rotationSPVR = renderingData.cameraData.GetViewMatrix();
            //inverseRotationSPVR = rotationSPVR.inverse;

        }
        private void FinalizeFrame()
        {
            renderingCounter++;

            previousRotation = rotation;
            previousRotationSPVR = rotationSPVR;

            int reproSize = reprojectionPixelSize * reprojectionPixelSize;
            subFrameNumber = frameList[renderingCounter % reproSize];
        }
        private bool UpdateFrameDimensions(RenderTextureDescriptor descriptor)
        {
            //Add downsampling
            int newFrameWidth = descriptor.width / EnviroSky.instance.cloudsSettings.cloudsQualitySettings.cloudsRenderResolution;
            int newFrameHeight = descriptor.height / EnviroSky.instance.cloudsSettings.cloudsQualitySettings.cloudsRenderResolution;

            //Reset temporal reprojection size when zero. Needed if SkyManager starts deactivated
            if (EnviroSky.instance != null && reprojectionPixelSize == 0)
                SetReprojectionPixelSize(EnviroSky.instance.cloudsSettings.cloudsQualitySettings.reprojectionPixelSize);

            //Calculate new frame width and height
            while (newFrameWidth % reprojectionPixelSize != 0)
            {
                newFrameWidth++;
            }

            while (newFrameHeight % reprojectionPixelSize != 0)
            {
                newFrameHeight++;
            }

            int newSubFrameWidth = newFrameWidth / reprojectionPixelSize;
            int newSubFrameHeight = newFrameHeight / reprojectionPixelSize;

            //Check if diemensions changed
            if (newFrameWidth != frameWidth || newSubFrameWidth != subFrameWidth || newFrameHeight != frameHeight || newSubFrameHeight != subFrameHeight)
            {
                //Cache new dimensions
                frameWidth = newFrameWidth;
                frameHeight = newFrameHeight;
                subFrameWidth = newSubFrameWidth;
                subFrameHeight = newSubFrameHeight;
                return true;
            }
            else
            {
                //Cache new dimensions
                frameWidth = newFrameWidth;
                frameHeight = newFrameHeight;
                subFrameWidth = newSubFrameWidth;
                subFrameHeight = newSubFrameHeight;
                return false;
            }
        }
        private int[] CalculateFrames(int reproSize)
        {
            subFrameNumber = 0;

            int i = 0;
            int reproCount = reproSize * reproSize;
            int[] frameNumbers = new int[reproCount];

            for (i = 0; i < reproCount; i++)
            {
                frameNumbers[i] = i;
            }

            while (i-- > 0)
            {
                int frame = frameNumbers[i];
                int count = (int)(UnityEngine.Random.Range(0, 1) * 1000.0f) % reproCount;
                frameNumbers[i] = frameNumbers[count];
                frameNumbers[count] = frame;
            }

            return frameNumbers;
        }
#endregion

        void UpdateMatrix(UnityEngine.Rendering.Universal.RenderingData renderingData, Material mat)
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

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

            if (currentReprojectionPixelSize != EnviroSky.instance.cloudsSettings.cloudsQualitySettings.reprojectionPixelSize)
                {
                    currentReprojectionPixelSize = EnviroSky.instance.cloudsSettings.cloudsQualitySettings.reprojectionPixelSize;
                    SetReprojectionPixelSize(EnviroSky.instance.cloudsSettings.cloudsQualitySettings.reprojectionPixelSize);
                }

        if (blitMat == null)
            blitMat = new Material(Shader.Find("Enviro/Standard/Blit"));

            StartFrame(renderingData);

            if (subFrameTex == null || prevFrameTex == null || textureDimensionChanged)
                CreateCloudsRenderTextures(renderingData.cameraData.cameraTargetDescriptor);

            if (cloudsMat == null)
                cloudsMat = new Material(Shader.Find("Enviro/Standard/RaymarchClouds"));

            RenderTargetIdentifier prevFrameId = new RenderTargetIdentifier(prevFrameTex);
            RenderTargetIdentifier subFrameId = new RenderTargetIdentifier(subFrameTex);

            if(EnviroSky.instance != null)
               EnviroSky.instance.RenderCloudMaps();


            SetCloudProperties(renderingData);

            if(myCam != null && EnviroSky.instance.cloudsSettings.depthBlending)
                UpdateMatrix(renderingData, cloudsMat);

            cloudsMat.EnableKeyword("ENVIROURP");
            CustomBlit(cmd,renderer.cameraColorTarget,subFrameId,cloudsMat);

            //Render Clouds with downsampling tex
           
 
            if (isFirstFrame)
            {
                CustomBlit(cmd,subFrameId,prevFrameId);
                isFirstFrame = false;
            }

             //Set blending type:
            if (EnviroSky.instance.cloudsSettings.depthBlending)
                Shader.EnableKeyword("ENVIRO_DEPTHBLENDING");
            else
                Shader.DisableKeyword("ENVIRO_DEPTHBLENDING");

            int downsampling = EnviroSky.instance.cloudsSettings.bilateralUpsampling ? reprojectionPixelSize * EnviroSky.instance.cloudsSettings.cloudsQualitySettings.cloudsRenderResolution : 1;

            if (downsampling > 1)
            {

                if (compose == null)
                    compose = new Material(Shader.Find("Hidden/Enviro/Upsample"));

                if (downsample == null)
                    downsample = new Material(Shader.Find("Hidden/Enviro/DepthDownsample"));


                RenderTexture lowDepth = DownsampleDepth(cmd,renderingData.cameraData.cameraTargetDescriptor, null, downsample, downsampling);

                compose.SetTexture("_CameraDepthLowRes", lowDepth);

                RenderTextureDescriptor upsampleDesc = renderingData.cameraData.cameraTargetDescriptor;
                upsampleDesc.width = myCam.pixelWidth / downsampling * 2;
                upsampleDesc.height = myCam.pixelHeight / downsampling * 2;
                upsampleDesc.depthBufferBits = 0;
                upsampleDesc.graphicsFormat = Experimental.Rendering.GraphicsFormat.R16G16B16A16_SFloat;

                RenderTexture upsampledTex = RenderTexture.GetTemporary(upsampleDesc);
                upsampledTex.filterMode = FilterMode.Bilinear;
      
                RenderTargetIdentifier upsampledId = new RenderTargetIdentifier(upsampledTex);
 
                // composite to screen
                Vector2 pixelSize = new Vector2(1.0f / lowDepth.width, 1.0f / lowDepth.height);
                compose.SetVector("_LowResPixelSize", pixelSize);
                compose.SetVector("_LowResTextureSize", new Vector2(lowDepth.width, lowDepth.height));
                compose.SetFloat("_DepthMult", 32.0f);
                compose.SetFloat("_Threshold", 0.0005f);
                compose.SetTexture("_LowResTexture", subFrameTex);

                compose.EnableKeyword("ENVIROURP"); 
                CustomBlit(cmd,subFrameId,upsampledId,compose);
                RenderTexture.ReleaseTemporary(lowDepth);

                //Blit clouds to final image
                blitMat.SetTexture("_SubFrame", upsampledTex);
                blitMat.SetTexture("_PrevFrame", prevFrameTex);
                SetBlitmaterialProperties(); 

                RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
                opaqueDesc.depthBufferBits = 0;
                cmd.GetTemporaryRT(m_TemporaryColorTexture.id, opaqueDesc, filterMode);

                CustomBlit(cmd,renderer.cameraColorTarget,m_TemporaryColorTexture.Identifier());
                CustomBlit(cmd, m_TemporaryColorTexture.Identifier(), renderer.cameraColorTarget,blitMat);
                cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
                CustomBlit(cmd,upsampledId,prevFrameId);
                RenderTexture.ReleaseTemporary(upsampledTex);
            }
            else
            {
                //Blit clouds to final image
                blitMat.SetTexture("_SubFrame", subFrameTex);
                blitMat.SetTexture("_PrevFrame", prevFrameTex);
                SetBlitmaterialProperties();

                RenderTextureDescriptor opaqueDesc = renderingData.cameraData.cameraTargetDescriptor;
                opaqueDesc.depthBufferBits = 0;

                cmd.GetTemporaryRT(m_TemporaryColorTexture.id, opaqueDesc, filterMode);

                blitMat.EnableKeyword("ENVIROURP"); 
                CustomBlit(cmd,renderer.cameraColorTarget,m_TemporaryColorTexture.Identifier());
                CustomBlit(cmd, m_TemporaryColorTexture.Identifier(), renderer.cameraColorTarget, blitMat);
                cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
                CustomBlit(cmd,subFrameId,prevFrameId);  
            }

            FinalizeFrame();
            cmd.SetRenderTarget(renderer.cameraColorTarget);
            context.ExecuteCommandBuffer(cmd);         
            CommandBufferPool.Release(cmd);
        }

        /// <inheritdoc/>
        public override void FrameCleanup(CommandBuffer cmd)
        {
          //  if (destination == UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget)
          //      cmd.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
        }
    }
}
#endif