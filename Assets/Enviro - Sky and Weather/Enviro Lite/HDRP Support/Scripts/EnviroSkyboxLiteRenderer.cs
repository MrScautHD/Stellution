#if ENVIRO_HDRP
using System.Collections;
using UnityEngine.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
    class EnviroSkyboxLiteRenderer : SkyRenderer
    {
        Material skyMat; // Renders a cubemap into a render texture (can be cube or 2D)
        MaterialPropertyBlock m_PropertyBlock = new MaterialPropertyBlock();


        public EnviroSkyboxLiteRenderer()
        {
        }

        public override void Build()
        {
            if (EnviroSkyLite.instance == null)
                return;

            if(skyMat == null)
               skyMat = CoreUtils.CreateEngineMaterial(Shader.Find("Enviro/HDRP/Skybox Lite"));

            if (EnviroSkyLite.instance.skySettings.starsCubeMap != null)
                skyMat.SetTexture("_Stars", EnviroSkyLite.instance.skySettings.starsCubeMap);
        } 

        public override void Cleanup()
        {
            CoreUtils.Destroy(skyMat);
        }

        public override void RenderSky(BuiltinSkyParameters builtinParams, bool renderForCubemap, bool renderSunDisk)
        {
            if (EnviroSkyLite.instance == null)
                return;

            if (skyMat == null)
                Build();

            EnviroSkyLite.instance.UpdateSkyShaderVariables(skyMat);

            var enviroSky = builtinParams.skySettings as EnviroSkyboxLite;
            Shader.SetGlobalMatrix("_PixelCoordToViewDirWS", builtinParams.pixelCoordToViewDirMatrix);
            Shader.SetGlobalFloat("_EnviroSkyIntensity", GetSkyIntensity(enviroSky, builtinParams.debugSettings));

            m_PropertyBlock.SetMatrix("_PixelCoordToViewDirWS", builtinParams.pixelCoordToViewDirMatrix);

            if (builtinParams.hdCamera.camera.cameraType != CameraType.Reflection)
                EnviroSkyLite.instance.skyMat = skyMat;

            if (EnviroSkyLite.instance.skySettings.starsCubeMap != null)
                skyMat.SetTexture("_Stars", EnviroSkyLite.instance.skySettings.starsCubeMap);

            CoreUtils.DrawFullScreen(builtinParams.commandBuffer, skyMat, m_PropertyBlock, renderForCubemap ? 0 : 1);


            if(!renderForCubemap)
            {
                CoreUtils.DrawFullScreen(builtinParams.commandBuffer, skyMat, m_PropertyBlock, 2);  
                CoreUtils.DrawFullScreen(builtinParams.commandBuffer, skyMat, m_PropertyBlock, 4); 
            }
            else
            {
                CoreUtils.DrawFullScreen(builtinParams.commandBuffer, skyMat, m_PropertyBlock, 3);
                CoreUtils.DrawFullScreen(builtinParams.commandBuffer, skyMat, m_PropertyBlock, 5);
            }
        }
    }
}
#endif
