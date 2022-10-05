using System.Collections.Generic;
using UnityEngine.Serialization;
#if ENVIRO_LWRP && ENVIRO_LW
namespace UnityEngine.Rendering.LWRP
{
    public class EnviroLiteFogLWRP : UnityEngine.Rendering.Universal.ScriptableRendererFeature
    {

        EnviroBlitPass blitPass;
        private Camera myCam;
        [HideInInspector]
        public Material material;
        private bool currentSimpleFog = false;


        private void RenderFog()
        {
            if (material == null)
                CreateFogMaterial();

            float FdotC = myCam.transform.position.y - EnviroSkyLite.instance.fogSettings.height;
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
            Shader.SetGlobalVector("_SceneFogParams", sceneParams);
            Shader.SetGlobalVector("_SceneFogMode", new Vector4((int)sceneMode, EnviroSkyLite.instance.fogSettings.useRadialDistance ? 1 : 0, 0, 0));
            Shader.SetGlobalVector("_HeightParams", new Vector4(EnviroSkyLite.instance.fogSettings.height, FdotC, paramK, EnviroSkyLite.instance.fogSettings.heightDensity * 0.5f));
            Shader.SetGlobalVector("_DistanceParams", new Vector4(-Mathf.Max(EnviroSkyLite.instance.fogSettings.startDistance, 0.0f), 0, 0, 0));
            material.EnableKeyword("ENVIROURP"); 
        }

    private void CreateFogMaterial ()
    {
        if (material != null)
            DestroyImmediate(material);

        if (!EnviroSkyLite.instance.fogSettings.useSimpleFog)
        {
            Shader shader = Shader.Find("Enviro/Lite/EnviroFogRendering");
            if (shader != null)
                material = new Material(shader);
        }
        else
        {
            Shader shader = Shader.Find("Enviro/Lite/EnviroFogRenderingSimple");
              if (shader != null)
                  material = new Material(shader);
            }

        if (EnviroSkyLite.instance.fogSettings.useSimpleFog)
        {
            currentSimpleFog = true;
            Shader.EnableKeyword("ENVIRO_SIMPLE_FOG");
        }
        else
        {
            currentSimpleFog = false;
            Shader.DisableKeyword("ENVIRO_SIMPLE_FOG");
        }
    }

        private void UpdateMatrix(UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            ///////////////////Matrix Information
            if (UnityEngine.XR.XRSettings.enabled && UnityEngine.XR.XRSettings.stereoRenderingMode == XR.XRSettings.StereoRenderingMode.SinglePassInstanced)
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
          if (EnviroSkyMgr.instance == null || EnviroSkyLite.instance == null)
                return;

            CreateFogMaterial();

            blitPass = new EnviroBlitPass(UnityEngine.Rendering.Universal.RenderPassEvent.BeforeRenderingTransparents, material, 0, name);
        }

        public override void AddRenderPasses(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.cameraType == CameraType.Preview)
                return;

            myCam = renderingData.cameraData.camera;
 
            if (EnviroSkyLite.instance != null && EnviroSkyLite.instance.usePostEffectFog)
            {
                //var src = renderer.cameraColorTarget;
                var dest = UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget;

                if (renderingData.cameraData.isSceneViewCamera || renderingData.cameraData.camera == EnviroSkyLite.instance.PlayerCamera)
                {
                    UpdateMatrix(renderingData);

                    if (currentSimpleFog != EnviroSkyLite.instance.fogSettings.useSimpleFog)
                    {
                        CreateFogMaterial();
                        blitPass = new EnviroBlitPass(UnityEngine.Rendering.Universal.RenderPassEvent.BeforeRenderingTransparents, material, 0, name);
                        currentSimpleFog = EnviroSkyLite.instance.fogSettings.useSimpleFog;
                    }

                    RenderFog();

                    if (blitPass == null || material == null)
                        Create();

                    blitPass.Setup(renderer, dest, material);

                    renderer.EnqueuePass(blitPass);
                }
            }
        }
    }
}
#endif