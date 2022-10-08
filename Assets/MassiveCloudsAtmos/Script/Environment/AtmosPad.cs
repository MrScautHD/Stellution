using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [HelpURL("http://massive-clouds-atmos.mewli.st/mca_atmospad_ja.html")]
    public class AtmosPad : MonoBehaviour
    {
        [SerializeField] private MassiveCloudsPhysicsCloud massiveClouds = null;
        [SerializeField] private List<AtmosProfile> profiles = new List<AtmosProfile>();
        [SerializeField] private Vector2 pointer = Vector2.zero;
        [SerializeField] private Light sun = null;
        [SerializeField] private bool updateEnvironmentOnChange = false;
        [SerializeField] private bool enableSunControl = false;
        [SerializeField] private bool enableSkyControl = true;
        [Range(0f, 90f)]
        [SerializeField] private float earthTilt = 23.4f;
        [Range(0f, 360f)]
        [SerializeField] private float earthAxis = 0f;
        [SerializeField] private List<AtmosGroup> groups = new List<AtmosGroup>();

        private Vector2 lastPointer;

        public void UpdateParameters()
        {
            if (!Enabled) return;
            if (!massiveClouds) return;

            if (updateEnvironmentOnChange)
                DynamicGI.UpdateEnvironment();

            var groups = TargetGroups();

            if (!groups.Any()) return;

            var intensity = 0f;
            var lightColor = Color.grey;
            var temperature = 6000f;
            if (groups.Count == 1)
            {
                var parameter = Lerp(groups.First().TargetProfiles(pointer));
                massiveClouds.SetMainCloudProfile(parameter.MainCloud.Profile);
                massiveClouds.SetMainCloudDensityAdjustment(parameter.MainCloud.DensityAdjustment);
                massiveClouds.SetMainCloudOverrideScattering(new MassiveCloudsPhysicsCloudPass.OverrideFloat(true, parameter.MainCloud.Scattering));
                massiveClouds.SetMainCloudOverrideShading(new MassiveCloudsPhysicsCloudPass.OverrideFloat(true, parameter.MainCloud.Shading));
                massiveClouds.SetMainCloudOverrideShadingDistance(new MassiveCloudsPhysicsCloudPass.OverrideFloat(true, parameter.MainCloud.ShadingDistance));
                massiveClouds.SetLayeredCloudProfile(parameter.LayeredCloud.Profile);
                massiveClouds.SetLayeredCloudDensityAdjustment(parameter.LayeredCloud.DensityAdjustment);
                massiveClouds.SetLayeredCloudOverrideScattering(new MassiveCloudsPhysicsCloudPass.OverrideFloat(true, parameter.LayeredCloud.Scattering));
                massiveClouds.SetLayeredCloudOverrideShading(new MassiveCloudsPhysicsCloudPass.OverrideFloat(true, parameter.LayeredCloud.Shading));
                massiveClouds.SetLayeredCloudOverrideShadingDistance(new MassiveCloudsPhysicsCloudPass.OverrideFloat(true, parameter.LayeredCloud.ShadingDistance));
                massiveClouds.SetCloudIntensityAdjustment(parameter.CloudIntensityAdjustment);
                massiveClouds.SetAtmosphere(parameter.Atmosphere);
                massiveClouds.SetFog(parameter.Fog);
                if (enableSkyControl)
                    massiveClouds.SetSky(parameter.SkyParameter);
                var ambientOverride = parameter.AmbientOverride;
                var skyColor = parameter.SkyColor;
                var equatorColor = parameter.EquatorColor;
                var groundColor = parameter.GroundColor;
                var luminanceFix = parameter.LuminanceFix;
                massiveClouds.SetAmbientColor(ambientOverride, skyColor, equatorColor, groundColor, luminanceFix);
                if (enableSunControl)
                    massiveClouds.SetMoon(parameter.MoonIntensity, parameter.MoonLightColor);
                
                intensity = parameter.Intensity;
                lightColor = parameter.LightColor;
                temperature = parameter.Temperature;
            }
            else
            {
                var a = groups[0];
                var b = groups[1];
                var ap = Lerp(a.TargetProfiles(pointer));
                var bp = Lerp(b.TargetProfiles(pointer));

                massiveClouds.SetMainCloudProfile(a.Weight > b.Weight ? ap.MainCloud.Profile : bp.MainCloud.Profile);
                massiveClouds.SetMainCloudDensityAdjustment(Mathf.Lerp(ap.MainCloud.DensityAdjustment, bp.MainCloud.DensityAdjustment, b.Weight));
                massiveClouds.SetMainCloudOverrideScattering(new MassiveCloudsPhysicsCloudPass.OverrideFloat(true, Mathf.Lerp(ap.MainCloud.Scattering, bp.MainCloud.Scattering, b.Weight)));
                massiveClouds.SetMainCloudOverrideShading(new MassiveCloudsPhysicsCloudPass.OverrideFloat(true, Mathf.Lerp(ap.MainCloud.Shading, bp.MainCloud.Shading, b.Weight)));
                massiveClouds.SetMainCloudOverrideShadingDistance(new MassiveCloudsPhysicsCloudPass.OverrideFloat(true, Mathf.Lerp(ap.MainCloud.ShadingDistance, bp.MainCloud.ShadingDistance, b.Weight)));
                massiveClouds.SetLayeredCloudProfile(a.Weight > b.Weight ? ap.LayeredCloud.Profile : bp.LayeredCloud.Profile);
                massiveClouds.SetLayeredCloudDensityAdjustment(Mathf.Lerp(ap.LayeredCloud.DensityAdjustment, bp.LayeredCloud.DensityAdjustment, b.Weight));
                massiveClouds.SetLayeredCloudOverrideScattering(new MassiveCloudsPhysicsCloudPass.OverrideFloat(true, Mathf.Lerp(ap.LayeredCloud.Scattering, bp.LayeredCloud.Scattering, b.Weight)));
                massiveClouds.SetLayeredCloudOverrideShading(new MassiveCloudsPhysicsCloudPass.OverrideFloat(true, Mathf.Lerp(ap.LayeredCloud.Shading, bp.LayeredCloud.Shading, b.Weight)));
                massiveClouds.SetLayeredCloudOverrideShadingDistance(new MassiveCloudsPhysicsCloudPass.OverrideFloat(true, Mathf.Lerp(ap.LayeredCloud.ShadingDistance, bp.LayeredCloud.ShadingDistance, b.Weight)));

                massiveClouds.SetCloudIntensityAdjustment(Mathf.Lerp(ap.CloudIntensityAdjustment, bp.CloudIntensityAdjustment, b.Weight));
                massiveClouds.SetAtmosphere(AtmosphereParameter.Lerp(ap.Atmosphere, bp.Atmosphere, b.Weight));
                massiveClouds.SetFog(FogParameter.Lerp(ap.Fog, bp.Fog, b.Weight));
                if (enableSkyControl)
                    massiveClouds.SetSky(SkyParameter.Lerp(ap.SkyParameter, bp.SkyParameter, b.Weight));
                var ambientOverride = Mathf.Lerp(ap.AmbientOverride, bp.AmbientOverride, b.Weight);
                var skyColor = Color.Lerp(ap.SkyColor, bp.SkyColor, b.Weight);
                var equatorColor = Color.Lerp(ap.EquatorColor, bp.EquatorColor, b.Weight);
                var groundColor = Color.Lerp(ap.GroundColor, bp.GroundColor, b.Weight);
                var luminanceFix = Mathf.Lerp(ap.LuminanceFix, bp.LuminanceFix, b.Weight);
                massiveClouds.SetAmbientColor(ambientOverride, skyColor, equatorColor, groundColor, luminanceFix);
                if (enableSunControl)
                    massiveClouds.SetMoon(
                        Mathf.Lerp(ap.MoonIntensity, bp.MoonIntensity, b.Weight),
                        Color.Lerp(ap.MoonLightColor, bp.MoonLightColor, b.Weight));

                intensity = Mathf.Lerp(ap.Intensity, bp.Intensity, b.Weight);
                lightColor = Color.Lerp(ap.LightColor, bp.LightColor, b.Weight);
                temperature = Mathf.Lerp(ap.Temperature, bp.Temperature, b.Weight);
            }

            if (enableSunControl && sun)
            {
                var upRot = Quaternion.Euler(-90f, 0f, 0f);
                var tiltRot = Quaternion.Euler(-earthTilt, 0f, 0f);
                var axisRot = Quaternion.Euler(0f, earthAxis, 0f);
                sun.transform.rotation = axisRot * Quaternion.Euler(0f, 0f, -360f * pointer.x) * tiltRot * upRot;
                MassiveCloudsPipelineDependent.SetLightIntensity(sun, intensity, lightColor, temperature);
            }
        }

        private bool InRange(float t, float from, float to)
        {
            return from <= t && t <= to;
        }
        private float Rate(float t, float from, float to)
        {
            return (t - from) / (to - from);
        }

        public Vector2 Pointer { get { return pointer; } }
        public float Hour { get { return pointer.x * 24f; } }
        public void SetHour(float hour)
        {
            pointer.x = hour / 24f;
        }
        public void SetVariation(float v)
        {
            pointer.y = v;
        }
        public void SetPointer(Vector2 v)
        {
            this.pointer = v;
        }

        public List<AtmosGroup> Groups { get { return groups; } }

        public void UpdateGroup()
        {
            if (profiles == null || !profiles.Any())
            {
                groups.Clear();
                return;
            }

            var sorted = profiles
                .OrderBy(x => x.Position.y)
                .ToList();
            groups.Clear();
            var lastY = float.MinValue;
            foreach (var x in sorted)
            {
                if (lastY + 0.1f < x.Position.y)
                    groups.Add(new AtmosGroup());

                groups.Last().Add(x);
                lastY = x.Position.y;
            }
        }

        public List<AtmosWeightedGroup> TargetGroups()
        {
            var under = groups
                .Where(x => pointer.y <= x.Bounds().yMax)
                .OrderBy(x => Mathf.Abs(pointer.y - x.Bounds().yMax)).FirstOrDefault();
            var underDist = (under == null) ? float.MaxValue : Mathf.Max(0, under.Bounds().yMin - pointer.y);
            var over = groups
                .Where(x => pointer.y >= x.Bounds().yMin)
                .OrderBy(x => Mathf.Abs(pointer.y - x.Bounds().yMin)).FirstOrDefault();
            if (under == over) over = null;
            var overDist = (over == null) ? float.MaxValue : Mathf.Max(0, pointer.y - over.Bounds().yMax);

            var result = new List<AtmosWeightedGroup>();
            if (under != null) result.Add(new AtmosWeightedGroup(under, over == null ? 1f : 1 - underDist / (underDist + overDist)));
            if (over != null) result.Add(new AtmosWeightedGroup(over, under == null ? 1f : 1 - overDist / (underDist + overDist)));
            return result;
        }

        public void Add(AtmosProfile atmosProfile)
        {
            profiles.Add(atmosProfile);
            UpdateParameters();
        }

        public bool Contains(AtmosProfile atmosProfile)
        {
            return profiles.Contains(atmosProfile);
        }

        public void Remove(AtmosProfile contextTarget)
        {
            profiles.Remove(contextTarget);
            UpdateParameters();
        }

        public void RemoveAll()
        {
            profiles.Clear();
            UpdateParameters();
        }

        private bool Enabled
        {
            get
            {
                return enabled && gameObject.activeSelf;
            }
        }

        private void Update()
        {
            if (!Enabled) return;

            if (lastPointer != pointer)
            {
                UpdateParameters();
                lastPointer = pointer;
            }
        }

        private Parameter Lerp(List<AtmosWeightedProfile> profiles)
        {
            if (profiles.Count == 1)
            {
                var profile = profiles.First().Profile;
                var mainCloud = new CloudParameter(
                    profile.MainCloud.Profile,
                    profile.MainCloud.DensityAdjustment,
                    profile.MainCloud.Scattering,
                    profile.MainCloud.Shading,
                    profile.MainCloud.ShadingDistance);
                var layeredCloud = new CloudParameter(
                    profile.LayeredCloud.Profile,
                    profile.LayeredCloud.DensityAdjustment,
                    profile.LayeredCloud.Scattering,
                    profile.LayeredCloud.Shading,
                    profile.LayeredCloud.ShadingDistance);
                return new Parameter(mainCloud,
                    layeredCloud,
                    profile.CloudIntensityAdjustment,
                    profile.Atmosphere,
                    profile.Fog,
                    profile.Sky,
                    profile.Intensity,
                    profile.LightColor,
                    profile.Temperature,
                    profile.MoonIntensity,
                    profile.MoonLightColor,
                    profile.AmbientOverride ? 1f : 0f,
                    profile.SkyColor,
                    profile.EquatorColor,
                    profile.GroundColor,
                    profile.LuminanceFix);
            }
            else
            {
                var a = profiles[0];
                var b = profiles[1];
                var mainCloud = new CloudParameter(
                    a.Weight > b.Weight ? a.Profile.MainCloud.Profile : b.Profile.MainCloud.Profile,
                    Mathf.Lerp(a.Profile.MainCloud.DensityAdjustment, b.Profile.MainCloud.DensityAdjustment, b.Weight),
                    Mathf.Lerp(a.Profile.MainCloud.Scattering, b.Profile.MainCloud.Scattering, b.Weight),
                    Mathf.Lerp(a.Profile.MainCloud.Shading, b.Profile.MainCloud.Shading, b.Weight),
                    Mathf.Lerp(a.Profile.MainCloud.ShadingDistance, b.Profile.MainCloud.ShadingDistance, b.Weight));
                var layeredCloud = new CloudParameter(
                    a.Weight > b.Weight ? a.Profile.LayeredCloud.Profile : b.Profile.LayeredCloud.Profile,
                    Mathf.Lerp(a.Profile.LayeredCloud.DensityAdjustment, b.Profile.LayeredCloud.DensityAdjustment, b.Weight),
                    Mathf.Lerp(a.Profile.LayeredCloud.Scattering, b.Profile.LayeredCloud.Scattering, b.Weight),
                    Mathf.Lerp(a.Profile.LayeredCloud.Shading, b.Profile.LayeredCloud.Shading, b.Weight),
                    Mathf.Lerp(a.Profile.LayeredCloud.ShadingDistance, b.Profile.LayeredCloud.ShadingDistance, b.Weight));
                return new Parameter(
                    mainCloud,
                    layeredCloud,
                    Mathf.Lerp(a.Profile.CloudIntensityAdjustment, b.Profile.CloudIntensityAdjustment, b.Weight),
                    AtmosphereParameter.Lerp(a.Profile.Atmosphere, b.Profile.Atmosphere, b.Weight),
                    FogParameter.Lerp(a.Profile.Fog, b.Profile.Fog, b.Weight),
                    SkyParameter.Lerp(a.Profile.Sky, b.Profile.Sky, b.Weight),
                    Mathf.Lerp(a.Profile.Intensity, b.Profile.Intensity, b.Weight),
                    Color.Lerp(a.Profile.LightColor, b.Profile.LightColor, b.Weight),
                    Mathf.Lerp(a.Profile.Temperature, b.Profile.Temperature, b.Weight),
                    Mathf.Lerp(a.Profile.MoonIntensity, b.Profile.MoonIntensity, b.Weight),
                    Color.Lerp(a.Profile.MoonLightColor, b.Profile.MoonLightColor, b.Weight),
                    Mathf.Lerp(a.Profile.AmbientOverride ? 1f : 0f, b.Profile.AmbientOverride ? 1f : 0f, b.Weight),
                    Color.Lerp(a.Profile.SkyColor, b.Profile.SkyColor, b.Weight),
                    Color.Lerp(a.Profile.EquatorColor, b.Profile.EquatorColor, b.Weight),
                    Color.Lerp(a.Profile.GroundColor, b.Profile.GroundColor, b.Weight),
                    Mathf.Lerp(a.Profile.LuminanceFix, b.Profile.LuminanceFix, b.Weight));
            }
        }

        
        private struct CloudParameter
        {
            public MassiveCloudsPhysicsCloudProfile Profile;
            public float DensityAdjustment;
            public float Scattering;
            public float Shading;
            public float ShadingDistance;

            public CloudParameter(
                MassiveCloudsPhysicsCloudProfile profile,
                float densityAdjustment,
                float scattering,
                float shading,
                float shadingDistance)
            {
                Profile = profile;
                DensityAdjustment = densityAdjustment;
                Scattering = scattering;
                Shading = shading;
                ShadingDistance = shadingDistance;
            }
        }

        private struct Parameter
        {
            public CloudParameter MainCloud;
            public CloudParameter LayeredCloud;
            public float CloudIntensityAdjustment;
            public AtmosphereParameter Atmosphere;
            public FogParameter Fog;
            public SkyParameter SkyParameter;
            public float Intensity;
            public Color LightColor;
            public float Temperature;
            public float MoonIntensity;
            public Color MoonLightColor;
            public float AmbientOverride;
            public Color SkyColor;
            public Color EquatorColor;
            public Color GroundColor;
            public float LuminanceFix;

            public Parameter(
                CloudParameter mainCloud,
                CloudParameter layeredCloud,
                float cloudIntensityAdjustment,
                AtmosphereParameter atmosphere,
                FogParameter fog,
                SkyParameter skyParameter,
                float intensity,
                Color lightColor,
                float temperature,
                float moonIntensity,
                Color moonLightColor,
                float ambientOverride,
                Color skyColor,
                Color equatorColor,
                Color groundColor,
                float luminanceFix)
            {
                MainCloud = mainCloud;
                LayeredCloud = layeredCloud;
                CloudIntensityAdjustment = cloudIntensityAdjustment;
                Atmosphere = atmosphere;
                Fog = fog;
                SkyParameter = skyParameter;
                Intensity = intensity;
                LightColor = lightColor;
                Temperature = temperature;
                MoonIntensity = moonIntensity;
                MoonLightColor = moonLightColor;
                AmbientOverride = ambientOverride;
                SkyColor = skyColor;
                EquatorColor = equatorColor;
                GroundColor = groundColor;
                LuminanceFix = luminanceFix;
            }
        }
    }
}