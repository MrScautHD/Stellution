#if ENVIRO_HDRP
using System.Collections;
using UnityEngine.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
    class EnviroSkyboxRenderer : SkyRenderer
    {
        Material skyMat; // Renders a cubemap into a render texture (can be cube or 2D)
        MaterialPropertyBlock m_PropertyBlock = new MaterialPropertyBlock();


        public EnviroSkyboxRenderer()
        {
        }

        public override void Build()
        {
            if (EnviroSky.instance == null)
                return;

            if(skyMat == null)
               skyMat = CoreUtils.CreateEngineMaterial(Shader.Find("Enviro/HDRP/Skybox"));

            if (EnviroSky.instance.skySettings.starsCubeMap != null)
                skyMat.SetTexture("_Stars", EnviroSky.instance.skySettings.starsCubeMap);

            if (EnviroSky.instance.skySettings.galaxyCubeMap != null)
                skyMat.SetTexture("_Galaxy", EnviroSky.instance.skySettings.galaxyCubeMap);

            if (EnviroSky.instance.ressources.aurora_layer_1 != null)
                skyMat.SetTexture("_Aurora_Layer_1", EnviroSky.instance.ressources.aurora_layer_1);

            if (EnviroSky.instance.ressources.aurora_layer_2 != null)
                skyMat.SetTexture("_Aurora_Layer_2", EnviroSky.instance.ressources.aurora_layer_2);

            if (EnviroSky.instance.ressources.aurora_colorshift != null)
                skyMat.SetTexture("_Aurora_Colorshift", EnviroSky.instance.ressources.aurora_colorshift);
        }

        public override void Cleanup()
        {
            CoreUtils.Destroy(skyMat);
        }

        public override void RenderSky(BuiltinSkyParameters builtinParams, bool renderForCubemap, bool renderSunDisk)
        {
            if (EnviroSky.instance == null)
                return;

            if (skyMat == null)
                Build();

            EnviroSky.instance.UpdateSkyShaderVariables(skyMat);

            var enviroSky = builtinParams.skySettings as EnviroSkybox;
            Shader.SetGlobalMatrix("_PixelCoordToViewDirWS", builtinParams.pixelCoordToViewDirMatrix);
            Shader.SetGlobalFloat("_EnviroSkyIntensity", GetSkyIntensity(enviroSky, builtinParams.debugSettings));

            m_PropertyBlock.SetMatrix("_PixelCoordToViewDirWS", builtinParams.pixelCoordToViewDirMatrix);

            if (builtinParams.hdCamera.camera.cameraType != CameraType.Reflection)
               EnviroSky.instance.skyMat = skyMat;

            if (EnviroSky.instance.skySettings.starsCubeMap != null)
                skyMat.SetTexture("_Stars", EnviroSky.instance.skySettings.starsCubeMap);
            if (EnviroSky.instance.skySettings.galaxyCubeMap != null)
                skyMat.SetTexture("_Galaxy", EnviroSky.instance.skySettings.galaxyCubeMap);

            CoreUtils.DrawFullScreen(builtinParams.commandBuffer, skyMat, m_PropertyBlock, renderForCubemap ? 0 : 1);

            if (!renderForCubemap)
               CoreUtils.DrawFullScreen(builtinParams.commandBuffer, skyMat, m_PropertyBlock, 2);  

            if(EnviroSky.instance.useFlatClouds && !renderForCubemap)
                CoreUtils.DrawFullScreen(builtinParams.commandBuffer, skyMat, m_PropertyBlock, 3);  

            if (EnviroSky.instance.useAurora && !renderForCubemap)
            {
                if (EnviroSky.instance.ressources.aurora_layer_1 != null)
                    skyMat.SetTexture("_Aurora_Layer_1", EnviroSky.instance.ressources.aurora_layer_1);
                if (EnviroSky.instance.ressources.aurora_layer_2 != null)
                    skyMat.SetTexture("_Aurora_Layer_2", EnviroSky.instance.ressources.aurora_layer_2);
                if (EnviroSky.instance.ressources.aurora_colorshift != null)
                    skyMat.SetTexture("_Aurora_Colorshift", EnviroSky.instance.ressources.aurora_colorshift);

                CoreUtils.DrawFullScreen(builtinParams.commandBuffer, skyMat, m_PropertyBlock, 4);
            }
        }
    }
}
#endif
