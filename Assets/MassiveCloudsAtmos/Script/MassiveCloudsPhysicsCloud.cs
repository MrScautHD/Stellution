using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [HelpURL("http://massive-clouds-atmos.mewli.st/mca_physics_cloud_renderer_ja.html")]
    [CreateAssetMenu(fileName = "MassiveCloudsPhysicsCloudRenderer", menuName = "Mewlist/MassiveClouds/Physics Cloud Renderer", order = 12)]
    public class MassiveCloudsPhysicsCloud : AbstractMassiveClouds
    {
        public enum BufferFormat
        {
            RGBAHalf,
            RGBAFloat,
            RGBA32
        }

        [Serializable]
        public struct UnityQuality
        {
            public int Level;
            public float Scale;
        }
        
        [SerializeField] private MassiveCloudsPhysicsCloudPass physicsCloudPass = null;
        [SerializeField] private MassiveCloudsPhysicsCloudPass layeredCloudPass = null;
        [SerializeField] private MassiveCloudsAtmospherePass atmospherePass = null;
        [SerializeField] private BufferFormat bufferFormat = BufferFormat.RGBAHalf;

        [Range(1, 3), SerializeField] private int progressive = 1;
        [Range(0f, 1f), SerializeField] private float adaptiveSampling = 0.1333333f;
        [HideInInspector, SerializeField] private Texture2D ditheringTexture = null;
        [SerializeField] private bool forcingFullQuality = false;
        [SerializeField] private List<UnityQuality> unityQualities = new List<UnityQuality>();
        
        public float TextureScaleFromQualitySetting
        {
            get
            {
                var scale = 1f;
                foreach (var unityQuality in unityQualities)
                {
                    if (unityQuality.Level == QualitySettings.GetQualityLevel())
                    {
                        scale = unityQuality.Scale;
                        break;
                    }
                }
                return scale > 0f ? scale : 1f;
            }
        }

        public MassiveCloudsPhysicsCloudPass PhysicsCloudPass { get { return physicsCloudPass;  } }
        public MassiveCloudsPhysicsCloudPass LayeredCloudPass { get { return layeredCloudPass; } }
        public MassiveCloudsAtmospherePass AtmospherePass { get { return atmospherePass;  } }

        public int Progressive { get { return forcingFullQuality ? 1 : progressive; } }
        public float AdaptiveSampling { get { return forcingFullQuality ? 1f : adaptiveSampling; } }
        public bool ForcingFullQuality { get { return forcingFullQuality; } }

        public RenderTextureFormat BufferTextureFormat
        {
            get
            {
                switch (bufferFormat)
                {
                    case BufferFormat.RGBAHalf: return RenderTextureFormat.ARGBHalf;
                    case BufferFormat.RGBAFloat: return RenderTextureFormat.ARGBFloat;
                    case BufferFormat.RGBA32: return RenderTextureFormat.ARGB32;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        private float cycle = 0f;

        private Vector2[] rotation = new []
        {
            new Vector2(0f, 0f), 
            new Vector2(1f, 0f), 
            new Vector2(1f, 1f), 
            new Vector2(0f, 1f), 
        };

        private int rotationIndex = 0;

        public override void BuildCommandBuffer(MassiveCloudsPassContext ctx, IFullScreenDrawable fullScreenDrawer)
        {
            var commandBuffer = ctx.cmd;
            var source = ctx.source;

            var progressiveFactor = 1f / Progressive;
            cycle += progressiveFactor;
            if (cycle >= 0.999f) cycle = 0f;

            commandBuffer.SetGlobalVector("_SampleRotation", rotation[rotationIndex]);
            rotationIndex = (rotationIndex + 1) % 4;
            commandBuffer.SetGlobalFloat("_ColorSpaceIsLinear", QualitySettings.activeColorSpace == ColorSpace.Linear ? 1f : 0f);
            commandBuffer.SetGlobalFloat("_Cycle", cycle);
            commandBuffer.SetGlobalFloat("_Progressive", progressiveFactor);
            commandBuffer.SetGlobalTexture("_DitheringTexture", ditheringTexture);
            commandBuffer.SetGlobalFloat("_MassiveCloudsAdaptiveSampling", AdaptiveSampling);

            var renderTextures = new FlippingRenderTextures(ctx.colorBufferDesc, BufferTextureFormat, commandBuffer, TextureScaleFromQualitySetting);
            commandBuffer.SetGlobalTexture("_ScreenTexture", source);
            physicsCloudPass.BuildCommandBuffer(this, ctx, renderTextures);
            layeredCloudPass.BuildCommandBuffer(this, ctx, renderTextures);
            atmospherePass.BuildCommandBuffer(this, ctx, renderTextures);

            fullScreenDrawer.Draw(commandBuffer, renderTextures.From);

            renderTextures.Release(commandBuffer);
        }

        public override void UpdateClouds(Light sun, Transform moon)
        {
            UpdateLightSources(sun, moon);

            physicsCloudPass.Update(this);
            layeredCloudPass.Update(this);
            atmospherePass.Update(this);
        }

        public override void Clear()
        {
            physicsCloudPass.Clear();
            layeredCloudPass.Clear();
            atmospherePass.Clear();
        }

        public void SetMainCloudProfile(MassiveCloudsPhysicsCloudProfile cloudProfile)
        {
            physicsCloudPass.Profile = cloudProfile;
        }
        public void SetMainCloudDensityAdjustment(float densityAdjustment)
        {
            physicsCloudPass.DensityAdjustment = densityAdjustment;
        }
        public void SetMainCloudOverrideScattering(MassiveCloudsPhysicsCloudPass.OverrideFloat scattering)
        {
            physicsCloudPass.Scattering = scattering;
        }
        public void SetMainCloudOverrideShading(MassiveCloudsPhysicsCloudPass.OverrideFloat shading)
        {
            physicsCloudPass.Shading = shading;
        }
        public void SetMainCloudOverrideShadingDistance(MassiveCloudsPhysicsCloudPass.OverrideFloat shadingDistance)
        {
            physicsCloudPass.ShadingDistance = shadingDistance;
        }

        public void SetLayeredCloudProfile(MassiveCloudsPhysicsCloudProfile cloudProfile)
        {
            layeredCloudPass.Profile = cloudProfile;
        }
        public void SetLayeredCloudDensityAdjustment(float densityAdjustment)
        {
            layeredCloudPass.DensityAdjustment = densityAdjustment;
        }
        public void SetLayeredCloudOverrideScattering(MassiveCloudsPhysicsCloudPass.OverrideFloat scattering)
        {
            layeredCloudPass.Scattering = scattering;
        }
        public void SetLayeredCloudOverrideShading(MassiveCloudsPhysicsCloudPass.OverrideFloat shading)
        {
            layeredCloudPass.Shading = shading;
        }
        public void SetLayeredCloudOverrideShadingDistance(MassiveCloudsPhysicsCloudPass.OverrideFloat shadingDistance)
        {
            layeredCloudPass.ShadingDistance = shadingDistance;
        }

        public void SetCloudIntensityAdjustment(float lerp)
        {
            cloudIntensityAdjustment = lerp;
        }

        public void SetAtmosphere(AtmosphereParameter parameterAtmosphere)
        {
            atmospherePass.SetAtmosphere(parameterAtmosphere);
        }

        public void SetFog(FogParameter parameterFog)
        {
            atmospherePass.SetFog(parameterFog);
        }

        public void SetSky(SkyParameter parameterSky)
        {
            skyPass.SetSky(parameterSky);
        }

        public void SetAmbientColor(float ambientOverride, Color skyColor, Color equatorColor, Color groundColor, float luminanceFix)
        {
            ambient.SetAmbientColor(ambientOverride, skyColor, equatorColor, groundColor);
            ambient.SetLuminanceFix(luminanceFix);
        }
    }
}