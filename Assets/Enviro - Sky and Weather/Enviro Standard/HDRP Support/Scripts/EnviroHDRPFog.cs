#if ENVIRO_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace UnityEngine.Rendering.HighDefinition
{

    [Serializable, VolumeComponentMenu("Post-processing/Enviro/Fog")]
 
    public class EnviroHDRPFog : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public bool IsActive() => EnviroSky.instance != null;
        public override CustomPostProcessInjectionPoint injectionPoint => (CustomPostProcessInjectionPoint)0;

        #region General Var
        private Camera myCam;
        private Material blitTrough;
#endregion
        ////////////////////////
        #region Fog Var
        public enum FogType
        {
            Disabled,
            Simple,
            Standard
        }
        [HideInInspector]
        public FogType currentFogType;
        [HideInInspector]
        public Material fogMat;
#endregion
        ////////////////////////

        private void CleanupMaterials()
        {
            if (fogMat != null)
                CoreUtils.Destroy(fogMat);

            if (blitTrough != null)
                CoreUtils.Destroy(blitTrough);
        }

        private void CreateFogMaterial()
        {
            if (EnviroSky.instance == null)
                return;

            //Cleanup
            if (fogMat != null)
                CoreUtils.Destroy(fogMat);

            if (!EnviroSky.instance.useFog)
            {
                Shader shader = Shader.Find("Enviro/Pro/EnviroFogHDRP");
                if(shader != null)
                fogMat = new Material(shader);

                currentFogType = FogType.Disabled;
            }
            else if (!EnviroSky.instance.fogSettings.useSimpleFog)
            {
                Shader shader = Shader.Find("Enviro/Pro/EnviroFogHDRP");

                    if (shader != null)
                        fogMat = new Material(shader);

                currentFogType = FogType.Standard;
            }
            else
            {
                Shader shader = Shader.Find("Enviro/Pro/EnviroFogHDRP");

                    if (shader != null)
                        fogMat = new Material(shader);

                currentFogType = FogType.Simple;
            }
        }

        private void CreateMaterialsAndTextures()
        {
            if (blitTrough == null)
                blitTrough = new Material(Shader.Find("Hidden/Enviro/BlitTroughHDRP"));

            if(fogMat == null)
               fogMat = new Material(Shader.Find("Enviro/Pro/EnviroFogHDRP"));
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

            if (EnviroSky.instance == null || myCam == null || camera.camera.cameraType == CameraType.Preview || EnviroSky.instance.RenderEnviroOnThisCam(camera.camera) == false)
            {
                blitTrough.SetTexture("_InputTexture", source);
                CoreUtils.DrawFullScreen(cmd, blitTrough);
                return;
            }

            if (Application.isPlaying)
            {
                if (EnviroSky.instance.useFog)
                {
                    Update();
                    SetMatrixes();
                    RenderFog(source, cmd);
                }
                else
                {
                    blitTrough.SetTexture("_InputTexture", source);
                    CoreUtils.DrawFullScreen(cmd, blitTrough);
                }
            }
            else
            {
                if (camera.camera == Camera.main)
                {
                    if (EnviroSky.instance.useFog)
                    {
                        Update();
                        SetMatrixes();
                        RenderFog(source, cmd);
                    }
                    else
                    {
                        blitTrough.SetTexture("_InputTexture", source);
                        CoreUtils.DrawFullScreen(cmd, blitTrough);
                    }

                }
                else if (EnviroSky.instance.useFog && EnviroSky.instance.showFogInEditor)
                {
                    Update();
                    SetMatrixes();
                    RenderFog(source, cmd);
                }
                else
                {
                    blitTrough.SetTexture("_InputTexture", source);
                    CoreUtils.DrawFullScreen(cmd, blitTrough);
                }
            }
        }
 
        private void Update()
        {
                CreateMaterialsAndTextures();       
        }

        private void RenderFog(RTHandle src, CommandBuffer cmd)
        {
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
                Shader.SetGlobalTexture("_FogNoiseTexture", EnviroSky.instance.ressources.detailNoiseTexture);
            }

            Shader.SetGlobalFloat("_EnviroVolumeDensity", EnviroSky.instance.globalVolumeLightIntensity);
            Shader.SetGlobalVector("_SceneFogParams", sceneParams);
            Shader.SetGlobalVector("_SceneFogMode", new Vector4((int)sceneMode, EnviroSky.instance.fogSettings.useRadialDistance ? 1 : 0, 0, Application.isPlaying ? 1f : 0f));
            Shader.SetGlobalVector("_HeightParams", new Vector4(EnviroSky.instance.fogSettings.height, FdotC, paramK, EnviroSky.instance.fogSettings.heightDensity * 0.5f));
            Shader.SetGlobalVector("_DistanceParams", new Vector4(-Mathf.Max(EnviroSky.instance.fogSettings.startDistance, 0.0f), 0, 0, 0));
            fogMat.SetFloat("_DitheringIntensity", EnviroSky.instance.fogSettings.fogDithering);
            fogMat.SetTexture("_MainTex", src);
            CoreUtils.DrawFullScreen(cmd, fogMat, null, EnviroSky.instance.fogSettings.useEnviroGroundFog ? 0 : 1);
            //cmd.SetGlobalTexture("_MainTex",src);
            //HDUtils.DrawFullScreen(cmd,fogMat,src,null,EnviroSky.instance.fogSettings.useEnviroGroundFog ? 0 : 1);
        }
    }
}
#endif