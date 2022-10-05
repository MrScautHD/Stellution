using System.Collections.Generic;
using UnityEngine.Serialization;
#if ENVIRO_LWRP
namespace UnityEngine.Rendering.LWRP
{
    public class EnviroSunShaftsLWRP : UnityEngine.Rendering.Universal.ScriptableRendererFeature
    {
        EnviroBlitPassShafts blitPass;
        private Camera myCam;

#region Shafts Var
        /// LightShafts
        public enum ShaftsScreenBlendMode
        {
            Screen = 0,
            Add = 1,
        }

        [HideInInspector]
        public int radialBlurIterations = 2;
        private Material shaftsMaterial;
        private Material clearMaterial;
#endregion

        void CreateMaterialsAndTextures()
        {
             if(shaftsMaterial == null)
            shaftsMaterial = new Material(Shader.Find("Enviro/Effects/LightShafts"));
            if(clearMaterial == null)
            clearMaterial = new Material(Shader.Find("Enviro/Effects/ClearLightShafts"));
        }

        void CleanupMaterials()
        {
            if (shaftsMaterial != null)
                DestroyImmediate(shaftsMaterial);
            if (clearMaterial != null)
                DestroyImmediate(clearMaterial);
        }
   
        public override void Create()
        {
           if (EnviroSkyMgr.instance == null || !EnviroSkyMgr.instance.HasInstance())
                return;

            CreateMaterialsAndTextures();

            blitPass = new EnviroBlitPassShafts(UnityEngine.Rendering.Universal.RenderPassEvent.AfterRenderingTransparents, shaftsMaterial, clearMaterial);
        }

        public override void AddRenderPasses(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
        {
            if (renderingData.cameraData.camera.cameraType == CameraType.Preview || renderingData.cameraData.camera.cameraType == CameraType.SceneView || renderingData.cameraData.camera.cameraType == CameraType.Reflection)
                return;

            myCam = renderingData.cameraData.camera;

            if (EnviroSkyMgr.instance != null && EnviroSkyMgr.instance.HasInstance() && EnviroSkyMgr.instance.useSunShafts)
            {

                if (renderingData.cameraData.isSceneViewCamera)
                    return;


               // var src = renderer.cameraColorTarget;
                var dest = UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget;

                if(blitPass == null)
                    Create(); 

                CreateMaterialsAndTextures();
                blitPass.Setup(myCam, renderer, dest, EnviroSkyMgr.instance.Components.Sun.transform, EnviroSkyMgr.instance.LightShaftsSettings.thresholdColorSun.Evaluate(EnviroSkyMgr.instance.Time.solarTime), EnviroSkyMgr.instance.LightShaftsSettings.lightShaftsColorSun.Evaluate(EnviroSkyMgr.instance.Time.solarTime),true);
                renderer.EnqueuePass(blitPass);

            }
        }
    }
}
#endif