using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Mewlist.MassiveClouds
{
    public enum MassiveCloudsColorMode
    {
        Constant,
        FogColor,
        Ambient
    }

    [HelpURL("http://massive-clouds-atmos.mewli.st/mca_stylized_cloud_renderer_ja.html")]
    [CreateAssetMenu(fileName = "MassiveCloudsStylizedCloudRenderer", menuName = "Mewlist/MassiveClouds/Stylized Cloud Renderer", order = 32)]
    public class MassiveCloudsStylizedCloud : AbstractMassiveClouds
    {
        [SerializeField] private List<MassiveCloudsStylizedCloudProfile> profiles;

        [Range(1 / 16.0f, 1f)] [SerializeField]
        private float resolution = 0.5f;

        [SerializeField] private bool lerp = false;
        [Range(0f, 5f)] [SerializeField] private float duration = 1f;

        [SerializeField] private bool save = false;
        [SerializeField] private MassiveCloudsColorMode shadowColorMode = MassiveCloudsColorMode.Ambient;
        [SerializeField] private Color shadowColor = new Color32(200, 200, 230, 255);

        [SerializeField] private MassiveCloudsColorMode cloudColorMode = MassiveCloudsColorMode.Ambient;
        [ColorUsage(false, true), SerializeField] private Color cloudColor = new Color32(200, 200, 230, 255);
        [SerializeField] private float cloudLuminanceFix = 0f;

        [SerializeField] private List<MassiveCloudsParameter> parameters;

        [SerializeField] private MassiveCloudsPass cloudsPass = null;
        [SerializeField] private MassiveCloudsShadowPass shadowPass = null;
        [SerializeField] private MassiveCloudsFogPass fogPass = null;
        [SerializeField] private MassiveCloudsVolumetricShadowPass volumetricShadowPass = null;

        private List<MassiveCloudsStylizedCloudProfile> currentProfiles = new List<MassiveCloudsStylizedCloudProfile>();
        private List<MassiveCloudsParameter> currentParameters = new List<MassiveCloudsParameter>();

        private readonly List<MassiveCloudsMixer> mixers = new List<MassiveCloudsMixer>();

        public IList<MassiveCloudsParameter> Parameters { get { return parameters; } }

        public IList<MassiveCloudsMixer> Mixers { get { return mixers; } }
        public IList<MassiveCloudsStylizedCloudProfile> Profiles { get { return profiles; } }

        public Color CloudColor
        {
            get
            {
                switch (cloudColorMode)
                {
                    case MassiveCloudsColorMode.FogColor: return RenderSettings.fogColor;
                    case MassiveCloudsColorMode.Ambient:
                    {
                        float h, s, v;
                        Color.RGBToHSV(ambient.SkyColor, out h, out s, out v);
                        var factor = Mathf.Pow(2, -cloudLuminanceFix);
                        return Color.white * v * 2 / factor;
                    }

                    case MassiveCloudsColorMode.Constant:
                    default: return cloudColor;
                }
            }
        }

        public Color ShadowColor
        {
            get
            {
                switch (shadowColorMode)
                {
                    case MassiveCloudsColorMode.FogColor: return RenderSettings.fogColor / 2;
                    case MassiveCloudsColorMode.Ambient: return ambient.EquatorColor / 2;
                    case MassiveCloudsColorMode.Constant:
                    default: return shadowColor;
                }
            }
        }

        public float Duration { get { return duration;  } }

        public bool HasVolumetricShadow
        {
            get
            {
                var hasVolumetricShadow = false;
                foreach (var massiveCloudsParameter in parameters)
                    hasVolumetricShadow = hasVolumetricShadow || massiveCloudsParameter.VolumetricShadow;
                return hasVolumetricShadow;
            }
        }

        public void SetOffset(Vector3 pos)
        {
            foreach (var massiveCloudsMixer in mixers)
                massiveCloudsMixer.Material.SetScrollOffset(pos);
        }

        public void SetProfiles(List<MassiveCloudsStylizedCloudProfile> profiles)
        {
            this.profiles = profiles;
            Build();
        }

        public void SetParameters(IEnumerable<MassiveCloudsParameter> parameters)
        {
            this.parameters = parameters.ToList();
        }

        public void Build()
        {
            if (profiles == null) return;
            // Mixers
            if (mixers.Count != profiles.Count)
            {
                var toRemove = Mathf.Max(0, mixers.Count - profiles.Count);
                var toAdd = Mathf.Max(0, profiles.Count - mixers.Count);
                for (var i = 0; i < toRemove; i++)
                {
                    mixers.RemoveAt(mixers.Count - 1);
                }

                for (var i = 0; i < toAdd; i++)
                {
                    var mixer = new MassiveCloudsMixer();
                    mixers.Add(mixer);
                }
            }

            // Parameters
            if (parameters.Count != profiles.Count)
            {
                parameters.Clear();
                foreach (var profile in profiles)
                    parameters.Add(profile == null ? default(MassiveCloudsParameter) : profile.Parameter);
            }

            if (currentParameters.Count != parameters.Count)
            {
                currentParameters.Clear();
                foreach (var _ in profiles)
                    currentParameters.Add(default(MassiveCloudsParameter));
            }

            // Current Profiles
            if (currentProfiles.Count != profiles.Count)
            {
                currentProfiles.Clear();
                currentProfiles.AddRange(profiles);
            }

            // Parameters
            for (var i = 0; i < profiles.Count; i++)
            {
                if (currentProfiles[i] == profiles[i]) continue;
                currentProfiles[i] = profiles[i];
                if (currentProfiles[i] == null)
                    parameters[i] = default(MassiveCloudsParameter);
                else
                    parameters[i] = currentProfiles[i].Parameter;
            }

            // Update Mixers
            for (var i = 0; i < profiles.Count; i++)
            {
                mixers[i].ChangeTo(currentProfiles[i], lerp);
                if (currentParameters[i] != parameters[i])
                {
                    mixers[i].SetParameter(parameters[i]);
                    currentParameters[i] = parameters[i];
                }
            }
        }

        public override void BuildCommandBuffer(MassiveCloudsPassContext ctx, IFullScreenDrawable fullScreenDrawer)
        {
            var commandBuffer = ctx.cmd;
            var targetCamera = ctx.targetCamera;
            var source = ctx.source;
            var formatAlpha = targetCamera.allowHDR
                ? RenderTextureFormat.DefaultHDR
                : RenderTextureFormat.Default;

            var renderTextures = new FlippingRenderTextures(ctx.colorBufferDesc, formatAlpha, commandBuffer, resolution);

            CommandBufferUtility.Blit(commandBuffer, source, renderTextures.From);
            shadowPass.BuildCommandBuffer(this, ctx, renderTextures);
            fogPass.BuildCommandBuffer(this, ctx, renderTextures);
            cloudsPass.BuildCommandBuffer(this, ctx, renderTextures);
            volumetricShadowPass.BuildCommandBuffer(this, ctx, renderTextures);
            fullScreenDrawer.Draw(commandBuffer, renderTextures.From);

            renderTextures.Release(commandBuffer);
        }

        private Color SafeColor(Color c)
        {
            float r, g, b;
            r = float.IsNaN(c.r) ? 0 : c.r;
            g = float.IsNaN(c.g) ? 0 : c.g;
            b = float.IsNaN(c.b) ? 0 : c.b;
            return new Color(r, g, b);
        }


        public override void UpdateClouds(Light sun, Transform moon)
        {
            if (profiles == null) return;
            Build();
            
            UpdateLightSources(sun, moon);

            // Update Materials
            for (var i = 0; i < Profiles.Count; i++)
            {
                var m = mixers[i];
                m.Material.SetShaodwColor(ShadowColor);
                m.Material.SetBaseColor(CloudColor);
                m.Update();
                m.SetDuration(Duration);
                m.SetAmbientColor(Ambient);
                m.SetLight(Sun, Moon, SunIntensityScale);
            }

            cloudsPass.Update(this);
            shadowPass.Update(this);
            fogPass.Update(this);
            volumetricShadowPass.Update(this);
        }

        private void OnValidate()
        {
            Build();

            if (save) SaveToProfile();
            save = false;
        }

        public override void Clear()
        {
            currentParameters.Clear();
            currentProfiles.Clear();
            cloudsPass.Clear();
            shadowPass.Clear();
            fogPass.Clear();
            volumetricShadowPass.Clear();
            foreach (var mixer in mixers)
                mixer.Dispose();
            mixers.Clear();
        }

        public void SaveToProfile()
        {
            for (var i = 0; i < profiles.Count; i++)
            {
                profiles[i].Parameter = parameters[i];
#if UNITY_EDITOR
                EditorUtility.SetDirty(profiles[i]);
#endif
            }
        }
    }
}