////////////////////////////////////////////////////////////////////////////
////////////                    EnviroSky.cs                        ////////
////////////////////////////////////////////////////////////////////////////

using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

[Serializable]
public class EnviroRessources
{ 
    //Sky
    public Cubemap starsTwinklingNoise;
    public Texture2D aurora_layer_1;
    public Texture2D aurora_layer_2;
    public Texture2D aurora_colorshift;
    //Volume Clouds
    public Texture2D curlMap;
    public Texture3D noiseTextureHigh;
    public Texture3D noiseTexture;
    public Texture3D detailNoiseTexture;
    public Texture3D detailNoiseTextureHigh;
    public Texture2D dither;
    public Texture2D blueNoise;

    //Distance Blur
    public Texture2D distributionTexture;
}



[ExecuteInEditMode]
public class EnviroSky : EnviroCore
{

    ////////////////////////////////

    private static EnviroSky _instance; // Creat a static instance for easy access!

    public static EnviroSky instance
    {
        get 
        {
            //If _instance hasn't been set yet, we grab it from the scene!
            //This will only happen the first time this reference is used.
            if (_instance == null)
                _instance = GameObject.FindObjectOfType<EnviroSky>();
            return _instance;
        }
    }

    #region Var
    [Tooltip("Enable this when using singlepass instanced rendering.")]
    public bool singlePassInstancedVR = false;

    [Tooltip("Enable this to activate volume lighing")]
    [HideInInspector]
    public bool useVolumeLighting = true;
    [HideInInspector]
    public bool useVolumeClouds = true;
    [HideInInspector]
    public bool useFog = true;
    [HideInInspector]
    public bool useFlatClouds = false;
    [HideInInspector]
    public bool useParticleClouds = false;
    [HideInInspector]
    public bool useDistanceBlur = true;
    [HideInInspector]
    public bool useAurora = false;
    private bool flatCloudsSkybox = false;

    public bool showVolumeLightingInEditor = true;
    public bool showVolumeCloudsInEditor = true;
    public bool showFlatCloudsInEditor = true;
    public bool showFogInEditor = true;
    public bool showDistanceBlurInEditor = true;
    public bool showSettings = false;

    //Camera Components
    [HideInInspector] 
    public Camera satCamera;
    [HideInInspector]
    public EnviroVolumeLight directVolumeLight;
    [HideInInspector]
    public EnviroVolumeLight additionalDirectVolumeLight;
    [HideInInspector]
    public EnviroSkyRendering EnviroSkyRender;

    public float auroraIntensity;
    public EnviroVolumeCloudsQuality currentActiveCloudsQualityPreset;

    // Render Textures
    [HideInInspector]
    public RenderTexture cloudsRenderTarget;
    [HideInInspector]
    public RenderTexture weatherMap;
    [HideInInspector]
    public RenderTexture satRenderTarget;
    [HideInInspector]
    public RenderTexture cloudShadowMap;

    //Materials
    [HideInInspector]
    public Material skyMat;
    [HideInInspector]
    public Material skyReflectionMat;
    private Material weatherMapMat;
    private Material cloudShadowMat;

    //Quality
    public List<EnviroVolumeCloudsQuality> cloudsQualityList = new List<EnviroVolumeCloudsQuality>();
    private string[] cloudsQualityPresetsFound;
    public int selectedCloudsQuality = 0;

    private float starsTwinklingRot;
    private EnviroSkySettings.SkyboxModi currentSkyboxMode;

    //Distance Blur
    public float blurDistance = 100;
    public float blurIntensity = 1f;
    public float blurSkyIntensity = 1f;


    //Floating Point Origin Mod 
    public Transform floatingPointOriginAnchor;
    public Vector3 floatingPointOriginMod = Vector3.zero;

    [HideInInspector]
    public EnviroRessources ressources;

    #endregion
    ////////////////////////////////
    #region Startup Setup
    void Start()
    {
        //Check for Manager first!
        if (EnviroSkyMgr.instance == null)
        {
            Debug.Log("Please use the EnviroSky Manager!");
            gameObject.SetActive(false);
            return;
        }

        started = false;

        //Time
        SetTime(GameTime.Years, GameTime.Days, GameTime.Hours, GameTime.Minutes, GameTime.Seconds);
        lastHourUpdate = GameTime.Hours;
        currentTimeInHours = GetInHours(internalHour, GameTime.Days, GameTime.Years, GameTime.DaysInYear);
        Weather.weatherFullyChanged = false;
        thunder = 0f;

        // Setup ReflectionProbe
        if (Components.GlobalReflectionProbe == null)
        {
            GameObject temp;

            foreach (Transform child in transform)
            {
                if (child.name == "GlobalReflections")
                {
                    temp = child.gameObject;
                    Components.GlobalReflectionProbe = temp.GetComponent<EnviroReflectionProbe>();
                    if (Components.GlobalReflectionProbe == null)
                        Components.GlobalReflectionProbe = temp.AddComponent<EnviroReflectionProbe>();
                }
            }
        }

        // Check for Profile
        if (profileLoaded)
        {
            InvokeRepeating("UpdateEnviroment", 0, qualitySettings.UpdateInterval);// Vegetation Updates

            if (PlayerCamera != null && Player != null && AssignInRuntime == false && startMode == EnviroStartMode.Started)
            {
                Init();
            }
        }
    }

    private IEnumerator SetSceneSettingsLate()
    {
        yield return 0;
        if (skyMat != null && RenderSettings.skybox != skyMat)
            SetupSkybox();

        // Setup Fog Mode
        if (RenderSettings.fogMode != fogSettings.Fogmode)
            RenderSettings.fogMode = fogSettings.Fogmode;
        // Set ambient mode
        if (RenderSettings.ambientMode != lightSettings.ambientMode)
            RenderSettings.ambientMode = lightSettings.ambientMode;
    }

    void OnEnable()
    {
        #if ENVIRO_HDRP
        Shader.EnableKeyword("ENVIROHDRP");
        #else
        Shader.DisableKeyword("ENVIROHDRP");
        #endif

        #if ENVIRO_LWRP
        Shader.EnableKeyword("ENVIROURP");
        #else
        Shader.DisableKeyword("ENVIROURP");
        #endif

        //Check for Manager first!
        if (EnviroSkyMgr.instance == null)
        {
            return;
        }
        // Load all ressources
        LoadRessources();

        //Set Weather
        Weather.currentActiveWeatherPreset = Weather.zones[0].currentActiveZoneWeatherPreset;
        Weather.lastActiveWeatherPreset = Weather.currentActiveWeatherPreset;
        
#if AURA_IN_PROJECT
        auraCameras = Aura2API.Aura.GetAuraCameras();
        auraDirLight = Aura2API.Aura.GetAuraLights(LightType.Directional);
#endif

        //Create material
        if (weatherMapMat == null)
            weatherMapMat = new Material(Shader.Find("Enviro/Standard/WeatherMap"));

        if (profile == null)
        {
            Debug.LogError("No profile assigned!");
            return;
        }

        // Auto Load profile
        if (profileLoaded == false)
            ApplyProfile(profile);

        PreInit();

        if (AssignInRuntime)
        {
            started = false;    //Wait for assignment
        }
        else if (PlayerCamera != null && Player != null && startMode == EnviroStartMode.Started)
        {
            Init();
        }

        //Update Quality Settings List
        PopulateCloudsQualityList();

        if (currentActiveCloudsQualityPreset != null)
            ApplyVolumeCloudsQualityPreset(currentActiveCloudsQualityPreset);
    }
    /// <summary>
    /// Re-Initilize the system.
    /// </summary>
    public void ReInit()
    {
        OnEnable();
    }

    /// <summary>
    /// Pee-Initilize the system.
    /// </summary>
    private void PreInit()
    {
        // Check time
        if (GameTime.solarTime < GameTime.dayNightSwitch)
            isNight = true;
        else
            isNight = false;

        CreateEffects("Enviro Effects");  //Create Weather Effects Holder

        // Instantiate Lightning Effect
        if (weatherSettings.lightningEffect != null && lightningEffect == null)
            lightningEffect = Instantiate(weatherSettings.lightningEffect, EffectsHolder.transform).GetComponent<ParticleSystem>();

        //return when in server mode!
        if (serverMode)
            return;

        CheckSatellites();

        // Setup ReflectionProbe
        if (Components.GlobalReflectionProbe == null)
        {
            GameObject temp;

            foreach (Transform child in transform)
            {
                if (child.name == "GlobalReflections")
                {
                    temp = child.gameObject;
                    Components.GlobalReflectionProbe = temp.GetComponent<EnviroReflectionProbe>();
                    if (Components.GlobalReflectionProbe == null)
                        Components.GlobalReflectionProbe = temp.AddComponent<EnviroReflectionProbe>();
                }
            }
        }

        if (!Components.Sun)
        {
            Debug.LogError("Please set sun object in inspector!");
        }

        if (!Components.satellites)
        {
            Debug.LogError("Please set satellite object in inspector!");
        }

        if (Components.Moon)
        {
            MoonTransform = Components.Moon.transform;
        }
        else
        {
            Debug.LogError("Please set moon object in inspector!");
        }

        if (weatherMap != null)
            DestroyImmediate(weatherMap);

        if (weatherMap == null)
        {
            weatherMap = new RenderTexture(512, 512, 0, RenderTextureFormat.ARGBHalf);
            weatherMap.wrapMode = TextureWrapMode.Repeat;
        }

        if (lightSettings.directionalLightMode == EnviroLightSettings.LightingMode.Single)
        {
            SetupMainLight();
        }
        else
        {
            SetupMainLight();
            SetupAdditionalLight();
        }

        if (cloudShadowMap != null)
            DestroyImmediate(cloudShadowMap);
 
#if !ENVIRO_HDRP

        if(cloudShadowMap == null)
        {
            cloudShadowMap = new RenderTexture(2048, 2048, 0, RenderTextureFormat.Default);
            cloudShadowMap.wrapMode = TextureWrapMode.Repeat;
        }    

        if (cloudShadowMat != null)
            DestroyImmediate(cloudShadowMat);

        cloudShadowMat = new Material(Shader.Find("Enviro/Standard/ShadowCookie"));

        if (cloudsSettings.shadowIntensity > 0)
        {
            Graphics.Blit(weatherMap, cloudShadowMap, cloudShadowMat);

//#if ENVIRO_HDRP
//            MainLightHDRP.SetCookie(cloudShadowMap,new Vector2(cloudsSettings.shadowCookieSize,cloudsSettings.shadowCookieSize));
//#else
            MainLight.cookie = cloudShadowMap;
            MainLight.cookieSize = 10000;
            
//#endif
        }
        else
            MainLight.cookie = null;
#endif
        if (Components.particleClouds)
        {
            ParticleSystem[] systems = Components.particleClouds.GetComponentsInChildren<ParticleSystem>();

            if (systems.Length > 0)
                particleClouds.layer1System = systems[0];
            if (systems.Length > 1)
                particleClouds.layer2System = systems[1];

            if (particleClouds.layer1System != null)
                particleClouds.layer1Material = particleClouds.layer1System.GetComponent<ParticleSystemRenderer>().sharedMaterial;

            if (particleClouds.layer2System != null)
                particleClouds.layer2Material = particleClouds.layer2System.GetComponent<ParticleSystemRenderer>().sharedMaterial;
        }
        else
        {
            Debug.LogError("Please set particleCLouds object in inspector!");
        }

    }
    /// <summary>
    /// Creation and assignment of skybox for standard and urp
    /// </summary>
    public void SetupSkybox()
    {
        if (skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.Simple)
        {
            if (skyMat != null)
                DestroyImmediate(skyMat);

            skyMat = new Material(Shader.Find("Enviro/Standard/SkyboxSimple"));

            if (skySettings.starsCubeMap != null)
                skyMat.SetTexture("_Stars", skySettings.starsCubeMap);

            if (skySettings.galaxyCubeMap != null)
                skyMat.SetTexture("_Galaxy", skySettings.galaxyCubeMap);

            if (ressources.starsTwinklingNoise != null)
                skyMat.SetTexture("_StarsTwinklingNoise", ressources.starsTwinklingNoise);

            //AURORA SETUP
            if (ressources.aurora_layer_1 != null)
                skyMat.SetTexture("_Aurora_Layer_1", ressources.aurora_layer_1);

            if (ressources.aurora_layer_2 != null)
                skyMat.SetTexture("_Aurora_Layer_2", ressources.aurora_layer_2);

            if (ressources.aurora_colorshift != null)
                skyMat.SetTexture("_Aurora_Colorshift", ressources.aurora_colorshift);

            RenderSettings.skybox = skyMat;
        }
        else if (skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.Default)
        {
            if (skyMat != null)
                DestroyImmediate(skyMat);

            if (!useFlatClouds)
            {
                skyMat = new Material(Shader.Find("Enviro/Standard/Skybox"));
                flatCloudsSkybox = false;
            }
            else
            {
                skyMat = new Material(Shader.Find("Enviro/Standard/SkyboxFlatClouds"));
                flatCloudsSkybox = true;
            }

            if (skySettings.starsCubeMap != null)
                skyMat.SetTexture("_Stars", skySettings.starsCubeMap);
            if (skySettings.galaxyCubeMap != null)
                skyMat.SetTexture("_Galaxy", skySettings.galaxyCubeMap);

            if (ressources.starsTwinklingNoise != null)
                skyMat.SetTexture("_StarsTwinklingNoise", ressources.starsTwinklingNoise);

            //AURORA SETUP
            if (ressources.aurora_layer_1 != null)
                skyMat.SetTexture("_Aurora_Layer_1", ressources.aurora_layer_1);

            if (ressources.aurora_layer_2 != null)
                skyMat.SetTexture("_Aurora_Layer_2", ressources.aurora_layer_2);

            if (ressources.aurora_colorshift != null)
                skyMat.SetTexture("_Aurora_Colorshift", ressources.aurora_colorshift);

            RenderSettings.skybox = skyMat;
        }
        else if (skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.CustomSkybox)
        {
            if (skySettings.customSkyboxMaterial != null)
            {
                RenderSettings.skybox = skySettings.customSkyboxMaterial;
                skyMat = skySettings.customSkyboxMaterial;
            }
        }
        else if (skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.CustomColor)
        {
            if (skyMat != null)
                DestroyImmediate(skyMat);
        }

        //Update environment texture in next frame!
        if (lightSettings.ambientMode == UnityEngine.Rendering.AmbientMode.Skybox)
            StartCoroutine(UpdateAmbientLightWithDelay());

        currentSkyboxMode = skySettings.skyboxMode;
    }


#if ENVIRO_HDRP
    /// <summary>
    /// Search for existing volume with visualEnvionment module and set sky to Enviro.
    /// </summary>
    public void CreateSkyAndFogVolume()
    {
        if(enviroVolumeHDRP == null)
        {
            UnityEngine.Rendering.Volume[] volume = GameObject.FindObjectsOfType<UnityEngine.Rendering.Volume>();
            
            bool found = false;
            
            for (int i = 0; i < volume.Length; i++)
            {
                 if (volume[i].name == "Enviro Sky and Fog Volume")
                 {
                     enviroVolumeHDRP = volume[i];
                     found = true;
                 }
            } 

            if(found == false)
               CreateNewSkyVolume();
        }
       
        UnityEngine.Rendering.HighDefinition.VisualEnvironment TempEnv;
        UnityEngine.Rendering.HighDefinition.EnviroSkybox TempSky;
        UnityEngine.Rendering.HighDefinition.Fog TempFog;
        UnityEngine.Rendering.HighDefinition.Exposure TempExposure;
        
        if(visualEnvironment == null)
        {
            if (enviroVolumeHDRP.sharedProfile != null && enviroVolumeHDRP.sharedProfile.TryGet<UnityEngine.Rendering.HighDefinition.VisualEnvironment>(out TempEnv))
            {
                visualEnvironment = TempEnv;
            }
            else 
            {
                enviroVolumeHDRP.sharedProfile.Add<UnityEngine.Rendering.HighDefinition.VisualEnvironment>();

                if (enviroVolumeHDRP.sharedProfile.TryGet<UnityEngine.Rendering.HighDefinition.VisualEnvironment>(out TempEnv))
                {
                    visualEnvironment = TempEnv;
                }
            }
        }

        // Sky
        if(enviroHDRPSky == null)
        {
            if (enviroVolumeHDRP.sharedProfile != null && enviroVolumeHDRP.sharedProfile.TryGet<UnityEngine.Rendering.HighDefinition.EnviroSkybox>(out TempSky))
            {
                enviroHDRPSky = TempSky;
            }
            else
            {
                enviroVolumeHDRP.sharedProfile.Add<UnityEngine.Rendering.HighDefinition.EnviroSkybox>();

                if (enviroVolumeHDRP.sharedProfile.TryGet<UnityEngine.Rendering.HighDefinition.EnviroSkybox>(out TempSky))
                {
                    enviroHDRPSky = TempSky;
                }
            }
        }

        // FOG
        if(hdrpFog == null)
        {
            if (enviroVolumeHDRP.sharedProfile != null && enviroVolumeHDRP.sharedProfile.TryGet<UnityEngine.Rendering.HighDefinition.Fog>(out TempFog))
            {
                hdrpFog = TempFog;
            }
            else
            {
                enviroVolumeHDRP.sharedProfile.Add<UnityEngine.Rendering.HighDefinition.Fog>();

                if (enviroVolumeHDRP.sharedProfile.TryGet<UnityEngine.Rendering.HighDefinition.Fog>(out TempFog))
                {
                    hdrpFog = TempFog;
                }
            }
        }


        // Exposure
        if(enviroExposure == null)
        {
            if (enviroVolumeHDRP.sharedProfile != null && enviroVolumeHDRP.sharedProfile.TryGet<UnityEngine.Rendering.HighDefinition.Exposure>(out TempExposure))
            {
                enviroExposure = TempExposure;
            }
            else
            {
                enviroVolumeHDRP.sharedProfile.Add<UnityEngine.Rendering.HighDefinition.Exposure>();

                if (enviroVolumeHDRP.sharedProfile.TryGet<UnityEngine.Rendering.HighDefinition.Exposure>(out TempExposure))
                {
                    enviroExposure = TempExposure;
                }
            }
        }

        //Don't Destroy on Load
        if(Application.isPlaying && EnviroSkyMgr.instance != null && EnviroSkyMgr.instance.dontDestroy)
            DontDestroyOnLoad(enviroVolumeHDRP.gameObject);


        if (enviroHDRPSky != null)
        {
            enviroHDRPSky.skyIntensityMode.overrideState = true;
            enviroHDRPSky.skyIntensityMode.value = UnityEngine.Rendering.HighDefinition.SkyIntensityMode.Exposure;
            enviroHDRPSky.exposure.overrideState = true;

            if(lightSettings.usePhysicalBasedLighting)
                enviroHDRPSky.exposure.value = lightSettings.skyExposurePhysical.Evaluate(GameTime.solarTime) * currentSkyExposureMod;
            else
                enviroHDRPSky.exposure.value = lightSettings.skyExposure;

            SetLightingUpdateMode();
        }

        if (hdrpFog != null)
        {
            hdrpFog.meanFreePath.overrideState = true;
            hdrpFog.maximumHeight.overrideState = true;
            hdrpFog.enabled.overrideState = true;
        }

        if (visualEnvironment != null)
        {
            if (skySettings.setEnviroSkybox)
            {
                visualEnvironment.skyType.value = 999;
                visualEnvironment.skyType.overrideState = true;
            }

            visualEnvironment.skyAmbientMode.value = UnityEngine.Rendering.HighDefinition.SkyAmbientMode.Dynamic;
            visualEnvironment.skyAmbientMode.overrideState = true;
        }
    }

    /// <summary>
    /// Creates a Sky and Fog Volume with a new profile. Saver than try to modify already existing profiles that could lead to a lot of issues.
    /// </summary>
    private void CreateNewSkyVolume()
    {
        GameObject volumeObj = new GameObject();
        volumeObj.name = "Enviro Sky and Fog Volume";
        mySkyAndFogVolume = volumeObj;
        UnityEngine.Rendering.Volume volumeCreated = volumeObj.AddComponent<UnityEngine.Rendering.Volume>();
        UnityEngine.Rendering.VolumeProfile profile = GetDefaultSkyAndFogProfile("Enviro Sky and Fog Profile");
        
        enviroVolumeHDRP = volumeCreated;

        volumeCreated.priority = 1;

        if (profile == null)
        {
            UnityEngine.Rendering.VolumeProfile profile1 = new UnityEngine.Rendering.VolumeProfile();
            volumeCreated.sharedProfile = profile1;
            visualEnvironment = volumeCreated.sharedProfile.Add<UnityEngine.Rendering.HighDefinition.VisualEnvironment>();
            visualEnvironment.skyType.overrideState = true;
            visualEnvironment.skyType.value = 999;
            visualEnvironment.skyAmbientMode.overrideState = true;
            visualEnvironment.skyAmbientMode.value = UnityEngine.Rendering.HighDefinition.SkyAmbientMode.Dynamic;

            //Sky
            enviroHDRPSky = volumeCreated.sharedProfile.Add<UnityEngine.Rendering.HighDefinition.EnviroSkybox>();
            enviroHDRPSky.skyIntensityMode.overrideState = true;
            SetLightingUpdateMode();
            enviroHDRPSky.exposure.overrideState = true;
            if (lightSettings.usePhysicalBasedLighting)
                enviroHDRPSky.exposure.value = lightSettings.skyExposurePhysical.Evaluate(GameTime.solarTime);
            else
                enviroHDRPSky.exposure.value = lightSettings.skyExposure;
                
            enviroHDRPSky.updateMode.value = UnityEngine.Rendering.HighDefinition.EnvironmentUpdateMode.OnChanged;

            // Fog
            hdrpFog = volumeCreated.sharedProfile.Add<UnityEngine.Rendering.HighDefinition.Fog>();
            hdrpFog.meanFreePath.overrideState = true;
            hdrpFog.maximumHeight.overrideState = true;
            hdrpFog.enabled.overrideState = true;

            // Exposure
            enviroExposure = volumeCreated.sharedProfile.Add<UnityEngine.Rendering.HighDefinition.Exposure>();
            enviroExposure.mode.overrideState = true;
            enviroExposure.mode.value = UnityEngine.Rendering.HighDefinition.ExposureMode.Fixed;
            enviroExposure.fixedExposure.overrideState = true;

            if (lightSettings.usePhysicalBasedLighting)
                enviroExposure.fixedExposure.value = lightSettings.exposurePhysical.Evaluate(GameTime.solarTime);
            else
                enviroExposure.fixedExposure.value = lightSettings.exposure;
        }
        else
        {
            volumeCreated.sharedProfile = profile;
        }
    }

    private void SetLightingUpdateMode()
    {
        if(lightSettings.indirectLightingUpdateMode == EnviroLightSettings.AmbientUpdateMode.Realtime)
           enviroHDRPSky.updateMode.value = UnityEngine.Rendering.HighDefinition.EnvironmentUpdateMode.Realtime;
        else
           enviroHDRPSky.updateMode.value = UnityEngine.Rendering.HighDefinition.EnvironmentUpdateMode.OnChanged;
    }

#endif
    /// <summary>
    /// Update the environment texture for skybox ambient mode with one frame delay. Somehow not working in same frame as we create the skybox material.
    /// </summary>
    private IEnumerator UpdateAmbientLightWithDelay()
    {
        yield return 0;
        DynamicGI.UpdateEnvironment();

    }

    public void LoadRessources()
    {
        if (ressources.starsTwinklingNoise == null)
            ressources.starsTwinklingNoise = Resources.Load("cube_enviro_starsNoise") as Cubemap;

        if (ressources.aurora_layer_1 == null)
            ressources.aurora_layer_1 = Resources.Load("tex_enviro_aurora_layer_1") as Texture2D;

        if (ressources.aurora_layer_2 == null)
            ressources.aurora_layer_2 = Resources.Load("tex_enviro_aurora_layer_2") as Texture2D;

        if (ressources.aurora_colorshift == null)
            ressources.aurora_colorshift = Resources.Load("tex_enviro_aurora_colorshift") as Texture2D;

        if (ressources.noiseTextureHigh == null)
            ressources.noiseTextureHigh = Resources.Load("enviro_clouds_base") as Texture3D;

        if (ressources.noiseTexture == null)
            ressources.noiseTexture = Resources.Load("enviro_clouds_base_low") as Texture3D;

        if (ressources.detailNoiseTexture == null)
            ressources.detailNoiseTexture = Resources.Load("enviro_clouds_detail_low") as Texture3D;

        if (ressources.detailNoiseTextureHigh == null)
            ressources.detailNoiseTextureHigh = Resources.Load("enviro_clouds_detail_high") as Texture3D;

        if (ressources.dither == null)
            ressources.dither = Resources.Load("tex_enviro_dither") as Texture2D;

        if (ressources.blueNoise == null)
            ressources.blueNoise = Resources.Load("tex_enviro_blueNoise", typeof(Texture2D)) as Texture2D;

        if (ressources.distributionTexture == null)
            ressources.distributionTexture = Resources.Load("tex_enviro_linear", typeof(Texture2D)) as Texture2D;

        if (ressources.curlMap == null)
            ressources.curlMap = Resources.Load("tex_enviro_curl", typeof(Texture2D)) as Texture2D;
    }

    /// <summary>
    /// Final Initilization and startup.
    /// </summary>
    private void Init()
    {
        if (profile == null)
            return;

        if(isNight)
            EnviroSkyMgr.instance.NotifyIsNight();
        else
            EnviroSkyMgr.instance.NotifyIsDay();

        if (serverMode)
        {
            started = true;
            return;
        }
#if ENVIRO_HDRP
        CreateSkyAndFogVolume();
#else
        if (skyMat != null && RenderSettings.skybox != skyMat)
            SetupSkybox();
        else if (skyMat == null)
            SetupSkybox();
#endif
        // Setup Fog Mode
        if (RenderSettings.fogMode != fogSettings.Fogmode)
            RenderSettings.fogMode = fogSettings.Fogmode;
        // Set ambient mode
        if (RenderSettings.ambientMode != lightSettings.ambientMode)
            RenderSettings.ambientMode = lightSettings.ambientMode;

        InitImageEffects();

        // Setup Camera
        if (PlayerCamera != null)
        {
            if (setCameraClearFlags)
                PlayerCamera.clearFlags = CameraClearFlags.Skybox;
        }

        if (satelliteSettings.additionalSatellites.Count > 0)
            CreateSatCamera();

        UpdateReflections(true);
        //Set enviro as started
        started = true;
    }
    /// <summary>
	/// Creation and setup of post processing components.
	/// </summary>
	private void InitImageEffects()
    {
    #if ENVIRO_HDRP || ENVIRO_LWRP
            EnviroSkyRender = PlayerCamera.gameObject.GetComponent<EnviroSkyRendering>();

            if (EnviroSkyRender != null)
                DestroyImmediate(EnviroSkyRender);

            EnviroPostProcessing = PlayerCamera.gameObject.GetComponent<EnviroPostProcessing>();

            if (EnviroPostProcessing != null)
                DestroyImmediate(EnviroPostProcessing);

            return;
    #else
        EnviroSkyRender = PlayerCamera.gameObject.GetComponent<EnviroSkyRendering>();

        if (EnviroSkyRender == null)
            EnviroSkyRender = PlayerCamera.gameObject.AddComponent<EnviroSkyRendering>();

#if UNITY_EDITOR
        string[] assets = UnityEditor.AssetDatabase.FindAssets("enviro_spot_cookie", null);
        for (int idx = 0; idx < assets.Length; idx++)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(assets[idx]);
            if (path.Length > 0)
            {
#if ENVIRO_HD
                EnviroSkyRender.DefaultSpotCookie = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture>(path);
#endif
            }
        }
#endif
        EnviroPostProcessing = PlayerCamera.gameObject.GetComponent<EnviroPostProcessing>();

        if (EnviroPostProcessing == null)
            EnviroPostProcessing = PlayerCamera.gameObject.AddComponent<EnviroPostProcessing>();
#endif
    }
    /// <summary>
    /// Re-create the camera and render texture for satellite rendering
    /// </summary>
    public void CreateSatCamera()
    {
        Camera[] cams = GameObject.FindObjectsOfType<Camera>();
        for (int i = 0; i < cams.Length; i++)
        {
            cams[i].cullingMask &= ~(1 << satelliteRenderingLayer);
        }

        DestroyImmediate(GameObject.Find("Enviro Sat Camera"));

        GameObject camObj = new GameObject();

        camObj.name = "Enviro Sat Camera";
        camObj.transform.position = PlayerCamera.transform.position;
        camObj.transform.rotation = PlayerCamera.transform.rotation;
        camObj.hideFlags = HideFlags.DontSave;
        satCamera = camObj.AddComponent<Camera>();
        satCamera.farClipPlane = PlayerCamera.farClipPlane;
        satCamera.nearClipPlane = PlayerCamera.nearClipPlane;
        satCamera.aspect = PlayerCamera.aspect;
        satCamera.useOcclusionCulling = false;
        satCamera.renderingPath = RenderingPath.Forward;
        satCamera.fieldOfView = PlayerCamera.fieldOfView;
        satCamera.clearFlags = CameraClearFlags.SolidColor;
        satCamera.backgroundColor = new Color(0f, 0f, 0f, 0f);
        satCamera.cullingMask = (1 << satelliteRenderingLayer);
        satCamera.depth = PlayerCamera.depth + 1;
        satCamera.enabled = true;
        PlayerCamera.cullingMask &= ~(1 << satelliteRenderingLayer);

        var format = GetCameraHDR(satCamera) ? RenderTextureFormat.DefaultHDR : RenderTextureFormat.Default;

        satRenderTarget = new RenderTexture(Screen.currentResolution.width, Screen.currentResolution.height, 16, format);
        satCamera.targetTexture = satRenderTarget;
        satCamera.enabled = false;
    }
    /// <summary>
    /// Setup Main light that is eather used for sun and moon or only sun based on used lighting mode.
    /// </summary>
    private void SetupMainLight()
    {
        if (Components.DirectLight)
        {
            MainLight = Components.DirectLight.GetComponent<Light>();
    #if ENVIRO_HDRP
            MainLightHDRP = Components.DirectLight.GetComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalLightData>();
    #endif

            if (directVolumeLight == null)
                directVolumeLight = Components.DirectLight.GetComponent<EnviroVolumeLight>();

            if (directVolumeLight == null)
                directVolumeLight = Components.DirectLight.gameObject.AddComponent<EnviroVolumeLight>();

            if (EnviroSkyMgr.instance.dontDestroy && Application.isPlaying)
                DontDestroyOnLoad(Components.DirectLight);
        }
        else
        {
            GameObject oldLight = GameObject.Find("Enviro Directional Light");

            if (oldLight != null)
                Components.DirectLight = oldLight.transform;
            else
                Components.DirectLight = CreateDirectionalLight(false);

            MainLight = Components.DirectLight.GetComponent<Light>();

            #if ENVIRO_HDRP
                    MainLightHDRP = Components.DirectLight.GetComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalLightData>();
            #endif

            if (directVolumeLight == null)
                directVolumeLight = Components.DirectLight.GetComponent<EnviroVolumeLight>();

            if (directVolumeLight == null)
                directVolumeLight = Components.DirectLight.gameObject.AddComponent<EnviroVolumeLight>();

            if (EnviroSkyMgr.instance.dontDestroy && Application.isPlaying)
                DontDestroyOnLoad(Components.DirectLight);
        }

        //Remove the additional light if in single mode
        if (lightSettings.directionalLightMode == EnviroLightSettings.LightingMode.Single)
        {
            if (Components.AdditionalDirectLight != null)
                DestroyImmediate(Components.AdditionalDirectLight.gameObject);
        }
    }
    /// <summary>
    /// Setup additional light that is used for moon in dual lighting mode.
    /// </summary>
    private void SetupAdditionalLight()
    {

        if (Components.AdditionalDirectLight)
        {
            AdditionalLight = Components.AdditionalDirectLight.GetComponent<Light>();
    #if ENVIRO_HDRP
            AdditionalLightHDRP = Components.AdditionalDirectLight.GetComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalLightData>();
    #endif

            if (additionalDirectVolumeLight == null)
                additionalDirectVolumeLight = Components.AdditionalDirectLight.GetComponent<EnviroVolumeLight>();

            if (additionalDirectVolumeLight == null)
                additionalDirectVolumeLight = Components.AdditionalDirectLight.gameObject.AddComponent<EnviroVolumeLight>();

            if (EnviroSkyMgr.instance.dontDestroy && Application.isPlaying)
                DontDestroyOnLoad(Components.AdditionalDirectLight);
        }
        else
        {
            GameObject oldLight = GameObject.Find("Enviro Directional Light - Moon");

            if (oldLight != null)
                Components.AdditionalDirectLight = oldLight.transform;
            else
                Components.AdditionalDirectLight = CreateDirectionalLight(true);

            AdditionalLight = Components.DirectLight.GetComponent<Light>();

            #if ENVIRO_HDRP
                    AdditionalLightHDRP = Components.AdditionalDirectLight.GetComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalLightData>();
            #endif

            if (additionalDirectVolumeLight == null)
                additionalDirectVolumeLight = Components.AdditionalDirectLight.GetComponent<EnviroVolumeLight>();

            if (additionalDirectVolumeLight == null)
                additionalDirectVolumeLight = Components.AdditionalDirectLight.gameObject.AddComponent<EnviroVolumeLight>();

            if (EnviroSkyMgr.instance.dontDestroy && Application.isPlaying)
                DontDestroyOnLoad(Components.AdditionalDirectLight);
        }


    } 

    #endregion
    ////////////////////////////////
    #region Runtime Update
    private void RenderWeatherMap()
    {
        if (cloudsSettings.customWeatherMap == null)
        {
            weatherMapMat.SetVector("_WindDir", cloudAnimNonScaled);
            weatherMapMat.SetFloat("_AnimSpeedScale", cloudsSettings.weatherAnimSpeedScale);
            weatherMapMat.SetInt("_Tiling", cloudsSettings.weatherMapTiling);
            weatherMapMat.SetVector("_Location", cloudsSettings.locationOffset);

            double cov = cloudsConfig.coverage * cloudsSettings.globalCloudCoverage;
            weatherMapMat.SetFloat("_Coverage", (float)System.Math.Round(cov, 4));
            weatherMapMat.SetFloat("_CloudsType", cloudsConfig.cloudType);
            weatherMapMat.SetFloat("_CoverageType", cloudsConfig.coverageType);

            weatherMapMat.SetVector("_LightingVariance", new Vector4(1f - cloudsConfig.lightVariance, cloudsSettings.lightingVarianceTiling, 0f, 0f));

            Graphics.Blit(null, weatherMap, weatherMapMat);
        }
    }

    public void RenderCloudMaps()
    {
        if (Application.isPlaying)
        {
            if (useVolumeClouds)
                RenderWeatherMap();
        }
        else
        {
            if (useVolumeClouds && showVolumeCloudsInEditor)
                RenderWeatherMap();
        }
    }

    void Update()
    {

#if ENVIRO_HDRP
        frames++;
#endif

        if (profile == null)
        {
            Debug.Log("No profile applied! Please create and assign a profile.");
            return;
        }

        if (!Application.isPlaying && startMode != EnviroStartMode.Started)
        {
            if (startMode == EnviroStartMode.Paused)
                Stop(true, true);
            else
            {
                GameTime.ProgressTime = EnviroTime.TimeProgressMode.Simulated;
                Stop(true, false);
            }
        }
        else if (!Application.isPlaying && startMode == EnviroStartMode.Started && started == false)
            Play(GameTime.ProgressTime);

        if (!started && !serverMode)
        {
            UpdateTime(GameTime.DaysInYear);
#if ENVIRO_HDRP
            if (frames == 2)
            {
                UpdateSunAndMoonPosition();
                UpdateSceneView();
                CalculateDirectLight();
                frames = 0;
            }
#else
            UpdateSunAndMoonPosition();
            UpdateSceneView();
            CalculateDirectLight();
#endif
            UpdateReflections(false);

            if (AssignInRuntime && PlayerTag != "" && CameraTag != "" && Application.isPlaying)
            {

                // Search for Player by tag
                GameObject plr = GameObject.FindGameObjectWithTag(PlayerTag);
                if (plr != null)
                    Player = plr;

                // Search for camera by tag
                for (int i = 0; i < Camera.allCameras.Length; i++)
                {
                    if (Camera.allCameras[i].tag == CameraTag)
                        PlayerCamera = Camera.allCameras[i];
                }

                if (Player != null && PlayerCamera != null)
                {
                    Init();
                    started = true;
                }
                else { started = false; return; }
            }
            else { started = false; return; }
        }

        UpdateTime(GameTime.DaysInYear);
        ValidateParameters();

        if (!serverMode)
        {
#if ENVIRO_HDRP
            CreateSkyAndFogVolume();
#else
            //Check if cloudmode changed
            if (useFlatClouds != flatCloudsSkybox)
                SetupSkybox();

            //Check if skyboxmode changed
            if (currentSkyboxMode != skySettings.skyboxMode)
                SetupSkybox();

            //Check if skybox material is wrong after scene load
            if (RenderSettings.skybox != skyMat)
                SetupSkybox();  
#endif

            UpdateSceneView();

            if (!Application.isPlaying && Weather.startWeatherPreset != null && startMode == EnviroStartMode.Started)
            {
                UpdateClouds(Weather.startWeatherPreset, false);
                UpdateFog(Weather.startWeatherPreset, false);
                UpdatePostProcessing(Weather.startWeatherPreset, false);
                UpdateWeatherVariables(Weather.startWeatherPreset);
#if AURA_IN_PROJECT
            if(EnviroSkyMgr.instance.aura2Support)
               UpdateAura2(Weather.startWeatherPreset, true);
#endif
            }

            UpdateAmbientLight();
            UpdateReflections(false);
            UpdateWeather();
            if (Weather.currentActiveWeatherPreset != null && Weather.currentActiveWeatherPreset.cloudsConfig.particleCloudsOverwrite)
                UpdateParticleClouds(true);
            else
                UpdateParticleClouds(useParticleClouds);

#if !ENVIRO_HDRP
            UpdateCloudShadows();
#endif

#if ENVIRO_HDRP

            UpdateSunAndMoonPosition();
            UpdateHDRPPostProcessing();
            
            if(Application.isPlaying)
            {
                if (frames >= lightingUpdateEachFrames)
                {
                    CalculateDirectLight();
            
                    frames = 0;
                }
            }
            else
            {
                if (frames >= 1)
                {
                    CalculateDirectLight();
           
                    frames = 0;
                }

            }

#else
            UpdateSunAndMoonPosition();
            CalculateDirectLight();
#endif
#if ENVIRO_HD
            UpdateSatelliteRender();
#endif
            SetMaterialsVariables();

#if !ENVIRO_LWRP && !ENVIRO_HDRP
            if (directVolumeLight != null && !directVolumeLight.isActiveAndEnabled && volumeLightSettings.dirVolumeLighting)
                directVolumeLight.enabled = true;
#endif

            if (!isNight && GameTime.solarTime < GameTime.dayNightSwitch)
            {
                isNight = true;
                if (Audio.AudioSourceAmbient != null)
                    TryPlayAmbientSFX();
                EnviroSkyMgr.instance.NotifyIsNight();
            }
            else if (isNight && GameTime.solarTime >= GameTime.dayNightSwitch)
            {
                isNight = false;
                if (Audio.AudioSourceAmbient != null)
                    TryPlayAmbientSFX();
                EnviroSkyMgr.instance.NotifyIsDay();
            }
        }
        else
        {
            UpdateWeather();

            if (!isNight && GameTime.solarTime < GameTime.dayNightSwitch)
            {
                isNight = true;
                EnviroSkyMgr.instance.NotifyIsNight();
            }
            else if (isNight && GameTime.solarTime >= GameTime.dayNightSwitch)
            {
                isNight = false;
                EnviroSkyMgr.instance.NotifyIsDay();
            }
        }
    }

    void LateUpdate()
    {
        if (!serverMode && PlayerCamera != null && Player != null)
        {
            transform.position = Player.transform.position;
            transform.localScale = new Vector3(PlayerCamera.farClipPlane, PlayerCamera.farClipPlane, PlayerCamera.farClipPlane);

            if (EffectsHolder != null)
            {
#if ENVIRO_HD
                if (cloudsSettings.cloudsQualitySettings != null && Player.transform.position.y > (cloudsSettings.cloudsQualitySettings.bottomCloudHeight + cloudsSettings.cloudsHeightMod))
                    EffectsHolder.transform.position = new Vector3(Player.transform.position.x, cloudsSettings.cloudsQualitySettings.bottomCloudHeight + cloudsSettings.cloudsHeightMod, Player.transform.position.z);
                else
#endif
                    EffectsHolder.transform.position = Player.transform.position;
            }
        }
    }

    private void UpdateCloudShadows()
    {
#if ENVIRO_HDRP
        if(MainLightHDRP == null)
            MainLightHDRP = Components.DirectLight.GetComponent<UnityEngine.Rendering.HighDefinition.HDAdditionalLightData>();
#elif ENVIRO_LWRP

        if(MainLightURP == null)
           MainLightURP = Components.DirectLight.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalLightData>();
#endif

        if (cloudsSettings.shadowIntensity == 0 || !useVolumeClouds)
        {
            if (MainLight.cookie != null)
                MainLight.cookie = null;
        }
        else if (cloudsSettings.shadowIntensity > 0)
        {
            //cloudShadowMap.DiscardContents(true, true);
            cloudShadowMat.SetFloat("_shadowIntensity", cloudsSettings.shadowIntensity);

            if (useVolumeClouds)
            {
                cloudShadowMat.SetTexture("_MainTex", weatherMap);
             
                Graphics.Blit(weatherMap, cloudShadowMap, cloudShadowMat);
            }
 
#if ENVIRO_HDRP
            if(MainLightHDRP != null)
               MainLightHDRP.SetCookie(cloudShadowMap,new Vector2(cloudsSettings.shadowCookieSize,cloudsSettings.shadowCookieSize));

#else
            if (Application.isPlaying)
                MainLight.cookie = cloudShadowMap;
            else
               MainLight.cookie = null;

            MainLight.cookieSize = cloudsSettings.shadowCookieSize;
#endif
 
#if ENVIRO_LWRP && UNITY_2021_2_OR_NEWER
            if(MainLightURP != null)
               MainLightURP.lightCookieSize = new Vector2(cloudsSettings.shadowCookieSize,cloudsSettings.shadowCookieSize);
#endif
        }
    }

    public void UpdateSkyShaderVariables(Material skyMat)
    {
        if (skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.Simple)
        {
            //Simple
            skyMat.SetColor("_SkyColor", skySettings.simpleSkyColor.Evaluate(GameTime.solarTime));
            skyMat.SetColor("_HorizonColor", skySettings.simpleHorizonColor.Evaluate(GameTime.solarTime));
            skyMat.SetColor("_HorizonBackColor", skySettings.simpleHorizonBackColor.Evaluate(GameTime.solarTime));
            skyMat.SetColor("_SunColor", skySettings.simpleSunColor.Evaluate(GameTime.solarTime));
            skyMat.SetColor("_GroundColor", skySettings.simpleGroundColor);
            skyMat.SetFloat("_SunDiskSizeSimple", skySettings.simpleSunDiskSize.Evaluate(GameTime.solarTime));
            skyMat.SetVector("_MoonDir", Components.Moon.transform.forward);
            skyMat.SetVector("_SunDir", -Components.Sun.transform.forward);
            skyMat.SetColor("_MoonColor", skySettings.moonColor);
            skyMat.SetColor("_weatherSkyMod", Color.Lerp(currentWeatherSkyMod, interiorZoneSettings.currentInteriorSkyboxMod, interiorZoneSettings.currentInteriorSkyboxMod.a));
            skyMat.SetFloat("_StarsIntensity", skySettings.starsIntensity.Evaluate(GameTime.solarTime));
            skyMat.SetFloat("_GalaxyIntensity", skySettings.galaxyIntensity.Evaluate(GameTime.solarTime));

            if (skySettings.moonPhaseMode == EnviroSkySettings.MoonPhases.Realistic)
            {
                float angle = Vector3.SignedAngle(Components.Moon.transform.forward, Components.Sun.transform.forward, -transform.forward);

              
                if (GameTime.Latitude >= 0)
                {
                    if (angle < 0)
                    {
                        customMoonPhase = Remap(angle, 0f, -180f, -2f, 0f);
                    }
                    else
                    {
                        customMoonPhase = Remap(angle, 0f, 180f, 2f, 0f);
                    }
                }
                else
                {
                    if (angle < 0)
                    {
                        customMoonPhase = Remap(angle, 0f, -180f, 2f, 0f);
                    }
                    else
                    {
                        customMoonPhase = Remap(angle, 0f, 180f, -2f, 0f);
                    }

                }
            }

            skyMat.SetColor("_moonGlowColor", skySettings.moonGlowColor);
            skyMat.SetVector("_moonParams", new Vector4(skySettings.moonSize, skySettings.glowSize, skySettings.moonGlow.Evaluate(GameTime.solarTime), customMoonPhase));

            //Moon
            if (skySettings.renderMoon)
            {
                skyMat.SetTexture("_MoonTex", skySettings.moonTexture);
                skyMat.SetTexture("_GlowTex", skySettings.glowTexture);
            }
            else
            {
                skyMat.SetTexture("_MoonTex", null);
                skyMat.SetTexture("_GlowTex", null);
            }

            skyMat.SetFloat("_StarsTwinkling", skySettings.starsTwinklingRate);

            if (skySettings.starsTwinklingRate > 0.0f)
            {
                starsTwinklingRot += skySettings.starsTwinklingRate * Time.deltaTime;
                Quaternion rot = Quaternion.Euler(starsTwinklingRot, starsTwinklingRot, starsTwinklingRot);
                Matrix4x4 NoiseRot = Matrix4x4.TRS(Vector3.zero, rot, new Vector3(1, 1, 1));
                skyMat.SetMatrix("_StarsTwinklingMatrix", NoiseRot);
            }

            if (useAurora)
            {
                skyMat.EnableKeyword("ENVIRO_AURORA");
                //Aurora
#if ENVIRO_HD
                skyMat.SetFloat("_AuroraIntensity", Mathf.Clamp01(auroraIntensity * auroraSettings.auroraIntensity.Evaluate(GameTime.solarTime)));
                skyMat.SetFloat("_AuroraBrightness", auroraSettings.auroraBrightness);
                skyMat.SetFloat("_AuroraContrast", auroraSettings.auroraContrast);
                skyMat.SetColor("_AuroraColor", auroraSettings.auroraColor);
                skyMat.SetFloat("_AuroraHeight", auroraSettings.auroraHeight);
                skyMat.SetFloat("_AuroraScale", auroraSettings.auroraScale);
                skyMat.SetFloat("_AuroraSpeed", auroraSettings.auroraSpeed);
                skyMat.SetFloat("_AuroraSteps", auroraSettings.auroraSteps);
                skyMat.SetFloat("_AuroraSteps", auroraSettings.auroraSteps);

                skyMat.SetVector("_Aurora_Tiling_Layer1", auroraSettings.auroraLayer1Settings);
                skyMat.SetVector("_Aurora_Tiling_Layer2", auroraSettings.auroraLayer2Settings);
                skyMat.SetVector("_Aurora_Tiling_ColorShift", auroraSettings.auroraColorshiftSettings);
#endif
            }
            else
            {
                skyMat.DisableKeyword("ENVIRO_AURORA");
            }
        }
        else
        {
            skyMat.SetVector("_SunDir", -Components.Sun.transform.forward);
            skyMat.SetVector("_MoonDir", Components.Moon.transform.forward);
            skyMat.SetColor("_MoonColor", skySettings.moonColor);
            skyMat.SetColor("_scatteringColor", skySettings.scatteringColor.Evaluate(GameTime.solarTime));
            skyMat.SetColor("_sunDiskColor", skySettings.sunDiskColor.Evaluate(GameTime.solarTime));
            skyMat.SetColor("_weatherSkyMod", Color.Lerp(currentWeatherSkyMod, interiorZoneSettings.currentInteriorSkyboxMod, interiorZoneSettings.currentInteriorSkyboxMod.a));
            skyMat.SetColor("_weatherFogMod", Color.Lerp(currentWeatherFogMod, interiorZoneSettings.currentInteriorFogColorMod, interiorZoneSettings.currentInteriorFogColorMod.a));
            skyMat.SetVector("_Bm", BetaMie(skySettings.turbidity, skySettings.waveLength) * (skySettings.mie * Fog.scatteringStrenght));
            skyMat.SetVector("_Br", BetaRay(skySettings.waveLength) * skySettings.rayleigh);
            skyMat.SetVector("_mieG", GetMieG(skySettings.g));
            skyMat.SetFloat("_SunIntensity", skySettings.sunIntensity);
            skyMat.SetFloat("_SunDiskSize", skySettings.sunDiskScale);
            skyMat.SetFloat("_SunDiskIntensity", skySettings.sunDiskIntensity);
            skyMat.SetFloat("_SunDiskSize", skySettings.sunDiskScale);
            skyMat.SetFloat("_Tonemapping", tonemapping ? 1f : 0f);
            skyMat.SetFloat("_SkyExposure", skySettings.skyExposure);
            skyMat.SetFloat("_SkyLuminance", skySettings.skyLuminence.Evaluate(GameTime.solarTime));
            skyMat.SetFloat("_scatteringPower", skySettings.scatteringCurve.Evaluate(GameTime.solarTime));
            skyMat.SetFloat("_SkyColorPower", skySettings.skyColorPower.Evaluate(GameTime.solarTime));
            skyMat.SetFloat("_StarsIntensity", skySettings.starsIntensity.Evaluate(GameTime.solarTime));
            skyMat.SetFloat("_GalaxyIntensity", skySettings.galaxyIntensity.Evaluate(GameTime.solarTime));
            skyMat.SetFloat("_DitheringIntensity", skySettings.dithering);
#if ENVIRO_HDRP
            skyMat.SetColor("_ambientColorMod", lightSettings.ambientColorMod.Evaluate(GameTime.solarTime));
#endif
            if (skySettings.moonPhaseMode == EnviroSkySettings.MoonPhases.Realistic)
            {
                float angle = Vector3.SignedAngle(Components.Moon.transform.forward, Components.Sun.transform.forward, transform.forward);
                
                if (GameTime.Latitude >= 0)
                {
                    if (angle < 0)
                    {
                        customMoonPhase = Remap(angle, 0f, -180f, -2f, 0f);
                    }
                    else
                    {
                        customMoonPhase = Remap(angle, 0f, 180f, 2f, 0f);
                    }
                }
                else
                {
                    if (angle < 0)
                    {
                        customMoonPhase = Remap(angle, 0f, -180f, 2f, 0f);
                    }
                    else
                    {
                        customMoonPhase = Remap(angle, 0f, 180f, -2f, 0f);
                    }

                }
            }


            skyMat.SetColor("_moonGlowColor", skySettings.moonGlowColor);
            skyMat.SetVector("_moonParams", new Vector4(skySettings.moonSize, skySettings.glowSize, skySettings.moonGlow.Evaluate(GameTime.solarTime), customMoonPhase));

            //Moon
            if (skySettings.renderMoon)
            {
                skyMat.SetTexture("_MoonTex", skySettings.moonTexture);
                skyMat.SetTexture("_GlowTex", skySettings.glowTexture);
            }
            else
            {
                skyMat.SetTexture("_MoonTex", null);
                skyMat.SetTexture("_GlowTex", null);
            }

            if (skySettings.blackGroundMode)
                skyMat.SetInt("_blackGround", 1);
            else
                skyMat.SetInt("_blackGround", 0);

            skyMat.SetFloat("_StarsTwinkling", skySettings.starsTwinklingRate);

            if (skySettings.starsTwinklingRate > 0.0f)
            {
                starsTwinklingRot += skySettings.starsTwinklingRate * Time.deltaTime;
                Quaternion rot = Quaternion.Euler(starsTwinklingRot, starsTwinklingRot, starsTwinklingRot);
                Matrix4x4 NoiseRot = Matrix4x4.TRS(Vector3.zero, rot, new Vector3(1, 1, 1));
                skyMat.SetMatrix("_StarsTwinklingMatrix", NoiseRot);
            }


            if (useAurora)
            {
                skyMat.EnableKeyword("ENVIRO_AURORA");
                //Aurora
#if ENVIRO_HD
                skyMat.SetFloat("_AuroraIntensity", Mathf.Clamp01(auroraIntensity * auroraSettings.auroraIntensity.Evaluate(GameTime.solarTime)));
                skyMat.SetFloat("_AuroraBrightness", auroraSettings.auroraBrightness);
                skyMat.SetFloat("_AuroraContrast", auroraSettings.auroraContrast);
                skyMat.SetColor("_AuroraColor", auroraSettings.auroraColor);
                skyMat.SetFloat("_AuroraHeight", auroraSettings.auroraHeight);
                skyMat.SetFloat("_AuroraScale", auroraSettings.auroraScale);
                skyMat.SetFloat("_AuroraSpeed", auroraSettings.auroraSpeed);
                skyMat.SetFloat("_AuroraSteps", auroraSettings.auroraSteps);
                skyMat.SetFloat("_AuroraSteps", auroraSettings.auroraSteps);

                skyMat.SetVector("_Aurora_Tiling_Layer1", auroraSettings.auroraLayer1Settings);
                skyMat.SetVector("_Aurora_Tiling_Layer2", auroraSettings.auroraLayer2Settings);
                skyMat.SetVector("_Aurora_Tiling_ColorShift", auroraSettings.auroraColorshiftSettings);
#endif
            }
            else
            {
                skyMat.DisableKeyword("ENVIRO_AURORA");
            }
        }

        //Clouds
        skyMat.SetVector("_CloudCirrusAnimation", cirrusAnim);

        if(useFlatClouds)
        {
            if (cloudsSettings.flatCloudsBaseTexture != null)
                skyMat.SetTexture("_FlatCloudsBaseTexture", cloudsSettings.flatCloudsBaseTexture);

            if (cloudsSettings.flatCloudsDetailTexture != null)
                skyMat.SetTexture("_FlatCloudsDetailTexture", cloudsSettings.flatCloudsDetailTexture);

            skyMat.SetVector("_FlatCloudsLightDirection", Components.DirectLight.transform.forward);

            skyMat.SetColor("_FlatCloudsLightColor", cloudsSettings.flatCloudsDirectLightColor.Evaluate(GameTime.solarTime));
            skyMat.SetColor("_FlatCloudsAmbientColor", cloudsSettings.flatCloudsAmbientLightColor.Evaluate(GameTime.solarTime));
            skyMat.SetVector("_FlatCloudsParams", new Vector4(cloudsConfig.flatCoverage, cloudsConfig.flatCloudsDensity, cloudsSettings.flatCloudsAltitude, tonemapping ? 1f : 0f));
            skyMat.SetVector("_FlatCloudsTiling", new Vector4(cloudsSettings.flatCloudsBaseTextureTiling, cloudsSettings.flatCloudsDetailTextureTiling, 0f, 0f));
            skyMat.SetVector("_FlatCloudsLightingParams", new Vector4(cloudsConfig.flatCloudsDirectLightIntensity, cloudsConfig.flatCloudsAmbientLightIntensity, cloudsConfig.flatCloudsAbsorbtion, cloudsConfig.flatCloudsHGPhase));
            skyMat.SetVector("_FlatCloudsAnimation", new Vector4(cloudFlatBaseAnim.x, cloudFlatBaseAnim.y, cloudFlatDetailAnim.x, cloudFlatDetailAnim.y));
            skyMat.SetFloat("_CloudsExposure", cloudsSettings.cloudsExposure);
        }

        //cirrus
        if (cloudsSettings.cirrusCloudsTexture != null)
            skyMat.SetTexture("_CloudMap", cloudsSettings.cirrusCloudsTexture);

        skyMat.SetColor("_CloudColor", cloudsSettings.cirrusCloudsColor.Evaluate(GameTime.solarTime));
        skyMat.SetFloat("_CloudAltitude", cloudsSettings.cirrusCloudsAltitude);
        skyMat.SetFloat("_CloudAlpha", cloudsConfig.cirrusAlpha);
        skyMat.SetFloat("_CloudCoverage", cloudsConfig.cirrusCoverage);
        skyMat.SetFloat("_CloudColorPower", cloudsConfig.cirrusColorPow);
    }

    private void SetMaterialsVariables()
    {
#if !ENVIRO_HDRP
        if (skyMat != null)
        {
            UpdateSkyShaderVariables(skyMat);
        }
#endif
        Shader.SetGlobalColor("_EnviroLighting", lightSettings.LightColor.Evaluate(GameTime.solarTime));
        Shader.SetGlobalVector("_SunDirection", -Components.Sun.transform.forward);

        Shader.SetGlobalVector("_SunPosition", Components.Sun.transform.localPosition + (-Components.Sun.transform.forward * 10000f));
        Shader.SetGlobalVector("_MoonPosition", Components.Moon.transform.localPosition);
        Shader.SetGlobalVector("_SunDir", -Components.Sun.transform.forward);
        Shader.SetGlobalVector("_MoonDir", -Components.Moon.transform.forward);
        Shader.SetGlobalColor("_scatteringColor", skySettings.scatteringColor.Evaluate(GameTime.solarTime));
        Shader.SetGlobalColor("_sunDiskColor", skySettings.sunDiskColor.Evaluate(GameTime.solarTime));
        Shader.SetGlobalColor("_weatherSkyMod", Color.Lerp(currentWeatherSkyMod, interiorZoneSettings.currentInteriorSkyboxMod, interiorZoneSettings.currentInteriorSkyboxMod.a));
        Shader.SetGlobalColor("_weatherFogMod", Color.Lerp(currentWeatherFogMod, interiorZoneSettings.currentInteriorFogColorMod, interiorZoneSettings.currentInteriorFogColorMod.a));
        Shader.SetGlobalFloat("_gameTime", Mathf.Clamp(1f - GameTime.solarTime, 0.5f, 1f));
        Shader.SetGlobalVector("_EnviroSkyFog", new Vector4(Fog.skyFogHeight, Fog.skyFogIntensity, Fog.skyFogStart, fogSettings.heightFogIntensity));
        Shader.SetGlobalFloat("_scatteringStrenght", Fog.scatteringStrenght);
        Shader.SetGlobalFloat("_SunBlocking", Fog.sunBlocking);
        Shader.SetGlobalVector("_EnviroParams", new Vector4(Mathf.Clamp(1f - GameTime.solarTime, 0.5f, 1f), fogSettings.distanceFog ? 1f : 0f, fogSettings.heightFog ? 1f : 0f, tonemapping ? 1f : 0f));
        Shader.SetGlobalVector("_Bm", BetaMie(skySettings.turbidity, skySettings.waveLength) * (skySettings.mie * (Fog.scatteringStrenght * GameTime.solarTime)));
        Shader.SetGlobalVector("_BmScene", BetaMie(skySettings.turbidity, skySettings.waveLength) * (fogSettings.mie * (Fog.scatteringStrenght * GameTime.solarTime)));
        Shader.SetGlobalVector("_Br", BetaRay(skySettings.waveLength) * skySettings.rayleigh);
        Shader.SetGlobalVector("_mieG", GetMieG(skySettings.g));
        Shader.SetGlobalVector("_mieGScene", GetMieGScene(skySettings.g));
        Shader.SetGlobalVector("_SunParameters", new Vector4(skySettings.sunIntensity, skySettings.sunDiskScale, skySettings.sunDiskIntensity, cloudsSettings.cloudsSkyFogHeightBlending));
        Shader.SetGlobalFloat("_FogExposure", fogSettings.fogExposure);
        Shader.SetGlobalFloat("_SkyLuminance", skySettings.skyLuminence.Evaluate(GameTime.solarTime));
        Shader.SetGlobalFloat("_scatteringPower", skySettings.scatteringCurve.Evaluate(GameTime.solarTime));
        Shader.SetGlobalFloat("_SkyColorPower", skySettings.skyColorPower.Evaluate(GameTime.solarTime));
        Shader.SetGlobalFloat("_distanceFogIntensity", fogSettings.distanceFogIntensity);

        if (cloudsSettings.depthBlending)
            Shader.SetGlobalTexture("_EnviroCloudsTex", cloudsRenderTarget);


#if ENVIRO_HDRP
#else
        if (Application.isPlaying || showFogInEditor)
        {
            Shader.SetGlobalFloat("_maximumFogDensity", 1 - fogSettings.maximumFogDensity);
        }
        else if (!showFogInEditor)
        {
            Shader.SetGlobalFloat("_maximumFogDensity", 1f);
        }
#endif


        Shader.SetGlobalFloat("_lightning", thunder);

        if (fogSettings.useSimpleFog)
            Shader.EnableKeyword("ENVIRO_SIMPLE_FOG");
        else
            Shader.DisableKeyword("ENVIRO_SIMPLE_FOG");

    }

    // Make the parameters stay in reasonable range
    private void ValidateParameters()
    {
        // Keep GameTime Parameters right!
        internalHour = Mathf.Repeat(internalHour, 24f);
        GameTime.Longitude = Mathf.Clamp(GameTime.Longitude, -180, 180);
        GameTime.Latitude = Mathf.Clamp(GameTime.Latitude, -90, 90);
#if UNITY_EDITOR
        if (GameTime.cycleLengthInMinutes <= 0f)
        {
            if (GameTime.cycleLengthInMinutes < 0f)
                GameTime.cycleLengthInMinutes = 0f;
            internalHour = 12f;
            customMoonPhase = 0f;
        }

        if (GameTime.Days < 0)
            GameTime.Days = 0;

        if (GameTime.Years < 0)
            GameTime.Years = 0;
#endif
    }

    private void UpdateClouds(EnviroWeatherPreset i, bool withTransition)
    {
        if (i == null)
            return;

        float speed = 500f * Time.deltaTime;

        if (withTransition)
            speed = weatherSettings.cloudTransitionSpeed * Time.deltaTime;

        cloudsConfig.cirrusAlpha = Mathf.Lerp(cloudsConfig.cirrusAlpha, i.cloudsConfig.cirrusAlpha, speed);
        cloudsConfig.cirrusCoverage = Mathf.Lerp(cloudsConfig.cirrusCoverage, i.cloudsConfig.cirrusCoverage, speed);
        cloudsConfig.cirrusColorPow = Mathf.Lerp(cloudsConfig.cirrusColorPow, i.cloudsConfig.cirrusColorPow, speed);

        //Needed for FV 3 Integration
        cloudsConfig.coverage = Mathf.Lerp(cloudsConfig.coverage, i.cloudsConfig.coverage, speed);
        cloudsConfig.ambientSkyColorIntensity = Mathf.Lerp(cloudsConfig.ambientSkyColorIntensity, i.cloudsConfig.ambientSkyColorIntensity, speed);

        if (useVolumeClouds)
        {
            cloudsConfig.raymarchingScale = Mathf.Lerp(cloudsConfig.raymarchingScale, i.cloudsConfig.raymarchingScale, speed);
            cloudsConfig.ambientSkyColorIntensity = Mathf.Lerp(cloudsConfig.ambientSkyColorIntensity, i.cloudsConfig.ambientSkyColorIntensity, speed);
            cloudsConfig.density = Mathf.Lerp(cloudsConfig.density, i.cloudsConfig.density, speed);
            cloudsConfig.lightStepModifier = Mathf.Lerp(cloudsConfig.lightStepModifier, i.cloudsConfig.lightStepModifier, speed);
            cloudsConfig.lightAbsorbtion = Mathf.Lerp(cloudsConfig.lightAbsorbtion, i.cloudsConfig.lightAbsorbtion, speed);
            cloudsConfig.lightVariance = Mathf.Lerp(cloudsConfig.lightVariance, i.cloudsConfig.lightVariance, speed);
            cloudsConfig.scatteringCoef = Mathf.Lerp(cloudsConfig.scatteringCoef, i.cloudsConfig.scatteringCoef, speed);
            cloudsConfig.cloudType = Mathf.Lerp(cloudsConfig.cloudType, i.cloudsConfig.cloudType, speed);
            cloudsConfig.coverageType = Mathf.Lerp(cloudsConfig.coverageType, i.cloudsConfig.coverageType, speed);
            cloudsConfig.edgeDarkness = Mathf.Lerp(cloudsConfig.edgeDarkness, i.cloudsConfig.edgeDarkness, speed);
            cloudsConfig.baseErosionIntensity = Mathf.Lerp(cloudsConfig.baseErosionIntensity, i.cloudsConfig.baseErosionIntensity, speed);
            cloudsConfig.detailErosionIntensity = Mathf.Lerp(cloudsConfig.detailErosionIntensity, i.cloudsConfig.detailErosionIntensity, speed);
        }

        if (useFlatClouds)
        {
            cloudsConfig.flatCloudsAbsorbtion = Mathf.Lerp(cloudsConfig.flatCloudsAbsorbtion, i.cloudsConfig.flatCloudsAbsorbtion, speed);
            cloudsConfig.flatCoverage = Mathf.Lerp(cloudsConfig.flatCoverage, i.cloudsConfig.flatCoverage, speed);
            cloudsConfig.flatCloudsAmbientLightIntensity = Mathf.Lerp(cloudsConfig.flatCloudsAmbientLightIntensity, i.cloudsConfig.flatCloudsAmbientLightIntensity, speed);
            cloudsConfig.flatCloudsDirectLightIntensity = Mathf.Lerp(cloudsConfig.flatCloudsDirectLightIntensity, i.cloudsConfig.flatCloudsDirectLightIntensity, speed);
            cloudsConfig.flatCloudsDensity = Mathf.Lerp(cloudsConfig.flatCloudsDensity, i.cloudsConfig.flatCloudsDensity, speed);
            cloudsConfig.flatCloudsHGPhase = Mathf.Lerp(cloudsConfig.flatCloudsHGPhase, i.cloudsConfig.flatCloudsHGPhase, speed);
        }

        cloudsConfig.particleLayer1Alpha = Mathf.Lerp(cloudsConfig.particleLayer1Alpha, i.cloudsConfig.particleLayer1Alpha, speed * 0.25f);
        cloudsConfig.particleLayer1Brightness = Mathf.Lerp(cloudsConfig.particleLayer1Brightness, i.cloudsConfig.particleLayer1Brightness, speed * 0.25f);
        cloudsConfig.particleLayer2Alpha = Mathf.Lerp(cloudsConfig.particleLayer2Alpha, i.cloudsConfig.particleLayer2Alpha, speed * 0.25f);
        cloudsConfig.particleLayer2Brightness = Mathf.Lerp(cloudsConfig.particleLayer2Brightness, i.cloudsConfig.particleLayer2Brightness, speed * 0.25f);

        globalVolumeLightIntensity = Mathf.Lerp(globalVolumeLightIntensity, i.volumeLightIntensity, speed);
        shadowIntensityMod = Mathf.Lerp(shadowIntensityMod, i.shadowIntensityMod, speed);
        currentWeatherSkyMod = Color.Lerp(currentWeatherSkyMod, i.weatherSkyMod.Evaluate(GameTime.solarTime), speed);
        currentWeatherFogMod = Color.Lerp(currentWeatherFogMod, i.weatherFogMod.Evaluate(GameTime.solarTime), speed * 10);
        currentWeatherLightMod = Color.Lerp(currentWeatherLightMod, i.weatherLightMod.Evaluate(GameTime.solarTime), speed);

#if ENVIRO_HD
        auroraIntensity = Mathf.Lerp(auroraIntensity, i.auroraIntensity, speed);
#endif

#if ENVIRO_HDRP
    if(lightSettings.usePhysicalBasedLighting)
    {
        currentSceneExposureMod = Mathf.Lerp(currentSceneExposureMod, i.sceneExposureMod, speed);
        currentSkyExposureMod = Mathf.Lerp(currentSkyExposureMod, i.skyExposureMod, speed);
        currentLightIntensityMod = Mathf.Lerp(currentLightIntensityMod, i.lightIntensityMod, speed);
    }
#endif
    }

    private void UpdateFog(EnviroWeatherPreset i, bool withTransition)
    {
        Color fogClr = Color.Lerp(fogSettings.simpleFogColor.Evaluate(GameTime.solarTime), customFogColor, customFogIntensity);
        RenderSettings.fogColor = Color.Lerp(fogClr, currentWeatherFogMod, currentWeatherFogMod.a);

        if (i != null)
        {

            float speed = 500f * Time.deltaTime;

            if (withTransition)
                speed = weatherSettings.fogTransitionSpeed * Time.deltaTime;

            if (fogSettings.Fogmode == FogMode.Linear)
            {
                RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, i.fogDistance, speed);
                RenderSettings.fogStartDistance = Mathf.Lerp(RenderSettings.fogStartDistance, i.fogStartDistance, speed);
            }
            else
            {
                if (updateFogDensity)
                    RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, i.fogDensity, speed) * interiorZoneSettings.currentInteriorFogMod;
            }

            fogSettings.heightDensity = Mathf.Lerp(fogSettings.heightDensity, i.heightFogDensity, speed);
            Fog.skyFogStart = Mathf.Lerp(Fog.skyFogStart, i.skyFogStart, speed);
            Fog.skyFogHeight = Mathf.Lerp(Fog.skyFogHeight, i.SkyFogHeight, speed);
            Fog.skyFogIntensity = Mathf.Lerp(Fog.skyFogIntensity, i.SkyFogIntensity, speed);
            fogSettings.skyFogIntensity = Mathf.Lerp(fogSettings.skyFogIntensity, i.SkyFogIntensity, speed);
            Fog.scatteringStrenght = Mathf.Lerp(Fog.scatteringStrenght, i.FogScatteringIntensity, speed);
            Fog.sunBlocking = Mathf.Lerp(Fog.sunBlocking, i.fogSunBlocking, speed);


#if ENVIRO_HDRP
            if(hdrpFog != null)
            {
                hdrpFog.meanFreePath.value = Mathf.Lerp(hdrpFog.meanFreePath.value, i.fogAttenuationDistance, speed);
                Fog.hdrpRelativeFogHeight = Mathf.Lerp(Fog.hdrpRelativeFogHeight, i.fogRelativeFogHeight, speed);
 
                if ((Fog.hdrpRelativeFogHeight + transform.position.y) >= (cloudsSettings.cloudsHeightMod + cloudsSettings.cloudsQualitySettings.bottomCloudHeight))
                    hdrpFog.maximumHeight.value = (cloudsSettings.cloudsHeightMod + cloudsSettings.cloudsQualitySettings.bottomCloudHeight);
                else
                    hdrpFog.maximumHeight.value = Fog.hdrpRelativeFogHeight + transform.position.y;

                    hdrpFog.tint.overrideState = true;
                    hdrpFog.tint.value = Color.Lerp(fogSettings.fogColorTint.Evaluate(GameTime.solarTime), currentWeatherFogMod, currentWeatherFogMod.a) * (thunder + 1);        
            }
#endif
        }
    }

    private void UpdatePostProcessing(EnviroWeatherPreset i, bool withTransition)
    {

        if (i != null)
        {

            float speed = 500f * Time.deltaTime;

            if (withTransition)
                speed = 10 * Time.deltaTime;

            blurDistance = Mathf.Lerp(blurDistance, i.blurDistance, speed);
            blurIntensity = Mathf.Lerp(blurIntensity, i.blurIntensity, speed);
            blurSkyIntensity = Mathf.Lerp(blurSkyIntensity, i.blurSkyIntensity, speed);
        }
    }

    private void UpdateEffectSystems(EnviroWeatherPrefab id, bool withTransition)
    {
        if (id != null)
        {
 
            float speed = 500f * Time.deltaTime;

            if (withTransition)
                speed = weatherSettings.effectTransitionSpeed * Time.deltaTime;

            for (int i = 0; i < id.effectSystems.Count; i++)
            {

                if (id.effectSystems[i].isStopped)
                    id.effectSystems[i].Play();

                // Set EmissionRate
                float val = Mathf.Lerp(EnviroSkyMgr.instance.GetEmissionRate(id.effectSystems[i]), id.effectEmmisionRates[i] * qualitySettings.GlobalParticleEmissionRates, speed) * interiorZoneSettings.currentInteriorWeatherEffectMod;
                EnviroSkyMgr.instance.SetEmissionRate(id.effectSystems[i], val);
            }

            for (int i = 0; i < Weather.WeatherPrefabs.Count; i++)
            {
                if (Weather.WeatherPrefabs[i].gameObject != id.gameObject)
                {
                    for (int i2 = 0; i2 < Weather.WeatherPrefabs[i].effectSystems.Count; i2++)
                    {
                        float val2 = Mathf.Lerp(EnviroSkyMgr.instance.GetEmissionRate(Weather.WeatherPrefabs[i].effectSystems[i2]), 0f, speed * 10);

                        if (val2 < 1f)
                            val2 = 0f;

                        EnviroSkyMgr.instance.SetEmissionRate(Weather.WeatherPrefabs[i].effectSystems[i2], val2);

                        if (val2 == 0f && !Weather.WeatherPrefabs[i].effectSystems[i2].isStopped)
                        {
                            Weather.WeatherPrefabs[i].effectSystems[i2].Stop();
                        }
                    }
                }
            }

            UpdateWeatherVariables(id.weatherPreset);
        }
    }

    private void UpdateWeather()
    {
        //Current active weather not matching current zones weather
        if (Weather.currentActiveWeatherPreset != Weather.currentActiveZone.currentActiveZoneWeatherPreset)
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
                    TryPlayAmbientSFX();
                    UpdateAudioSource(Weather.currentActiveWeatherPreset);

                    if (Weather.currentActiveWeatherPreset.isLightningStorm)
                        StartCoroutine(PlayThunderRandom());
                    else
                    {
                        StopCoroutine(PlayThunderRandom());
                        Components.LightningGenerator.StopLightning();
                    }
                }
            }
        }

        if (Weather.currentActiveWeatherPrefab != null && !serverMode)
        {
            UpdateClouds(Weather.currentActiveWeatherPreset, true);
            UpdateFog(Weather.currentActiveWeatherPreset, true);
            UpdatePostProcessing(Weather.currentActiveWeatherPreset, true);
            UpdateEffectSystems(Weather.currentActiveWeatherPrefab, true);
#if AURA_IN_PROJECT
            if(EnviroSkyMgr.instance.aura2Support)
               UpdateAura2(Weather.currentActiveWeatherPreset, true);
#endif

            if (!Weather.weatherFullyChanged)
                CalcWeatherTransitionState();
        }
        else if (Weather.currentActiveWeatherPrefab != null)
        {
            UpdateWeatherVariables(Weather.currentActiveWeatherPrefab.weatherPreset);
        }
    }

#if ENVIRO_HDRP
    private void UpdateHDRPPostProcessing()
    {
        if (enviroExposure != null)
        {
            if (lightSettings.usePhysicalBasedLighting)
                enviroExposure.fixedExposure.value = lightSettings.exposurePhysical.Evaluate(GameTime.solarTime) * currentSceneExposureMod;
            else
                enviroExposure.fixedExposure.value = lightSettings.exposure;
        }
        else
        {
            CreateSkyAndFogVolume();
        }

        if (enviroHDRPSky != null)
        {
            if (lightSettings.usePhysicalBasedLighting)
                enviroHDRPSky.exposure.value = lightSettings.skyExposurePhysical.Evaluate(GameTime.solarTime) * currentSkyExposureMod;
            else
                enviroHDRPSky.exposure.value = lightSettings.skyExposure;

            SetLightingUpdateMode();
        }
        else
        {
            CreateSkyAndFogVolume();
        }

        if (hdrpFog != null)
        {
            hdrpFog.enabled.value = fogSettings.useHDRPFog;
        }
        else
        {
            CreateSkyAndFogVolume();
        }
    }
#endif
    #endregion
    ////////////////////////////////
    #region API

    public void PopulateCloudsQualityList()
    {
#if UNITY_EDITOR
        cloudsQualityPresetsFound = UnityEditor.AssetDatabase.FindAssets("t:EnviroVolumeCloudsQuality");
        if (cloudsQualityPresetsFound.Length > 0)
        {
            cloudsQualityList = new List<EnviroVolumeCloudsQuality>();

            for (int i = 0; i < cloudsQualityPresetsFound.Length; i++)
            {
                cloudsQualityPresetsFound[i] = UnityEditor.AssetDatabase.GUIDToAssetPath(cloudsQualityPresetsFound[i]);
                EnviroVolumeCloudsQuality preset = UnityEditor.AssetDatabase.LoadAssetAtPath<EnviroVolumeCloudsQuality>(cloudsQualityPresetsFound[i]);
                cloudsQualityList.Add(preset);
            }
        }
#endif
    }


    public void ApplyVolumeCloudsQualityPreset(EnviroVolumeCloudsQuality preset)
    {
#if ENVIRO_HD
        cloudsSettings.cloudsQualitySettings = preset.qualitySettings;
        currentActiveCloudsQualityPreset = preset;
#endif
    }

    public void ApplyVolumeCloudsQualityPreset(string name)
    {
#if ENVIRO_HD
        for (int i = 0; i < cloudsQualityList.Count; i++)
        {
            if (cloudsQualityList[i].name == name)
            {
                cloudsSettings.cloudsQualitySettings = cloudsQualityList[i].qualitySettings;
                currentActiveCloudsQualityPreset = cloudsQualityList[i];
                selectedCloudsQuality = i;
            }

        }
#endif
    }

    public void ApplyVolumeCloudsQualityPreset(int id)
    {
#if ENVIRO_HD
        if (id < cloudsQualityList.Count && id >= 0)
        {
            cloudsSettings.cloudsQualitySettings = cloudsQualityList[id].qualitySettings;
            currentActiveCloudsQualityPreset = cloudsQualityList[id];
            selectedCloudsQuality = id;
        }
#endif
    }

    /// <summary>
    /// Changes clouds, fog and particle effects to current weather settings instantly.
    /// </summary>
    public void InstantWeatherChange(EnviroWeatherPreset preset, EnviroWeatherPrefab prefab)
    {
        UpdateClouds(preset, false);
        UpdateFog(preset, false);
        UpdatePostProcessing(preset, false);
        UpdateEffectSystems(prefab, false);
    }

    /// <summary>
    /// Assign your Player and Camera and Initilize.////
    /// </summary>
    public void AssignAndStart(GameObject player, Camera Camera)
    {
        this.Player = player;
        PlayerCamera = Camera;
        Init();
        started = true;

        // Call reflection probe update
        if (reflectionSettings.globalReflections)
        {
            if (Components.GlobalReflectionProbe != null)
            {
                if (reflectionSettings.globalReflectionCustomRendering)
                    Components.GlobalReflectionProbe.RefreshReflection(false);
                else
                    Components.GlobalReflectionProbe.myProbe.RenderProbe();
            }

        }
    }

    /// <summary>
    /// Assign your Player and Camera and Initilize.////
    /// </summary>
    public void StartAsServer()
    {
        Player = gameObject;
        serverMode = true;
        Init();
    }

    /// <summary>
    /// Changes focus on other Player or Camera on runtime.////
    /// </summary>
    /// <param name="Player">Player.</param>
    /// <param name="Camera">Camera.</param>
    public void ChangeFocus(GameObject player, Camera Camera)
    {
        this.Player = player;

        if (PlayerCamera != null)
            RemoveEnviroCameraComponents(PlayerCamera);

        PlayerCamera = Camera;
        InitImageEffects();
    }
    /// <summary>
    /// Destroy all enviro related camera components on this camera.
    /// </summary> 
    private void RemoveEnviroCameraComponents(Camera cam)
    {

        EnviroSkyRendering renderComponent;
        EnviroPostProcessing postProcessingComponent;

        renderComponent = cam.GetComponent<EnviroSkyRendering>();
        if (renderComponent != null)
            Destroy(renderComponent);

        postProcessingComponent = cam.GetComponent<EnviroPostProcessing>();
        if (postProcessingComponent != null)
            Destroy(postProcessingComponent);
    }

    public void Play(EnviroTime.TimeProgressMode progressMode = EnviroTime.TimeProgressMode.Simulated)
    {
        //SetupSkybox();
        StartCoroutine(SetSceneSettingsLate());

        if (!Components.DirectLight.gameObject.activeSelf)
            Components.DirectLight.gameObject.SetActive(true);

        GameTime.ProgressTime = progressMode;
        if (EffectsHolder != null)
            EffectsHolder.SetActive(true);

        if (EnviroSkyRender != null)
            EnviroSkyRender.enabled = true;

        if (EnviroPostProcessing != null)
            EnviroPostProcessing.enabled = true;

#if ENVIRO_HDRP
        if(mySkyAndFogVolume != null)
           mySkyAndFogVolume.SetActive(true);
#endif

        TryPlayAmbientSFX();

        if (Weather.currentAudioSource != null)
            Weather.currentAudioSource.audiosrc.Play();

        started = true;
    }

    public void Stop(bool disableLight = false, bool stopTime = true)
    {
        if (disableLight)
            Components.DirectLight.gameObject.SetActive(false);
        if (stopTime)
            GameTime.ProgressTime = EnviroTime.TimeProgressMode.None;

        if (EffectsHolder != null)
            EffectsHolder.SetActive(false);

        if (EnviroSkyRender != null)
            EnviroSkyRender.enabled = false;

        if (EnviroPostProcessing != null)
            EnviroPostProcessing.enabled = false;

#if ENVIRO_HDRP
        if(mySkyAndFogVolume != null)
           mySkyAndFogVolume.SetActive(false);
#endif

        started = false;
    }

    public void Deactivate(bool disableLight = false)
    {
        if (disableLight)
            Components.DirectLight.gameObject.SetActive(false);

        if (EffectsHolder != null)
            EffectsHolder.SetActive(false);

        if (EnviroSkyRender != null)
            EnviroSkyRender.enabled = false;

        if (EnviroPostProcessing != null)
            EnviroPostProcessing.enabled = false;

#if ENVIRO_HDRP
        if(mySkyAndFogVolume != null)
           mySkyAndFogVolume.SetActive(false);
#endif

    }

    public void Activate()
    {
        Components.DirectLight.gameObject.SetActive(true);

        if (EffectsHolder != null)
            EffectsHolder.SetActive(true);

        if (EnviroSkyRender != null)
            EnviroSkyRender.enabled = true;
 
        if (EnviroPostProcessing != null)
            EnviroPostProcessing.enabled = true;

#if ENVIRO_HDRP
        if(mySkyAndFogVolume != null)
           mySkyAndFogVolume.SetActive(true);
#endif

        TryPlayAmbientSFX();

        if (Weather.currentAudioSource != null)
            Weather.currentAudioSource.audiosrc.Play();
    }

    #endregion
    ////////////////////////////////
}