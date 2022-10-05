using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#region Classes
[Serializable]
public class EnviroSeasons
{
    public enum Seasons
    {
        Spring,
        Summer,
        Autumn,
        Winter,
    }
    [Tooltip("When enabled the system will change seasons automaticly when enough days passed.")]
    public bool calcSeasons; // if unticked you can manually overwrite current seas. Ticked = automaticly updates seasons
    [Tooltip("The current season.")]
    public Seasons currentSeasons;
    [HideInInspector]
    public Seasons lastSeason;
}

[Serializable]
public class EnviroLightshafts
{
    [Tooltip("Use sunlight shafts?")]
    public bool sunLightShafts = true;
    [Tooltip("Use moonlight shafts?")]
    public bool moonLightShafts = true;
}

[Serializable]
public class EnviroAudio // AudioSetup variables
{
    [Tooltip("The prefab with AudioSources used by Enviro. Will be instantiated at runtime.")]
    public GameObject SFXHolderPrefab;

    [Range(0f, 1f)]
    [Tooltip("The volume of ambient sounds played by enviro.")]
    public float ambientSFXVolume = 0.5f;
    [Range(0f, 1f)]
    [Tooltip("The volume of weather sounds played by enviro.")]
    public float weatherSFXVolume = 1.0f;

    [HideInInspector]
    public EnviroAudioSource currentAmbientSource;
    [HideInInspector]
    public float ambientSFXVolumeMod = 0f;
    [HideInInspector]
    public float weatherSFXVolumeMod = 0f;

    [HideInInspector]
    public EnviroAudioSource AudioSourceWeather;
    [HideInInspector]
    public EnviroAudioSource AudioSourceWeather2;
    [HideInInspector]
    public EnviroAudioSource AudioSourceAmbient;
    [HideInInspector]
    public EnviroAudioSource AudioSourceAmbient2;
    [HideInInspector]
    public EnviroAudioSource AudioSourceThunder;
    [HideInInspector]
    public EnviroAudioSource AudioSourceZone;
}

[Serializable]
public class EnviroParticleCloud
{
    public ParticleSystem layer1System;
    public Material layer1Material;
    public ParticleSystem layer2System;
    public Material layer2Material;
}

[Serializable]
public class EnviroComponents // References - setup these in inspector! Or use the provided prefab.
{
    [Tooltip("The Enviro sun object.")]
    public GameObject Sun = null;
    [Tooltip("The Enviro moon object.")]
    public GameObject Moon = null;
    [Tooltip("The directional light for directional sun lighting when using dual mode. Used for sun and moon in single mode.")]
    public Transform DirectLight;
    [Tooltip("The directional light for directional moon lighting when using the dual mode.")]
    public Transform AdditionalDirectLight;
    [Tooltip("The Enviro global reflection probe for dynamic reflections.")]
    public EnviroReflectionProbe GlobalReflectionProbe;
    [Tooltip("Your WindZone that reflect our weather wind settings.")]
    public WindZone windZone;
    [Tooltip("The Enviro Lighting Flash Component.")]
    public EnviroLightning LightningGenerator; // Creates lightning Flashes
    [Tooltip("Link to the object that hold all additional satellites as childs.")]
    public Transform satellites;
    [Tooltip("Just a transform for stars rotation calculations. ")]
    public Transform starsRotation = null;
    [Tooltip("Plane to cast cloud shadows.")]
    public GameObject particleClouds = null;
}

[Serializable]
public class EnviroTime //GameTime Variables
{
    public enum TimeProgressMode
    {
        None,
        Simulated,
        OneDay,
        SystemTime
    }

    [Tooltip("None = No time auto time progressing, Simulated = Time calculated with DayLenghtInMinutes, SystemTime = uses your systemTime.")]
    public TimeProgressMode ProgressTime = TimeProgressMode.Simulated;
    [Tooltip("Current Time: minutes")]
    [Range(0, 60)]
    public int Seconds = 0;
    [Tooltip("Current Time: minutes")]
    [Range(0, 60)]
    public int Minutes = 0;
    [Tooltip("Current Time: hours")]
    [Range(0, 24)]
    public int Hours = 12;
    [Tooltip("Current Time: Days")]
    public int Days = 1;
    [Tooltip("Current Time: Years")]
    public int Years = 1;
    [Tooltip("How many days in one year?")]
    public int DaysInYear = 365;
    [Tooltip("Ful 24h cycle in realtime minutes.")]
    public float cycleLengthInMinutes = 5f; 
    [Tooltip("Day lenght modifier.")]
    [Range(0.1f, 10f)]
    public float dayLengthModifier = 1f;
    [Tooltip("Night lenght modifier.")]
    [Range(0.1f, 10f)]
    public float nightLengthModifier = 1f;
    [Range(-13, 13)]
    [Tooltip("Time offset for timezones")]
    public int utcOffset = 0;
    [Range(-90, 90)]
    [Tooltip("-90,  90   Horizontal earth lines")]
    public float Latitude = 0f;
    [Range(-180, 180)]
    [Tooltip("-180, 180  Vertical earth line")]
    public float Longitude = 0f;
    [HideInInspector]
    public float solarTime;
    [HideInInspector]
    public float lunarTime;
    [Tooltip("This setting will change the timing when system switches from day to night and night to day. It uses the sun position in sky: 0 -> Night, ~0.5 -> Dawn/Dusk, 1 -> Midday.")]
    [Range(0.3f, 0.7f)]
    public float dayNightSwitch = 0.45f;
}

[Serializable]
public class EnviroInteriorZoneSettings
{
    [HideInInspector]
    public Color currentInteriorDirectLightMod;
    [HideInInspector]
    public Color currentInteriorAmbientLightMod;
    [HideInInspector]
    public Color currentInteriorAmbientEQLightMod;
    [HideInInspector]
    public Color currentInteriorAmbientGRLightMod;
    [HideInInspector]
    public Color currentInteriorSkyboxMod;
    [HideInInspector]
    public Color currentInteriorFogColorMod = new Color(0, 0, 0, 0);
    [HideInInspector]
    public float currentInteriorFogMod = 1f;
    [HideInInspector]
    public float currentInteriorWeatherEffectMod = 1f;
    [HideInInspector]
    public float currentInteriorZoneAudioVolume = 1f;
    [HideInInspector]
    public float currentInteriorZoneAudioFadingSpeed = 1f;
}

[Serializable]
public class EnviroSatellite
{
    [Tooltip("Name of this satellite")]
    public string name;
    [Tooltip("Prefab with model that get instantiated.")]
    public GameObject prefab = null;

    [Tooltip("Orbit distance.")]
    public float orbit;
    [Tooltip("Orbit modification on x axis.")]
    public float xRot;
    [Tooltip("Orbit modification on y axis.")]
    public float yRot;
    [Tooltip("Orbit modification on z axis.")]
    public float zRot;
}

[Serializable]
public class EnviroWeather
{
    [Tooltip("If disabled the weather will never change.")]
    public bool updateWeather = true;
    // [HideInInspector]
    public List<EnviroWeatherPreset> weatherPresets = new List<EnviroWeatherPreset>();
    // [HideInInspector]
    public List<EnviroWeatherPrefab> WeatherPrefabs = new List<EnviroWeatherPrefab>();
    [Tooltip("List of additional zones. Will be updated on startup!")]
    public List<EnviroZone> zones = new List<EnviroZone>();
    public EnviroWeatherPreset startWeatherPreset;
    [Tooltip("The current active zone.")]
    public EnviroZone currentActiveZone;
    [Tooltip("The current active weather conditions.")]
    public EnviroWeatherPrefab currentActiveWeatherPrefab;
    public EnviroWeatherPreset currentActiveWeatherPreset;

    [HideInInspector]
    public EnviroWeatherPrefab lastActiveWeatherPrefab;
    [HideInInspector]
    public EnviroWeatherPreset lastActiveWeatherPreset;

    [HideInInspector]
    public GameObject VFXHolder;
    public GameObject SFXHolder;
    [HideInInspector]
    public float wetness;
    [HideInInspector]
    public float curWetness;
    [HideInInspector]
    public float snowStrength;
    [HideInInspector]
    public float curSnowStrength;
    [HideInInspector]
    public int thundersfx;
    [HideInInspector]
    public EnviroAudioSource currentAudioSource;
    [HideInInspector]
    public bool weatherFullyChanged = false;
    [HideInInspector]
    public float currentTemperature;
    [HideInInspector]
    public float temperatureModifier = 0f;

}

[Serializable]
public class EnviroFogging
{
    [HideInInspector]
    public float skyFogStart = 0f;
    [HideInInspector]
    public float skyFogHeight = 1f;
    [HideInInspector]
    public float skyFogIntensity = 0.1f;
    [HideInInspector]
    public float skyFogStrength = 0.1f;
    [HideInInspector]
    public float scatteringStrenght = 0.5f;
    [HideInInspector]
    public float sunBlocking = 0.5f;
    [HideInInspector]
    public float moonIntensity = 1f;

    [HideInInspector]
    public float hdrpRelativeFogHeight = 0f;
}
#endregion

public class EnviroCore : MonoBehaviour
{
    #region General  
    //Profile
    [Header("Profile")]
    public EnviroProfile profile = null;
    [HideInInspector]
    public bool profileLoaded = false;

    //Setup
    [Tooltip("Assign your player gameObject here. Required Field! or enable AssignInRuntime!")]
    public GameObject Player;
    [Tooltip("Assign your main camera here. Required Field! or enable AssignInRuntime!")]
    public Camera PlayerCamera;
    [Tooltip("If enabled Enviro will search for your Player and Camera by Tag!")]
    public bool AssignInRuntime;
    [Tooltip("Your Player Tag")]
    public string PlayerTag = "";
    [Tooltip("Your CameraTag")]
    public string CameraTag = "MainCamera";
    [Tooltip("Enables enviro tonemapping. Disable that one later and use a third party tonemapping effect for best results!")]
    public bool tonemapping = false;

    public enum EnviroStartMode
    {
        Started,
        Paused,
        PausedButTimeProgress
    }

    public EnviroStartMode startMode;

    //Runtime
    [HideInInspector]
    public bool started;
    [HideInInspector]
    public bool serverMode = false;
    [HideInInspector]
    public EnviroWeatherCloudsConfig cloudsConfig;
    [HideInInspector]
    public float thunder = 0f;
    [HideInInspector]
    public bool isNight = true;

    // Satellites
    [HideInInspector]
    public List<GameObject> satellites = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> satellitesRotation = new List<GameObject>();

    // Vegeation Growth
    [HideInInspector]
    public List<EnviroVegetationInstance> EnviroVegetationInstances = new List<EnviroVegetationInstance>();

    // Runtime profile
    [HideInInspector]
    public EnviroLightSettings lightSettings = new EnviroLightSettings();
    [HideInInspector]
    public EnviroVolumeLightingSettings volumeLightSettings = new EnviroVolumeLightingSettings();
    [HideInInspector]
    public EnviroSkySettings skySettings = new EnviroSkySettings();
    [HideInInspector]
    public EnviroReflectionSettings reflectionSettings = new EnviroReflectionSettings();
    [HideInInspector]
    public EnviroCloudSettings cloudsSettings = new EnviroCloudSettings();
    [HideInInspector]
    public EnviroWeatherSettings weatherSettings = new EnviroWeatherSettings();
    [HideInInspector]
    public EnviroFogSettings fogSettings = new EnviroFogSettings();
    [HideInInspector]
    public EnviroLightShaftsSettings lightshaftsSettings = new EnviroLightShaftsSettings();
    [HideInInspector]
    public EnviroSeasonSettings seasonsSettings = new EnviroSeasonSettings();
    [HideInInspector]
    public EnviroAudioSettings audioSettings = new EnviroAudioSettings();
    [HideInInspector]
    public EnviroSatellitesSettings satelliteSettings = new EnviroSatellitesSettings();
    [HideInInspector]
    public EnviroQualitySettings qualitySettings = new EnviroQualitySettings();
    [HideInInspector]
    public EnviroInteriorZoneSettings interiorZoneSettings = new EnviroInteriorZoneSettings();
    [HideInInspector]
    public EnviroDistanceBlurSettings distanceBlurSettings = new EnviroDistanceBlurSettings();
#if ENVIRO_HD
    [HideInInspector]
    public EnviroAuroraSettings auroraSettings = new EnviroAuroraSettings();
#endif

    // Time
    [HideInInspector]
    public DateTime dateTime;
    [HideInInspector]
    public float internalHour;
    [HideInInspector]
    public float currentHour;
    [HideInInspector]
    public float currentDay;
    [HideInInspector]
    public float currentYear;
    [HideInInspector]
    public double currentTimeInHours;
    [HideInInspector]
    public float LST;
    [HideInInspector]
    public float lastHourUpdate;
    [HideInInspector]
    public float hourTime;


    //Wind
    [HideInInspector]
    public Vector3 cloudAnim;
    [HideInInspector]
    public Vector2 cloudFlatBaseAnim;
    [HideInInspector]
    public Vector2 cloudFlatDetailAnim;

    [HideInInspector]
    public Vector2 cloudAnimNonScaled;
    [HideInInspector]
    public Vector2 cirrusAnim;
    [HideInInspector]
    public float windIntensity = 0;

    //Shadows
    [HideInInspector]
    public float shadowIntensityMod;

    //Interior Mods
    [HideInInspector]
    public bool interiorMode = false;
    [HideInInspector]
    public EnviroInterior lastInteriorZone;


    //AQUAS Fog Handling
    [HideInInspector]
    public bool updateFogDensity = true;
    [HideInInspector]
    public Color customFogColor = Color.black;
    [HideInInspector]
    public float customFogIntensity = 0f;

    //weather
    [HideInInspector]
    public Color currentWeatherSkyMod;
    [HideInInspector]
    public Color currentWeatherLightMod;
    [HideInInspector]
    public Color currentWeatherFogMod;

    //sky
    [HideInInspector]
    [Range(-2f, 2f)]
    public float customMoonPhase = 0.0f;
    //VolumeLighting
    public float globalVolumeLightIntensity;
    //References
    public Light MainLight;
    public Light AdditionalLight;
    public Transform MoonTransform;
#if ENVIRO_HDRP
    public UnityEngine.Rendering.Volume enviroVolumeHDRP;
    public UnityEngine.Rendering.HighDefinition.HDAdditionalLightData MainLightHDRP;
    public UnityEngine.Rendering.HighDefinition.HDAdditionalLightData AdditionalLightHDRP;
#endif

#if ENVIRO_LWRP
    public UnityEngine.Rendering.Universal.UniversalAdditionalLightData MainLightURP; 
#endif
    //Lighting
    [HideInInspector]
    public float lastAmbientSkyUpdate;
    [HideInInspector]
    public double lastRelfectionUpdate;
    [HideInInspector]
    public Vector3 lastRelfectionPositionUpdate;
    public Cubemap defaultSkyReflectionTex;
    public float directLightIntensityMod = 1f;
    public float ambientLightIntensityMod = 1f;
    //SFX
    [HideInInspector]
    public GameObject EffectsHolder;

    //Lightning
    public ParticleSystem lightningEffect;

    // Scattering constants
    public const float pi = Mathf.PI;
    private Vector3 K = new Vector3(686.0f, 678.0f, 666.0f);
    private const float n = 1.0003f;
    private const float N = 2.545E25f;
    private const float pn = 0.035f;

    // Classes
    public EnviroTime GameTime = null;
    public EnviroAudio Audio = null;
    public EnviroWeather Weather = null;
    public EnviroSeasons Seasons = null;
    public EnviroComponents Components = null;
    public EnviroFogging Fog = null;
    public EnviroLightshafts LightShafts = null;
    public EnviroParticleCloud particleClouds = null;

    //Effects
    [HideInInspector]
    public EnviroPostProcessing EnviroPostProcessing;

    //Layers
    [Header("Layer Setup")]
    [Tooltip("This is the layer id forfor the moon.")]
    public int moonRenderingLayer = 29;
    [Tooltip("This is the layer id for additional satellites like moons, planets.")]
    public int satelliteRenderingLayer = 30;
    [Tooltip("Activate to set recommended maincamera clear flag.")]
    public bool setCameraClearFlags = true;

 
    //HDRP
    [HideInInspector]
    public int frames = 0;
    [HideInInspector]
    public int lightingUpdateEachFrames = 10;
    public float currentSceneExposureMod = 1f;
    public float currentSkyExposureMod = 1f;
    public float currentLightIntensityMod = 1f;

#if ENVIRO_HDRP
    public UnityEngine.Rendering.HighDefinition.VisualEnvironment visualEnvironment;
#if ENVIRO_HD
    public UnityEngine.Rendering.HighDefinition.EnviroSkybox enviroHDRPSky;
#endif
#if ENVIRO_LW
    public UnityEngine.Rendering.HighDefinition.EnviroSkyboxLite enviroHDRPSkyLite;
#endif
    public UnityEngine.Rendering.HighDefinition.Exposure enviroExposure;
    public UnityEngine.Rendering.HighDefinition.Fog hdrpFog;
    public GameObject mySkyAndFogVolume;
#endif
    public List<Camera> additionalCameras;


    public bool RenderEnviroOnThisCam(Camera cam)
    {
        if(cam.cameraType == CameraType.SceneView)
           return true;

        if(cam == PlayerCamera)
           return true;

        if(Components.GlobalReflectionProbe.renderCam != null && cam == Components.GlobalReflectionProbe.renderCam)
           return true;

        if(Components.GlobalReflectionProbe.bakingCam != null && cam == Components.GlobalReflectionProbe.bakingCam)
           return true;

        for (int i = 0; i < additionalCameras.Count; i++)
        {
            if(cam == additionalCameras[i])
               return true;
        }

        return false;
    }
    
    public void UpdateEnviroment() // Update the all GrowthInstances
    {
        // Set correct Season.
        if (Seasons.calcSeasons)
            UpdateSeason();

        // Update all EnviroGrowInstancesSeason in scene!
        if (EnviroVegetationInstances.Count > 0)
        {
            for (int i = 0; i < EnviroVegetationInstances.Count; i++)
            {
                if (EnviroVegetationInstances[i] != null)
                    EnviroVegetationInstances[i].UpdateInstance();
            }
        }
    }

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Ambient and Weather SFX  

    public void PlayAmbient(AudioClip sfx)
    {
        if (sfx == Audio.currentAmbientSource.audiosrc.clip)
        {
            if (!Audio.currentAmbientSource.audiosrc.isPlaying)
                Audio.currentAmbientSource.audiosrc.Play();
            return;
        }
        if (Audio.currentAmbientSource == Audio.AudioSourceAmbient)
        {
            Audio.AudioSourceAmbient.FadeOut();
            Audio.AudioSourceAmbient2.FadeIn(sfx);
            Audio.currentAmbientSource = Audio.AudioSourceAmbient2;
        }
        else if (Audio.currentAmbientSource == Audio.AudioSourceAmbient2)
        {
            Audio.AudioSourceAmbient2.FadeOut();
            Audio.AudioSourceAmbient.FadeIn(sfx);
            Audio.currentAmbientSource = Audio.AudioSourceAmbient;
        }
    }


    public void TryPlayAmbientSFX()
    {
        if (Weather.currentActiveWeatherPreset == null)
            return;

        if (isNight)
        {
            switch (Seasons.currentSeasons)
            {
                case EnviroSeasons.Seasons.Spring:
                    if (Weather.currentActiveWeatherPreset.SpringNightAmbient != null)
                        PlayAmbient(Weather.currentActiveWeatherPreset.SpringNightAmbient);
                    else
                    {
                        Audio.AudioSourceAmbient.FadeOut();
                        Audio.AudioSourceAmbient2.FadeOut();
                    }
                    break;

                case EnviroSeasons.Seasons.Summer:
                    if (Weather.currentActiveWeatherPreset.SummerNightAmbient != null)
                        PlayAmbient(Weather.currentActiveWeatherPreset.SummerNightAmbient);
                    else
                    {
                        Audio.AudioSourceAmbient.FadeOut();
                        Audio.AudioSourceAmbient2.FadeOut();
                    }
                    break;
                case EnviroSeasons.Seasons.Autumn:
                    if (Weather.currentActiveWeatherPreset.AutumnNightAmbient != null)
                        PlayAmbient(Weather.currentActiveWeatherPreset.AutumnNightAmbient);
                    else
                    {
                        Audio.AudioSourceAmbient.FadeOut();
                        Audio.AudioSourceAmbient2.FadeOut();
                    }
                    break;
                case EnviroSeasons.Seasons.Winter:
                    if (Weather.currentActiveWeatherPreset.WinterNightAmbient != null)
                        PlayAmbient(Weather.currentActiveWeatherPreset.WinterNightAmbient);
                    else
                    {
                        Audio.AudioSourceAmbient.FadeOut();
                        Audio.AudioSourceAmbient2.FadeOut();
                    }
                    break;
            }
        }
        else
        {
            switch (Seasons.currentSeasons)
            {
                case EnviroSeasons.Seasons.Spring:
                    if (Weather.currentActiveWeatherPreset.SpringDayAmbient != null)
                        PlayAmbient(Weather.currentActiveWeatherPreset.SpringDayAmbient);
                    else
                    {
                        Audio.AudioSourceAmbient.FadeOut();
                        Audio.AudioSourceAmbient2.FadeOut();
                    }
                    break;
                case EnviroSeasons.Seasons.Summer:
                    if (Weather.currentActiveWeatherPreset.SummerDayAmbient != null)
                        PlayAmbient(Weather.currentActiveWeatherPreset.SummerDayAmbient);
                    else
                    {
                        Audio.AudioSourceAmbient.FadeOut();
                        Audio.AudioSourceAmbient2.FadeOut();
                    }
                    break;
                case EnviroSeasons.Seasons.Autumn:
                    if (Weather.currentActiveWeatherPreset.AutumnDayAmbient != null)
                        PlayAmbient(Weather.currentActiveWeatherPreset.AutumnDayAmbient);
                    else
                    {
                        Audio.AudioSourceAmbient.FadeOut();
                        Audio.AudioSourceAmbient2.FadeOut();
                    }
                    break;
                case EnviroSeasons.Seasons.Winter:
                    if (Weather.currentActiveWeatherPreset.WinterDayAmbient != null)
                        PlayAmbient(Weather.currentActiveWeatherPreset.WinterDayAmbient);
                    else
                    {
                        Audio.AudioSourceAmbient.FadeOut();
                        Audio.AudioSourceAmbient2.FadeOut();
                    }
                    break;
            }
        }
    }

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Effects Setup
    /// <summary>
    /// Create Effect Holder Gameobject
    /// </summary>
    public void CreateWeatherEffectHolder(string name)
    {

        if (Weather.VFXHolder == null)
        {
            Weather.VFXHolder = GameObject.Find(name + "/VFX");
        }

        if (Weather.VFXHolder == null)
        {
            GameObject VFX = new GameObject();
            VFX.name = "VFX";
            VFX.transform.parent = EffectsHolder.transform;
            VFX.transform.localPosition = Vector3.zero;
            Weather.VFXHolder = VFX;
        }
    }

    /// <summary>
    /// Create Effect Holder Gameobject and adds audiofeatures.
    /// </summary>
    public void CreateEffects(string name)
    {

        if (EffectsHolder == null)
            EffectsHolder = GameObject.Find(name);

        if (EffectsHolder == null)
        {
            EffectsHolder = new GameObject();
            EffectsHolder.name = name;
            // Fix from Thorskin - Make sure that Effect Object will be moved to same scene as envirosky object.
            EffectsHolder.transform.parent = transform;
            EffectsHolder.transform.parent = null;
        }
        /*else
        {
            int childs = EffectsHolder.transform.childCount;

            for (int i = childs - 1; i >= 0; i--)
            {
                DestroyImmediate(EffectsHolder.transform.GetChild(i).gameObject);
            }
        }*/

        CreateWeatherEffectHolder(name);

        if (Application.isPlaying && EnviroSkyMgr.instance.dontDestroy)
            DontDestroyOnLoad(EffectsHolder);

        if (Player != null)
            EffectsHolder.transform.position = Player.transform.position;
        else
            EffectsHolder.transform.position = transform.position;

        if(Weather.SFXHolder == null)
           Weather.SFXHolder = GameObject.Find(name + "/SFX Holder(Clone)");

        if (Weather.SFXHolder == null)
        {
            Weather.SFXHolder = (GameObject)Instantiate(Audio.SFXHolderPrefab, Vector3.zero, Quaternion.identity);
            Weather.SFXHolder.transform.parent = EffectsHolder.transform;
        }

        EnviroAudioSource[] srcs = Weather.SFXHolder.GetComponentsInChildren<EnviroAudioSource>();

        for (int i = 0; i < srcs.Length; i++)
        {
            switch (srcs[i].myFunction)
            {
                case EnviroAudioSource.AudioSourceFunction.Weather1:
                    Audio.AudioSourceWeather = srcs[i];
                    break;
                case EnviroAudioSource.AudioSourceFunction.Weather2:
                    Audio.AudioSourceWeather2 = srcs[i];
                    break;
                case EnviroAudioSource.AudioSourceFunction.Ambient:
                    Audio.AudioSourceAmbient = srcs[i];
                    break;
                case EnviroAudioSource.AudioSourceFunction.Ambient2:
                    Audio.AudioSourceAmbient2 = srcs[i];
                    break;
                case EnviroAudioSource.AudioSourceFunction.Thunder:
                    Audio.AudioSourceThunder = srcs[i];
                    break;
                case EnviroAudioSource.AudioSourceFunction.ZoneAmbient:
                    Audio.AudioSourceZone = srcs[i];
                    break;
            }
        }

        Weather.currentAudioSource = Audio.AudioSourceWeather;
        Audio.currentAmbientSource = Audio.AudioSourceAmbient;
        TryPlayAmbientSFX();
    }

    public Transform CreateDirectionalLight(bool additional)
    {
        GameObject newGO = new GameObject();

        if (!additional)
            newGO.name = "Enviro Directional Light";
        else
            newGO.name = "Enviro Directional Light - Moon";

        newGO.transform.parent = transform;
        newGO.transform.parent = null;
        Light newLight = newGO.AddComponent<Light>();
        newLight.type = LightType.Directional;
        newLight.shadows = LightShadows.Soft;
        return newGO.transform;
    }
    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Scattering
    public Vector3 BetaRay(Vector3 waveLength)
    {
        Vector3 Br;

        Vector3 realWavelength = waveLength * 1.0e-9f;

        Br.x = (((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f))) * (6.0f + 3.0f * pn)) / ((3.0f * N * Mathf.Pow(realWavelength.x, 4.0f)) * (6.0f - 7.0f * pn))) * 2000f;
        Br.y = (((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f))) * (6.0f + 3.0f * pn)) / ((3.0f * N * Mathf.Pow(realWavelength.y, 4.0f)) * (6.0f - 7.0f * pn))) * 2000f;
        Br.z = (((8.0f * Mathf.Pow(pi, 3.0f) * (Mathf.Pow(Mathf.Pow(n, 2.0f) - 1.0f, 2.0f))) * (6.0f + 3.0f * pn)) / ((3.0f * N * Mathf.Pow(realWavelength.z, 4.0f)) * (6.0f - 7.0f * pn))) * 2000f;

        return Br;
    }


    public Vector3 BetaMie(float turbidity, Vector3 waveLength)
    {
        Vector3 Bm;

        float c = (0.2f * turbidity) * 10.0f;

        Bm.x = (434.0f * c * pi * Mathf.Pow((2.0f * pi) / waveLength.x, 2.0f) * K.x);
        Bm.y = (434.0f * c * pi * Mathf.Pow((2.0f * pi) / waveLength.y, 2.0f) * K.y);
        Bm.z = (434.0f * c * pi * Mathf.Pow((2.0f * pi) / waveLength.z, 2.0f) * K.z);

        Bm.x = Mathf.Pow(Bm.x, -1.0f);
        Bm.y = Mathf.Pow(Bm.y, -1.0f);
        Bm.z = Mathf.Pow(Bm.z, -1.0f);

        return Bm;
    }

    public Vector3 GetMieG(float g)
    {
        if (g == 1.0f)
            g = 0.99f;

        return new Vector3(1.0f - g * g, 1.0f + g * g, 2.0f * g);
    }

    public Vector3 GetMieGScene(float g)
    {
        if (g == 1.0f)
            g = 0.99f;

        return new Vector3(1.0f - g * g, 1.0f + g * g, 2.0f * g);
    }

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Time and sun/moon/stars position

    // Update the GameTime
    public void UpdateTime(int daysInYear)
    {
        if (Application.isPlaying)
        {

            float t = 0f;

            float timeProgressionSpeed = GameTime.cycleLengthInMinutes;


            if (!isNight)
                t = (24.0f / 60.0f) / (timeProgressionSpeed * GameTime.dayLengthModifier);
            else
                t = (24.0f / 60.0f) / (timeProgressionSpeed * GameTime.nightLengthModifier);

            hourTime = t * Time.deltaTime;

            switch (GameTime.ProgressTime)
            {
                case EnviroTime.TimeProgressMode.None://Set Time over editor or other scripts.
                    SetTime(GameTime.Years, GameTime.Days, GameTime.Hours, GameTime.Minutes, GameTime.Seconds);
                    break;
                case EnviroTime.TimeProgressMode.Simulated:
                    internalHour += hourTime;
                    SetGameTime();
                    //customMoonPhase += Time.deltaTime / (30f * (GameTime.DayLengthInMinutes * 60f)) * 2f;
                    break;
                case EnviroTime.TimeProgressMode.OneDay:
                    internalHour += hourTime;
                    SetGameTime();
                    //customMoonPhase += Time.deltaTime / (30f * (GameTime.DayLengthInMinutes * 60f)) * 2f;
                    break;
                case EnviroTime.TimeProgressMode.SystemTime:
                    SetTime(System.DateTime.Now);
                    // customMoonPhase += Time.deltaTime / (30f * (1440f * 60f)) * 2f;
                    break;
            }
        }
        else
        {
            SetTime(GameTime.Years, GameTime.Days, GameTime.Hours, GameTime.Minutes, GameTime.Seconds);
        }

        //if (customMoonPhase < -1) customMoonPhase += 2;
        //else if (customMoonPhase > 1) customMoonPhase -= 2;

        //Fire OnHour Event
        if (internalHour > (lastHourUpdate + 1f))
        {
            lastHourUpdate = internalHour;
            EnviroSkyMgr.instance.NotifyHourPassed();
        }

        // Check Days
        if (GameTime.Days >= daysInYear)
        {
            GameTime.Years = GameTime.Years + 1;
            GameTime.Days = 0;
            EnviroSkyMgr.instance.NotifyYearPassed();
        }

        currentHour = internalHour;
        currentDay = GameTime.Days;
        currentYear = GameTime.Years;

        currentTimeInHours = GetInHours(internalHour, currentDay, currentYear, daysInYear);
    }

    public void SetInternalTime(int year, int dayOfYear, int hour, int minute, int seconds)
    {
        GameTime.Years = year;
        GameTime.Days = dayOfYear;
        GameTime.Minutes = minute;
        GameTime.Hours = hour;
        internalHour = hour + (minute * 0.0166667f) + (seconds * 0.000277778f);
    }

    /// <summary>
    /// Updates Game Time days and years. Used internaly only.
    /// </summary>
    public void SetGameTime()
    {
        if (internalHour >= 24f)
        {
            internalHour = internalHour - 24f;
            EnviroSkyMgr.instance.NotifyHourPassed();
            lastHourUpdate = internalHour;
            if (GameTime.ProgressTime != EnviroTime.TimeProgressMode.OneDay)
            {
                GameTime.Days = GameTime.Days + 1;
                EnviroSkyMgr.instance.NotifyDayPassed();
            }
        }
        else if (internalHour < 0f)
        {
            internalHour = 24f + internalHour;
            lastHourUpdate = internalHour;

            if (GameTime.ProgressTime != EnviroTime.TimeProgressMode.OneDay)
            {
                GameTime.Days = GameTime.Days - 1;
                EnviroSkyMgr.instance.NotifyDayPassed();
            }
        }

        float inHours = internalHour;
        GameTime.Hours = (int)(inHours);
        inHours -= GameTime.Hours;
        GameTime.Minutes = (int)(inHours * 60f);
        inHours -= GameTime.Minutes * 0.0166667f;
        GameTime.Seconds = (int)(inHours * 3600f);
    }

    /// <summary>
    /// Set the exact date. by DateTime
    /// </summary>
    public void SetTime(DateTime date)
    {
        GameTime.Years = date.Year;
        GameTime.Days = date.DayOfYear;
        GameTime.Minutes = date.Minute;
        GameTime.Seconds = date.Second;
        GameTime.Hours = date.Hour;
        internalHour = date.Hour + (date.Minute * 0.0166667f) + (date.Second * 0.000277778f);
    }

    /// <summary>
    /// Set the hourly event time to current hour. Use this if you change date and want to reset hourly events to be executed in next hour after time change.
    /// </summary>
    public void ResetHourEventTimer()
    {
        lastHourUpdate = internalHour;
    }

    /// <summary>
    /// Set the exact date.
    /// </summary>
    public void SetTime(int year, int dayOfYear, int hour, int minute, int seconds)
    {
        GameTime.Years = year;
        GameTime.Days = dayOfYear;
        GameTime.Minutes = minute;
        GameTime.Hours = hour;
        GameTime.Seconds = seconds;
        internalHour = hour + (minute * 0.0166667f) + (seconds * 0.000277778f);
    }

    /// <summary>
    /// Set the time of day in hours. (12.5 = 12:30)
    /// </summary>
    public void SetInternalTimeOfDay(float inHours)
    {
        internalHour = inHours;
        GameTime.Hours = (int)(inHours);
        inHours -= GameTime.Hours;
        GameTime.Minutes = (int)(inHours * 60f);
        inHours -= GameTime.Minutes * 0.0166667f;
        GameTime.Seconds = (int)(inHours * 3600f);
    }

    /// <summary>
    /// Get current time in a nicely formatted string with seconds!
    /// </summary>
    /// <returns>The time string.</returns>
    public string GetTimeStringWithSeconds()
    {
        return string.Format("{0:00}:{1:00}:{2:00}", GameTime.Hours, GameTime.Minutes, GameTime.Seconds);
    }

    /// <summary>
    /// Get current time in a nicely formatted string!
    /// </summary>
    /// <returns>The time string.</returns>
    public string GetTimeString()
    {
        return string.Format("{0:00}:{1:00}", GameTime.Hours, GameTime.Minutes);
    }

    public DateTime CreateSystemDate()
    {
        DateTime date = new DateTime();

        date = date.AddYears(GameTime.Years - 1);
        date = date.AddDays(GameTime.Days - 1);

        return date;
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    public Vector3 OrbitalToLocal(float theta, float phi)
    {
        Vector3 pos;

        float sinTheta = Mathf.Sin(theta);
        float cosTheta = Mathf.Cos(theta);
        float sinPhi = Mathf.Sin(phi);
        float cosPhi = Mathf.Cos(phi);

        pos.z = sinTheta * cosPhi;
        pos.y = cosTheta;
        pos.x = sinTheta * sinPhi;

        return pos;
    }

    public void CalculateSunPosition(float d, float ecl, bool simpleMoon)
    {
        /////http://www.stjarnhimlen.se/comp/ppcomp.html#5////
        ///////////////////////// SUN ////////////////////////
        float w = 282.9404f + 4.70935E-5f * d;
        float e = 0.016709f - 1.151E-9f * d;
        float M = 356.0470f + 0.9856002585f * d;

        float E = M + e * Mathf.Rad2Deg * Mathf.Sin(Mathf.Deg2Rad * M) * (1 + e * Mathf.Cos(Mathf.Deg2Rad * M));

        float xv = Mathf.Cos(Mathf.Deg2Rad * E) - e;
        float yv = Mathf.Sin(Mathf.Deg2Rad * E) * Mathf.Sqrt(1 - e * e);

        float v = Mathf.Rad2Deg * Mathf.Atan2(yv, xv);
        float r = Mathf.Sqrt(xv * xv + yv * yv);

        float l = v + w;

        float xs = r * Mathf.Cos(Mathf.Deg2Rad * l);
        float ys = r * Mathf.Sin(Mathf.Deg2Rad * l);

        float xe = xs;
        float ye = ys * Mathf.Cos(Mathf.Deg2Rad * ecl);
        float ze = ys * Mathf.Sin(Mathf.Deg2Rad * ecl);

        float decl_rad = Mathf.Atan2(ze, Mathf.Sqrt(xe * xe + ye * ye));
        float decl_sin = Mathf.Sin(decl_rad);
        float decl_cos = Mathf.Cos(decl_rad);

        float GMST0 = (l + 180);
        float GMST = GMST0 + GetUniversalTimeOfDay() * 15;
        LST = GMST + GameTime.Longitude;

        float HA_deg = LST - Mathf.Rad2Deg * Mathf.Atan2(ye, xe);
        float HA_rad = Mathf.Deg2Rad * HA_deg;
        float HA_sin = Mathf.Sin(HA_rad);
        float HA_cos = Mathf.Cos(HA_rad);

        float x = HA_cos * decl_cos;
        float y = HA_sin * decl_cos;
        float z = decl_sin;

        float sin_Lat = Mathf.Sin(Mathf.Deg2Rad * GameTime.Latitude);
        float cos_Lat = Mathf.Cos(Mathf.Deg2Rad * GameTime.Latitude);

        float xhor = x * sin_Lat - z * cos_Lat;
        float yhor = y;
        float zhor = x * cos_Lat + z * sin_Lat;

        float azimuth = Mathf.Atan2(yhor, xhor) + Mathf.Deg2Rad * 180;
        float altitude = Mathf.Atan2(zhor, Mathf.Sqrt(xhor * xhor + yhor * yhor));

        float sunTheta = (90 * Mathf.Deg2Rad) - altitude;
        float sunPhi = azimuth;

        //Set SolarTime: 1 = mid-day (sun directly above you), 0.5 = sunset/dawn, 0 = midnight;
        GameTime.solarTime = Mathf.Clamp01(Remap(sunTheta, -1.5f, 0f, 1.5f, 1f));

        Components.Sun.transform.localPosition = OrbitalToLocal(sunTheta, sunPhi);
        Components.Sun.transform.LookAt(transform.position);

        if (simpleMoon)
        {
            Components.Moon.transform.localPosition = OrbitalToLocal(sunTheta - pi, sunPhi);
            GameTime.lunarTime = Mathf.Clamp01(Remap(sunTheta - pi, -3.0f, 0f, 0f, 1f));
            Components.Moon.transform.LookAt(transform.position);
        }
    }

    public void CalculateMoonPosition(float d, float ecl)
    {
        float N = 125.1228f - 0.0529538083f * d;
        float i = 5.1454f;
        float w = 318.0634f + 0.1643573223f * d;
        float a = 60.2666f;
        float e = 0.054900f;
        float M = 115.3654f + 13.0649929509f * d;

        float rad_M = Mathf.Deg2Rad * M;

        float E = rad_M + e * Mathf.Sin(rad_M) * (1f + e * Mathf.Cos(rad_M));

        float xv = a * (Mathf.Cos(E) - e);
        float yv = a * (Mathf.Sqrt(1f - e * e) * Mathf.Sin(E));

        float v = Mathf.Rad2Deg * Mathf.Atan2(yv, xv);
        float r = Mathf.Sqrt(xv * xv + yv * yv);

        float rad_N = Mathf.Deg2Rad * N;
        float sin_N = Mathf.Sin(rad_N);
        float cos_N = Mathf.Cos(rad_N);

        float l = Mathf.Deg2Rad * (v + w);
        float sin_l = Mathf.Sin(l);
        float cos_l = Mathf.Cos(l);

        float rad_i = Mathf.Deg2Rad * i;
        float cos_i = Mathf.Cos(rad_i);

        float xh = r * (cos_N * cos_l - sin_N * sin_l * cos_i);
        float yh = r * (sin_N * cos_l + cos_N * sin_l * cos_i);
        float zh = r * (sin_l * Mathf.Sin(rad_i));

        float cos_ecl = Mathf.Cos(Mathf.Deg2Rad * ecl);
        float sin_ecl = Mathf.Sin(Mathf.Deg2Rad * ecl);

        float xe = xh;
        float ye = yh * cos_ecl - zh * sin_ecl;
        float ze = yh * sin_ecl + zh * cos_ecl;

        float ra = Mathf.Atan2(ye, xe);
        float decl = Mathf.Atan2(ze, Mathf.Sqrt(xe * xe + ye * ye));

        float HA = Mathf.Deg2Rad * LST - ra;

        float x = Mathf.Cos(HA) * Mathf.Cos(decl);
        float y = Mathf.Sin(HA) * Mathf.Cos(decl);
        float z = Mathf.Sin(decl);

        float latitude = Mathf.Deg2Rad * GameTime.Latitude;
        float sin_latitude = Mathf.Sin(latitude);
        float cos_latitude = Mathf.Cos(latitude);

        float xhor = x * sin_latitude - z * cos_latitude;
        float yhor = y;
        float zhor = x * cos_latitude + z * sin_latitude;

        float azimuth = Mathf.Atan2(yhor, xhor) + Mathf.Deg2Rad * 180f;
        float altitude = Mathf.Atan2(zhor, Mathf.Sqrt(xhor * xhor + yhor * yhor));

        float MoonTheta = (90f * Mathf.Deg2Rad) - altitude;
        float MoonPhi = azimuth;

        Components.Moon.transform.localPosition = OrbitalToLocal(MoonTheta, MoonPhi);
        GameTime.lunarTime = Mathf.Clamp01(Remap(MoonTheta, -1.5f, 0f, 1.5f, 1f));
        Components.Moon.transform.LookAt(transform.position);
    }

    public void CalculateSatPosition(float d, float ecl, Transform sat, Transform satRota, EnviroSatellite satSetting)
    {
        float N = 125.1228f - 0.0529538083f * d;
        float i = 5.1454f;
        float w = 318.0634f + 0.1643573223f * d;
        float a = 60.2666f;
        float e = 0.054900f;
        float M = 115.3654f + 13.0649929509f * d;

        float rad_M = Mathf.Deg2Rad * M;

        float E = rad_M + e * Mathf.Sin(rad_M) * (1f + e * Mathf.Cos(rad_M));

        float xv = a * (Mathf.Cos(E) - e);
        float yv = a * (Mathf.Sqrt(1f - e * e) * Mathf.Sin(E));

        float v = Mathf.Rad2Deg * Mathf.Atan2(yv, xv);
        float r = Mathf.Sqrt(xv * xv + yv * yv);

        float rad_N = Mathf.Deg2Rad * N;
        float sin_N = Mathf.Sin(rad_N);
        float cos_N = Mathf.Cos(rad_N);

        float l = Mathf.Deg2Rad * (v + w);
        float sin_l = Mathf.Sin(l);
        float cos_l = Mathf.Cos(l);

        float rad_i = Mathf.Deg2Rad * i;
        float cos_i = Mathf.Cos(rad_i);

        float xh = r * (cos_N * cos_l - sin_N * sin_l * cos_i);
        float yh = r * (sin_N * cos_l + cos_N * sin_l * cos_i);
        float zh = r * (sin_l * Mathf.Sin(rad_i));

        float cos_ecl = Mathf.Cos(Mathf.Deg2Rad * ecl);
        float sin_ecl = Mathf.Sin(Mathf.Deg2Rad * ecl);

        float xe = xh;
        float ye = yh * cos_ecl - zh * sin_ecl;
        float ze = yh * sin_ecl + zh * cos_ecl;

        float ra = Mathf.Atan2(ye, xe);
        float decl = Mathf.Atan2(ze, Mathf.Sqrt(xe * xe + ye * ye));

        float HA = Mathf.Deg2Rad * LST - ra;

        float x = Mathf.Cos(HA) * Mathf.Cos(decl);
        float y = Mathf.Sin(HA) * Mathf.Cos(decl);
        float z = Mathf.Sin(decl);

        float latitude = Mathf.Deg2Rad * GameTime.Latitude;
        float sin_latitude = Mathf.Sin(latitude);
        float cos_latitude = Mathf.Cos(latitude);

        float xhor = x * sin_latitude - z * cos_latitude;
        float yhor = y;
        float zhor = x * cos_latitude + z * sin_latitude;

        float azimuth = Mathf.Atan2(yhor, xhor) + Mathf.Deg2Rad * 180f;
        float altitude = Mathf.Atan2(zhor, Mathf.Sqrt(xhor * xhor + yhor * yhor));

        float MoonTheta = (90f * Mathf.Deg2Rad) - altitude;
        float MoonPhi = azimuth;

        sat.localPosition = OrbitalToLocal(MoonTheta, MoonPhi);
        sat.LookAt(transform.position);

        Vector3 rotation =  new Vector3(satSetting.xRot,satSetting.yRot,satSetting.zRot);
        satRota.eulerAngles = rotation;
    }

    public void CalculateStarsPosition(float siderealTime)
    {

        if (siderealTime > 24) siderealTime -= 24;
        else if (siderealTime < 0) siderealTime += 24;

        Quaternion starsRotation = Quaternion.Euler(90 - GameTime.Latitude, Mathf.Deg2Rad * GameTime.Longitude, 0);
        starsRotation *= Quaternion.Euler(0, siderealTime, 0);

        Components.starsRotation.localRotation = starsRotation;
        Shader.SetGlobalMatrix("_StarsMatrix", Components.starsRotation.worldToLocalMatrix);
    }


    public void UpdateSunAndMoonPosition()
    {
        DateTime date = CreateSystemDate();
        float d = 367 * date.Year - 7 * (date.Year + (date.Month / 12 + 9) / 12) / 4 + 275 * date.Month / 9 + date.Day - 730530;

        d += (GetUniversalTimeOfDay() / 24f);

        float ecl = 23.4393f - 3.563E-7f * d;

        if (skySettings.sunAndMoonPosition == EnviroSkySettings.SunAndMoonCalc.Realistic)
        {
            CalculateSunPosition(d, ecl, false);
            CalculateMoonPosition(d, ecl);
        }
        else
        {
            CalculateSunPosition(d, ecl, true);
        }

        CalculateStarsPosition(LST);
        CalculateSatPositions(LST);
    }
    /// <summary>
    /// Get current time in hours. UTC0 (12.5 = 12:30)
    /// </summary>
    /// <returns>The the current time of day in hours.</returns>
    public float GetUniversalTimeOfDay()
    {
        return internalHour - GameTime.utcOffset;
    }

    /// <summary>
    /// Get current time in hours with UTC time offset.
    /// </summary>
    /// <returns>The the current time of day in hours.</returns>
    public float GetTimeOfDay()
    {
        return internalHour;
    }

    /// <summary>
    /// Calculate total time in hours.
    /// </summary>
    /// <returns>The the current date in hours.</returns>
    public double GetInHours(float hours, float days, float years, int daysInYear)
    {
        double inHours = hours + (days * 24f) + ((years * daysInYear) * 24f);
        return inHours;
    }

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Seasons

    /// <summary>
    /// Updates and switches seasons
    /// </summary>
    public void UpdateSeason()
    {
        if (currentDay >= seasonsSettings.SpringStart && currentDay <= seasonsSettings.SpringEnd)
        {
            ChangeSeason(EnviroSeasons.Seasons.Spring);
        }
        else if (currentDay >= seasonsSettings.SummerStart && currentDay <= seasonsSettings.SummerEnd)
        {
            ChangeSeason(EnviroSeasons.Seasons.Summer);
        }
        else if (currentDay >= seasonsSettings.AutumnStart && currentDay <= seasonsSettings.AutumnEnd)
        {
            ChangeSeason(EnviroSeasons.Seasons.Autumn);
        }
        else if (currentDay >= seasonsSettings.WinterStart || currentDay <= seasonsSettings.WinterEnd)
        {
            ChangeSeason(EnviroSeasons.Seasons.Winter);
        }
    }

    /// <summary>
    /// Manual change of Season
    /// </summary>
    /// <param name="season">Season.</param>
    public void ChangeSeason(EnviroSeasons.Seasons season)
    {
        if (Seasons.currentSeasons != season)
        {
            EnviroSkyMgr.instance.NotifySeasonChanged(season);
            Seasons.lastSeason = Seasons.currentSeasons;
            Seasons.currentSeasons = season;
        }
    }
    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Profile
    /// <summary>
    /// Loads a profile into system.
    /// </summary>
    public void ApplyProfile(EnviroProfile p)
    {
        profile = p;
        lightSettings = JsonUtility.FromJson<EnviroLightSettings>(JsonUtility.ToJson(p.lightSettings));
        volumeLightSettings = JsonUtility.FromJson<EnviroVolumeLightingSettings>(JsonUtility.ToJson(p.volumeLightSettings));
        distanceBlurSettings = JsonUtility.FromJson<EnviroDistanceBlurSettings>(JsonUtility.ToJson(p.distanceBlurSettings));
        skySettings = JsonUtility.FromJson<EnviroSkySettings>(JsonUtility.ToJson(p.skySettings));
        cloudsSettings = JsonUtility.FromJson<EnviroCloudSettings>(JsonUtility.ToJson(p.cloudsSettings));
        weatherSettings = JsonUtility.FromJson<EnviroWeatherSettings>(JsonUtility.ToJson(p.weatherSettings));
        fogSettings = JsonUtility.FromJson<EnviroFogSettings>(JsonUtility.ToJson(p.fogSettings));
        lightshaftsSettings = JsonUtility.FromJson<EnviroLightShaftsSettings>(JsonUtility.ToJson(p.lightshaftsSettings));
        audioSettings = JsonUtility.FromJson<EnviroAudioSettings>(JsonUtility.ToJson(p.audioSettings));
        satelliteSettings = JsonUtility.FromJson<EnviroSatellitesSettings>(JsonUtility.ToJson(p.satelliteSettings));
        qualitySettings = JsonUtility.FromJson<EnviroQualitySettings>(JsonUtility.ToJson(p.qualitySettings));
        seasonsSettings = JsonUtility.FromJson<EnviroSeasonSettings>(JsonUtility.ToJson(p.seasonsSettings));
        reflectionSettings = JsonUtility.FromJson<EnviroReflectionSettings>(JsonUtility.ToJson(p.reflectionSettings));
#if ENVIRO_HD
        auroraSettings = JsonUtility.FromJson<EnviroAuroraSettings>(JsonUtility.ToJson(p.auroraSettings));
#endif
        profileLoaded = true;
    }

    /// <summary>
    /// Saves current settings in assigned profile.
    /// </summary>
    public void SaveProfile()
    {
        profile.lightSettings = JsonUtility.FromJson<EnviroLightSettings>(JsonUtility.ToJson(lightSettings));
        profile.volumeLightSettings = JsonUtility.FromJson<EnviroVolumeLightingSettings>(JsonUtility.ToJson(volumeLightSettings));
        profile.distanceBlurSettings = JsonUtility.FromJson<EnviroDistanceBlurSettings>(JsonUtility.ToJson(distanceBlurSettings));
        profile.skySettings = JsonUtility.FromJson<EnviroSkySettings>(JsonUtility.ToJson(skySettings));
        profile.cloudsSettings = JsonUtility.FromJson<EnviroCloudSettings>(JsonUtility.ToJson(cloudsSettings));
        profile.weatherSettings = JsonUtility.FromJson<EnviroWeatherSettings>(JsonUtility.ToJson(weatherSettings));
        profile.fogSettings = JsonUtility.FromJson<EnviroFogSettings>(JsonUtility.ToJson(fogSettings));
        profile.lightshaftsSettings = JsonUtility.FromJson<EnviroLightShaftsSettings>(JsonUtility.ToJson(lightshaftsSettings));
        profile.audioSettings = JsonUtility.FromJson<EnviroAudioSettings>(JsonUtility.ToJson(audioSettings));
        profile.satelliteSettings = JsonUtility.FromJson<EnviroSatellitesSettings>(JsonUtility.ToJson(satelliteSettings));
        profile.qualitySettings = JsonUtility.FromJson<EnviroQualitySettings>(JsonUtility.ToJson(qualitySettings));
        profile.seasonsSettings = JsonUtility.FromJson<EnviroSeasonSettings>(JsonUtility.ToJson(seasonsSettings));
        profile.reflectionSettings = JsonUtility.FromJson<EnviroReflectionSettings>(JsonUtility.ToJson(reflectionSettings));
#if ENVIRO_HD
        profile.auroraSettings = JsonUtility.FromJson<EnviroAuroraSettings>(JsonUtility.ToJson(auroraSettings));
#endif

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(profile);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Lighting
    /// <summary>
    /// Updates reflection probe.
    /// </summary>
    public void UpdateReflections(bool forced)
    {
        if (Components.GlobalReflectionProbe == null)
        {
            Debug.Log("Global Reflection Probe not assigned in 'Components' menu of Enviro Sky Instance!");
            return;
        }

#if ENVIRO_HD
        if (EnviroSkyMgr.instance != null && EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
        {

#if !ENVIRO_HDRP
            Components.GlobalReflectionProbe.customRendering = reflectionSettings.globalReflectionCustomRendering;

            if (reflectionSettings.reflectionCloudsQuality != null)
                Components.GlobalReflectionProbe.customCloudsQuality = reflectionSettings.reflectionCloudsQuality;

            Components.GlobalReflectionProbe.useFog = reflectionSettings.globalReflectionUseFog;
#else
            Components.GlobalReflectionProbe.customRendering = false;
#endif
        }
#endif

        if (!reflectionSettings.globalReflections)
        {
            Components.GlobalReflectionProbe.enabled = false;
            //RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox;
            return;
        }

        if (!Components.GlobalReflectionProbe.isActiveAndEnabled)
            Components.GlobalReflectionProbe.enabled = true;


#if !ENVIRO_HDRP
        Components.GlobalReflectionProbe.myProbe.cullingMask = reflectionSettings.globalReflectionLayers;
        Components.GlobalReflectionProbe.myProbe.intensity = reflectionSettings.globalReflectionsIntensity;
        Components.GlobalReflectionProbe.myProbe.size = transform.localScale * reflectionSettings.globalReflectionsScale;

        switch (reflectionSettings.globalReflectionResolution)
        {
            case EnviroReflectionSettings.GlobalReflectionResolution.R16:
                Components.GlobalReflectionProbe.myProbe.resolution = 16;
                break;
            case EnviroReflectionSettings.GlobalReflectionResolution.R32:
                Components.GlobalReflectionProbe.myProbe.resolution = 32;
                break;
            case EnviroReflectionSettings.GlobalReflectionResolution.R64:
                Components.GlobalReflectionProbe.myProbe.resolution = 64;
                break;
            case EnviroReflectionSettings.GlobalReflectionResolution.R128:
                Components.GlobalReflectionProbe.myProbe.resolution = 128;
                break;
            case EnviroReflectionSettings.GlobalReflectionResolution.R256:
                Components.GlobalReflectionProbe.myProbe.resolution = 256;
                break;
            case EnviroReflectionSettings.GlobalReflectionResolution.R512:
                Components.GlobalReflectionProbe.myProbe.resolution = 512;
                break;
            case EnviroReflectionSettings.GlobalReflectionResolution.R1024:
                Components.GlobalReflectionProbe.myProbe.resolution = 1024;
                break;
            case EnviroReflectionSettings.GlobalReflectionResolution.R2048:
                Components.GlobalReflectionProbe.myProbe.resolution = 2048;
                break;
        }
#else 
        if(Components.GlobalReflectionProbe.hdprobe != null)
        {
            Components.GlobalReflectionProbe.hdprobe.multiplier = reflectionSettings.globalReflectionsIntensity;
            Components.GlobalReflectionProbe.hdprobe.influenceVolume.boxSize = transform.localScale * reflectionSettings.globalReflectionsScale;
            Components.GlobalReflectionProbe.hdprobe.influenceVolume.sphereRadius = reflectionSettings.globalReflectionsScale;
            Components.GlobalReflectionProbe.hdprobe.settingsRaw.cameraSettings.culling.cullingMask = reflectionSettings.globalReflectionLayers;
        }
#endif

        //Force update, return after that one.
        if(forced)
        {
            ReflectionProbeUpdate (forced);
            return;
        }

        if ((currentTimeInHours > lastRelfectionUpdate + reflectionSettings.globalReflectionsTimeTreshold || currentTimeInHours < lastRelfectionUpdate - reflectionSettings.globalReflectionsTimeTreshold) && reflectionSettings.globalReflectionsUpdateOnGameTime)
        {
            ReflectionProbeUpdate (forced);
        }
        else if ((transform.position.magnitude > lastRelfectionPositionUpdate.magnitude + reflectionSettings.globalReflectionsPositionTreshold || transform.position.magnitude < lastRelfectionPositionUpdate.magnitude - reflectionSettings.globalReflectionsPositionTreshold) && reflectionSettings.globalReflectionsUpdateOnPosition)
        {
            lastRelfectionPositionUpdate = transform.position;
            Components.GlobalReflectionProbe.RefreshReflection(reflectionSettings.globalReflectionTimeSlicing);
        }
    } 

    private void ReflectionProbeUpdate (bool forced)
    {
            lastRelfectionUpdate = currentTimeInHours;

            if(!forced)
              Components.GlobalReflectionProbe.RefreshReflection(reflectionSettings.globalReflectionTimeSlicing);
            else
              Components.GlobalReflectionProbe.RefreshReflection(false);
#if !ENVIRO_HDRP
            //Update Default Reflections
#if UNITY_EDITOR
            if((reflectionSettings.updateDefaultEnvironmentReflections && UnityEngine.SceneManagement.SceneManager.GetActiveScene() == gameObject.scene) || (reflectionSettings.updateDefaultEnvironmentReflections && EnviroSkyMgr.instance != null && EnviroSkyMgr.instance.dontDestroy))
#else
            if(reflectionSettings.updateDefaultEnvironmentReflections)
#endif
            {
                StartCoroutine(UpdateDefaultReflections());
            }
            else
            {
                RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Skybox;
            }
#endif
    }

    private IEnumerator UpdateDefaultReflections()
    {
        //Wait for 7 frames in case of timeslicing
        for (int i = 0; i < 8; i++)
        {
            yield return null;
        }

        RenderSettings.defaultReflectionMode = UnityEngine.Rendering.DefaultReflectionMode.Custom;

        if(defaultSkyReflectionTex == null || defaultSkyReflectionTex.height != Components.GlobalReflectionProbe.myProbe.resolution || defaultSkyReflectionTex.width != Components.GlobalReflectionProbe.myProbe.resolution)
        {
            if(defaultSkyReflectionTex != null)
            DestroyImmediate(defaultSkyReflectionTex);
        
            defaultSkyReflectionTex = new Cubemap(Components.GlobalReflectionProbe.myProbe.resolution, Components.GlobalReflectionProbe.myProbe.hdr ? TextureFormat.RGBAHalf : TextureFormat.RGBA32, true);
            defaultSkyReflectionTex.name = "Enviro Default Sky Reflection";
        }

        if(Components.GlobalReflectionProbe.myProbe.texture != null)   
        Graphics.CopyTexture(Components.GlobalReflectionProbe.myProbe.texture, defaultSkyReflectionTex as Texture);
            
        RenderSettings.customReflection = defaultSkyReflectionTex;
    } 
    /// <summary>
    /// Updates ambient lighting.
    /// </summary>
    public void UpdateAmbientLight()
    {
         float intensity = lightSettings.ambientIntensity.Evaluate(GameTime.solarTime) * ambientLightIntensityMod;
        //float intensity = Mathf.Lerp(lightSettings.ambientIntensity.Evaluate(GameTime.solarTime), lightSettings.ambientIntensity.Evaluate(GameTime.solarTime) * 0.25f, GameTime.lunarTime * (1f - GameTime.solarTime));

        switch (lightSettings.ambientMode)
        {
            case UnityEngine.Rendering.AmbientMode.Flat:
                Color lightClr = Color.Lerp(lightSettings.ambientSkyColor.Evaluate(GameTime.solarTime), currentWeatherLightMod, currentWeatherLightMod.a) * intensity;
                RenderSettings.ambientSkyColor = Color.Lerp(lightClr, interiorZoneSettings.currentInteriorAmbientLightMod, interiorZoneSettings.currentInteriorAmbientLightMod.a);
                break;

            case UnityEngine.Rendering.AmbientMode.Trilight:
                Color lClr = Color.Lerp(lightSettings.ambientSkyColor.Evaluate(GameTime.solarTime), currentWeatherLightMod, currentWeatherLightMod.a) * intensity;
                RenderSettings.ambientSkyColor = Color.Lerp(lClr, interiorZoneSettings.currentInteriorAmbientLightMod, interiorZoneSettings.currentInteriorAmbientLightMod.a);
                Color eqClr = Color.Lerp(lightSettings.ambientEquatorColor.Evaluate(GameTime.solarTime), currentWeatherLightMod, currentWeatherLightMod.a) * intensity;
                RenderSettings.ambientEquatorColor = Color.Lerp(eqClr, interiorZoneSettings.currentInteriorAmbientEQLightMod, interiorZoneSettings.currentInteriorAmbientEQLightMod.a);
                Color grClr = Color.Lerp(lightSettings.ambientGroundColor.Evaluate(GameTime.solarTime), currentWeatherLightMod, currentWeatherLightMod.a) * intensity;
                RenderSettings.ambientGroundColor = Color.Lerp(grClr, interiorZoneSettings.currentInteriorAmbientGRLightMod, interiorZoneSettings.currentInteriorAmbientGRLightMod.a);
                break;

            case UnityEngine.Rendering.AmbientMode.Skybox:

                RenderSettings.ambientIntensity = intensity;

                if (lastAmbientSkyUpdate < internalHour || lastAmbientSkyUpdate > internalHour + 0.101f)
                {
                    DynamicGI.UpdateEnvironment();
                    lastAmbientSkyUpdate = internalHour + 0.1f;
                }
                break;
        }
    }

    /// <summary>
    /// Updates direct lighting.
    /// </summary>

    public void CalculateDirectLight()
    {
        if (MainLight == null)
            MainLight = Components.DirectLight.GetComponent<Light>();
         
#if ENVIRO_HDRP
                if(MainLightHDRP == null)
                   MainLightHDRP = Components.DirectLight.GetComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalLightData>();
#endif

        //Single Mode
        if (lightSettings.directionalLightMode == EnviroLightSettings.LightingMode.Single || Components.AdditionalDirectLight == null)
        {
#if ENVIRO_HDRP
            if (lightSettings.usePhysicalBasedLighting)
            {
                Color lightClr = lightSettings.lightColorTint.Evaluate(GameTime.solarTime);
                MainLight.color = lightClr;
                MainLight.useColorTemperature = true;
                MainLight.colorTemperature = lightSettings.lightColorTemperature.Evaluate(GameTime.solarTime);
            }
            else
            {
                MainLight.useColorTemperature = false;
                Color lightClr = Color.Lerp(lightSettings.LightColor.Evaluate(GameTime.solarTime), currentWeatherLightMod, currentWeatherLightMod.a);
                MainLight.color = Color.Lerp(lightClr, interiorZoneSettings.currentInteriorDirectLightMod, interiorZoneSettings.currentInteriorDirectLightMod.a);
            }
#else
                Color lightClr = Color.Lerp(lightSettings.LightColor.Evaluate(GameTime.solarTime), currentWeatherLightMod, currentWeatherLightMod.a);
                MainLight.color = Color.Lerp(lightClr, interiorZoneSettings.currentInteriorDirectLightMod, interiorZoneSettings.currentInteriorDirectLightMod.a);
#endif
            float lightIntensity;
            // Set sun and moon intensity
            if (!isNight)
            {
#if ENVIRO_HDRP

                if (lightSettings.usePhysicalBasedLighting)
                {
                    lightIntensity = lightSettings.sunIntensityLux.Evaluate(GameTime.solarTime);
                }
                else
                {
                    lightIntensity = lightSettings.directLightSunIntensity.Evaluate(GameTime.solarTime) * lightSettings.lightIntensityLuxMult * 250f;
                }
#else
                lightIntensity = lightSettings.directLightSunIntensity.Evaluate(GameTime.solarTime) * directLightIntensityMod;
#endif

                Components.Sun.transform.LookAt(new Vector3(transform.position.x, transform.position.y - lightSettings.directLightAngleOffset, transform.position.z));
                if (lightSettings.stopRotationAtHigh == false || lightSettings.stopRotationAtHigh && GameTime.solarTime >= lightSettings.rotationStopHigh)
                {
                   Components.DirectLight.rotation = Components.Sun.transform.rotation;
                }
            }
            else
            {
#if ENVIRO_HDRP
                if (lightSettings.usePhysicalBasedLighting)
                {
                    lightIntensity = lightSettings.moonIntensityLux.Evaluate(GameTime.lunarTime);
                }
                else
                {
                    lightIntensity = lightSettings.directLightMoonIntensity.Evaluate(GameTime.lunarTime) * lightSettings.lightIntensityLuxMult * 250f;
                }
#else
                lightIntensity = lightSettings.directLightMoonIntensity.Evaluate(GameTime.lunarTime) * directLightIntensityMod;
#endif

                Components.Moon.transform.LookAt(new Vector3(transform.position.x, transform.position.y - lightSettings.directLightAngleOffset, transform.position.z));
                if (lightSettings.stopRotationAtHigh == false || lightSettings.stopRotationAtHigh && GameTime.lunarTime >= lightSettings.rotationStopHigh)
                {
                    Components.DirectLight.rotation = Components.Moon.transform.rotation;
                }
            }
            // Set the light and shadow intensity
#if ENVIRO_HDRP
            if(MainLightHDRP != null)
            {
                MainLightHDRP.SetIntensity(lightIntensity * currentLightIntensityMod);
                MainLightHDRP.SetShadowDimmer(Mathf.Clamp01(lightSettings.shadowIntensity.Evaluate(GameTime.solarTime) + shadowIntensityMod));
                MainLightHDRP.volumetricDimmer = globalVolumeLightIntensity;
            }

#else
            MainLight.intensity = Mathf.Lerp(MainLight.intensity, lightIntensity, Time.deltaTime * lightSettings.lightIntensityTransitionSpeed);
            MainLight.shadowStrength = Mathf.Clamp01(lightSettings.shadowIntensity.Evaluate(GameTime.solarTime) + shadowIntensityMod);
#endif   
        }
        //DUAL Mode
        else if (lightSettings.directionalLightMode == EnviroLightSettings.LightingMode.Dual && Components.AdditionalDirectLight != null)
        {
            if (AdditionalLight == null)
                AdditionalLight = Components.AdditionalDirectLight.GetComponent<Light>();

#if ENVIRO_HDRP
                if(AdditionalLightHDRP == null)
                    AdditionalLightHDRP = Components.AdditionalDirectLight.GetComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalLightData>();

            if (lightSettings.usePhysicalBasedLighting)
            {
                MainLight.useColorTemperature = true;
                AdditionalLight.useColorTemperature = true;
                MainLight.colorTemperature = lightSettings.lightColorTemperature.Evaluate(GameTime.solarTime);
                AdditionalLight.colorTemperature = lightSettings.lightColorTemperature.Evaluate(GameTime.solarTime);
                Color lightClr = lightSettings.lightColorTint.Evaluate(GameTime.solarTime);
                MainLight.color = lightClr;
                AdditionalLight.color = lightClr;
            }
            else
            {
                MainLight.useColorTemperature = false;
                AdditionalLight.useColorTemperature = false;
                Color lightClr = Color.Lerp(lightSettings.LightColor.Evaluate(GameTime.solarTime), currentWeatherLightMod, currentWeatherLightMod.a);
                MainLight.color = Color.Lerp(lightClr, interiorZoneSettings.currentInteriorDirectLightMod, interiorZoneSettings.currentInteriorDirectLightMod.a);
                AdditionalLight.color = MainLight.color;
            }
#else
                Color lightClr = Color.Lerp(lightSettings.LightColor.Evaluate(GameTime.solarTime), currentWeatherLightMod, currentWeatherLightMod.a);
                MainLight.color = Color.Lerp(lightClr, interiorZoneSettings.currentInteriorDirectLightMod, interiorZoneSettings.currentInteriorDirectLightMod.a);
                AdditionalLight.color = MainLight.color;
#endif

            float lightIntensitySun, lightIntensityMoon;

#if ENVIRO_HDRP
            if (lightSettings.usePhysicalBasedLighting)
            {
                lightIntensitySun = lightSettings.sunIntensityLux.Evaluate(GameTime.solarTime);
                lightIntensityMoon = lightSettings.moonIntensityLux.Evaluate(GameTime.lunarTime);
            }
            else
            {
                lightIntensitySun = lightSettings.directLightSunIntensity.Evaluate(GameTime.solarTime) * lightSettings.lightIntensityLuxMult * 250f;
                lightIntensityMoon = lightSettings.directLightMoonIntensity.Evaluate(GameTime.lunarTime) * lightSettings.lightIntensityLuxMult * 250f * (1 - GameTime.solarTime);
            }

#else
            lightIntensitySun = lightSettings.directLightSunIntensity.Evaluate(GameTime.solarTime) * directLightIntensityMod;
            lightIntensityMoon = lightSettings.directLightMoonIntensity.Evaluate(GameTime.lunarTime) * (1 - GameTime.solarTime) * directLightIntensityMod;
#endif
            Components.Sun.transform.LookAt(new Vector3(transform.position.x, transform.position.y - lightSettings.directLightAngleOffset, transform.position.z));

            if (lightSettings.stopRotationAtHigh == false || lightSettings.stopRotationAtHigh && GameTime.solarTime >= lightSettings.rotationStopHigh)
            {
                Components.DirectLight.rotation = Components.Sun.transform.rotation;
            }

            Components.Moon.transform.LookAt(new Vector3(transform.position.x, transform.position.y - lightSettings.directLightAngleOffset, transform.position.z));
            if (lightSettings.stopRotationAtHigh == false || lightSettings.stopRotationAtHigh && GameTime.lunarTime >= lightSettings.rotationStopHigh)
            {
                Components.AdditionalDirectLight.rotation = Components.Moon.transform.rotation;
            }
 
#if ENVIRO_HDRP
           // MainLight.intensity = lightIntensitySun * currentLightIntensityMod;
            if(MainLightHDRP != null)
            {
                MainLightHDRP.SetIntensity(lightIntensitySun * currentLightIntensityMod);
                MainLightHDRP.SetShadowDimmer(Mathf.Clamp01(lightSettings.shadowIntensity.Evaluate(GameTime.solarTime) + shadowIntensityMod));
            }
#else
            MainLight.intensity = Mathf.Lerp(MainLight.intensity, lightIntensitySun, Time.deltaTime * lightSettings.lightIntensityTransitionSpeed);
            MainLight.shadowStrength = Mathf.Clamp01(lightSettings.shadowIntensity.Evaluate(GameTime.solarTime) + shadowIntensityMod);
#endif     

#if ENVIRO_HDRP
           // AdditionalLight.intensity = lightIntensityMoon * currentLightIntensityMod;
            if(AdditionalLightHDRP != null)
               {
                    AdditionalLightHDRP.SetShadowDimmer(Mathf.Clamp01(lightSettings.shadowIntensity.Evaluate(GameTime.solarTime) + shadowIntensityMod));
                    AdditionalLightHDRP.SetIntensity(lightIntensityMoon * currentLightIntensityMod);     
                    AdditionalLightHDRP.volumetricDimmer = globalVolumeLightIntensity;
               }
#else 
            AdditionalLight.intensity = Mathf.Lerp(AdditionalLight.intensity, lightIntensityMoon, Time.deltaTime * lightSettings.lightIntensityTransitionSpeed);
            AdditionalLight.shadowStrength = Mathf.Clamp01(lightSettings.shadowIntensity.Evaluate(GameTime.solarTime) + shadowIntensityMod);
#endif

#if ENVIRO_HDRP
            //Switch shadow cast for directional as HDRP only support one at a time!
            if(AdditionalLightHDRP != null && MainLightHDRP != null)
            {
                if (!isNight)
                {
                    AdditionalLightHDRP.EnableShadows(false);
                    MainLightHDRP.EnableShadows(true);
                }
                else
                {
                    AdditionalLightHDRP.EnableShadows(true);
                    MainLightHDRP.EnableShadows(false);
                }
            }
#endif
        }
    }
    /// <summary>
    /// Updates lighting and sky in scene view to match runtime settings on start.
    /// </summary>
    public void UpdateSceneView()
    {
        if (Weather.startWeatherPreset != null && !Application.isPlaying)
        {
            currentWeatherSkyMod = Weather.startWeatherPreset.weatherSkyMod.Evaluate(GameTime.solarTime);
            currentWeatherFogMod = Weather.startWeatherPreset.weatherFogMod.Evaluate(GameTime.solarTime);
            currentWeatherLightMod = Weather.startWeatherPreset.weatherLightMod.Evaluate(GameTime.solarTime);
        }
    }

    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Weather

    /// <summary>
    /// Update Wind Settings
    /// </summary>
    public void UpdateWind(EnviroWeatherPreset preset)
    {
        if (preset != null)
            windIntensity = Mathf.Lerp(windIntensity, preset.WindStrenght, weatherSettings.windIntensityTransitionSpeed * Time.deltaTime);

        if (cloudsSettings.useWindZoneDirection)
        {
            cloudsSettings.cloudsWindDirectionX = -Components.windZone.transform.forward.x;
            cloudsSettings.cloudsWindDirectionY = -Components.windZone.transform.forward.z;
        }

        cloudAnim += new Vector3(((cloudsSettings.cloudsTimeScale * (windIntensity * cloudsSettings.cloudsWindDirectionX)) * cloudsSettings.cloudsWindIntensity) * Time.deltaTime, ((cloudsSettings.cloudsTimeScale * (windIntensity * cloudsSettings.cloudsWindDirectionY)) * cloudsSettings.cloudsWindIntensity) * Time.deltaTime, ((cloudsSettings.cloudsTimeScale * -1) * cloudsSettings.cloudsUpwardsWindIntensity) * Time.deltaTime);
        cloudAnimNonScaled += new Vector2(((cloudsSettings.cloudsTimeScale * (windIntensity * cloudsSettings.cloudsWindDirectionX)) * cloudsSettings.cloudsWindIntensity) * Time.deltaTime * 0.1f, ((cloudsSettings.cloudsTimeScale * (windIntensity * cloudsSettings.cloudsWindDirectionY)) * cloudsSettings.cloudsWindIntensity) * Time.deltaTime * 0.1f);

        cloudFlatBaseAnim += new Vector2(((cloudsSettings.cloudsTimeScale * (windIntensity * cloudsSettings.cloudsWindDirectionX)) * cloudsSettings.cloudsWindIntensity) * Time.deltaTime * 0.1f, ((cloudsSettings.cloudsTimeScale * (windIntensity * cloudsSettings.cloudsWindDirectionY)) * cloudsSettings.cloudsWindIntensity) * Time.deltaTime * 0.1f);
        cloudFlatDetailAnim += new Vector2(((cloudsSettings.cloudsTimeScale * (windIntensity * cloudsSettings.cloudsWindDirectionX)) * cloudsSettings.cloudsDetailWindIntensity) * Time.deltaTime, ((cloudsSettings.cloudsTimeScale * (windIntensity * cloudsSettings.cloudsWindDirectionY)) * cloudsSettings.cloudsDetailWindIntensity) * Time.deltaTime);

        cirrusAnim += new Vector2(((cloudsSettings.cloudsTimeScale * (windIntensity * cloudsSettings.cloudsWindDirectionX)) * cloudsSettings.cirrusWindIntensity) * Time.deltaTime * 0.1f, ((cloudsSettings.cloudsTimeScale * (windIntensity * cloudsSettings.cloudsWindDirectionY)) * cloudsSettings.cirrusWindIntensity) * Time.deltaTime * 0.1f);

        if (cloudAnim.x > 1f)
            cloudAnim.x = -1f;
        else if (cloudAnim.x < -1f)
            cloudAnim.x = 1f;

        if (cloudAnim.y > 1f)
            cloudAnim.y = -1f;
        else if (cloudAnim.y < -1f)
            cloudAnim.y = 1f;

        if (cloudAnim.z > 1f)
            cloudAnim.z = -1f;
        else if (cloudAnim.z < -1f)
            cloudAnim.z = 1f;

        if (cirrusAnim.x > 1f)
            cirrusAnim.x = -1f;
        else if (cirrusAnim.x < -1f)
            cirrusAnim.x = 1f;

        if (cirrusAnim.y > 1f)
            cirrusAnim.y = -1f;
        else if (cirrusAnim.y < -1f)
            cirrusAnim.y = 1f;


        if (cloudFlatBaseAnim.x > 1f)
            cloudFlatBaseAnim.x = -1f;
        else if (cloudFlatBaseAnim.x < -1f)
            cloudFlatBaseAnim.x = 1f;

        if (cloudFlatBaseAnim.y > 1f)
            cloudFlatBaseAnim.y = -1f;
        else if (cloudFlatBaseAnim.y < -1f)
            cloudFlatBaseAnim.y = 1f;


        if (cloudFlatDetailAnim.x > 1f)
            cloudFlatDetailAnim.x = -1f;
        else if (cloudFlatDetailAnim.x < -1f)
            cloudFlatDetailAnim.x = 1f;

        if (cloudFlatDetailAnim.y > 1f)
            cloudFlatDetailAnim.y = -1f;
        else if (cloudFlatDetailAnim.y < -1f)
            cloudFlatDetailAnim.y = 1f;

        //Set Wind Intensity
        Components.windZone.windMain = windIntensity;
    }


    /// <summary>
    /// Get Active Weather ID
    /// </summary>
    public int GetActiveWeatherID()
    {
        for (int i = 0; i < Weather.WeatherPrefabs.Count; i++)
        {
            //  if (Weather.WeatherPrefabs[i] == null || Weather.currentActiveWeatherPreset == null)
            //      return -1;

            if (Weather.WeatherPrefabs[i].weatherPreset == Weather.currentActiveWeatherPreset)
                return i;
        }
        return -1;
    }

    public void UpdateWeatherVariables(EnviroWeatherPreset p)
    {
        // Update Wind
        UpdateWind(p);

        // The wetness raise
        if (Weather.wetness < p.wetnessLevel)
        {
            Weather.wetness = Mathf.Lerp(Weather.curWetness, p.wetnessLevel, weatherSettings.wetnessAccumulationSpeed * Time.deltaTime);
        }
        else
        { // Drying
            Weather.wetness = Mathf.Lerp(Weather.curWetness, p.wetnessLevel, weatherSettings.wetnessDryingSpeed * Time.deltaTime);
        }

        Weather.wetness = Mathf.Clamp(Weather.wetness, 0f, 1f);
        Weather.curWetness = Weather.wetness;

        //Snowing
        if (Weather.snowStrength < p.snowLevel)
            Weather.snowStrength = Mathf.Lerp(Weather.curSnowStrength, p.snowLevel, weatherSettings.snowAccumulationSpeed * Time.deltaTime);
        else if (Weather.currentTemperature > weatherSettings.snowMeltingTresholdTemperature)//Melting
            Weather.snowStrength = Mathf.Lerp(Weather.curSnowStrength, p.snowLevel, weatherSettings.snowMeltingSpeed * Time.deltaTime);

        Weather.snowStrength = Mathf.Clamp(Weather.snowStrength, 0f, 1f);
        Weather.curSnowStrength = Weather.snowStrength;

        Shader.SetGlobalFloat("_EnviroGrassSnow", Weather.curSnowStrength);


        //Temperature
        float temperature = 0f;

        switch (Seasons.currentSeasons)
        {
            case EnviroSeasons.Seasons.Spring:
                temperature = seasonsSettings.springBaseTemperature.Evaluate(GetUniversalTimeOfDay() / 24f);
                break;
            case EnviroSeasons.Seasons.Summer:
                temperature = seasonsSettings.summerBaseTemperature.Evaluate(GetUniversalTimeOfDay() / 24f);
                break;
            case EnviroSeasons.Seasons.Autumn:
                temperature = seasonsSettings.autumnBaseTemperature.Evaluate(GetUniversalTimeOfDay() / 24f);
                break;
            case EnviroSeasons.Seasons.Winter:
                temperature = seasonsSettings.winterBaseTemperature.Evaluate(GetUniversalTimeOfDay() / 24f);
                break;
        }

        temperature += p.temperatureLevel;
        temperature += Weather.temperatureModifier;

        Weather.currentTemperature = Mathf.Lerp(Weather.currentTemperature, temperature, Time.deltaTime * weatherSettings.temperatureChangingSpeed);
    }

    public IEnumerator PlayThunderRandom()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(Weather.currentActiveWeatherPreset.lightningInterval, Weather.currentActiveWeatherPreset.lightningInterval * 2));

        if (Weather.currentActiveWeatherPrefab.weatherPreset.isLightningStorm)
        {
            if (Weather.weatherFullyChanged)
                PlayLightning();

            StartCoroutine(PlayThunderRandom());
        }
        else
        {
            StopCoroutine(PlayThunderRandom());
            Components.LightningGenerator.StopLightning();
        }
    }


    public IEnumerator PlayLightningEffect(Vector3 position)
    {
        lightningEffect.transform.position = position;
        lightningEffect.transform.eulerAngles = new Vector3(UnityEngine.Random.Range(-80f, -100f), 0f, 0f);
        lightningEffect.Play();
        yield return new WaitForSeconds(0.5f);
        lightningEffect.Stop();
    }

    public void PlayLightning()
    {
        if (lightningEffect != null)
            StartCoroutine(PlayLightningEffect(new Vector3(transform.position.x + UnityEngine.Random.Range(-weatherSettings.lightningRange, weatherSettings.lightningRange), weatherSettings.lightningHeight, transform.position.z + UnityEngine.Random.Range(-weatherSettings.lightningRange, weatherSettings.lightningRange))));
        
        if (audioSettings.ThunderSFX.Count > 0)
        {
            int i = UnityEngine.Random.Range(0, audioSettings.ThunderSFX.Count);
            Audio.AudioSourceThunder.PlayOneShot(audioSettings.ThunderSFX[i]);
        }
        
        Components.LightningGenerator.Lightning();
    }

    /// <summary>
    /// Forces a internal Weather Update and applies current active weatherpreset values and send out a weather changed event!
    /// </summary>
    public void ForceWeatherUpdate()
    {
        Weather.lastActiveWeatherPreset = Weather.currentActiveWeatherPreset;
        Weather.lastActiveWeatherPrefab = Weather.currentActiveWeatherPrefab;
        Weather.currentActiveWeatherPreset = Weather.currentActiveZone.currentActiveZoneWeatherPreset;
        Weather.currentActiveWeatherPrefab = Weather.currentActiveZone.currentActiveZoneWeatherPrefab;
        if (Weather.currentActiveWeatherPreset != null)
        {
            EnviroSkyMgr.instance.NotifyWeatherChanged(Weather.currentActiveWeatherPreset);
            Weather.weatherFullyChanged = false;
            if (!serverMode)
            {
                if (Weather.currentActiveWeatherPrefab.weatherPreset.isLightningStorm)
                    StartCoroutine(PlayThunderRandom());
                else
                {
                    StopCoroutine(PlayThunderRandom());
                    Components.LightningGenerator.StopLightning();
                }
            }
        }
    }
    /// <summary>
    /// Check if clouds already full rolled up to start thunder effects.
    /// </summary>
    public void CalcWeatherTransitionState()
    {
        bool changed = false;

#if ENVIRO_HD && ENVIRO_LW
        if (EnviroSkyMgr.instance != null && EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
        {
            if (cloudsConfig.coverage >= Weather.currentActiveWeatherPreset.cloudsConfig.coverage - 0.01f)
                changed = true;
            else
                changed = false;
        }
        else if (EnviroSkyMgr.instance != null && EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.LW)
        {
            if ((cloudsConfig.particleLayer1Alpha >= Weather.currentActiveWeatherPreset.cloudsConfig.particleLayer1Alpha - 0.01f) && (cloudsConfig.particleLayer2Alpha >= Weather.currentActiveWeatherPreset.cloudsConfig.particleLayer2Alpha - 0.01f))
                changed = true;
            else
                changed = false;
        }
#elif ENVIRO_HD
        if (EnviroSkyMgr.instance != null && EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
        {
            if (cloudsConfig.coverage >= Weather.currentActiveWeatherPreset.cloudsConfig.coverage - 0.01f)
                changed = true;
            else
                changed = false;
        }
#elif ENVIRO_LW
        if (EnviroSkyMgr.instance != null && EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.LW)
        {
            if ((cloudsConfig.particleLayer1Alpha >= Weather.currentActiveWeatherPreset.cloudsConfig.particleLayer1Alpha - 0.01f) && (cloudsConfig.particleLayer2Alpha >= Weather.currentActiveWeatherPreset.cloudsConfig.particleLayer2Alpha - 0.01f))
                changed = true;
            else
                changed = false;
        }
#endif

        Weather.weatherFullyChanged = changed;
    }

    /// <summary>
    /// Set weather directly with list id of Weather.WeatherTemplates. No transtions!
    /// </summary>
    public void SetWeatherOverwrite(int weatherId)
    {
        if (weatherId < 0 || weatherId > Weather.WeatherPrefabs.Count)
            return;

        if (Weather.WeatherPrefabs[weatherId] != Weather.currentActiveWeatherPrefab)
        {
            Weather.currentActiveZone.currentActiveZoneWeatherPrefab = Weather.WeatherPrefabs[weatherId];
            Weather.currentActiveZone.currentActiveZoneWeatherPreset = Weather.WeatherPrefabs[weatherId].weatherPreset;
            EnviroSkyMgr.instance.NotifyZoneWeatherChanged(Weather.WeatherPrefabs[weatherId].weatherPreset, Weather.currentActiveZone);
        }

        EnviroSkyMgr.instance.InstantWeatherChange(Weather.currentActiveZone.currentActiveZoneWeatherPreset, Weather.currentActiveZone.currentActiveZoneWeatherPrefab);
    }
    /// <summary>
    /// Set weather directly with preset of Weather.WeatherTemplates. No transtions!
    /// </summary>
    public void SetWeatherOverwrite(EnviroWeatherPreset preset)
    {
        if (preset == null)
            return;

        if (preset != Weather.currentActiveWeatherPreset)
        {
            for (int i = 0; i < Weather.WeatherPrefabs.Count; i++)
            {
                if (preset == Weather.WeatherPrefabs[i].weatherPreset)
                {
                    Weather.currentActiveZone.currentActiveZoneWeatherPrefab = Weather.WeatherPrefabs[i];
                    Weather.currentActiveZone.currentActiveZoneWeatherPreset = preset;
                    EnviroSkyMgr.instance.NotifyZoneWeatherChanged(preset, Weather.currentActiveZone);
                }
            }
        }

        EnviroSkyMgr.instance.InstantWeatherChange(Weather.currentActiveZone.currentActiveZoneWeatherPreset, Weather.currentActiveZone.currentActiveZoneWeatherPrefab);
    }

    /// <summary>
    /// Set weather over id with smooth transtion.
    /// </summary>
    public void ChangeWeather(int weatherId)
    {
        if (weatherId < 0 || weatherId > Weather.WeatherPrefabs.Count)
            return;

        if (Weather.WeatherPrefabs[weatherId] != Weather.currentActiveWeatherPrefab)
        {
            Weather.currentActiveZone.currentActiveZoneWeatherPrefab = Weather.WeatherPrefabs[weatherId];
            Weather.currentActiveZone.currentActiveZoneWeatherPreset = Weather.WeatherPrefabs[weatherId].weatherPreset;
            EnviroSkyMgr.instance.NotifyZoneWeatherChanged(Weather.WeatherPrefabs[weatherId].weatherPreset, Weather.currentActiveZone);
        }
    }

    /// <summary>
    /// Set weather over id with smooth transtion.
    /// </summary>
    public void ChangeWeather(EnviroWeatherPreset preset)
    {
        if (preset == null)
            return;

        if (preset != Weather.currentActiveWeatherPreset)
        {
            for (int i = 0; i < Weather.WeatherPrefabs.Count; i++)
            {
                if (preset == Weather.WeatherPrefabs[i].weatherPreset)
                {
                    Weather.currentActiveZone.currentActiveZoneWeatherPrefab = Weather.WeatherPrefabs[i];
                    Weather.currentActiveZone.currentActiveZoneWeatherPreset = preset;
                    EnviroSkyMgr.instance.NotifyZoneWeatherChanged(preset, Weather.currentActiveZone);
                }
            }
        }
    }

    /// <summary>
    /// Set weather over name.
    /// </summary>
    public void ChangeWeather(string weatherName)
    {
        for (int i = 0; i < Weather.WeatherPrefabs.Count; i++)
        {
            if (Weather.WeatherPrefabs[i].weatherPreset.Name == weatherName && Weather.WeatherPrefabs[i] != Weather.currentActiveWeatherPrefab)
            {
                ChangeWeather(i);
                EnviroSkyMgr.instance.NotifyZoneWeatherChanged(Weather.WeatherPrefabs[i].weatherPreset, Weather.currentActiveZone);
            }
        }
    }

    public void UpdateAudioSource(EnviroWeatherPreset i)
    {
        if (i != null && i.weatherSFX != null)
        {
            if (i.weatherSFX == Weather.currentAudioSource.audiosrc.clip)
            {
                if (Weather.currentAudioSource.audiosrc.volume < 0.1f)
                    Weather.currentAudioSource.FadeIn(i.weatherSFX);

                return;
            }

            if (Weather.currentAudioSource == Audio.AudioSourceWeather)
            {
                Audio.AudioSourceWeather.FadeOut();
                Audio.AudioSourceWeather2.FadeIn(i.weatherSFX);
                Weather.currentAudioSource = Audio.AudioSourceWeather2;
            }
            else if (Weather.currentAudioSource == Audio.AudioSourceWeather2)
            {
                Audio.AudioSourceWeather2.FadeOut();
                Audio.AudioSourceWeather.FadeIn(i.weatherSFX);
                Weather.currentAudioSource = Audio.AudioSourceWeather;
            }
        }
        else
        {
            Audio.AudioSourceWeather.FadeOut();
            Audio.AudioSourceWeather2.FadeOut();
        }
    }

    void RenderSatCamera(Camera targetCam, Camera.MonoOrStereoscopicEye eye)
    {
        targetCam.fieldOfView = PlayerCamera.fieldOfView;
        targetCam.aspect = PlayerCamera.aspect;

        switch (eye)
        {
            case Camera.MonoOrStereoscopicEye.Mono:
                targetCam.transform.position = PlayerCamera.transform.position;
                targetCam.transform.rotation = PlayerCamera.transform.rotation;
                targetCam.worldToCameraMatrix = PlayerCamera.worldToCameraMatrix;
                targetCam.Render();
                break;

            case Camera.MonoOrStereoscopicEye.Left:

                targetCam.transform.position = PlayerCamera.transform.position;
                targetCam.transform.rotation = PlayerCamera.transform.rotation;
                targetCam.projectionMatrix = PlayerCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
                targetCam.worldToCameraMatrix = PlayerCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
                targetCam.Render();

                /*	if (EnviroSky.instance.singlePassVR == true) 
                    {
                       if (targetCam == EnviroSky.instance.skyCamera && spSkyCam != null) {
                            spSkyCam.fieldOfView = EnviroSky.instance.PlayerCamera.fieldOfView;	
                            spSkyCam.aspect = EnviroSky.instance.PlayerCamera.aspect;
                            spSkyCam.projectionMatrix = EnviroSky.instance.PlayerCamera.GetStereoProjectionMatrix (Camera.StereoscopicEye.Right);
                            spSkyCam.worldToCameraMatrix = EnviroSky.instance.PlayerCamera.GetStereoViewMatrix (Camera.StereoscopicEye.Right);
                            spSkyCam.Render ();
                            material.SetTexture ("_SkySPSR", spSkytex);
                        }
                    }*/
                break;

            case Camera.MonoOrStereoscopicEye.Right:
                targetCam.transform.position = PlayerCamera.transform.position;
                targetCam.transform.rotation = PlayerCamera.transform.rotation;
                targetCam.projectionMatrix = PlayerCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
                targetCam.worldToCameraMatrix = PlayerCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
                targetCam.Render();
                break;
        }
    } 
#if ENVIRO_HD
    public void UpdateSatelliteRender()
    {
        if (EnviroSky.instance.satCamera != null)
        {
            RenderSatCamera(EnviroSky.instance.satCamera, Camera.MonoOrStereoscopicEye.Mono);    
            RenderSettings.skybox.SetTexture("_SatTex", EnviroSky.instance.satCamera.targetTexture);
        }
    }
#endif
    public void RegisterZone(EnviroZone zoneToAdd)
    {
        Weather.zones.Add(zoneToAdd);
    }


    public void EnterZone(EnviroZone zone)
    {
        Weather.currentActiveZone = zone;
    }

    public void ExitZone()
    {

    }


    public void UpdateParticleClouds(bool active)
    {
        if (particleClouds.layer1System == null || particleClouds.layer2System == null)
            return;

        if (active)
        {

            if(!cloudsSettings.dualLayerParticleClouds)
            {
                if (particleClouds.layer2System.gameObject.activeSelf)
                    particleClouds.layer2System.gameObject.SetActive(false);

                particleClouds.layer1System.transform.position = new Vector3(particleClouds.layer1System.transform.position.x, transform.localScale.y * cloudsSettings.ParticleCloudsLayer1.height, particleClouds.layer1System.transform.position.z);
                Color layer1Color = cloudsSettings.ParticleCloudsLayer1.particleCloudsColor.Evaluate(GameTime.solarTime) * cloudsConfig.particleLayer1Brightness;
                layer1Color.a = cloudsConfig.particleLayer1Alpha;
                particleClouds.layer1Material.SetColor("_CloudsColor", layer1Color);

                if (!particleClouds.layer1System.gameObject.activeSelf)
                    particleClouds.layer1System.gameObject.SetActive(true);
            }
            else
            {
                particleClouds.layer1System.transform.position = new Vector3(particleClouds.layer1System.transform.position.x, transform.localScale.y * cloudsSettings.ParticleCloudsLayer1.height, particleClouds.layer1System.transform.position.z);
                particleClouds.layer2System.transform.position = new Vector3(particleClouds.layer2System.transform.position.x, transform.localScale.y * cloudsSettings.ParticleCloudsLayer2.height, particleClouds.layer2System.transform.position.z);

                //Render Queue
                if (cloudsSettings.ParticleCloudsLayer1.height >= cloudsSettings.ParticleCloudsLayer2.height)
                {
                    particleClouds.layer1Material.renderQueue = 3001;
                    particleClouds.layer2Material.renderQueue = 3002;
                }
                else
                {
                    particleClouds.layer1Material.renderQueue = 3002;
                    particleClouds.layer2Material.renderQueue = 3001;
                }

                Color layer1Color = cloudsSettings.ParticleCloudsLayer1.particleCloudsColor.Evaluate(GameTime.solarTime) * cloudsConfig.particleLayer1Brightness;
                layer1Color.a = cloudsConfig.particleLayer1Alpha;
                particleClouds.layer1Material.SetColor("_CloudsColor", layer1Color);


                //Layer2
                Color layer2Color = cloudsSettings.ParticleCloudsLayer2.particleCloudsColor.Evaluate(GameTime.solarTime) * cloudsConfig.particleLayer2Brightness;
                layer2Color.a = cloudsConfig.particleLayer2Alpha;
                particleClouds.layer2Material.SetColor("_CloudsColor", layer2Color);

                if (!particleClouds.layer1System.gameObject.activeSelf)
                    particleClouds.layer1System.gameObject.SetActive(true);
                if (!particleClouds.layer2System.gameObject.activeSelf)
                    particleClouds.layer2System.gameObject.SetActive(true);
            }
        }
        else
        {
            if (particleClouds.layer1System != null && particleClouds.layer1System.isPlaying)
                particleClouds.layer1System.gameObject.SetActive(false);

            if (particleClouds.layer2System != null && particleClouds.layer2System.isPlaying)
                particleClouds.layer2System.gameObject.SetActive(false);

        }
    }
    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Satellites
    /// <summary>
    /// Instantiates a new satellite
    /// </summary>
    /// <param name="id">Identifier.</param>
    public void CreateSatellite(int id)
    {
        if (satelliteSettings.additionalSatellites[id].prefab == null)
        {
            Debug.Log("Satellite without prefab! Please assign a prefab to all satellites.");
            return;
        }
        GameObject satRot = new GameObject();
        satRot.name = satelliteSettings.additionalSatellites[id].name;
        satRot.transform.parent = Components.satellites;
        satRot.transform.localScale = Vector3.one;
        satRot.transform.localPosition = Vector3.zero;
        satellitesRotation.Add(satRot);
        GameObject sat = (GameObject)Instantiate(satelliteSettings.additionalSatellites[id].prefab, satRot.transform);
        sat.layer = satelliteRenderingLayer;
        satellites.Add(sat);
    }

    /// <summary>
    /// Destroy and recreate all satellites
    /// </summary>
    public void CheckSatellites()
    {
        satellites = new List<GameObject>();

        int childs = Components.satellites.childCount;
        for (int i = childs - 1; i >= 0; i--)
        {
            DestroyImmediate(Components.satellites.GetChild(i).gameObject);
        }

        satellites.Clear();
        satellitesRotation.Clear();

        for (int i = 0; i < satelliteSettings.additionalSatellites.Count; i++)
        {
            CreateSatellite(i);
        }
    }

    
    public void CalculateSatPositions(float siderealTime)
    {
        for (int i = 0; i < satelliteSettings.additionalSatellites.Count; i++)
        {
            Quaternion satRotation = Quaternion.Euler(90 - GameTime.Latitude, GameTime.Longitude, 0);
            satRotation *= Quaternion.Euler(satelliteSettings.additionalSatellites[i].xRot, siderealTime, satelliteSettings.additionalSatellites[i].yRot);

            if (satellites.Count >= 1 && satellites.Count >= i)
                satellites[i].transform.localPosition = new Vector3(0f, satelliteSettings.additionalSatellites[i].orbit, 0f);
            if (satellitesRotation.Count >= 1 && satellitesRotation.Count >= i)
                satellitesRotation[i].transform.localRotation = satRotation;
        }
    }
    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Helper
    /// <summary>
    /// Helper function to set camera hdr for different unity versions.
    /// </summary>
    public void SetCameraHDR(Camera cam, bool hdr)
    {
#if UNITY_5_6_OR_NEWER
        cam.allowHDR = hdr;
#else
		cam.hdr = hdr;
#endif
    }
    /// <summary>
    /// Helper function to get camera hdr bool for different unity versions.
    /// </summary>
    public bool GetCameraHDR(Camera cam)
    {
#if UNITY_5_6_OR_NEWER
        return cam.allowHDR;
#else
		return cam.hdr;
#endif
    }

    private Quaternion LightLookAt(Quaternion inputRotation, Quaternion newRotation)
    {
        return Quaternion.Lerp(inputRotation, newRotation, 500f * Time.deltaTime);
    }

    /// <summary>
    /// Called internaly from growth objects
    /// </summary>
    /// <param name="season">Season.</param>
    public int RegisterMe(EnviroVegetationInstance me)
    {
        EnviroVegetationInstances.Add(me);
        return EnviroVegetationInstances.Count - 1;
    }

    /// <summary>
    /// Saves the current time and weather in Playerprefs.
    /// </summary>
    public void Save()
    {
        PlayerPrefs.SetFloat("Time_Hours", internalHour);
        PlayerPrefs.SetInt("Time_Days", GameTime.Days);
        PlayerPrefs.SetInt("Time_Years", GameTime.Years);
        for (int i = 0; i < Weather.WeatherPrefabs.Count; i++)
        {
            if (Weather.WeatherPrefabs[i] == Weather.currentActiveWeatherPrefab)
                PlayerPrefs.SetInt("currentWeather", i);
        }
/*
        for(int i = 0; i < EnviroSky.instance.cloudsQualityList.Count; i++)
        {
            if(EnviroSky.instance.cloudsQualityList[i] == EnviroSky.instance.currentActiveCloudsQualityPreset)
            {
                PlayerPrefs.SetInt("Clouds_Quality",i);
                break;
            }            
        }   
  */
    }

    /// <summary>
    /// Loads the saved time and weather from Playerprefs.
    /// </summary>
    public void Load()
    {
        if (PlayerPrefs.HasKey("Time_Hours"))
            SetInternalTimeOfDay(PlayerPrefs.GetFloat("Time_Hours"));
        if (PlayerPrefs.HasKey("Time_Days"))
            GameTime.Days = PlayerPrefs.GetInt("Time_Days");
        if (PlayerPrefs.HasKey("Time_Years"))
            GameTime.Years = PlayerPrefs.GetInt("Time_Years");
        if (PlayerPrefs.HasKey("currentWeather"))
            SetWeatherOverwrite(PlayerPrefs.GetInt("currentWeather"));
/*
        if (PlayerPrefs.HasKey("Clouds_Quality"))
        {
            EnviroSky.instance.ApplyVolumeCloudsQualityPreset(PlayerPrefs.GetInt("Clouds_Quality"));
        }
*/
    }

#if ENVIRO_HDRP
        public UnityEngine.Rendering.VolumeProfile GetDefaultSkyAndFogProfile(string name)
        {
#if UNITY_EDITOR
            string[] assets = UnityEditor.AssetDatabase.FindAssets(name, null);

            for (int idx = 0; idx < assets.Length; idx++)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(assets[idx]);

                if (path.Contains(name + ".asset")) 
                {
                    return UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Rendering.VolumeProfile>(path);
                }
            }
#endif
            return null;
        }
#endif


    #endregion
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    #region Third Party
#if AURA_IN_PROJECT
    public EnviroAura2Config enviroAura2Config = new EnviroAura2Config();
    public Aura2API.AuraCamera[] auraCameras;
    public Aura2API.AuraLight[] auraDirLight;

    public void UpdateAura2(EnviroWeatherPreset id, bool withTransition)
    {
        if (id != null && EnviroSkyMgr.instance != null)
        {

            float speed = 500f * Time.deltaTime;

            if (withTransition)
                speed = EnviroSkyMgr.instance.aura2TransitionSpeed * Time.deltaTime;

            enviroAura2Config.aura2GlobalDensity = Mathf.Lerp(enviroAura2Config.aura2GlobalDensity, id.enviroAura2Config.aura2GlobalDensity, speed);
            enviroAura2Config.aura2GlobalScattering = Mathf.Lerp(enviroAura2Config.aura2GlobalScattering, id.enviroAura2Config.aura2GlobalScattering, speed);
            enviroAura2Config.aura2GlobalAmbientLight = Mathf.Lerp(enviroAura2Config.aura2GlobalAmbientLight, id.enviroAura2Config.aura2GlobalAmbientLight, speed);
            enviroAura2Config.aura2GlobalExtinction = Mathf.Lerp(enviroAura2Config.aura2GlobalExtinction, id.enviroAura2Config.aura2GlobalExtinction, speed);

            for (int i = 0; i < auraCameras.Length; i++)
            { 
                //Check if aura is not ready
                if (auraCameras[i].frustumSettings.baseSettings == null)
                {
                    auraCameras[i].frustumSettings.baseSettings = new Aura2API.AuraBaseSettings();
                    Debug.Log("No Aura 2 Basesetting. Enviro will create new one.");
                }
 
                auraCameras[i].frustumSettings.baseSettings.density = enviroAura2Config.aura2GlobalDensity;
                auraCameras[i].frustumSettings.baseSettings.scattering = enviroAura2Config.aura2GlobalScattering;
                auraCameras[i].frustumSettings.baseSettings.ambientLightingStrength = enviroAura2Config.aura2GlobalAmbientLight;
                auraCameras[i].frustumSettings.baseSettings.extinction = enviroAura2Config.aura2GlobalExtinction;
            }

            for (int i = 0; i < auraDirLight.Length; i++)
            {
                if(auraDirLight[i].gameObject.name == "Enviro Directional Light")
                {
                    auraDirLight[i].strength = EnviroSkyMgr.instance.aura2DirectionalLightIntensity.Evaluate(GameTime.solarTime);
                }

                if(auraDirLight[i].gameObject.name == "Enviro Directional Light - Moon")
                {
                    auraDirLight[i].strength = EnviroSkyMgr.instance.aura2DirectionalLightIntensityMoon.Evaluate(GameTime.lunarTime);
                }
            }
        }
    }
#endif
    #endregion

}
