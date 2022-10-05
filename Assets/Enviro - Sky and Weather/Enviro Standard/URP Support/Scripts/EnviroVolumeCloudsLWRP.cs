using System.Collections.Generic;
using UnityEngine.Serialization;
#if ENVIRO_LWRP && ENVIRO_HD
namespace UnityEngine.Rendering.LWRP
{
    public class EnviroVolumeCloudsLWRP : UnityEngine.Rendering.Universal.ScriptableRendererFeature
    {

        EnviroBlitPassVolumeClouds blitPass;
        
        private Material cloudsMat;
        private Material blitMat;
        private Material compose;
        private Material downsample;

        private void CreateMaterialsAndTextures()
        {

            if (cloudsMat == null)
                cloudsMat = new Material(Shader.Find("Enviro/Standard/RaymarchClouds"));

            if (blitMat == null)
                blitMat = new Material(Shader.Find("Enviro/Standard/Blit"));

            if (compose == null)
                compose = new Material(Shader.Find("Hidden/Enviro/Upsample"));

            if (downsample == null)
                downsample = new Material(Shader.Find("Hidden/Enviro/DepthDownsample"));
        }


        public override void Create()
        {
          if (EnviroSkyMgr.instance == null || EnviroSky.instance == null)
              return;

            CreateMaterialsAndTextures();
            
            blitPass = new EnviroBlitPassVolumeClouds(UnityEngine.Rendering.Universal.RenderPassEvent.BeforeRenderingTransparents, cloudsMat,blitMat,compose,downsample, name);
        }

        public override void AddRenderPasses(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            if (EnviroSkyMgr.instance != null && EnviroSky.instance != null && EnviroSkyMgr.instance.useVolumeClouds && EnviroSky.instance.PlayerCamera != null)
            { 
                if(EnviroSky.instance.RenderEnviroOnThisCam(renderingData.cameraData.camera) == false)
                   return;  

                if (renderingData.cameraData.camera.cameraType == CameraType.Preview)
                    return;

                //var src = renderer.cameraColorTarget;
                var dest = UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget;

                if (renderingData.cameraData.isSceneViewCamera && !EnviroSky.instance.showVolumeCloudsInEditor)
                    return;

                if (blitPass == null)
                    Create();

                CreateMaterialsAndTextures();
                blitPass.Setup(renderer, dest, renderingData.cameraData.camera);

                renderer.EnqueuePass(blitPass);
            }
        }
    }
}
#endif