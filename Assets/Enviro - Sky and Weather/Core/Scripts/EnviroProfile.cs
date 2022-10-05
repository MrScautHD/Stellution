using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

#region Classes
[Serializable]
public class EnviroQualitySettings
{
    [Range(0, 1)]
    [Tooltip("Modifies the amount of particles used in weather effects.")]
    public float GlobalParticleEmissionRates = 1f;
    [Tooltip("How often Enviro Growth Instances should be updated. Lower value = smoother growth and more frequent updates but more perfomance hungry!")]
    public float UpdateInterval = 0.5f; //Attention: lower value = smoother growth and more frequent updates but more perfomance hungry!
}

[Serializable]
public class EnviroSeasonSettings
{
    [Header("Spring")]
    [Tooltip("Start Day of Year for Spring")]
    [Range(0, 366)]
    public int SpringStart = 60;
    [Tooltip("End Day of Year for Spring")]
    [Range(0, 366)]
    public int SpringEnd = 92;
    [Tooltip("Base Temperature in Spring")]
    public AnimationCurve springBaseTemperature = new AnimationCurve();

    [Header("Summer")]
    [Tooltip("Start Day of Year for Summer")]
    [Range(0, 366)]
    public int SummerStart = 93;
    [Tooltip("End Day of Year for Summer")]
    [Range(0, 366)]
    public int SummerEnd = 185;
    [Tooltip("Base Temperature in Summer")]
    public AnimationCurve summerBaseTemperature = new AnimationCurve();

    [Header("Autumn")]
    [Tooltip("Start Day of Year for Autumn")]
    [Range(0, 366)]
    public int AutumnStart = 186;
    [Tooltip("End Day of Year for Autumn")]
    [Range(0, 366)]
    public int AutumnEnd = 276;
    [Tooltip("Base Temperature in Autumn")]
    public AnimationCurve autumnBaseTemperature = new AnimationCurve();

    [Header("Winter")]
    [Tooltip("Start Day of Year for Winter")]
    [Range(0, 366)]
    public int WinterStart = 277;
    [Tooltip("End Day of Year for Winter")]
    [Range(0, 366)]
    public int WinterEnd = 59;
    [Tooltip("Base Temperature in Winter")]
    public AnimationCurve winterBaseTemperature = new AnimationCurve();
}

[Serializable]
public class EnviroWeatherSettings
{
    [Header("Zones Setup:")]
    [Tooltip("Tag for zone triggers. Create and assign a tag to this gameObject")]
    public bool useTag;

    [Header("Weather Transition Settings:")]
    [Tooltip("Defines the speed of wetness will raise when it is raining.")]
    public float wetnessAccumulationSpeed = 0.05f;
    [Tooltip("Defines the speed of wetness will dry when it is not raining.")]
    public float wetnessDryingSpeed = 0.05f;
    [Tooltip("Defines the speed of snow will raise when it is snowing.")]
    public float snowAccumulationSpeed = 0.05f;
    [Tooltip("Defines the speed of snow will meld when it is not snowing.")]
    public float snowMeltingSpeed = 0.05f;
    [Tooltip("Defines the temperature when snow starts to melt.")]
    public float snowMeltingTresholdTemperature = 1f;

    [Tooltip("Defines the speed of clouds will change when weather conditions changed.")]
    public float cloudTransitionSpeed = 1f;
    [Tooltip("Defines the speed of fog will change when weather conditions changed.")]
    public float fogTransitionSpeed = 1f;
    [Tooltip("Defines the speed of wind intensity will change when weather conditions changed.")]
    public float windIntensityTransitionSpeed = 1f;
    [Tooltip("Defines the speed of particle effects will change when weather conditions changed.")]
    public float effectTransitionSpeed = 1f;
    [Tooltip("Defines the speed of sfx will fade in and out when weather conditions changed.")]
    public float audioTransitionSpeed = 0.1f;

    [Header("Lightning Effect:")]
    public GameObject lightningEffect;
    [Range(500f, 10000f)]
    public float lightningRange = 500f;
    [Range(500f, 5000f)]
    public float lightningHeight = 750f;

    [Header("Temperature:")]
    [Tooltip("Defines the speed of temperature changes.")]
    public float temperatureChangingSpeed = 10f;
}

[Serializable]
public class EnviroSkySettings
{
    public enum SunAndMoonCalc
    {
        Simple,
        Realistic
    }

    public enum MoonPhases
    {
        Custom,
        Realistic
    }

    public enum SkyboxModi
    {
        Default,
        Simple,
        CustomSkybox,
        CustomColor
    }

    public enum SkyboxModiLW
    {
        Simple,
        CustomSkybox,
        CustomColor
    }

    [Header("Sky Mode:")]
    [Tooltip("Select if you want to use enviro skybox your custom material.")]
    public SkyboxModi skyboxMode;
    [Tooltip("Select if you want to use enviro skybox your custom material.")]
    public SkyboxModiLW skyboxModeLW;
    [Tooltip("If SkyboxMode == CustomSkybox : Assign your skybox material here!")]
    public Material customSkyboxMaterial;
    [Tooltip("If SkyboxMode == CustomColor : Select your sky color here!")]
    public Color customSkyboxColor;
    [Tooltip("Enable to render black skybox at ground level.")]
    public bool blackGroundMode = false;

    [Header("Scattering")]
    [Tooltip("Light Wavelength used for atmospheric scattering. Keep it near defaults for earthlike atmospheres, or change for alien or fantasy atmospheres for example.")]
    public Vector3 waveLength = new Vector3(540f, 496f, 437f);
    [Tooltip("Influence atmospheric scattering.")]
    public float rayleigh = 5.15f;
    [Tooltip("Sky turbidity. Particle in air. Influence atmospheric scattering.")]
    public float turbidity = 1f;
    [Tooltip("Influence scattering near sun.")]
    public float mie = 5f;
    [Tooltip("Influence scattering near sun.")]
    public float g = 0.8f;
    [Tooltip("Intensity gradient for atmospheric scattering. Influence atmospheric scattering based on current sun altitude.")]
    public AnimationCurve scatteringCurve = new AnimationCurve();
    [Tooltip("Color gradient for atmospheric scattering. Influence atmospheric scattering based on current sun altitude.")]
    public Gradient scatteringColor;

    [Header("Sun")]
    public SunAndMoonCalc sunAndMoonPosition = SunAndMoonCalc.Realistic;
    [Tooltip("Intensity of Sun Influence Scale and Dropoff of sundisk.")]
    public float sunIntensity = 100f;
    [Tooltip("Scale of rendered sundisk.")]
    public float sunDiskScale = 20f;
    [Tooltip("Intenisty of rendered sundisk.")]
    public float sunDiskIntensity = 3f;
    [Tooltip("Color gradient for sundisk. Influence sundisk color based on current sun altitude")]
#if UNITY_2018_3_OR_NEWER
    [GradientUsageAttribute(true)]
#endif
    public Gradient sunDiskColor;

    [Tooltip("Top color of simple skybox.")]
    public Gradient simpleSkyColor;
    [Tooltip("Horizon color of simple skybox.")]
    public Gradient simpleHorizonColor;
    [Tooltip("Horizon color of opposite side of sun.")]
    public Gradient simpleHorizonBackColor;
    [Tooltip("Sun color of simple skybox.")]
    public Gradient simpleSunColor;
    [Tooltip("Ground color of simple skybox.")]
    public Color simpleGroundColor;
    [Tooltip("Size of sun in simple skybox mode.")]
    public AnimationCurve simpleSunDiskSize = new AnimationCurve();

    [Header("Moon")]
    [Tooltip("Whether to render the moon.")]
    public bool renderMoon = true;
    [Tooltip("The Moon phase mode. Custom = for customizable phase.")]
    public MoonPhases moonPhaseMode = MoonPhases.Realistic;
    [Tooltip("The Moon texture.")]
    public Texture moonTexture;
    [Tooltip("The Moon's Glow texture.")]
    public Texture glowTexture;
    [ColorUsage(true,true)]
    [Tooltip("The color of the moon")]
    public Color moonColor;
    [Range(0f, 5f)]
    [Tooltip("Brightness of the moon.")]
    public float moonBrightness = 1f;
    [Range(0f, 20f)]
    [Tooltip("Size of the moon.")]
    public float moonSize = 10f;
    [Range(0f, 20f)]
    [Tooltip("Size of the moon glowing effect.")]
    public float glowSize = 10f;
    [Tooltip("Glow around moon.")]
    public AnimationCurve moonGlow = new AnimationCurve();
    [Tooltip("Glow color around moon.")]
    public Color moonGlowColor;

    [Header("Sky Color Corrections")]
    [Tooltip("Higher values = brighter sky.")]
    public AnimationCurve skyLuminence = new AnimationCurve();
    [Tooltip("Higher values = stronger colors applied BEFORE clouds rendered!")]
    public AnimationCurve skyColorPower = new AnimationCurve();

    [Header("Tonemapping")]
    [Tooltip("Sky exposure when using Enviro Tonemapping option in Rendering Setup.")]
    public float skyExposure = 1.5f;

    [Header("Stars")]
    [Tooltip("A cubemap for night sky.")]
    public Cubemap starsCubeMap;
    [Tooltip("Intensity of stars based on time of day.")]
    public AnimationCurve starsIntensity = new AnimationCurve();
    [Tooltip("Stars Twinkling Speed")]
    [Range(0f, 10f)]
    public float starsTwinklingRate = 1f;

    [Header("Galaxy")]
    [Tooltip("A cubemap for night galaxy.")]
    public Cubemap galaxyCubeMap;
    [Tooltip("Intensity of galaxy based on time of day.")]
    public AnimationCurve galaxyIntensity = new AnimationCurve();

    [Header("Sky Dithering")]
    [Range(0f, 1f)]
    public float dithering = 0.5f;

    [Tooltip("HDRP only. Set sky type to EnviroSkybox on start.")]
    public bool setEnviroSkybox = true;
}

[Serializable]
public class EnviroSatellitesSettings
{
    [Tooltip("List of satellites.")]
    public List<EnviroSatellite> additionalSatellites = new List<EnviroSatellite>();
}

[Serializable]
public class EnviroReflectionSettings
{

    public enum GlobalReflectionResolution
    {
        R16,
        R32,
        R64,
        R128,
        R256,
        R512,
        R1024,
        R2048
    }

    [Header("Global Reflections Settings")]
    [Tooltip("Enable/disable enviro reflection probe..")]
    public bool globalReflections = true;
    [Header("Global Reflections Custom Rendering")]
    [Tooltip("Enable/disable if enviro reflection probe should render in custom mode to support clouds and other enviro effects.")]
    public bool globalReflectionCustomRendering = true;
    [Tooltip("Enable/disable if enviro reflection probe should render with fog.")]
    public bool globalReflectionUseFog = false;
    [Tooltip("Set if enviro reflection probe should update faces individual on different frames.")]
    public bool globalReflectionTimeSlicing = true;
    [Header("Global Reflections Updates Settings")]
    [Tooltip("Enable/disable enviro reflection probe updates based on gametime changes..")]
    public bool globalReflectionsUpdateOnGameTime = true;
    [Tooltip("Enable/disable enviro reflection probe updates based on transform position changes..")]
    public bool globalReflectionsUpdateOnPosition = true;
    [Tooltip("Reflection probe intensity.")]
    [Range(0f, 2f)]
    public float globalReflectionsIntensity = 0.5f;
    [Tooltip("Reflection probe update rate based on game time.")]
    public float globalReflectionsTimeTreshold = 0.025f;
    [Tooltip("Reflection probe update rate based on camera position.")]
    public float globalReflectionsPositionTreshold = 0.25f;
    [Tooltip("Reflection probe scale. Increase that one to increase the area where reflection probe will influence your scene.")]
    [Range(0.1f, 10f)]
    public float globalReflectionsScale = 1f;
    [Tooltip("Reflection probe resolution.")]
    public GlobalReflectionResolution globalReflectionResolution = GlobalReflectionResolution.R256;
    [Tooltip("Reflection probe rendered Layers.")]
    public LayerMask globalReflectionLayers;

    [Tooltip("Enable this option to update default EnvironmentReflections. This option could be needed for instanced indirect rendered materials that can't get reflections from reflection probes directly.")]
    public bool updateDefaultEnvironmentReflections = false;

#if ENVIRO_HD
    [Tooltip("Set the quality of clouds in reflection rendering. Leave empty to use global settings.")]
    public EnviroVolumeCloudsQuality reflectionCloudsQuality;
#endif
}

[Serializable]
public class EnviroLightSettings
{
    public enum LightingMode
    {
        Single,
        Dual
    }
     
    [Tooltip("Whether you want to use two direcitonal lights for sun and moon or only one that will switch. Dual mode can be expensive in complex scenes!")]
    public LightingMode directionalLightMode = LightingMode.Single;
    [Tooltip("Color gradient for sun and moon light based on sun position in sky.")]
#if UNITY_2018_3_OR_NEWER
    [GradientUsageAttribute(true)]
#endif
    public Gradient LightColor;
    [Tooltip("Direct light sun intensity based on sun position in sky")]
    public AnimationCurve directLightSunIntensity = new AnimationCurve();
    [Tooltip("Direct light moon intensity based on moon position in sky")]
    public AnimationCurve directLightMoonIntensity = new AnimationCurve();
    [Tooltip("Set the speed of how fast light intensity will update.")]
    [Range(0.01f, 10f)]
    public float lightIntensityTransitionSpeed = 1f;
    [Tooltip("Realtime shadow strength of the directional light.")]
    public AnimationCurve shadowIntensity = new AnimationCurve();
    [Tooltip("Direct lighting y-offset.")]
    [Range(0f, 5000f)]
    public float directLightAngleOffset = 0f;
    [Header("Ambient")]
    [Tooltip("Ambient Rendering Mode.")]
    public UnityEngine.Rendering.AmbientMode ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
    [Tooltip("Ambientlight intensity based on sun position in sky.")]
    public AnimationCurve ambientIntensity = new AnimationCurve();
    [Tooltip("Ambientlight sky color based on sun position in sky.")]
#if UNITY_2018_3_OR_NEWER
    [GradientUsageAttribute(true)]
#endif
    public Gradient ambientSkyColor;
    [Tooltip("Ambientlight Equator color based on sun position in sky.")]
#if UNITY_2018_3_OR_NEWER
    [GradientUsageAttribute(true)]
#endif
    public Gradient ambientEquatorColor;
    [Tooltip("Ambientlight Ground color based on sun position in sky.")]
#if UNITY_2018_3_OR_NEWER
    [GradientUsageAttribute(true)]
#endif 
    public Gradient ambientGroundColor;
    [Tooltip("Activate to stop the rotation of sun and moon at 'rotationStopHigh' sun/moon altitude in sky.")]
    public bool stopRotationAtHigh = false;
    [Range(0f, 1f)]
    [Tooltip("The altitude of sun/moon in sky (Same as 'DayNightSwitch' or the evaluatation of gradients.")]
    public float rotationStopHigh = 0.5f;

    //HDRP
    public bool usePhysicalBasedLighting = true;
    [Tooltip("Direct light sun intensity based on sun position in sky in LUX.")]
    public AnimationCurve sunIntensityLux = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 120000));
    [Tooltip("Direct light moon intensity based on moon position in sky in LUX.")]
    public AnimationCurve moonIntensityLux = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1));
    [Tooltip("Color Temperature based on sun position in sky.")]
    public AnimationCurve lightColorTemperature = new AnimationCurve();
#if UNITY_2018_3_OR_NEWER
    [GradientUsageAttribute(true)]
#endif
    public Gradient lightColorTint;
    [Tooltip("Enable this to control the scene exposure with Enviro. Otherwise check the Enviro - Post Processing Volume.")]
    public bool controlSceneExposure = true;
    [Tooltip("Set fixed exposure for your scene.")]
    [Range(0f,20f)]
    public float exposure = 7f;
    [Tooltip("Set sky exposure.")]
    [Range(0f, 20f)]
    public float skyExposure = 7f;
    [Range(0f, 20f)]
    [Tooltip("Increase standard light intensity to LUX values.")]
    public float lightIntensityLuxMult = 7f;
    [Tooltip("Set fixed exposure for your scene based on sun position in sky.")]
    public AnimationCurve exposurePhysical = new AnimationCurve(new Keyframe(0, 3), new Keyframe(1, 14.5f));
    [Tooltip("Set sky exposure based on sun position in sky.")]
    public AnimationCurve skyExposurePhysical = new AnimationCurve(new Keyframe(0, 3), new Keyframe(1, 14.5f));

    [Tooltip("Modify the ambient color in HDRP.")]
    public Gradient ambientColorMod;

    public enum AmbientUpdateMode 
    {
        Realtime,
        OnChange
    }

    public AmbientUpdateMode indirectLightingUpdateMode;
    //

}

[Serializable]
public class EnviroDistanceBlurSettings
{
    public bool antiFlicker = true;
    public bool highQuality = true;
    [Range(1, 7)]
    public float radius = 7f;
}

[Serializable]
public class EnviroFogSettings
{
    [Header("Mode")]
    [Tooltip("Unity's fog mode.")]
    public FogMode Fogmode = FogMode.Exponential;
    [Tooltip("Simple fog = just plain color without scattering.")]
    public bool useSimpleFog = false;
    [Tooltip("Use Unity Forward Rendering Fog.")]
    public bool useUnityFog = false;
    [Header("Distance Fog")]
    [Tooltip("Use distance fog?")]
    public bool distanceFog = true;
    [Tooltip("Use radial distance fog?")]
    public bool useRadialDistance = true;
    [Tooltip("The distance where fog starts.")]
    public float startDistance = 0.0f;
    [Range(0f, 10f)]
    [Tooltip("The intensity of distance fog.")]
    public float distanceFogIntensity = 4.0f;
    [Range(0f, 1f)]
    [Tooltip("The maximum density of fog.")]
    public float maximumFogDensity = 0.9f;
    [Header("Height Fog")]
    [Tooltip("Use heightbased fog?")]
    public bool heightFog = true;
    [Tooltip("The height of heightbased fog.")]
    public float height = 90.0f;
    [Range(0f, 1f)]
    [Tooltip("The intensity of heightbased fog.")]
    public float heightFogIntensity = 1f;
    [HideInInspector]
    public float heightDensity = 0.15f;
    [Header("Height Fog Noise")]
    [Range(0f, 1f)]
    [Tooltip("The noise intensity of height based fog.")]
    public float noiseIntensity = 1f;
    [Tooltip("The noise intensity offset of height based fog.")]
    [Range(0f, 1f)]
    public float noiseIntensityOffset = 0.3f;
    [Range(0f, 0.1f)]
    [Tooltip("The noise scaling of height based fog.")]
    public float noiseScale = 0.001f;
    [Tooltip("The speed and direction of height based fog.")]
    public Vector2 noiseVelocity = new Vector2(3f, 1.5f);
    // [Header("Fog Scattering")]
    [Tooltip("Influence scattering near sun.")]
    public float mie = 5f;
    [Tooltip("Influence scattering near sun.")]
    public float g = 5f;

    [Header("Fog Dithering")]
    [Range(0f, 1f)]
    public float fogDithering = 0.5f;

    [Tooltip("Color gradient for Top Fog")]
#if UNITY_2018_3_OR_NEWER
    [GradientUsageAttribute(true)]
#endif
    public Gradient simpleFogColor;

    [HideInInspector]
    public float skyFogIntensity = 1f;
    [Tooltip("Fog tonemapping exposure when using the enviro tonemapper.")]
    public float fogExposure = 1f;

    //HDRP Only Settings
    public bool useEnviroGroundFog = false;
    public bool useHDRPFog = true;

        [Tooltip("Fog color tint based on sun altitude.")]
#if UNITY_2018_3_OR_NEWER
    [GradientUsageAttribute(true)]
#endif
    public Gradient fogColorTint;
}

[Serializable]
public class EnviroVolumeLightingSettings
{
#if ENVIRO_HD
    [Tooltip("Downsampling of volume light rendering.")]
    public EnviroSkyRendering.VolumtericResolution Resolution = EnviroSkyRendering.VolumtericResolution.Quarter;
#endif
    [Tooltip("Activate or deactivate directional volume light rendering.")]
    public bool dirVolumeLighting = true;
    [Header("Quality")]
    [Range(1, 64)]
    public int SampleCount = 8;
    [Header("Light Settings")]
    public AnimationCurve ScatteringCoef = new AnimationCurve();
    [Range(0.0f, 0.1f)]
    public float ExtinctionCoef = 0.05f;
    [Range(0.0f, 0.999f)]
    public float Anistropy = 0.1f;
    public float MaxRayLength = 10.0f;
    [Header("3D Noise")]
    [Tooltip("Use 3D noise for directional lighting. Attention: Expensive operation for directional lights with high sample count!")]
    public bool directLightNoise = false;
    [Range(0f, 1f)]
    [Tooltip("The noise intensity volume lighting.")]
    public float noiseIntensity = 1f;
    [Tooltip("The noise intensity offset of volume lighting.")]
    [Range(0f, 1f)]
    public float noiseIntensityOffset = 0.3f;
    [Range(0f, 0.1f)]
    [Tooltip("The noise scaling of volume lighting.")]
    public float noiseScale = 0.001f;
    [Tooltip("The speed and direction of volume lighting.")]
    public Vector2 noiseVelocity = new Vector2(3f, 1.5f);
}

[Serializable]
public class EnviroLightShaftsSettings
{
    [Header("Quality Settings")]
    [Tooltip("Lightshafts resolution quality setting.")]
    public EnviroPostProcessing.SunShaftsResolution resolution = EnviroPostProcessing.SunShaftsResolution.Normal;
    [Tooltip("Lightshafts blur mode.")]
    public EnviroPostProcessing.ShaftsScreenBlendMode screenBlendMode = EnviroPostProcessing.ShaftsScreenBlendMode.Screen;
    [Tooltip("Use cameras depth to hide lightshafts?")]
    public bool useDepthTexture = true;

    [Header("Intensity Settings")]
    [Tooltip("Color gradient for lightshafts based on sun position.")]
    public Gradient lightShaftsColorSun;
    [Tooltip("Color gradient for lightshafts based on moon position.")]
    public Gradient lightShaftsColorMoon;
    [Tooltip("Treshhold gradient for lightshafts based on sun position. This will influence lightshafts intensity!")]
    [GradientUsage(true)]
    public Gradient thresholdColorSun;
    [GradientUsage(true)]
    [Tooltip("Treshhold gradient for lightshafts based on moon position. This will influence lightshafts intensity!")]
    public Gradient thresholdColorMoon;
    [Tooltip("Radius of blurring applied.")]
    public float blurRadius = 6f;
    [Tooltip("Global Lightshafts intensity.")]
    public float intensity = 0.6f;
    [Tooltip("Lightshafts maximum radius.")]
    public float maxRadius = 10f;
}

[Serializable]
public class EnviroAudioSettings
{
    [Tooltip("A list of all possible thunder audio effects.")]
    public List<AudioClip> ThunderSFX = new List<AudioClip>();
}

[Serializable]
public class EnviroCloudSettings
{
    public enum FlatCloudResolution
    {
        R512,
        R1024,
        R2048,
        R4096,
    }
#if ENVIRO_HD
    public EnviroVolumeCloudsQualitySettings cloudsQualitySettings;
#endif
    [Range(10000f, 486000f)]
    [Tooltip("Clouds world scale. This settings will influece rendering of clouds at horizon.")]
    public float cloudsWorldScale = 113081f;

    [Tooltip("Change Clouds Height.")]
    [Range(-2000f, 2000f)]
    public float cloudsHeightMod = 0f;

    [Tooltip("Changes the way sky fog will blend with clouds when inside/above the volumetric clouds.")]
    [Range(1000f, 20000f)]
    public float cloudsSkyFogHeightBlending = 15000f;

    [Tooltip("Enable this option to blend clouds with your scene.")]
    public bool depthBlending = false;
    [Tooltip("Use this option to minimize blending artifacts of downsampled clouds with full resolution scene.")]
    public bool bilateralUpsampling = false;

    [Header("Clouds Wind Animation")]
    public bool useWindZoneDirection;
    [Range(-1f, 1f)]
    [Tooltip("Time scale / wind animation speed of clouds.")]
    public float cloudsTimeScale = 1f;
    [Range(0f, 1f)]
    [Tooltip("Global clouds wind speed modificator.")]
    public float cloudsWindIntensity = 0.001f;
    [Range(0f, 1f)]
    [Tooltip("Global clouds detail wind speed modificator.")]
    public float cloudsDetailWindIntensity = 0.001f;
    [Range(0f, 1f)]
    [Tooltip("Global clouds upwards wind speed modificator.")]
    public float cloudsUpwardsWindIntensity = 0.001f;
    [Range(0f, 1f)]
    [Tooltip("Cirrus clouds wind speed modificator.")]
    public float cirrusWindIntensity = 0.001f;
    [Range(-1f, 1f)]
    [Tooltip("Global clouds wind direction X axes.")]
    public float cloudsWindDirectionX = 1f;
    [Range(-1f, 1f)]
    [Tooltip("Global clouds wind direction Y axes.")]
    public float cloudsWindDirectionY = 1f;
    [Tooltip("Clamps directional shadows on clouds.")]
    public AnimationCurve attenuationClamp = new AnimationCurve();
    [Tooltip("Sun highlight in near of sun.")]
    [Range(0.01f, 1f)]
    public float hgPhase = 0.5f;
    [Range(0.01f, 1f)]
    [Tooltip("SilverLining intensity away from sun. Evaluated based on sun position. Keep between 0-1 range!")]
    public float silverLiningIntensity = 0.5f;
    [Tooltip("SilverLining spread away from sun. Evaluated based on sun position. Keep between 0-1 range!")]
    public AnimationCurve silverLiningSpread = new AnimationCurve();
    [Tooltip("Global Color for volume clouds based sun positon.")]
#if UNITY_2018_3_OR_NEWER
    [GradientUsageAttribute(true)]
#endif
    public Gradient volumeCloudsColor = new Gradient();
    [Tooltip("Global Color for clouds based moon positon.")]
#if UNITY_2018_3_OR_NEWER
    [GradientUsageAttribute(true)]
#endif
    public Gradient volumeCloudsMoonColor = new Gradient();
    [Tooltip("Global ambient color add for volume clouds based sun positon.")]
#if UNITY_2018_3_OR_NEWER
    [GradientUsageAttribute(true)]
#endif
    public Gradient volumeCloudsAmbientColor = new Gradient();
    [Tooltip("Raie or lower the light intensity based on sun altitude.")]
    public AnimationCurve lightIntensity = new AnimationCurve();
    [Tooltip("Tweak the ambient lighting from sky based on sun altitude.")]
    public AnimationCurve ambientLightIntensity = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

    [Tooltip("Tonemapping exposure")]
    public float cloudsExposure = 1f;

    [Tooltip("Use Halton Sequence based raymarching offset to help with undersampling raymarching. This option will make clouds more noisy, so use it with TAA only.")]
    public bool useHaltonRaymarchOffset = false;
    [Tooltip("Enable this to only use half the amount of raymarching steps when using the halton sequence offset together with TAA.")]
    public bool useLessSteps = false;
    [Tooltip("Tiling of the generated weather map.")]
    public int weatherMapTiling = 5;
    [Tooltip("Tiling modification of lighting variance.")]
    [Range(0f, 5f)]
    public float lightingVarianceTiling = 1f;
    [Tooltip("Option to add own weather map. Red Channel = Coverage, Blue = Clouds Height")]
    public Texture2D customWeatherMap;
    [Tooltip("Weathermap sampling offset.")]
    public Vector2 locationOffset;
    [Range(0f, 1f)]
    [Tooltip("Weathermap animation speed.")]
    public float weatherAnimSpeedScale = 0.33f;

    [Header("Global Clouds Control")]
    [Range(0f, 2f)]
    public float globalCloudCoverage = 1f;

    [Tooltip("Texture for cirrus clouds.")]
    public Texture cirrusCloudsTexture;
    [Tooltip("Global Color for flat clouds based sun positon.")]
    public Gradient cirrusCloudsColor;
    [Range(5f, 15f)]
    [Tooltip("Flat Clouds Altitude")]
    public float cirrusCloudsAltitude = 10f;

    //Flat Clouds
    [Tooltip("Base texture for 2D clouds.")]
    public Texture2D flatCloudsBaseTexture;
    [Range(1f, 10f)]
    public float flatCloudsBaseTextureTiling = 2f;
    [Tooltip("Detail texture for 2D clouds.")]
    public Texture2D flatCloudsDetailTexture;
    [Range(1f, 20f)]
    public float flatCloudsDetailTextureTiling = 8f;
    [Tooltip("Direct Light Color for flat clouds based sun positon.")]
#if UNITY_2018_3_OR_NEWER
    [GradientUsageAttribute(true)]
#endif
    public Gradient flatCloudsDirectLightColor;
    [Tooltip("Light Color for flat clouds based sun positon.")]
#if UNITY_2018_3_OR_NEWER
    [GradientUsageAttribute(true)]
#endif
    public Gradient flatCloudsAmbientLightColor;

    [Tooltip("Flat Clouds Altitude")]
    [Range(50f, 250f)]
    public float flatCloudsAltitude = 150.0f;

    [Tooltip("Clouds Shadowcast Intensity. 0 = disabled")]
    [Range(0f, 1f)]
    public float shadowIntensity = 0.0f;
    [Tooltip("Size of the shadow cookie.")]
    [Range(100, 100000)]
    public int shadowCookieSize = 100000;

    public EnviroParticleClouds ParticleCloudsLayer1 = new EnviroParticleClouds();
    public EnviroParticleClouds ParticleCloudsLayer2 = new EnviroParticleClouds();
    [Tooltip("Enable this to use two layer of particle clouds.")]
    public bool dualLayerParticleClouds = true;
}

[Serializable]
public class EnviroParticleClouds
{
    [Tooltip("Particle clouds height.")]
    [Range(0f, 0.2f)]
    public float height = 0.1f;
    [Tooltip("Global Color for flat clouds based sun positon.")]
#if UNITY_2018_3_OR_NEWER
    [GradientUsageAttribute(true)]
#endif
    public Gradient particleCloudsColor;
}

#if ENVIRO_HD
[Serializable]
public class EnviroAuroraSettings
{
    public AnimationCurve auroraIntensity = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.5f, 0.1f), new Keyframe(1f, 0f));
    //
    [Header("Aurora Color and Brightness")]
    public Color auroraColor = new Color(0.1f, 0.5f, 0.7f);
    public float auroraBrightness = 75f;
    public float auroraContrast = 10f;
    //
    [Header("Aurora Height and Scale")]
    public float auroraHeight = 20000f;
    [Range(0f, 0.025f)]
    public float auroraScale = 0.01f;
    //
    [Header("Aurora Performance")]
    [Range(8, 32)]
    public int auroraSteps = 20;
    //
    [Header("Aurora Modelling and Animation")]
    public Vector4 auroraLayer1Settings = new Vector4(0.1f, 0.1f, 0f, 0.5f);
    public Vector4 auroraLayer2Settings = new Vector4(5f, 5f, 0f, 0.5f);
    public Vector4 auroraColorshiftSettings = new Vector4(0.05f, 0.05f, 0f, 5f);
    [Range(0f, 0.1f)]
    public float auroraSpeed = 0.005f;
}
#endif

#endregion

[Serializable]
public class EnviroProfile : ScriptableObject
{

    public string version;
    public EnviroLightSettings lightSettings = new EnviroLightSettings();
    public EnviroReflectionSettings reflectionSettings = new EnviroReflectionSettings();
    public EnviroVolumeLightingSettings volumeLightSettings = new EnviroVolumeLightingSettings();
    public EnviroDistanceBlurSettings distanceBlurSettings = new EnviroDistanceBlurSettings();
    public EnviroSkySettings skySettings = new EnviroSkySettings();
    public EnviroCloudSettings cloudsSettings = new EnviroCloudSettings();
    public EnviroWeatherSettings weatherSettings = new EnviroWeatherSettings();
    public EnviroFogSettings fogSettings = new EnviroFogSettings();
    public EnviroLightShaftsSettings lightshaftsSettings = new EnviroLightShaftsSettings();
    public EnviroSeasonSettings seasonsSettings = new EnviroSeasonSettings();
    public EnviroAudioSettings audioSettings = new EnviroAudioSettings();
    public EnviroSatellitesSettings satelliteSettings = new EnviroSatellitesSettings();
    public EnviroQualitySettings qualitySettings = new EnviroQualitySettings();
#if ENVIRO_HD
    public EnviroAuroraSettings auroraSettings = new EnviroAuroraSettings();
#endif
    // Inspector categories
    public enum settingsMode
    {
        Lighting,
        Sky,
        Reflections,
        Weather,
        Season,
        Clouds,
        Fog,
        VolumeLighting,
        Lightshafts,
        DistanceBlur,
        Aurora,
        Satellites,
        Audio,
        Quality
    }

    public enum settingsModeLW
    {
        Lighting,
        Sky,
        Reflections,
        Weather,
        Season,
        Clouds,
        Fog,
        Lightshafts,
        Satellites,
        Audio,
        Quality
    }

    [HideInInspector]
    public settingsMode viewMode;
    [HideInInspector]
    public settingsModeLW viewModeLW;
    [HideInInspector]
    public bool showPlayerSetup = true;
    [HideInInspector]
    public bool showRenderingSetup = false;
    [HideInInspector]
    public bool showComponentsSetup = false;
    [HideInInspector]
    public bool showTimeUI = false;
    [HideInInspector]
    public bool showWeatherUI = false;
    [HideInInspector]
    public bool showAudioUI = false;
    [HideInInspector]
    public bool showEffectsUI = false;
    [HideInInspector]
    public bool modified;
}

public static class EnviroProfileCreation
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/Enviro/Profile")]
    public static EnviroProfile CreateNewEnviroProfile()
    {
        EnviroProfile profile = ScriptableObject.CreateInstance<EnviroProfile>();

        profile.version = "2.4.1";
        // Setup new profile with default settings
        SetupDefaults(profile);

        // Create and save the new profile with unique name
        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets/Enviro - Sky and Weather/Core/Profiles";
        }
        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + "Enviro Profile" + ".asset");
        AssetDatabase.CreateAsset(profile, assetPathAndName);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        return profile;
    }
#endif

    public static void SetupDefaults(EnviroProfile profile)
    {
        EnviroProfile defaultProfile = GetDefaultProfile("enviro_internal_default_profile");
        profile.audioSettings = defaultProfile.audioSettings;
        profile.reflectionSettings = defaultProfile.reflectionSettings;
        profile.cloudsSettings = defaultProfile.cloudsSettings;
        profile.distanceBlurSettings = defaultProfile.distanceBlurSettings;
        profile.fogSettings = defaultProfile.fogSettings;
        profile.lightSettings = defaultProfile.lightSettings;
        profile.lightshaftsSettings = defaultProfile.lightshaftsSettings;
        profile.qualitySettings = defaultProfile.qualitySettings;
        profile.satelliteSettings = defaultProfile.satelliteSettings;
        profile.seasonsSettings = defaultProfile.seasonsSettings;
        profile.skySettings = defaultProfile.skySettings;
        profile.volumeLightSettings = defaultProfile.volumeLightSettings;
        profile.reflectionSettings = defaultProfile.reflectionSettings;
#if ENVIRO_HD
        profile.auroraSettings = defaultProfile.auroraSettings;
#endif
        profile.weatherSettings = defaultProfile.weatherSettings;
        profile.version = defaultProfile.version;
    }

    public static bool UpdateProfile(EnviroProfile profile, string fromV, string toV)
    {
        if (profile == null)
            return false;

        EnviroProfile defaultProfile = GetDefaultProfile("enviro_internal_default_profile");

        List<Color> gradientColors = new List<Color>();
        List<float> gradientTimes = new List<float>();

        if (fromV == "2.3.0" && toV == "2.4.2")
        {
            if (defaultProfile != null)
            {
                profile.cloudsSettings.flatCloudsBaseTexture = defaultProfile.cloudsSettings.flatCloudsBaseTexture;
                profile.cloudsSettings.flatCloudsDetailTexture = defaultProfile.cloudsSettings.flatCloudsDetailTexture;
                profile.cloudsSettings.flatCloudsDirectLightColor = defaultProfile.cloudsSettings.flatCloudsDirectLightColor;
                profile.cloudsSettings.flatCloudsAmbientLightColor = defaultProfile.cloudsSettings.flatCloudsAmbientLightColor;

                profile.skySettings.moonTexture = defaultProfile.skySettings.moonTexture;
                profile.skySettings.dithering = 0.015f;
                profile.fogSettings.fogDithering = 0.015f;
            
                profile.lightSettings.sunIntensityLux = defaultProfile.lightSettings.sunIntensityLux;
                profile.lightSettings.moonIntensityLux = defaultProfile.lightSettings.sunIntensityLux;
                profile.lightSettings.exposurePhysical = defaultProfile.lightSettings.exposurePhysical;
                profile.lightSettings.skyExposurePhysical = defaultProfile.lightSettings.skyExposurePhysical;
                profile.lightSettings.lightColorTemperature = defaultProfile.lightSettings.lightColorTemperature;
                profile.lightSettings.lightColorTint = defaultProfile.lightSettings.lightColorTint;
                profile.fogSettings.fogColorTint = defaultProfile.fogSettings.fogColorTint;

                profile.version = toV;
                #if UNITY_EDITOR
                EditorUtility.SetDirty(profile);
                #endif
                return true;
            }
            else return false;
        }

        if (fromV == "2.3.1" && toV == "2.4.2")
        {
            if (defaultProfile != null)
            {
                profile.lightSettings.sunIntensityLux = defaultProfile.lightSettings.sunIntensityLux;
                profile.lightSettings.moonIntensityLux = defaultProfile.lightSettings.sunIntensityLux;
                profile.lightSettings.exposurePhysical = defaultProfile.lightSettings.exposurePhysical;
                profile.lightSettings.skyExposurePhysical = defaultProfile.lightSettings.skyExposurePhysical;
                profile.lightSettings.lightColorTemperature = defaultProfile.lightSettings.lightColorTemperature;
                profile.lightSettings.lightColorTint = defaultProfile.lightSettings.lightColorTint;
                profile.fogSettings.fogColorTint = defaultProfile.fogSettings.fogColorTint;
                profile.version = toV;
                #if UNITY_EDITOR
                EditorUtility.SetDirty(profile);
                #endif
                return true;
            }
            else return false;
        }

        if (fromV == "2.3.2" || fromV == "2.3.3" || fromV == "2.4.0" || fromV == "2.4.1" && toV == "2.4.2")
        {
            if (defaultProfile != null)
            {
                profile.version = toV;
                #if UNITY_EDITOR
                EditorUtility.SetDirty(profile);
                #endif
                return true;
            }
            else return false;
        }


        return false;
    }

    public static GameObject GetAssetPrefab(string name)
    {
#if UNITY_EDITOR
        string[] assets = AssetDatabase.FindAssets(name, null);
        for (int idx = 0; idx < assets.Length; idx++)
        {
            string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
            if (path.Contains(".prefab"))
            {
                return AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
        }
#endif
        return null;
    }

    public static AudioClip GetAudioClip(string name)
    {
#if UNITY_EDITOR
        string[] assets = AssetDatabase.FindAssets(name, null);
        for (int idx = 0; idx < assets.Length; idx++)
        {
            string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
            if (path.Contains(".wav"))
            {
                return AssetDatabase.LoadAssetAtPath<AudioClip>(path);
            }
        }
#endif
        return null;
    }

    public static Cubemap GetAssetCubemap(string name)
    {
#if UNITY_EDITOR
        string[] assets = AssetDatabase.FindAssets(name, null);
        for (int idx = 0; idx < assets.Length; idx++)
        {
            string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
            if (path.Contains(".png"))
            {
                return AssetDatabase.LoadAssetAtPath<Cubemap>(path);
            }
            else if (path.Contains(".jpg"))
            {
                return AssetDatabase.LoadAssetAtPath<Cubemap>(path);
            }
        }
#endif
        return null;
    }

    public static EnviroProfile GetDefaultProfile(string name)
    {
#if UNITY_EDITOR
        string[] assets = AssetDatabase.FindAssets(name, null);

        for (int idx = 0; idx < assets.Length; idx++)
        {
            string path = AssetDatabase.GUIDToAssetPath(assets[idx]);

            if (path.Contains(".asset"))
            {
                return AssetDatabase.LoadAssetAtPath<EnviroProfile>(path);
            }
        }
#endif
        return null;
    }

    public static Texture GetAssetTexture(string name)
    {
#if UNITY_EDITOR
        string[] assets = AssetDatabase.FindAssets(name, null);
        for (int idx = 0; idx < assets.Length; idx++)
        {
            string path = AssetDatabase.GUIDToAssetPath(assets[idx]);
            if (path.Length > 0)
            {
                return AssetDatabase.LoadAssetAtPath<Texture>(path);
            }
        }
#endif
        return null;
    }

    public static Gradient CreateGradient(Color clr1, float time1, Color clr2, float time2)
    {
        Gradient nG = new Gradient();
        GradientColorKey[] gClr = new GradientColorKey[2];
        GradientAlphaKey[] gAlpha = new GradientAlphaKey[2];

        gClr[0].color = clr1;
        gClr[0].time = time1;
        gClr[1].color = clr2;
        gClr[1].time = time2;

        gAlpha[0].alpha = 1f;
        gAlpha[0].time = 0f;
        gAlpha[1].alpha = 1f;
        gAlpha[1].time = 1f;

        nG.SetKeys(gClr, gAlpha);

        return nG;
    }

    public static Gradient CreateGradient(List<Color> clrs, List<float> times)
    {
        Gradient nG = new Gradient();

        GradientColorKey[] gClr = new GradientColorKey[clrs.Count];
        GradientAlphaKey[] gAlpha = new GradientAlphaKey[2];

        for (int i = 0; i < clrs.Count; i++)
        {
            gClr[i].color = clrs[i];
            gClr[i].time = times[i];
        }

        gAlpha[0].alpha = 1f;
        gAlpha[0].time = 0f;
        gAlpha[1].alpha = 1f;
        gAlpha[1].time = 1f;

        nG.SetKeys(gClr, gAlpha);
        return nG;
    }

    public static Gradient CreateGradient(List<Color> clrs, List<float> times, List<float> alpha, List<float> timesAlpha)
    {
        Gradient nG = new Gradient();

        GradientColorKey[] gClr = new GradientColorKey[clrs.Count];
        GradientAlphaKey[] gAlpha = new GradientAlphaKey[alpha.Count];

        for (int i = 0; i < clrs.Count; i++)
        {
            gClr[i].color = clrs[i];
            gClr[i].time = times[i];
        }

        for (int i = 0; i < alpha.Count; i++)
        {
            gAlpha[i].alpha = alpha[i];
            gAlpha[i].time = timesAlpha[i];
        }

        nG.SetKeys(gClr, gAlpha);
        return nG;
    }

    public static Color GetColor(string hex)
    {
        Color clr = new Color();
        ColorUtility.TryParseHtmlString(hex, out clr);
        return clr;
    }

    public static Keyframe CreateKey(float value, float time)
    {
        Keyframe k = new Keyframe();
        k.value = value;
        k.time = time;
        return k;
    }

    public static Keyframe CreateKey(float value, float time, float inTangent, float outTangent)
    {
        Keyframe k = new Keyframe();
        k.value = value;
        k.time = time;
        k.inTangent = inTangent;
        k.outTangent = outTangent;
        return k;
    }
}
