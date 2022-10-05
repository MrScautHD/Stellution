using System.Collections.Generic;
using UnityEngine.Serialization;
#if ENVIRO_LWRP && ENVIRO_HD
namespace UnityEngine.Rendering.LWRP
{
    public class EnviroFogLWRP : UnityEngine.Rendering.Universal.ScriptableRendererFeature
    {

        EnviroBlitPass blitPass;

        private Camera myCam;

        #region Fog Var
        public enum FogType
        {
            Disabled,
            Simple,
            Standard
        }

        [HideInInspector]
        public FogType currentFogType;
        private Material fogMat;
        private Texture2D dither;
        private Texture2D blackTexture;
        private Texture3D detailNoiseTexture = null;
#endregion

        private void RenderFog()
        {

            if (fogMat == null)
                CreateFogMaterial();

            //////////////// FOG
            float FdotC = myCam.transform.position.y - EnviroSky.instance.fogSettings.height;
            float paramK = (FdotC <= 0.0f ? 1.0f : 0.0f);
            var sceneMode = RenderSettings.fogMode;
            var sceneDensity = RenderSettings.fogDensity;
            var sceneStart = RenderSettings.fogStartDistance;
            var sceneEnd = RenderSettings.fogEndDistance;
            Vector4 sceneParams;
            bool linear = (sceneMode == FogMode.Linear);
            float diff = linear ? sceneEnd - sceneStart : 0.0f;
            float invDiff = Mathf.Abs(diff) > 0.0001f ? 1.0f / diff : 0.0f;
            sceneParams.x = sceneDensity * 1.2011224087f; // density / sqrt(ln(2)), used by Exp2 fog mode
            sceneParams.y = sceneDensity * 1.4426950408f; // density / ln(2), used by Exp fog mode
            sceneParams.z = linear ? -invDiff : 0.0f;
            sceneParams.w = linear ? sceneEnd * invDiff : 0.0f;
            //////////////////

            if (!EnviroSky.instance.fogSettings.useSimpleFog)
            {
                Shader.SetGlobalVector("_FogNoiseVelocity", new Vector4(-EnviroSky.instance.Components.windZone.transform.forward.x * EnviroSky.instance.windIntensity * 5, -EnviroSky.instance.Components.windZone.transform.forward.z * EnviroSky.instance.windIntensity * 5) * EnviroSky.instance.fogSettings.noiseScale);
                Shader.SetGlobalVector("_FogNoiseData", new Vector4(EnviroSky.instance.fogSettings.noiseScale, EnviroSky.instance.fogSettings.noiseIntensity, EnviroSky.instance.fogSettings.noiseIntensityOffset));
                Shader.SetGlobalTexture("_FogNoiseTexture", detailNoiseTexture);
            }

            Shader.SetGlobalFloat("_EnviroVolumeDensity", EnviroSky.instance.globalVolumeLightIntensity);
            Shader.SetGlobalVector("_SceneFogParams", sceneParams);
            Shader.SetGlobalVector("_SceneFogMode", new Vector4((int)sceneMode, EnviroSky.instance.fogSettings.useRadialDistance ? 1 : 0, 0, Application.isPlaying ? 1f : 0f));
            Shader.SetGlobalVector("_HeightParams", new Vector4(EnviroSky.instance.fogSettings.height, FdotC, paramK, EnviroSky.instance.fogSettings.heightDensity * 0.5f));
            Shader.SetGlobalVector("_DistanceParams", new Vector4(-Mathf.Max(EnviroSky.instance.fogSettings.startDistance, 0.0f), 0, 0, 0));
            fogMat.SetFloat("_DitheringIntensity", EnviroSky.instance.fogSettings.fogDithering);
            Shader.SetGlobalTexture("_EnviroVolumeLightingTex", blackTexture);
            fogMat.EnableKeyword("ENVIROURP");
        }

       
        private void CreateFogMaterial()
        {
            if (EnviroSky.instance == null)
                return;

            if (dither == null)
                dither = Resources.Load("tex_enviro_dither") as Texture2D;

            if (detailNoiseTexture == null)
                detailNoiseTexture = Resources.Load("enviro_clouds_detail_low") as Texture3D;

            if (blackTexture == null)
                blackTexture = Resources.Load("tex_enviro_black") as Texture2D;

            //Cleanup
            if (fogMat != null)
                DestroyImmediate(fogMat);

            if (!EnviroSky.instance.useFog)
            {
                Shader shader = Shader.Find("Enviro/Standard/EnviroFogRenderingDisabled");
                if (shader != null)
                {
                fogMat = new Material(shader);     
                currentFogType = FogType.Disabled;
                }
            }
            else if (!EnviroSky.instance.fogSettings.useSimpleFog)
            {
                Shader shader = Shader.Find("Enviro/Standard/EnviroFogRendering");
                if (shader != null)
                {
                fogMat = new Material(shader);
                currentFogType = FogType.Standard;
                }
            }
            else
            {
                Shader shader = Shader.Find("Enviro/Standard/EnviroFogRenderingSimple");
                if (shader != null)
                {
                fogMat = new Material(shader);
                currentFogType = FogType.Simple;
                }
            }
        }
        private void UpdateMatrix(UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            ///////////////////Matrix Information
            if (UnityEngine.XR.XRSettings.enabled && EnviroSky.instance.singlePassInstancedVR)
            {
        #if UNITY_2020_3_OR_NEWER
                Matrix4x4 left_world_from_view = renderingData.cameraData.GetViewMatrix(0).inverse;
                Matrix4x4 right_world_from_view = renderingData.cameraData.GetViewMatrix(1).inverse;
                Matrix4x4 left_screen_from_view = renderingData.cameraData.GetProjectionMatrix(0);
                Matrix4x4 right_screen_from_view = renderingData.cameraData.GetProjectionMatrix(1);
        #else
                Matrix4x4 left_world_from_view = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Left).inverse;
                Matrix4x4 right_world_from_view = myCam.GetStereoViewMatrix(Camera.StereoscopicEye.Right).inverse;
                Matrix4x4 left_screen_from_view = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                Matrix4x4 right_screen_from_view = myCam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
        #endif       

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
                Matrix4x4 left_world_from_view = renderingData.cameraData.GetViewMatrix().inverse;

                // Inverse projection matrices, plumbed through GetGPUProjectionMatrix to compensate for render texture
                Matrix4x4 screen_from_view = renderingData.cameraData.GetProjectionMatrix();
                Matrix4x4 left_view_from_screen = GL.GetGPUProjectionMatrix(screen_from_view, true).inverse;

                // Negate [1,1] to reflect Unity's CBuffer state
                if (SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.OpenGLCore && SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.OpenGLES3)
                    left_view_from_screen[1, 1] *= -1;
 
                // Store matrices
                Shader.SetGlobalMatrix("_LeftWorldFromView", left_world_from_view);
                Shader.SetGlobalMatrix("_LeftViewFromScreen", left_view_from_screen);
            }
        }

        public override void Create()
        {
          if (EnviroSkyMgr.instance == null || EnviroSky.instance == null)
                return;

            CreateFogMaterial();

            blitPass = new EnviroBlitPass(UnityEngine.Rendering.Universal.RenderPassEvent.BeforeRenderingTransparents, fogMat, 0, name);
        }


        public override void AddRenderPasses(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.cameraType == CameraType.Preview)
                return;
 
            myCam = renderingData.cameraData.camera;

            if (EnviroSky.instance != null && EnviroSky.instance.useFog && EnviroSky.instance.PlayerCamera != null)
            {           
                if(EnviroSky.instance.RenderEnviroOnThisCam(renderingData.cameraData.camera) == false)
                    return;

                //var src = renderer.cameraColorTarget;
                var dest = UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget;

                if (renderingData.cameraData.isSceneViewCamera && !EnviroSky.instance.showFogInEditor)
                    return;

                    UpdateMatrix(renderingData);
 
                    RenderFog();

                if (blitPass == null|| fogMat == null)
                    Create();

                    blitPass.Setup(renderer, dest, fogMat);

                    renderer.EnqueuePass(blitPass);
            }
        }
    }
}
#endif