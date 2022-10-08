using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [HelpURL("http://massive-clouds-atmos.mewli.st/mca_atmos_profile_ja.html")]
    [CreateAssetMenu(fileName = "AtmosProfile", menuName = "Mewlist/MassiveClouds/AtmosProfile", order = 0)]
    public class AtmosProfile : ScriptableObject
    {
        [Serializable]
        public class Cloud
        {
            [SerializeField] private MassiveCloudsPhysicsCloudProfile profile = null;
            [Range(-1f, 1f), SerializeField] private float densityAdjustment = 0f;
            [SerializeField] private MassiveCloudsPhysicsCloudPass.OverrideFloat scattering = new MassiveCloudsPhysicsCloudPass.OverrideFloat();
            [SerializeField] private MassiveCloudsPhysicsCloudPass.OverrideFloat shading = new MassiveCloudsPhysicsCloudPass.OverrideFloat();
            [SerializeField] private MassiveCloudsPhysicsCloudPass.OverrideFloat shadingDistance = new MassiveCloudsPhysicsCloudPass.OverrideFloat();
            public MassiveCloudsPhysicsCloudProfile Profile { get { return profile; } }
            public float DensityAdjustment { get { return densityAdjustment; } }
            public float Scattering { get { return scattering.IsOverride ? scattering.Value : (profile ? profile.Lighting.Scattering : 0f); } }
            public float Shading { get { return shading.IsOverride ? shading.Value : (profile ? profile.Lighting.Shading : 0f); } }
            public float ShadingDistance { get { return shadingDistance.IsOverride ? shadingDistance.Value : (profile ? profile.Lighting.ShadingDistance : 0f); } }
        }

        [SerializeField] private string displayName = "";
        [SerializeField] private Color labelColor = Color.black;
        [SerializeField] private Vector2 position = Vector2.zero;
        [SerializeField] private Cloud mainCloud = null;
        [SerializeField] private Cloud layeredCloud = null;
        [Range(-1f, 1f), SerializeField] private float cloudIntensityAdjustment = 0f;
        [SerializeField] private bool intensityOverride = true;
        [SerializeField] private float intensity = 1f;
        [SerializeField] private Color lightColor = Color.white;
        [SerializeField] private float moonIntensity = 1f;
        [SerializeField] private Color moonLightColor = Color.white;
        [Range(1000f, 20000f), SerializeField] private float temperature = 6000f;
        [SerializeField] private bool ambientOverride = false;
        [SerializeField] private MassiveCloudsAmbient ambient = null;
        [SerializeField] private AtmosphereParameter atmosphere = null;
        [SerializeField] private FogParameter fog = null;
        [SerializeField] private SkyParameter sky = null;
        
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        
        public string DisplayName { get { return string.IsNullOrEmpty(displayName) ? name : displayName; } }
        public Cloud MainCloud { get { return mainCloud; } }
        public Cloud LayeredCloud { get { return layeredCloud; } }
        public float CloudIntensityAdjustment { get { return cloudIntensityAdjustment; } }
        public AtmosphereParameter Atmosphere { get { return atmosphere; } }
        public FogParameter Fog { get { return fog; } }
        public SkyParameter Sky { get { return sky; } }
        public bool IntensityOverride { get { return intensityOverride; } }
        public float Intensity { get { return intensity; } }
        public Color LightColor { get { return lightColor; } }
        public float Temperature { get { return temperature; } }
        public float MoonIntensity { get { return moonIntensity; } }
        public Color MoonLightColor { get { return moonLightColor; } }
        public Color LabelColor { get { return labelColor; } }
        public bool AmbientOverride { get { return ambientOverride; } }
        public Color SkyColor { get { return ambient.SkyColor; } }
        public Color EquatorColor { get { return ambient.EquatorColor; } }
        public Color GroundColor { get { return ambient.GroundColor; } }
        public float LuminanceFix { get { return ambient.LuminanceFix; } }
    }
}