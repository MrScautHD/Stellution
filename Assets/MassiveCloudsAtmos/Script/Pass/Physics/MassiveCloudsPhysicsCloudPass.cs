using System;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public class MassiveCloudsPhysicsCloudPass : IMassiveCloudsPass<MassiveCloudsPhysicsCloud>
    {
        [Serializable]
        public struct OverrideFloat
        {
            public bool IsOverride;
            [Range(0f, 1f)]
            public float Value;

            public OverrideFloat(float value)
            {
                IsOverride = false;
                Value = value;
            }

            public OverrideFloat(bool isOverride, float value)
            {
                IsOverride = isOverride;
                Value = value;
            }
        }
        [SerializeField] private MassiveCloudsPhysicsCloudProfile profile;
        [Range(0f, 1f)]
        [SerializeField] private float transition = 1f;

        [Range(-1f, 1f)]
        [SerializeField] private float densityAdjustment = 0f;
        [SerializeField] private OverrideFloat scattering = new OverrideFloat(0.5f);
        [SerializeField] private OverrideFloat shading = new OverrideFloat(0.5f);
        [SerializeField] private OverrideFloat shadingDistance = new OverrideFloat(0.5f);
        [Range(0.1f, 1f)]
        [SerializeField] private float resolution = 0.5f;
        [SerializeField] public AnimationParameter Animation;
        
        public MassiveCloudsPhysicsCloudProfile Profile
        {
            get { return currentProfile; }
            set { profile = value; }
        }

        private DynamicRenderTexture[] scaledRt = new DynamicRenderTexture[2];
        private DynamicRenderTexture[] rt = new DynamicRenderTexture[2];
        private DynamicRenderTexture[] captureRt = new DynamicRenderTexture[2];
        private Texture2D empty;

        private Material physicsCloudMaterial;
        private Material PhysicsCloudMaterial
        {
            get
            {
                if (physicsCloudMaterial == null) physicsCloudMaterial = new Material(Shader.Find("MassiveCloudsPhysicsCloud"));
                physicsCloudMaterial.DisableKeyword("_ADAPTIVE_PASS");
                return physicsCloudMaterial;
            }
        }
        private Material physicsCloudAdaptiveMaterial;
        private Material PhysicsCloudAdaptiveMaterial
        {
            get
            {
                if (physicsCloudAdaptiveMaterial == null) physicsCloudAdaptiveMaterial = new Material(Shader.Find("MassiveCloudsPhysicsCloud"));
                physicsCloudAdaptiveMaterial.EnableKeyword("_ADAPTIVE_PASS");
                return physicsCloudAdaptiveMaterial;
            }
        }

        public DynamicRenderTexture[] CloudsTexture { get { return rt;  } }
        public bool IsActive { get { return Profile != null; } }
        public float DensityAdjustment
        {
            get { return densityAdjustment; }
            set { densityAdjustment = value; }
        }
        public OverrideFloat Scattering
        {
            get { return scattering; }
            set { scattering = value; }
        }
        public OverrideFloat Shading
        {
            get { return shading; }
            set { shading = value; }
        }
        public OverrideFloat ShadingDistance
        {
            get { return shadingDistance; }
            set { shadingDistance = value; }
        }

        private MassiveCloudsPhysicsCloudParameter parameter = new MassiveCloudsPhysicsCloudParameter();

        public void ApplyTo(Material mat)
        {
            if (!profile) return;

            Animation.ApplyTo(mat);
            if (IsActive)
            {
                parameter.Set(Profile);
                parameter.ApplyTo(mat);
                parameter.ApplyDensity(mat, profile.Sampler.Density + densityAdjustment);
                if (scattering.IsOverride) parameter.ApplyScattering(mat, scattering.Value);
                if (shading.IsOverride) parameter.ApplyShading(mat, shading.Value);
                if (shadingDistance.IsOverride) parameter.ApplyShadingDistance(mat, shadingDistance.Value);
            }
        }

        public void Update(MassiveCloudsPhysicsCloud context)
        {
            Animation.Update();
            if (currentProfile != profile && nextProfile != profile)
            {
                TransitTo(profile);
            }

            var actualResolution = context.ForcingFullQuality ? 1f : resolution;
            PhysicsCloudMaterial.SetFloat("_Resolution", actualResolution);
            PhysicsCloudAdaptiveMaterial.SetFloat("_Resolution", actualResolution);
            PhysicsCloudMaterial.SetFloat("_Atmosphere", context.AtmospherePass.Atmosphere);
            PhysicsCloudAdaptiveMaterial.SetFloat("_Atmosphere", context.AtmospherePass.Atmosphere);
            PhysicsCloudMaterial.EnableKeyword("_HORIZONTAL_ON");
            PhysicsCloudAdaptiveMaterial.EnableKeyword("_HORIZONTAL_ON");
            PhysicsCloudMaterial.SetFloat("_CloudIntensityAdjustment", context.CloudIntensityAdjustment);
            PhysicsCloudAdaptiveMaterial.SetFloat("_CloudIntensityAdjustment", context.CloudIntensityAdjustment);

                        
            context.Sun.ApplySunParameters(PhysicsCloudMaterial, context.SunIntensityScale);
            context.Moon.ApplyMoonParameters(PhysicsCloudMaterial, 1);
            context.Sun.ApplySunParameters(PhysicsCloudAdaptiveMaterial, context.SunIntensityScale);
            context.Moon.ApplyMoonParameters(PhysicsCloudAdaptiveMaterial, 1);

            ApplyTo(PhysicsCloudMaterial);
            ApplyTo(PhysicsCloudAdaptiveMaterial);
        }
        
        public void BuildCommandBuffer(MassiveCloudsPhysicsCloud context, MassiveCloudsPassContext ctx, FlippingRenderTextures renderTextures)
        {
            var targetCamera = ctx.targetCamera;
            var commandBuffer = ctx.cmd;
            var colorBufferDesc = ctx.colorBufferDesc;

            var eyeIndex = 0;
            switch (targetCamera.stereoActiveEye)
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

            if (rt[eyeIndex] == null) rt[eyeIndex] = new DynamicRenderTexture(context.BufferTextureFormat);
            if (scaledRt[eyeIndex] == null) scaledRt[eyeIndex] = new DynamicRenderTexture(context.BufferTextureFormat);
            if (captureRt[eyeIndex] == null)
            {
                captureRt[eyeIndex] = new DynamicRenderTexture(context.BufferTextureFormat);
                commandBuffer.Blit(empty, captureRt[eyeIndex].GetRenderTexture(targetCamera));
            }

            var actualResolution = context.ForcingFullQuality ? 1f : resolution * context.TextureScaleFromQualitySetting;
            rt[eyeIndex].Update(targetCamera, colorBufferDesc);
            captureRt[eyeIndex].Update(targetCamera, colorBufferDesc * 0.1f);
            scaledRt[eyeIndex].Update(targetCamera, colorBufferDesc * actualResolution);

            if (transitionPhase == TransitionPhase.Capture)
            {
                commandBuffer.Blit(rt[eyeIndex].GetRenderTexture(targetCamera), captureRt[eyeIndex].GetRenderTexture(targetCamera));
                transition = 0f;
            }

            PhysicsCloudMaterial.SetTexture("_CaptureTexture", captureRt[eyeIndex].GetRenderTexture(targetCamera));
            PhysicsCloudMaterial.SetFloat("_CaptureMix", 1f - transition);

            // Render Cloud
            if (IsActive)
                commandBuffer.Blit(rt[eyeIndex].GetRenderTexture(targetCamera), scaledRt[eyeIndex].GetRenderTexture(targetCamera), PhysicsCloudMaterial, 0);
            else
                commandBuffer.Blit(empty, scaledRt[eyeIndex].GetRenderTexture(targetCamera));

            if (transition >= 1f && IsActive && context.AdaptiveSampling > 0f)
                commandBuffer.Blit(scaledRt[eyeIndex].GetRenderTexture(targetCamera), rt[eyeIndex].GetRenderTexture(targetCamera), PhysicsCloudMaterial, 1);
            else
                commandBuffer.Blit(scaledRt[eyeIndex].GetRenderTexture(targetCamera), rt[eyeIndex].GetRenderTexture(targetCamera), PhysicsCloudMaterial, 2);
            
            if (transitionPhase == TransitionPhase.Capture)
            {
                transitionPhase = TransitionPhase.FadeIn;
                currentProfile = nextProfile;
            }
            else if (transitionPhase == TransitionPhase.FadeIn)
            {
                transition = Mathf.Clamp01(transition + 2 * Time.deltaTime);
                if (transition >= 1f) transitionPhase = TransitionPhase.Finished;
            }
        }

        public enum TransitionPhase
        {
            Capture,
            FadeIn,
            Finished
        }
        private MassiveCloudsPhysicsCloudProfile currentProfile;
        private MassiveCloudsPhysicsCloudProfile nextProfile;
        private TransitionPhase transitionPhase = TransitionPhase.Finished;
        public void TransitTo(MassiveCloudsPhysicsCloudProfile profile)
        {
            nextProfile = profile;
            transition = 1f;
            transitionPhase = TransitionPhase.Capture;
        }

        public void Clear()
        {
            if (rt[0] != null) rt[0].Dispose();
            if (rt[1] != null) rt[1].Dispose();
            if (scaledRt[0] != null) scaledRt[0].Dispose();
            if (scaledRt[1] != null) scaledRt[1].Dispose();
            if (captureRt[0] != null) captureRt[0].Dispose();
            if (captureRt[1] != null) captureRt[1].Dispose();
            
            if (Application.isPlaying)
            {
                Object.Destroy(physicsCloudMaterial);
            }
            else
            {
                Object.DestroyImmediate(physicsCloudMaterial);
            }

            rt[0] = null;
            rt[1] = null;
            scaledRt[0] = null;
            scaledRt[1] = null;
            captureRt[0] = null;
            captureRt[1] = null;
            physicsCloudMaterial = null;
        }
    }
}