#if ENVIRO_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace UnityEngine.Rendering.HighDefinition
{ 

    [Serializable, VolumeComponentMenu("Post-processing/Enviro/Distance Blur")]
 
    public class EnviroHDRPDistanceBlur : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public bool IsActive() => EnviroSky.instance != null;
        public override CustomPostProcessInjectionPoint injectionPoint => (CustomPostProcessInjectionPoint)0;

        #region General Var
        private Camera myCam;
        private Material blitTrough;
         #endregion
        ////////////////////////
        #region Blur Var
        /////////////////// Blur //////////////////////
        private Material postProcessMat;
        private const int kMaxIterations = 16;
        private RenderTexture[] _blurBuffer1 = new RenderTexture[kMaxIterations];
        private RenderTexture[] _blurBuffer2 = new RenderTexture[kMaxIterations];
        // private float _threshold = 0f;
        public float thresholdGamma
        {
            get { return Mathf.Max(0f, 0); }
            // set { _threshold = value; }
        }
        public float thresholdLinear
        {
            get { return Mathf.GammaToLinearSpace(thresholdGamma); }
            //   set { _threshold = Mathf.LinearToGammaSpace(value); }
        }
        #endregion
        ////////////////////////

        private void CleanupMaterials()
        {
            if (postProcessMat != null)
                CoreUtils.Destroy(postProcessMat);

            if (blitTrough != null)
                CoreUtils.Destroy(blitTrough);
        }

        private void CreateMaterialsAndTextures()
        {
            if (postProcessMat != null)
                postProcessMat = new Material(Shader.Find("Hidden/EnviroDistanceBlurHDRP"));

            if(blitTrough == null)
               blitTrough = new Material(Shader.Find("Hidden/Enviro/BlitTroughHDRP"));
        }

        private void SetMatrixes ()
        {
            if (myCam.stereoEnabled)
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

                Shader.SetGlobalMatrix("_LeftWorldFromView", left_world_from_view);
                Shader.SetGlobalMatrix("_RightWorldFromView", right_world_from_view);
                Shader.SetGlobalMatrix("_LeftViewFromScreen", left_view_from_screen);
                Shader.SetGlobalMatrix("_RightViewFromScreen", right_view_from_screen);
            }
            else
            {
                // Main eye inverse view matrix
                Matrix4x4 left_world_from_view = myCam.cameraToWorldMatrix;

                // Inverse projection matrices, plumbed through GetGPUProjectionMatrix to compensate for render texture
                Matrix4x4 screen_from_view = myCam.projectionMatrix;
                Matrix4x4 left_view_from_screen = GL.GetGPUProjectionMatrix(screen_from_view, true).inverse;

                // Negate [1,1] to reflect Unity's CBuffer state
                if (SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3)
                    left_view_from_screen[1, 1] *= -1;

                // Store matrices
                Shader.SetGlobalMatrix("_LeftWorldFromView", left_world_from_view);
                Shader.SetGlobalMatrix("_LeftViewFromScreen", left_view_from_screen);
            }
        }

        public override void Setup()
        {
            CreateMaterialsAndTextures();      
        }

        public override void Cleanup()
        {
            CleanupMaterials();
        }

        public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
        {
            myCam = camera.camera;
            
            if (EnviroSky.instance == null || myCam == null || camera.camera.cameraType == CameraType.SceneView || camera.camera.cameraType == CameraType.Preview || camera.camera.cameraType == CameraType.Reflection || EnviroSky.instance.RenderEnviroOnThisCam(camera.camera) == false)
            {
                blitTrough.SetTexture("_InputTexture", source);
                CoreUtils.DrawFullScreen(cmd, blitTrough);
                return;
            } 


            if (EnviroSkyMgr.instance.useDistanceBlur)
            {
                SetMatrixes();
                RenderDistanceBlur(source, destination, cmd);
            }
            else
            {
                blitTrough.SetTexture("_InputTexture", source);
                CoreUtils.DrawFullScreen(cmd, blitTrough);
            }

        }

        private void RenderDistanceBlur(RTHandle source, RTHandle destination, CommandBuffer cmd)
        {
            var useRGBM = myCam.allowHDR;

            // source texture size
            var tw = source.rt.width;
            var th = source.rt.height;

            // halve the texture size for the low quality mode
            if (!EnviroSky.instance.distanceBlurSettings.highQuality)
            {
                tw /= 2;
                th /= 2;
            }

            if (postProcessMat == null)
                postProcessMat = new Material(Shader.Find("Hidden/EnviroDistanceBlurHDRP"));

            postProcessMat.SetTexture("_DistTex", EnviroSky.instance.ressources.distributionTexture);
            postProcessMat.SetFloat("_Distance", EnviroSky.instance.blurDistance);
            postProcessMat.SetFloat("_Radius", EnviroSky.instance.distanceBlurSettings.radius);

            // blur buffer format
            var rtFormat = useRGBM ?
                RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;
 
            // determine the iteration count
            var logh = Mathf.Log(th, 2) + EnviroSky.instance.distanceBlurSettings.radius - 8;
            var logh_i = (int)logh;
            var iterations = Mathf.Clamp(logh_i, 1, kMaxIterations);

            // update the shader properties
            var lthresh = thresholdLinear;
            postProcessMat.SetFloat("_Threshold", lthresh);

            var knee = lthresh * 0.5f + 1e-5f;
            var curve = new Vector3(lthresh - knee, knee * 2, 0.25f / knee);
            postProcessMat.SetVector("_Curve", curve);

            var pfo = !EnviroSky.instance.distanceBlurSettings.highQuality && EnviroSky.instance.distanceBlurSettings.antiFlicker;
            postProcessMat.SetFloat("_PrefilterOffs", pfo ? -0.5f : 0.0f);
             
            postProcessMat.SetFloat("_SampleScale", 0.5f + logh - logh_i);
            postProcessMat.SetFloat("_Intensity", EnviroSky.instance.blurIntensity);
            postProcessMat.SetFloat("_SkyBlurring", EnviroSky.instance.blurSkyIntensity);

            // prefilter pass
            var prefiltered = RenderTexture.GetTemporary(destination.rt.descriptor);
            var pass = EnviroSky.instance.distanceBlurSettings.antiFlicker ? 1 : 0;
            postProcessMat.SetTexture("_MainTex", source);
            postProcessMat.SetVector("_ScaledSize", source.rtHandleProperties.rtHandleScale);
            cmd.Blit(source, prefiltered, postProcessMat, pass);

            // construct a mip pyramid
            var last = prefiltered;
            for (var level = 0; level < iterations; level++)
            {
                RenderTextureDescriptor lastd = last.descriptor;
                lastd.width = (int)(lastd.width * 0.5);
                lastd.height = (int)(lastd.height * 0.5);
                _blurBuffer1[level] = RenderTexture.GetTemporary(lastd);
                pass = (level == 0) ? (EnviroSky.instance.distanceBlurSettings.antiFlicker ? 3 : 2) : 4;
                postProcessMat.SetTexture("_MainTex", last);
                postProcessMat.SetVector("_MainTex_TexelSize", new Vector4(1f / last.width, 1f / last.height, last.width, last.height));
                cmd.Blit(last, _blurBuffer1[level], postProcessMat, pass);
                last = _blurBuffer1[level];
            }

            // upsample and combine loop
            for (var level = iterations - 2; level >= 0; level--)
            { 
                var basetex = _blurBuffer1[level];
                postProcessMat.SetTexture("_BaseTex", basetex);
                postProcessMat.SetVector("_BaseTex_TexelSize", new Vector4(1f / basetex.width, 1f / basetex.height, basetex.width, basetex.height));
                _blurBuffer2[level] = RenderTexture.GetTemporary(basetex.descriptor);
                pass = EnviroSky.instance.distanceBlurSettings.highQuality ? 6 : 5;
                postProcessMat.SetTexture("_MainTex", last);
                postProcessMat.SetVector("_MainTex_TexelSize", new Vector4(1f / last.width, 1f / last.height, last.width, last.height));
                postProcessMat.SetVector("_TexelSize", new Vector4(last.width, last.height, 1f / last.width, 1f / last.height));
                cmd.Blit(last, _blurBuffer2[level], postProcessMat, pass);
                last = _blurBuffer2[level]; 
            }

            // finish process
            pass = EnviroSky.instance.distanceBlurSettings.highQuality ? 8 : 7;
            postProcessMat.SetTexture("_BaseTex", source);        
            postProcessMat.SetTexture("_MainTex", last);
            postProcessMat.SetVector("_MainTex_TexelSize", new Vector4(1 / last.width, 1 / last.height, last.width, last.height));
            postProcessMat.SetVector("_TexelSize", new Vector4(last.width, last.height, 1f / last.width, 1f / last.height));
            cmd.SetRenderTarget(destination);
            CoreUtils.DrawFullScreen(cmd, postProcessMat,null,pass);
            
            //cmd.SetGlobalTexture("_BaseTex", source);
            //cmd.SetGlobalTexture("_MainTex", last);
            //cmd.SetGlobalVector("_MainTex_TexelSize", new Vector4(1 / last.width, 1 / last.height, last.width, last.height));
            //cmd.SetGlobalVector("_TexelSize", new Vector4(last.width, last.height, 1f / last.width, 1f / last.height));
            //cmd.SetRenderTarget(destination);
            //cmd.Blit(last,destination,postProcessMat,pass);           
           // HDUtils.DrawFullScreen(cmd,postProcessMat,destination,null,pass);
            
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
        }
    }
}
#endif