#if ENVIRO_HDRP
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using System;

namespace UnityEngine.Rendering.HighDefinition
{

    [Serializable, VolumeComponentMenu("Post-processing/Enviro/Moon Shafts")]
 
    public class EnviroHDRPMoonShafts : CustomPostProcessVolumeComponent, IPostProcessComponent
    {
        public bool IsActive() => EnviroSkyMgr.instance != null;
        public override CustomPostProcessInjectionPoint injectionPoint => (CustomPostProcessInjectionPoint)1;

#region General Var
        private Camera myCam;
        private Material blitTrough;
#endregion

        [HideInInspector]
        public int radialBlurIterations = 2;
        private Material moonShaftsMaterial;
        private Material simpleMoonClearMaterial;

        private RenderTexture lrDepthBuffer;
        private RenderTexture lrColorB;

        private void CleanupMaterials()
        {
            if (moonShaftsMaterial != null)
                CoreUtils.Destroy(moonShaftsMaterial);
            if (simpleMoonClearMaterial != null)
                CoreUtils.Destroy(simpleMoonClearMaterial);
            if (blitTrough != null)
                CoreUtils.Destroy(blitTrough);
           // if (lrDepthBuffer != null)
           //     CoreUtils.Destroy(lrDepthBuffer);
           // if (lrColorB != null)
           //     CoreUtils.Destroy(lrColorB);
        }

        private void CreateMaterialsAndTextures()
        {
            if (moonShaftsMaterial == null)
                moonShaftsMaterial = new Material(Shader.Find("Enviro/Pro/LightShafts"));
            if (simpleMoonClearMaterial == null)
                simpleMoonClearMaterial = new Material(Shader.Find("Enviro/Effects/ClearLightShafts"));
            if (blitTrough == null)
                blitTrough = new Material(Shader.Find("Hidden/Enviro/BlitTroughHDRP"));
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

            if (EnviroSkyMgr.instance == null || !EnviroSkyMgr.instance.HasInstance() || camera.camera.cameraType == CameraType.SceneView || camera.camera.cameraType == CameraType.Reflection || camera.camera.cameraType == CameraType.Preview)
            {
                blitTrough.SetTexture("_InputTexture", source);
                CoreUtils.DrawFullScreen(cmd, blitTrough);
                return;
            }

            if (!EnviroSkyMgr.instance.useMoonShafts)
            {
                blitTrough.SetTexture("_InputTexture", source);
                CoreUtils.DrawFullScreen(cmd, blitTrough);
            }
            else
            {
                RenderLightShaft(cmd, source, destination, moonShaftsMaterial, simpleMoonClearMaterial, EnviroSkyMgr.instance.Components.Moon.transform, EnviroSkyMgr.instance.LightShaftsSettings.thresholdColorMoon.Evaluate(EnviroSkyMgr.instance.Time.solarTime), EnviroSkyMgr.instance.LightShaftsSettings.lightShaftsColorMoon.Evaluate(EnviroSkyMgr.instance.Time.solarTime));
            }
        } 


        void RenderLightShaft(CommandBuffer cmd, RTHandle source, RTHandle destination, Material mat, Material clearMat,  Transform lightSource, Color treshold, Color clr)
        {
            int divider = 1;

            if (EnviroSkyMgr.instance.LightShaftsSettings.resolution == EnviroPostProcessing.SunShaftsResolution.Low)
            {
                divider = 4;
                mat.SetFloat("_UpsampleFactor", 1f/4f);
            }
            else if (EnviroSkyMgr.instance.LightShaftsSettings.resolution == EnviroPostProcessing.SunShaftsResolution.Normal)
            {
                mat.SetFloat("_UpsampleFactor", 1f/2f);
                divider = 2;
            }
            else if (EnviroSkyMgr.instance.LightShaftsSettings.resolution == EnviroPostProcessing.SunShaftsResolution.High)
            {
                mat.SetFloat("_UpsampleFactor", 1f);
                divider = 1;
            }

            Vector3 v = Vector3.one * 0.5f;

            if (lightSource)
                v = myCam.WorldToViewportPoint(lightSource.position);

            int rtW = source.rt.width / divider;
            int rtH = source.rt.height / divider;

            RenderTextureDescriptor desc = source.rt.descriptor;
            desc.width = rtW;
            desc.height = rtH;

            lrDepthBuffer = RenderTexture.GetTemporary(desc);
            lrDepthBuffer.name = "Moonshafts Blur Buffer 1";
 
            lrColorB = RenderTexture.GetTemporary(desc);
            lrColorB.name = "Moonshafts Blur Buffer 2";


            mat.SetVector("_BlurRadius4", new Vector4(1.0f, 1.0f, 0.0f, 0.0f) * EnviroSkyMgr.instance.LightShaftsSettings.blurRadius);
            mat.SetVector("_SunPosition", new Vector4(v.x, v.y, v.z, EnviroSkyMgr.instance.LightShaftsSettings.maxRadius * 0.1f));
            mat.SetVector("_SunThreshold", treshold);
 
            //mat.SetTexture("_MainTex", source);
            cmd.SetGlobalTexture("_MainTex", source);
            cmd.SetRenderTarget(lrDepthBuffer);
            cmd.Blit(source, lrDepthBuffer, mat, 2);
 
            // paint a small black small border to get rid of clamping problems
            // if (myCam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Mono)
            //     DrawBorder(lrDepthBuffer, clearMat);
 
            // radial blur:        
            radialBlurIterations = Mathf.Clamp(radialBlurIterations, 1, 4);

            float ofs = EnviroSkyMgr.instance.LightShaftsSettings.blurRadius * (1.0f / 768.0f);
 
            mat.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
            mat.SetVector("_SunPosition", new Vector4(v.x, v.y, v.z, EnviroSkyMgr.instance.LightShaftsSettings.maxRadius * 0.1f));

            for (int it2 = 0; it2 < radialBlurIterations; it2++)
            {
                //mat.SetTexture("_BlurTex", lrDepthBuffer);
                cmd.SetGlobalTexture("_BlurTex", lrDepthBuffer);    
                cmd.SetRenderTarget(lrColorB);
                cmd.Blit(lrDepthBuffer,lrColorB,mat,1); 

                ofs = EnviroSkyMgr.instance.LightShaftsSettings.blurRadius * (((it2 * 2.0f + 1.0f) * 1.75f)) / 768.0f;
                mat.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));


               // mat.SetTexture("_BlurTex", lrColorB);
                cmd.SetGlobalTexture("_BlurTex", lrColorB);
                cmd.SetRenderTarget(lrDepthBuffer);
                cmd.Blit(lrColorB, lrDepthBuffer, mat, 1);

                ofs = EnviroSkyMgr.instance.LightShaftsSettings.blurRadius * (((it2 * 2.0f + 2.0f) * 1.75f)) / 768.0f;
                mat.SetVector("_BlurRadius4", new Vector4(ofs, ofs, 0.0f, 0.0f));
            }

            // put together:
            if (v.z >= 0.0f)
                mat.SetVector("_SunColor", new Vector4(clr.r, clr.g, clr.b, clr.a) * EnviroSkyMgr.instance.LightShaftsSettings.intensity);
            else
                mat.SetVector("_SunColor", Vector4.zero); // no backprojection !

            //mat.SetTexture("_ColorBuffer", lrDepthBuffer);
            //mat.SetTexture("_MainTex", source);
            cmd.SetGlobalTexture("_ColorBuffer", lrDepthBuffer); 
            cmd.SetGlobalTexture("_MainTex", source);
            cmd.SetRenderTarget(destination);
            //CoreUtils.DrawFullScreen(cmd, mat, null, (EnviroSkyMgr.instance.LightShaftsSettings.screenBlendMode == EnviroPostProcessing.ShaftsScreenBlendMode.Screen) ? 0 : 4);
            //HDUtils.DrawFullScreen(cmd, mat, destination,null,(EnviroSkyMgr.instance.LightShaftsSettings.screenBlendMode == EnviroPostProcessing.ShaftsScreenBlendMode.Screen) ? 0 : 4);
            cmd.Blit(source,destination,mat);
            RenderTexture.ReleaseTemporary(lrDepthBuffer);
            RenderTexture.ReleaseTemporary(lrColorB);
        }

        void DrawBorder(RenderTexture dest, Material material)
        {
            float x1;
            float x2;
            float y1;
            float y2;

            RenderTexture.active = dest;
            bool invertY = true; // source.texelSize.y < 0.0ff;
                                 // Set up the simple Matrix
            GL.PushMatrix();
            GL.LoadOrtho();

            for (int i = 0; i < material.passCount; i++)
            {
                material.SetPass(i);

                float y1_; float y2_;
                if (invertY)
                {
                    y1_ = 1.0f; y2_ = 0.0f;
                }
                else
                {
                    y1_ = 0.0f; y2_ = 1.0f;
                }

                // left
                x1 = 0.0f;
                x2 = 0.0f + 1.0f / (dest.width * 1.0f);
                y1 = 0.0f;
                y2 = 1.0f;
                GL.Begin(GL.QUADS);

                GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
                GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
                GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
                GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

                // right
                x1 = 1.0f - 1.0f / (dest.width * 1.0f);
                x2 = 1.0f;
                y1 = 0.0f;
                y2 = 1.0f;

                GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
                GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
                GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
                GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

                // top
                x1 = 0.0f;
                x2 = 1.0f;
                y1 = 0.0f;
                y2 = 0.0f + 1.0f / (dest.height * 1.0f);

                GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
                GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
                GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
                GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

                // bottom
                x1 = 0.0f;
                x2 = 1.0f;
                y1 = 1.0f - 1.0f / (dest.height * 1.0f);
                y2 = 1.0f;

                GL.TexCoord2(0.0f, y1_); GL.Vertex3(x1, y1, 0.1f);
                GL.TexCoord2(1.0f, y1_); GL.Vertex3(x2, y1, 0.1f);
                GL.TexCoord2(1.0f, y2_); GL.Vertex3(x2, y2, 0.1f);
                GL.TexCoord2(0.0f, y2_); GL.Vertex3(x1, y2, 0.1f);

                GL.End();
            }

            GL.PopMatrix();
        }
    }
}
#endif