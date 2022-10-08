using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public class MassiveCloudsAtmospherePass : IMassiveCloudsPass<MassiveCloudsPhysicsCloud>
    {
        [SerializeField] private AtmosphereParameter atmosphere;
        [Range(0f, 1f)] 
        [SerializeField] private float screenBlending = 0f;
        [Range(0.01f, 1f)]
        [SerializeField] private float shaftQuality = 0.5f;
        [Range(0.01f, 1f)]
        [SerializeField] private float godRayQuality = 0.5f;
        [Range(0.05f, 1f)]
        [SerializeField] private float resolution = 0.3333333f;

        // Height Fog
        [SerializeField] private FogParameter fog;
        
        private DynamicRenderTexture[] scaledRt = new DynamicRenderTexture[2];
        private DynamicRenderTexture[] rt = new DynamicRenderTexture[2];
        private Texture2D empty;

        private Material scatteringMaterial;
        private Material scatteringMixMaterial;
        private Material ScatteringMaterial
        {
            get
            {
                if (scatteringMaterial == null) scatteringMaterial = new Material(Shader.Find("MassiveCloudsAtmosphere"));
                return scatteringMaterial;
            }
        }

        private Material ScatteringMixMaterial
        {
            get
            {
                if (scatteringMixMaterial == null) scatteringMixMaterial = new Material(Shader.Find("MassiveCloudsAtmosphereMix"));
                return scatteringMixMaterial;
            }
        }

        public float Atmosphere
        {
            get { return atmosphere.Atmosphere;  }
        }

        public void Update(MassiveCloudsPhysicsCloud context)
        {
            var actualResolution = context.ForcingFullQuality ? 1f : resolution;
            var actualShaftQuality = context.ForcingFullQuality ? 1f : shaftQuality;
            var actualGodRayQuality = context.ForcingFullQuality ? 1f : godRayQuality;
            ScatteringMaterial.SetFloat("_Atmosphere", atmosphere.Atmosphere);
            ScatteringMaterial.SetFloat("_SunShaft", atmosphere.SunShaft);
            ScatteringMaterial.SetFloat("_GodRay", context.PhysicsCloudPass.IsActive ? atmosphere.GodRay : 0f);
            ScatteringMaterial.SetFloat("_Shadow", atmosphere.Shadow);
            ScatteringMaterial.SetFloat("_CloudOcclusion", atmosphere.CloudOcclusion);
            ScatteringMaterial.SetFloat("_GodRayStartDistance", atmosphere.GodRayStartDistance);
            ScatteringMaterial.SetFloat("_ShaftQuality", actualShaftQuality);
            ScatteringMaterial.SetFloat("_GodRayQuality", actualGodRayQuality);
            ScatteringMaterial.SetFloat("_Resolution", actualResolution);
            ScatteringMixMaterial.SetFloat("_Atmosphere", atmosphere.Atmosphere);
            ScatteringMixMaterial.SetFloat("_AtmosphereColoring", atmosphere.AtmosphereColoring);
            ScatteringMixMaterial.SetColor("_AtmosphereColor", atmosphere.AtmosphereColor);
            ScatteringMixMaterial.SetFloat("_AtmosphereHighLightColoring", atmosphere.AtmosphereHighLightColoring);
            ScatteringMixMaterial.SetColor("_AtmosphereHighLightColor", atmosphere.AtmosphereHighLightColor);
            ScatteringMixMaterial.SetFloat("_Shadow", atmosphere.Shadow);
            ScatteringMixMaterial.SetFloat("_ScreenBlending", screenBlending);
            ScatteringMixMaterial.SetFloat("_CloudOcclusion", atmosphere.CloudOcclusion);
            ScatteringMixMaterial.SetFloat("_CloudAtmospheric", atmosphere.CloudAtmospheric);
            ScatteringMixMaterial.SetFloat("_HeightFogGroundHeight", fog.GroundHeight);
            ScatteringMixMaterial.SetFloat("_HeightFogRange", fog.Range);
            ScatteringMixMaterial.SetFloat("_HeightFogDensity", fog.Density);
            ScatteringMixMaterial.SetFloat("_HeightFogColoring", fog.Coloring);
            ScatteringMixMaterial.SetColor("_HeightFogColor", fog.Color);
            ScatteringMixMaterial.SetFloat("_HeightFogScattering", fog.Scattering);
            ScatteringMaterial.EnableKeyword("_HORIZONTAL_ON");

            context.Sun.ApplySunParameters(ScatteringMaterial, context.SunIntensityScale);
            context.Sun.ApplySunParameters(ScatteringMixMaterial, context.SunIntensityScale);
            context.Moon.ApplyMoonParameters(ScatteringMaterial, 1);
            context.Moon.ApplyMoonParameters(ScatteringMixMaterial, 1);
            context.Ambient.ApplyShaderParameters(ScatteringMixMaterial);
            context.SkyPass.ApplyTo(ScatteringMixMaterial);

            context.PhysicsCloudPass.ApplyTo(ScatteringMaterial);
            context.PhysicsCloudPass.ApplyTo(ScatteringMixMaterial);
        }
        
        public void BuildCommandBuffer(MassiveCloudsPhysicsCloud massvieClouds, MassiveCloudsPassContext ctx, FlippingRenderTextures renderTextures)
        {
            if (ctx.targetCamera.renderingPath == RenderingPath.DeferredShading)
                ScatteringMaterial.EnableKeyword("_DEFERRED_SHADING");
            else
                ScatteringMaterial.DisableKeyword("_DEFERRED_SHADING");

            var eyeIndex = 0;
            switch (ctx.targetCamera.stereoActiveEye)
            {
                case Camera.MonoOrStereoscopicEye.Left:
                    break;
                case Camera.MonoOrStereoscopicEye.Right:
                    eyeIndex = 1;
                    break;
                case Camera.MonoOrStereoscopicEye.Mono:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (empty == null)
            {
                empty = new Texture2D(1,1, TextureFormat.ARGB32, false);
                empty.SetPixel(0, 0, new Color(0, 0, 0, 0));
                empty.Apply();
            }

            if (rt[eyeIndex] == null) rt[eyeIndex] = new DynamicRenderTexture(massvieClouds.BufferTextureFormat);
            if (scaledRt[eyeIndex] == null) scaledRt[eyeIndex] = new DynamicRenderTexture(massvieClouds.BufferTextureFormat);

            var actualResolution = massvieClouds.ForcingFullQuality ? 1f : resolution * massvieClouds.TextureScaleFromQualitySetting;
            rt[eyeIndex].Update(ctx.targetCamera, ctx.colorBufferDesc);
            scaledRt[eyeIndex].Update(ctx.targetCamera, ctx.colorBufferDesc * actualResolution);

            // Render Atmosphere
            var cloudsTexture = massvieClouds.PhysicsCloudPass.CloudsTexture;
            if (cloudsTexture[eyeIndex] != null)
                ctx.cmd.SetGlobalTexture("_CloudsTexture", cloudsTexture[eyeIndex].GetRenderTexture(ctx.targetCamera));
            ctx.cmd.Blit(rt[eyeIndex].GetRenderTexture(ctx.targetCamera), scaledRt[eyeIndex].GetRenderTexture(ctx.targetCamera), ScatteringMaterial, 0);
            if (massvieClouds.AdaptiveSampling > 0f)
                ctx.cmd.Blit(scaledRt[eyeIndex].GetRenderTexture(ctx.targetCamera), rt[eyeIndex].GetRenderTexture(ctx.targetCamera), ScatteringMaterial, 1);
            else
                ctx.cmd.Blit(scaledRt[eyeIndex].GetRenderTexture(ctx.targetCamera), rt[eyeIndex].GetRenderTexture(ctx.targetCamera));

            // Mix
            var cameraHeight = ctx.targetCamera.transform.position.y;
            var cloudPasses = new List<MassiveCloudsPhysicsCloudPass>() { massvieClouds.PhysicsCloudPass, NullPass };
            if (massvieClouds.LayeredCloudPass != null)
                cloudPasses[1] = massvieClouds.LayeredCloudPass;

            cloudPasses.Sort((l, r) =>
            {
                if (!l.IsActive || !r.IsActive) return 1;
                var dl = Mathf.Abs(l.Profile.HorizontalShape.FromHeight - cameraHeight);
                var dr = Mathf.Abs(r.Profile.HorizontalShape.FromHeight - cameraHeight);
                return dl < dr ? -1 : 1;
            });
            var i = 0;
            foreach (var layer in cloudPasses)
            {
                ctx.cmd.SetGlobalTexture(cloudsTextureNames[i], layer.CloudsTexture[eyeIndex].GetRenderTexture(ctx.targetCamera));
                i++;
            }
            ctx.cmd.Blit(rt[eyeIndex].GetRenderTexture(ctx.targetCamera), renderTextures.To, ScatteringMixMaterial);
            renderTextures.Flip();
        }

        private static MassiveCloudsPhysicsCloudPass NullPass = new MassiveCloudsPhysicsCloudPass();
        private static string[] cloudsTextureNames = { "_CloudsTexture1", "_CloudsTexture2" };

        public void Clear()
        {
            if (rt[0] != null) rt[0].Dispose();
            if (rt[1] != null) rt[1].Dispose();
            if (scaledRt[0] != null) scaledRt[0].Dispose();
            if (scaledRt[1] != null) scaledRt[1].Dispose();
            if (Application.isPlaying)
            {
                Object.Destroy(scatteringMaterial);
                Object.Destroy(scatteringMixMaterial);
            }
            else
            {
                Object.DestroyImmediate(scatteringMaterial);
                Object.DestroyImmediate(scatteringMixMaterial);
            }

            rt[0] = null;
            rt[1] = null;
            scaledRt[0] = null;
            scaledRt[1] = null;
            scatteringMaterial = null;
            scatteringMixMaterial = null;
        }

        public void SetAtmosphere(AtmosphereParameter parameterAtmosphere)
        {
            atmosphere = parameterAtmosphere.ShallowCopy();
        }

        public void SetFog(FogParameter parameterFog)
        {
            fog = parameterFog.ShallowCopy();
        }
    }
}