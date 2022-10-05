using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System.IO;


[CustomEditor(typeof(EnviroSky))]
public class EnviroSkyEditor : Editor
{
#if ENVIRO_HD
    private string latestVersion = "2.4.2";
    // GUI Styles 
    private GUIStyle boxStyle;
    private GUIStyle boxStyleModified;
    private GUIStyle wrapStyle;
    private GUIStyle headerStyle;
    private GUIStyle headerFoldout;
    private GUIStyle popUpStyle;
    //Target
    private EnviroSky myTarget;
    private Color modifiedColor;
    private Color greenColor;
    private Color boxColor1;
    private bool showWeatherMap;


    //Profile Properties
    SerializedObject serializedObj;
    SerializedProperty AdditionalDirectLight, Sun, Moon, DirectLight, GlobalReflectionProbe, windZone, LightningGenerator, satellites, starsRotation;
    SerializedProperty Player, Camera, PlayerTag, CameraTag, AssignOnRuntime, tonemapping, SatTag, startMode, singlePassInstancedVR, floatingPointOriginAnchor;
    SerializedProperty cycleLenghtInMinutes, ProgressMode, Years, Days, Hours, Minutes, Seconds, Longitude, Latitude, DayLength, NightLength, UTC, UpdateSeason, CurrentSeason, DaysInYear;
    SerializedProperty UpdateWeather, StartWeather, EnableVolumeLighting, EnableSunShafts, EnableMoonShafts, AmbientVolume, WeatherVolume;
    SerializedProperty lightIntensityLuxMult, exposure, angleOffset, lightColorGradient, lightIntensityCurveSun, lightIntensityCurveMoon, shadowStrength, VolumeLightingResolution, globalReflectionsUpdateOnPosition, globalReflectionsUpdateOnGameTime;
    SerializedProperty stopRotationAtHigh, rotationStopHigh, directionalLightMode, lightIntensityTransitionSpeed, ambientMode, ambientIntensityCurve, ambientSkyGradient, ambientEquatorGradient, ambientGroundGradient;
    SerializedProperty updateDefaultEnvironmentReflections, globalReflectionLayers, globalReflectionResolution, reflectionCloudsQuality, useFogInReflection, globalReflectionsScale, reflectionBool, reflectionIntensity, reflectionUpdate;
    SerializedProperty sunDiskSizeSimple, simpleGroundColor, simpleSunColor, simpleSkyColor, simpleHorizonColor, simpleHorizonBackColor, renderMoon, moonGlowSize, blackGroundMode, galaxyCubeMap, galaxyIntensity, starsTwinklingRate, skyboxMode, customSkyboxMaterial, customSkyboxColor, rayleigh, g, mie, scatteringCurve, scatteringColor, sunMoonPos, sunIntensity, sunDiskScale, sunDiskIntensity, sunDiskColor, moonPhaseMode, moonTexture, moonGlowTexture, moonSize, moonGlow, currentMoonPhase, skyLuminance, skyColorPower, skyExposure, fogExposure, starsCubemap, starsIntensity, moonColor, moonGlowColor;
    SerializedProperty cloudsSkyFogHeightBlending,dualLayerParticleClouds, setEnviroSkybox, skyboxExposure, controlSceneExposure, lightingVarianceTiling, useLessSteps, useHaltonRaymarchOffset, attenuationClamp, volumeCloudsAmbientColor, cloudsHeightMod, shadowCookieSize, shadowIntensity, customWeatherMap, silverLiningIntensity, silverLiningSpread, weatherAnimSpeedScale, globalCloudCoverage, cirrusCloudsAltitude, cloudsWorldScale, hgPhase, cloudsExposure, cirrusCloudsColor, volumeCloudsColor, volumeCloudsMoonColor, cirrusCloudsTexture, weatherMapTiling, LightIntensity, depthBlending, bilateralUpsampling;
    SerializedProperty useTag, wetnessAccumulationSpeed, wetnessDryingSpeed, snowAccumulationSpeed, snowMeltingSpeed, cloudTransitionSpeed, fogTransitionSpeed, effectTransitionSpeed, audioTransitionSpeed, useWindZoneDirection, windTimeScale, windIntensity, windDirectionX, windDirectionY;
    SerializedProperty useSimpleFog, simpleFogColor, fogMie, fogG, fogmode, distanceFog, startDistance, distanceFogIntensity, maximumFogIntensity, heightFog, height, heightFogIntensity, useNoise, noiseIntensity, noiseIntensityOffset, noiseScale, fogDithering, skyDithering;
    SerializedProperty resolution, screenBlendMode, useDepthTexture, lightShaftsColorSun, lightShaftsColorMoon, treshholdColorSun, treshholdColorMoon, blurRadius, shaftsIntensity, maxRadius;
    SerializedProperty SpringStart, SpringEnd, SummerStart, SummerEnd, AutumnStart, AutumnEnd, WinterStart, WinterEnd;
    SerializedProperty effectQuality, updateInterval, lightningEffect, lightningRange, lightningHeight;
    SerializedProperty setCameraClearFlags, reflectionUpdatePos;
    SerializedProperty useFog, particleClouds, useVolumeClouds, useDistanceBlur, useFlatClouds, useParticleClouds, particleCloudsHeight1, particleCloudsColor1, particleCloudsHeight2, particleCloudsColor2, ambientLightIntensity;
    SerializedProperty volumeLighting, SampleCount, ScatteringCoef, ExtinctionCoef, Anistropy, MaxRayLength, VolumeResolution, globalVolumeIntensity;
    SerializedProperty showVolumeCloudsEditor, showDistanceBlurInEditor, showVolumeLightingEditor, showFogEditor, showFlatCloudsEditor, volumeNoiseIntensity, volumeNoiseScale, volumeNoiseIntensityOffset, windUpwardsIntensity, cirrusWindIntensity;
    SerializedProperty antiFlicker, highQuality, radius, globalReflectionClouds, globalReflectionTimeSlicing;
    SerializedProperty springBaseTemperature, summerBaseTemperature, autumnBaseTemperature, winterBaseTemperature, temperatureChangingSpeed, dayNightSwitch, snowMeltingTresholdTemperature, windIntensityTransitionSpeed;
    SerializedProperty useAurora, auroraIntensity, auroraColor, auroraBrightness, auroraContrast, auroraHeight, auroraScale, auroraSteps, auroraSpeed;
    SerializedProperty flatCloudsBaseTexture, flatCloudsDetailTexture, flatCloudsDirectLightColor, flatCloudsAmbientLightColor, flatCloudsAltitude, flatCloudsBaseTextureTiling, flatCloudsDetailTextureTiling, cloudsDetailWindIntensity;
    //HDRP
    SerializedProperty ambientColorMod, indirectLightingUpdateMode,fogColorTint,exposurePhysical, skyExposurePhysical, lightingUpdateEachFrames, useEnviroFog, useHDRPFog, usePhysicalBasedLighting, sunIntensityLux, moonIntensityLux, lightColorTemperature, lightColorTint;
    ReorderableList thunderSFX;

    void OnEnable()
    {
        myTarget = (EnviroSky)target;
        serializedObj = new SerializedObject(myTarget);
        //Components
        Sun = serializedObj.FindProperty("Components.Sun");
        Moon = serializedObj.FindProperty("Components.Moon");
        DirectLight = serializedObj.FindProperty("Components.DirectLight");
        GlobalReflectionProbe = serializedObj.FindProperty("Components.GlobalReflectionProbe");
        windZone = serializedObj.FindProperty("Components.windZone");
        LightningGenerator = serializedObj.FindProperty("Components.LightningGenerator");
        satellites = serializedObj.FindProperty("Components.satellites");
        starsRotation = serializedObj.FindProperty("Components.starsRotation");
        particleClouds = serializedObj.FindProperty("Components.particleClouds");
        AdditionalDirectLight = serializedObj.FindProperty("Components.AdditionalDirectLight");
        /// Setup
        Player = serializedObj.FindProperty("Player");
        Camera = serializedObj.FindProperty("PlayerCamera");
        PlayerTag = serializedObj.FindProperty("PlayerTag");
        CameraTag = serializedObj.FindProperty("CameraTag");
        AssignOnRuntime = serializedObj.FindProperty("AssignInRuntime");
        tonemapping = serializedObj.FindProperty("tonemapping");
        startMode = serializedObj.FindProperty("startMode");
        SatTag = serializedObj.FindProperty("satelliteRenderingLayer");
        singlePassInstancedVR = serializedObj.FindProperty("singlePassInstancedVR");
        setCameraClearFlags = serializedObj.FindProperty("setCameraClearFlags");
        floatingPointOriginAnchor = serializedObj.FindProperty("floatingPointOriginAnchor");
        // Weather Controls
        UpdateWeather = serializedObj.FindProperty("Weather.updateWeather");
        StartWeather = serializedObj.FindProperty("Weather.startWeatherPreset");
        //Feature Controls:
        EnableVolumeLighting = serializedObj.FindProperty("useVolumeLighting");
        EnableSunShafts = serializedObj.FindProperty("LightShafts.sunLightShafts");
        EnableMoonShafts = serializedObj.FindProperty("LightShafts.moonLightShafts");
        useAurora = serializedObj.FindProperty("useAurora");
        // Audio Controls
        AmbientVolume = serializedObj.FindProperty("Audio.ambientSFXVolume");
        WeatherVolume = serializedObj.FindProperty("Audio.weatherSFXVolume");
        // Time Controls
        ProgressMode = serializedObj.FindProperty("GameTime.ProgressTime");
        DayLength = serializedObj.FindProperty("GameTime.dayLengthModifier");
        NightLength = serializedObj.FindProperty("GameTime.nightLengthModifier");
        cycleLenghtInMinutes = serializedObj.FindProperty("GameTime.cycleLengthInMinutes");
        UpdateSeason = serializedObj.FindProperty("Seasons.calcSeasons");
        CurrentSeason = serializedObj.FindProperty("Seasons.currentSeasons");
        Years = serializedObj.FindProperty("GameTime.Years");
        Days = serializedObj.FindProperty("GameTime.Days");
        Hours = serializedObj.FindProperty("GameTime.Hours");
        Minutes = serializedObj.FindProperty("GameTime.Minutes");
        Seconds = serializedObj.FindProperty("GameTime.Seconds");
        Longitude = serializedObj.FindProperty("GameTime.Longitude");
        Latitude = serializedObj.FindProperty("GameTime.Latitude");
        UTC = serializedObj.FindProperty("GameTime.utcOffset");
        DaysInYear = serializedObj.FindProperty("GameTime.DaysInYear");
        dayNightSwitch = serializedObj.FindProperty("GameTime.dayNightSwitch");
        //Lighting Category
        lightColorGradient = serializedObj.FindProperty("lightSettings.LightColor");
        lightIntensityCurveSun = serializedObj.FindProperty("lightSettings.directLightSunIntensity");
        lightIntensityCurveMoon = serializedObj.FindProperty("lightSettings.directLightMoonIntensity");
        shadowStrength = serializedObj.FindProperty("lightSettings.shadowIntensity");
        angleOffset = serializedObj.FindProperty("lightSettings.directLightAngleOffset");
        ambientMode = serializedObj.FindProperty("lightSettings.ambientMode");
        ambientIntensityCurve = serializedObj.FindProperty("lightSettings.ambientIntensity");
        ambientSkyGradient = serializedObj.FindProperty("lightSettings.ambientSkyColor");
        ambientEquatorGradient = serializedObj.FindProperty("lightSettings.ambientEquatorColor");
        ambientGroundGradient = serializedObj.FindProperty("lightSettings.ambientGroundColor");
        stopRotationAtHigh = serializedObj.FindProperty("lightSettings.stopRotationAtHigh");
        rotationStopHigh = serializedObj.FindProperty("lightSettings.rotationStopHigh");
        reflectionBool = serializedObj.FindProperty("reflectionSettings.globalReflections");
        reflectionIntensity = serializedObj.FindProperty("reflectionSettings.globalReflectionsIntensity");
        reflectionUpdate = serializedObj.FindProperty("reflectionSettings.globalReflectionsTimeTreshold");
        reflectionUpdatePos = serializedObj.FindProperty("reflectionSettings.globalReflectionsPositionTreshold");
        updateDefaultEnvironmentReflections = serializedObj.FindProperty("reflectionSettings.updateDefaultEnvironmentReflections"); 
        globalReflectionsScale = serializedObj.FindProperty("reflectionSettings.globalReflectionsScale");
        globalReflectionsUpdateOnPosition = serializedObj.FindProperty("reflectionSettings.globalReflectionsUpdateOnPosition");
        globalReflectionsUpdateOnGameTime = serializedObj.FindProperty("reflectionSettings.globalReflectionsUpdateOnGameTime");
        globalReflectionClouds = serializedObj.FindProperty("reflectionSettings.globalReflectionCustomRendering");
        globalReflectionTimeSlicing = serializedObj.FindProperty("reflectionSettings.globalReflectionTimeSlicing");
        reflectionCloudsQuality = serializedObj.FindProperty("reflectionSettings.reflectionCloudsQuality");
        useFogInReflection = serializedObj.FindProperty("reflectionSettings.globalReflectionUseFog");
        globalReflectionResolution = serializedObj.FindProperty("reflectionSettings.globalReflectionResolution");
        globalReflectionLayers = serializedObj.FindProperty("reflectionSettings.globalReflectionLayers");
        lightIntensityTransitionSpeed = serializedObj.FindProperty("lightSettings.lightIntensityTransitionSpeed");
        directionalLightMode = serializedObj.FindProperty("lightSettings.directionalLightMode");
     
        //HDRP
        lightingUpdateEachFrames = serializedObj.FindProperty("lightingUpdateEachFrames");
        exposure = serializedObj.FindProperty("lightSettings.exposure");
        controlSceneExposure = serializedObj.FindProperty("lightSettings.controlSceneExposure");
        skyboxExposure = serializedObj.FindProperty("lightSettings.skyExposure");
        lightIntensityLuxMult = serializedObj.FindProperty("lightSettings.lightIntensityLuxMult");
        setEnviroSkybox = serializedObj.FindProperty("skySettings.setEnviroSkybox");
        useEnviroFog = serializedObj.FindProperty("fogSettings.useEnviroGroundFog");
        useHDRPFog = serializedObj.FindProperty("fogSettings.useHDRPFog");
        usePhysicalBasedLighting = serializedObj.FindProperty("lightSettings.usePhysicalBasedLighting");
        sunIntensityLux = serializedObj.FindProperty("lightSettings.sunIntensityLux");
        moonIntensityLux = serializedObj.FindProperty("lightSettings.moonIntensityLux");
        lightColorTemperature = serializedObj.FindProperty("lightSettings.lightColorTemperature");
        lightColorTint = serializedObj.FindProperty("lightSettings.lightColorTint");
        exposurePhysical = serializedObj.FindProperty("lightSettings.exposurePhysical");
        skyExposurePhysical = serializedObj.FindProperty("lightSettings.skyExposurePhysical");
        indirectLightingUpdateMode = serializedObj.FindProperty("lightSettings.indirectLightingUpdateMode");
        ambientColorMod = serializedObj.FindProperty("lightSettings.ambientColorMod");

        //Volume Lighting
        VolumeLightingResolution = serializedObj.FindProperty("volumeLightSettings.Resolution");
        volumeLighting = serializedObj.FindProperty("volumeLightSettings.dirVolumeLighting");
        SampleCount = serializedObj.FindProperty("volumeLightSettings.SampleCount");
        ScatteringCoef = serializedObj.FindProperty("volumeLightSettings.ScatteringCoef");
        ExtinctionCoef = serializedObj.FindProperty("volumeLightSettings.ExtinctionCoef");
        Anistropy = serializedObj.FindProperty("volumeLightSettings.Anistropy");
        MaxRayLength = serializedObj.FindProperty("volumeLightSettings.MaxRayLength");
        useNoise = serializedObj.FindProperty("volumeLightSettings.directLightNoise");
        volumeNoiseIntensity = serializedObj.FindProperty("volumeLightSettings.noiseIntensity");
        volumeNoiseScale = serializedObj.FindProperty("volumeLightSettings.noiseScale");
        volumeNoiseIntensityOffset = serializedObj.FindProperty("volumeLightSettings.noiseIntensityOffset");
        //Sky Category
        skyboxMode = serializedObj.FindProperty("skySettings.skyboxMode");
        customSkyboxMaterial = serializedObj.FindProperty("skySettings.customSkyboxMaterial");
        customSkyboxColor = serializedObj.FindProperty("skySettings.customSkyboxColor");
        blackGroundMode = serializedObj.FindProperty("skySettings.blackGroundMode");
        rayleigh = serializedObj.FindProperty("skySettings.rayleigh");
        g = serializedObj.FindProperty("skySettings.g");
        mie = serializedObj.FindProperty("skySettings.mie");
        scatteringCurve = serializedObj.FindProperty("skySettings.scatteringCurve");
        scatteringColor = serializedObj.FindProperty("skySettings.scatteringColor");
        sunMoonPos = serializedObj.FindProperty("skySettings.sunAndMoonPosition");
        sunIntensity = serializedObj.FindProperty("skySettings.sunIntensity");
        sunDiskScale = serializedObj.FindProperty("skySettings.sunDiskScale");
        sunDiskIntensity = serializedObj.FindProperty("skySettings.sunDiskIntensity");
        sunDiskColor = serializedObj.FindProperty("skySettings.sunDiskColor");
        renderMoon = serializedObj.FindProperty("skySettings.renderMoon");
        moonPhaseMode = serializedObj.FindProperty("skySettings.moonPhaseMode");
        moonTexture = serializedObj.FindProperty("skySettings.moonTexture");
        moonGlowTexture = serializedObj.FindProperty("skySettings.glowTexture");
        moonGlow = serializedObj.FindProperty("skySettings.moonGlow");
        currentMoonPhase = serializedObj.FindProperty("customMoonPhase");
        skyLuminance = serializedObj.FindProperty("skySettings.skyLuminence");
        skyColorPower = serializedObj.FindProperty("skySettings.skyColorPower");
        skyExposure = serializedObj.FindProperty("skySettings.skyExposure");
        starsCubemap = serializedObj.FindProperty("skySettings.starsCubeMap");
        starsIntensity = serializedObj.FindProperty("skySettings.starsIntensity");
        starsTwinklingRate = serializedObj.FindProperty("skySettings.starsTwinklingRate");
        moonGlowColor = serializedObj.FindProperty("skySettings.moonGlowColor");
        moonColor = serializedObj.FindProperty("skySettings.moonColor");
        moonSize = serializedObj.FindProperty("skySettings.moonSize");
        moonGlowSize = serializedObj.FindProperty("skySettings.glowSize");
        simpleSkyColor = serializedObj.FindProperty("skySettings.simpleSkyColor");
        simpleHorizonColor = serializedObj.FindProperty("skySettings.simpleHorizonColor");
        simpleHorizonBackColor = serializedObj.FindProperty("skySettings.simpleHorizonBackColor");
        sunDiskSizeSimple = serializedObj.FindProperty("skySettings.simpleSunDiskSize");
        simpleSunColor = serializedObj.FindProperty("skySettings.simpleSunColor");
        simpleGroundColor = serializedObj.FindProperty("skySettings.simpleGroundColor");
        galaxyCubeMap = serializedObj.FindProperty("skySettings.galaxyCubeMap");
        galaxyIntensity = serializedObj.FindProperty("skySettings.galaxyIntensity");
        //Clouds Category
        cloudsWorldScale = serializedObj.FindProperty("cloudsSettings.cloudsWorldScale");
        depthBlending = serializedObj.FindProperty("cloudsSettings.depthBlending");
        bilateralUpsampling = serializedObj.FindProperty("cloudsSettings.bilateralUpsampling");
        attenuationClamp = serializedObj.FindProperty("cloudsSettings.attenuationClamp");
        useHaltonRaymarchOffset = serializedObj.FindProperty("cloudsSettings.useHaltonRaymarchOffset");
        useLessSteps = serializedObj.FindProperty("cloudsSettings.useLessSteps");
        weatherAnimSpeedScale = serializedObj.FindProperty("cloudsSettings.weatherAnimSpeedScale");
        cloudsHeightMod = serializedObj.FindProperty("cloudsSettings.cloudsHeightMod");
        hgPhase = serializedObj.FindProperty("cloudsSettings.hgPhase");
        weatherMapTiling = serializedObj.FindProperty("cloudsSettings.weatherMapTiling");
        volumeCloudsColor = serializedObj.FindProperty("cloudsSettings.volumeCloudsColor");
        cloudsExposure = serializedObj.FindProperty("cloudsSettings.cloudsExposure");
        volumeCloudsMoonColor = serializedObj.FindProperty("cloudsSettings.volumeCloudsMoonColor");
        ambientLightIntensity = serializedObj.FindProperty("cloudsSettings.ambientLightIntensity");
        lightingVarianceTiling = serializedObj.FindProperty("cloudsSettings.lightingVarianceTiling");
        volumeCloudsAmbientColor = serializedObj.FindProperty("cloudsSettings.volumeCloudsAmbientColor");
        cirrusCloudsTexture = serializedObj.FindProperty("cloudsSettings.cirrusCloudsTexture");
        cirrusCloudsAltitude = serializedObj.FindProperty("cloudsSettings.cirrusCloudsAltitude");
        cirrusCloudsColor = serializedObj.FindProperty("cloudsSettings.cirrusCloudsColor");
        globalCloudCoverage = serializedObj.FindProperty("cloudsSettings.globalCloudCoverage");
        silverLiningIntensity = serializedObj.FindProperty("cloudsSettings.silverLiningIntensity");
        customWeatherMap = serializedObj.FindProperty("cloudsSettings.customWeatherMap");
        LightIntensity = serializedObj.FindProperty("cloudsSettings.lightIntensity");
        silverLiningSpread = serializedObj.FindProperty("cloudsSettings.silverLiningSpread");
        shadowIntensity = serializedObj.FindProperty("cloudsSettings.shadowIntensity");
        shadowCookieSize = serializedObj.FindProperty("cloudsSettings.shadowCookieSize");
        particleCloudsHeight1 = serializedObj.FindProperty("cloudsSettings.ParticleCloudsLayer1.height");
        particleCloudsColor1 = serializedObj.FindProperty("cloudsSettings.ParticleCloudsLayer1.particleCloudsColor");
        particleCloudsHeight2 = serializedObj.FindProperty("cloudsSettings.ParticleCloudsLayer2.height");
        particleCloudsColor2 = serializedObj.FindProperty("cloudsSettings.ParticleCloudsLayer2.particleCloudsColor");
        dualLayerParticleClouds = serializedObj.FindProperty("cloudsSettings.dualLayerParticleClouds");
        flatCloudsBaseTexture = serializedObj.FindProperty("cloudsSettings.flatCloudsBaseTexture");
        flatCloudsDetailTexture = serializedObj.FindProperty("cloudsSettings.flatCloudsDetailTexture");
        flatCloudsDirectLightColor = serializedObj.FindProperty("cloudsSettings.flatCloudsDirectLightColor");
        flatCloudsAmbientLightColor = serializedObj.FindProperty("cloudsSettings.flatCloudsAmbientLightColor");
        flatCloudsAltitude = serializedObj.FindProperty("cloudsSettings.flatCloudsAltitude");
        flatCloudsBaseTextureTiling = serializedObj.FindProperty("cloudsSettings.flatCloudsBaseTextureTiling");
        flatCloudsDetailTextureTiling = serializedObj.FindProperty("cloudsSettings.flatCloudsDetailTextureTiling");
        cloudsDetailWindIntensity = serializedObj.FindProperty("cloudsSettings.cloudsDetailWindIntensity");
        cloudsSkyFogHeightBlending = serializedObj.FindProperty("cloudsSettings.cloudsSkyFogHeightBlending");
        // Weather Category
        useTag = serializedObj.FindProperty("weatherSettings.useTag");
        wetnessAccumulationSpeed = serializedObj.FindProperty("weatherSettings.wetnessAccumulationSpeed");
        wetnessDryingSpeed = serializedObj.FindProperty("weatherSettings.wetnessDryingSpeed");
        snowAccumulationSpeed = serializedObj.FindProperty("weatherSettings.snowAccumulationSpeed");
        snowMeltingSpeed = serializedObj.FindProperty("weatherSettings.snowMeltingSpeed");
        cloudTransitionSpeed = serializedObj.FindProperty("weatherSettings.cloudTransitionSpeed");
        fogTransitionSpeed = serializedObj.FindProperty("weatherSettings.fogTransitionSpeed");
        effectTransitionSpeed = serializedObj.FindProperty("weatherSettings.effectTransitionSpeed");
        audioTransitionSpeed = serializedObj.FindProperty("weatherSettings.audioTransitionSpeed");
        snowMeltingTresholdTemperature = serializedObj.FindProperty("weatherSettings.snowMeltingTresholdTemperature");
        windIntensityTransitionSpeed = serializedObj.FindProperty("weatherSettings.windIntensityTransitionSpeed");
        lightningEffect = serializedObj.FindProperty("weatherSettings.lightningEffect");
        lightningRange = serializedObj.FindProperty("weatherSettings.lightningRange");
        lightningHeight = serializedObj.FindProperty("weatherSettings.lightningHeight");
        temperatureChangingSpeed = serializedObj.FindProperty("weatherSettings.temperatureChangingSpeed");
        useWindZoneDirection = serializedObj.FindProperty("cloudsSettings.useWindZoneDirection");
        useVolumeClouds = serializedObj.FindProperty("useVolumeClouds");
        useFlatClouds = serializedObj.FindProperty("useFlatClouds");
        useDistanceBlur = serializedObj.FindProperty("useDistanceBlur");
        useParticleClouds = serializedObj.FindProperty("useParticleClouds");
        useFog = serializedObj.FindProperty("useFog");
        windTimeScale = serializedObj.FindProperty("cloudsSettings.cloudsTimeScale");
        cirrusWindIntensity = serializedObj.FindProperty("cloudsSettings.cirrusWindIntensity");
        windIntensity = serializedObj.FindProperty("cloudsSettings.cloudsWindIntensity");
        windUpwardsIntensity = serializedObj.FindProperty("cloudsSettings.cloudsUpwardsWindIntensity");
        windDirectionX = serializedObj.FindProperty("cloudsSettings.cloudsWindDirectionX");
        windDirectionY = serializedObj.FindProperty("cloudsSettings.cloudsWindDirectionY");
        fogmode = serializedObj.FindProperty("fogSettings.Fogmode");
        distanceFog = serializedObj.FindProperty("fogSettings.distanceFog");
        startDistance = serializedObj.FindProperty("fogSettings.startDistance");
        distanceFogIntensity = serializedObj.FindProperty("fogSettings.distanceFogIntensity");
        maximumFogIntensity = serializedObj.FindProperty("fogSettings.maximumFogDensity");
        heightFog = serializedObj.FindProperty("fogSettings.heightFog");
        height = serializedObj.FindProperty("fogSettings.height");
        heightFogIntensity = serializedObj.FindProperty("fogSettings.heightFogIntensity");
        noiseIntensity = serializedObj.FindProperty("fogSettings.noiseIntensity");
        noiseIntensityOffset = serializedObj.FindProperty("fogSettings.noiseIntensityOffset");
        noiseScale = serializedObj.FindProperty("fogSettings.noiseScale");
        fogDithering = serializedObj.FindProperty("fogSettings.fogDithering");
        skyDithering = serializedObj.FindProperty("skySettings.dithering");
        fogMie = serializedObj.FindProperty("fogSettings.mie");
        fogG = serializedObj.FindProperty("fogSettings.g");
        useSimpleFog = serializedObj.FindProperty("fogSettings.useSimpleFog");
        simpleFogColor = serializedObj.FindProperty("fogSettings.simpleFogColor");
        fogExposure = serializedObj.FindProperty("fogSettings.fogExposure");

        fogColorTint = serializedObj.FindProperty("fogSettings.fogColorTint");

        //AURORA
        auroraIntensity = serializedObj.FindProperty("auroraSettings.auroraIntensity");
        auroraColor = serializedObj.FindProperty("auroraSettings.auroraColor");
        auroraBrightness = serializedObj.FindProperty("auroraSettings.auroraBrightness");
        auroraContrast = serializedObj.FindProperty("auroraSettings.auroraContrast");
        auroraHeight = serializedObj.FindProperty("auroraSettings.auroraHeight");
        auroraScale = serializedObj.FindProperty("auroraSettings.auroraScale");
        auroraSteps = serializedObj.FindProperty("auroraSettings.auroraSteps");
        auroraSpeed = serializedObj.FindProperty("auroraSettings.auroraSpeed");
        //LightShafts
        resolution = serializedObj.FindProperty("lightshaftsSettings.resolution");
        screenBlendMode = serializedObj.FindProperty("lightshaftsSettings.screenBlendMode");
        useDepthTexture = serializedObj.FindProperty("lightshaftsSettings.useDepthTexture");
        lightShaftsColorSun = serializedObj.FindProperty("lightshaftsSettings.lightShaftsColorSun");
        lightShaftsColorMoon = serializedObj.FindProperty("lightshaftsSettings.lightShaftsColorMoon");
        treshholdColorSun = serializedObj.FindProperty("lightshaftsSettings.thresholdColorSun");
        treshholdColorMoon = serializedObj.FindProperty("lightshaftsSettings.thresholdColorMoon");
        blurRadius = serializedObj.FindProperty("lightshaftsSettings.blurRadius");
        shaftsIntensity = serializedObj.FindProperty("lightshaftsSettings.intensity");
        maxRadius = serializedObj.FindProperty("lightshaftsSettings.maxRadius");
        //Distance Blur
        antiFlicker = serializedObj.FindProperty("distanceBlurSettings.antiFlicker");
        highQuality = serializedObj.FindProperty("distanceBlurSettings.highQuality");
        radius = serializedObj.FindProperty("distanceBlurSettings.radius");
        //Season
        SpringStart = serializedObj.FindProperty("seasonsSettings.SpringStart");
        SpringEnd = serializedObj.FindProperty("seasonsSettings.SpringEnd");
        SummerStart = serializedObj.FindProperty("seasonsSettings.SummerStart");
        SummerEnd = serializedObj.FindProperty("seasonsSettings.SummerEnd");
        AutumnStart = serializedObj.FindProperty("seasonsSettings.AutumnStart");
        AutumnEnd = serializedObj.FindProperty("seasonsSettings.AutumnEnd");
        WinterStart = serializedObj.FindProperty("seasonsSettings.WinterStart");
        WinterEnd = serializedObj.FindProperty("seasonsSettings.WinterEnd");
        springBaseTemperature = serializedObj.FindProperty("seasonsSettings.springBaseTemperature");
        summerBaseTemperature = serializedObj.FindProperty("seasonsSettings.summerBaseTemperature");
        autumnBaseTemperature = serializedObj.FindProperty("seasonsSettings.autumnBaseTemperature");
        winterBaseTemperature = serializedObj.FindProperty("seasonsSettings.winterBaseTemperature");
        //Quality
        effectQuality = serializedObj.FindProperty("qualitySettings.GlobalParticleEmissionRates");
        updateInterval = serializedObj.FindProperty("qualitySettings.UpdateInterval");
        //Editor
        showVolumeCloudsEditor = serializedObj.FindProperty("showVolumeCloudsInEditor");
        showDistanceBlurInEditor = serializedObj.FindProperty("showDistanceBlurInEditor");
        showVolumeLightingEditor = serializedObj.FindProperty("showVolumeLightingInEditor");
        showFogEditor = serializedObj.FindProperty("showFogInEditor");
        showFlatCloudsEditor = serializedObj.FindProperty("showFlatCloudsInEditor");
        //Audio
        thunderSFX = new ReorderableList(serializedObject,
            serializedObject.FindProperty("audioSettings.ThunderSFX"),
            true, true, true, true);

        thunderSFX.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Thunder SFX");
        };
        thunderSFX.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                var element = thunderSFX.serializedProperty.GetArrayElementAtIndex(index);
                rect.y += 2;
                EditorGUI.PropertyField(
                    new Rect(rect.x, rect.y, Screen.width * .8f, EditorGUIUtility.singleLineHeight),
                    element, GUIContent.none);
            };
        thunderSFX.onAddCallback = (ReorderableList l) =>
        {
            var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;
        };

        modifiedColor = Color.red;
        modifiedColor.a = 0.5f;

        greenColor = Color.green;
        greenColor.a = 0.5f;
#if UNITY_2019_3_OR_NEWER
        boxColor1 = new Color(0.95f, 0.95f, 0.95f, 1f);
#else
        boxColor1 = new Color(0.85f, 0.85f, 0.85f, 1f);
#endif
        ////
    }
    /// <summary>
    /// Applies the changes and set profile to modifed but not saved.
    /// </summary>
    private void ApplyChanges()
    {
        if (EditorGUI.EndChangeCheck())
        {
            serializedObj.ApplyModifiedProperties();
            myTarget.profile.modified = true;
        }
    }

    private string[] cloudsQualityPresetsNames;

    public override void OnInspectorGUI()
    {
        myTarget = (EnviroSky)target;

        if (boxStyle == null)
        {
            boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.normal.textColor = GUI.skin.label.normal.textColor;
            boxStyle.fontStyle = FontStyle.Bold;
            boxStyle.alignment = TextAnchor.UpperLeft;
        }

        if (boxStyleModified == null)
        {
            boxStyleModified = new GUIStyle(EditorStyles.helpBox);
            boxStyleModified.normal.textColor = GUI.skin.label.normal.textColor;
            boxStyleModified.fontStyle = FontStyle.Bold;
            boxStyleModified.fontSize = 11;
            boxStyleModified.alignment = TextAnchor.UpperLeft;
        }

        if (wrapStyle == null)
        {
            wrapStyle = new GUIStyle(GUI.skin.label);
            wrapStyle.fontStyle = FontStyle.Normal;
            wrapStyle.wordWrap = true;
        }

        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.alignment = TextAnchor.UpperLeft;
        }

        if (headerFoldout == null)
        {
            headerFoldout = new GUIStyle(EditorStyles.foldout);
            headerFoldout.fontStyle = FontStyle.Bold;
        }

        if (popUpStyle == null)
        {
            popUpStyle = new GUIStyle(EditorStyles.popup);
            popUpStyle.alignment = TextAnchor.MiddleCenter;
            popUpStyle.fixedHeight = 20f;
            popUpStyle.fontStyle = FontStyle.Bold;
        }



        GUILayout.BeginVertical("Enviro - Sky and Weather " + latestVersion, boxStyle);
        GUILayout.Space(20);

        GUI.backgroundColor = boxColor1;
        GUILayout.BeginVertical("Profile", boxStyleModified);
        GUI.backgroundColor = Color.white;
        GUILayout.Space(20);
        myTarget.profile = (EnviroProfile)EditorGUILayout.ObjectField(myTarget.profile, typeof(EnviroProfile), false);
        GUILayout.Space(10);
        if (myTarget.profile != null)
            EditorGUILayout.LabelField("Profile Version:", myTarget.profile.version,headerStyle);
        else
            EditorGUILayout.LabelField("No Profile Assigned!");
        GUILayout.Space(5);

        if (myTarget.profile != null && myTarget.profile.version != latestVersion)
            if (GUILayout.Button("Update Profile"))
            {
                if (EnviroProfileCreation.UpdateProfile(myTarget.profile, myTarget.profile.version, latestVersion) == true)
                    myTarget.ApplyProfile(myTarget.profile);
            }
        // Runtime Settings
        if (GUILayout.Button("Apply all Settings"))
        {
            myTarget.ReInit();
        }
        GUILayout.EndHorizontal();

        if (myTarget.profile != null)
        {

            if (myTarget.profile.modified) // Change color when modified
                GUI.backgroundColor = modifiedColor;
            else
                GUI.backgroundColor = boxColor1;

            GUILayout.BeginVertical("", boxStyleModified);
            GUI.backgroundColor = Color.white;

            serializedObj.UpdateIfRequiredOrScript();

            myTarget.showSettings = GUILayout.Toggle(myTarget.showSettings, "Edit Profile", headerFoldout);
            if (myTarget.showSettings)
            {
                GUILayout.BeginVertical("", boxStyleModified);
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();

                if (GUILayout.Button("Save to Profile"))
                {
                    myTarget.SaveProfile();
                    myTarget.profile.modified = false;
                }

                if (GUILayout.Button("Load from Profile"))
                {
                    myTarget.ApplyProfile(myTarget.profile);
                    myTarget.profile.modified = false;
                    serializedObj.UpdateIfRequiredOrScript();
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                EditorGUILayout.LabelField("Category", headerStyle);
                myTarget.profile.viewMode = (EnviroProfile.settingsMode)EditorGUILayout.EnumPopup(myTarget.profile.viewMode, popUpStyle);
                GUILayout.Space(10);
                GUILayout.EndVertical();

                switch (myTarget.profile.viewMode)
                {
                    case EnviroProfile.settingsMode.Lighting:
                        GUILayout.BeginVertical("", boxStyleModified);
                        EditorGUI.BeginChangeCheck();

#if ENVIRO_HDRP                 
                        EditorGUILayout.PropertyField(directionalLightMode, true, null);
                        EditorGUILayout.PropertyField(lightingUpdateEachFrames, true, null);
                        EditorGUILayout.PropertyField(indirectLightingUpdateMode, true, null);                      
                        EditorGUILayout.PropertyField(usePhysicalBasedLighting, true, null);

                        if (myTarget.lightSettings.usePhysicalBasedLighting == true)
                        {
                            GUILayout.Space(10);
                            EditorGUILayout.LabelField("Light Intensity", headerStyle);
                            EditorGUILayout.PropertyField(sunIntensityLux, true, null);
                            EditorGUILayout.PropertyField(moonIntensityLux, true, null);
                            EditorGUILayout.PropertyField(shadowStrength, true, null);
                             GUILayout.Space(10);
                            EditorGUILayout.LabelField("Light Color", headerStyle);
                            EditorGUILayout.PropertyField(lightColorTemperature, true, null);
                            EditorGUILayout.PropertyField(lightColorTint, true, null);
                            EditorGUILayout.PropertyField(ambientColorMod, true, null);
                            GUILayout.Space(10);
                            EditorGUILayout.LabelField("Exposure Settings", headerStyle);

                            EditorGUILayout.PropertyField(controlSceneExposure, true, null);
                            
                            if (myTarget.lightSettings.controlSceneExposure)
                            EditorGUILayout.PropertyField(exposurePhysical, true, null);
                            EditorGUILayout.PropertyField(skyExposurePhysical, true, null);
                            GUILayout.Space(10);
                            EditorGUILayout.LabelField("Current Lighting Settings:", headerStyle);
                            EditorGUILayout.LabelField("Current Sun light intensity in LUX: " + (myTarget.lightSettings.sunIntensityLux.Evaluate(myTarget.GameTime.solarTime) * myTarget.currentLightIntensityMod).ToString(), wrapStyle);
                            EditorGUILayout.LabelField("Current Moon light intensity in LUX: " + (myTarget.lightSettings.moonIntensityLux.Evaluate(myTarget.GameTime.lunarTime) * myTarget.currentLightIntensityMod).ToString(), wrapStyle);
                            GUILayout.Space(5);
                            EditorGUILayout.LabelField("Current scene exposure: " + (myTarget.lightSettings.exposurePhysical.Evaluate(myTarget.GameTime.solarTime) * myTarget.currentSceneExposureMod).ToString(), wrapStyle);
                            EditorGUILayout.LabelField("Current sky exposure: " + (myTarget.lightSettings.skyExposurePhysical.Evaluate(myTarget.GameTime.solarTime) * myTarget.currentSkyExposureMod).ToString(), wrapStyle);
         
                        }
                        else
                        {
                            GUILayout.Space(10);
                            EditorGUILayout.LabelField("Light Intensity and Color", headerStyle);
                            EditorGUILayout.PropertyField(lightColorGradient, true, null);
                            EditorGUILayout.PropertyField(lightIntensityCurveSun, true, null);
                            EditorGUILayout.PropertyField(lightIntensityCurveMoon, true, null);
                            EditorGUILayout.PropertyField(shadowStrength, true, null);
                            GUILayout.Space(10);
                            EditorGUILayout.LabelField("Exposure Settings", headerStyle);
                            EditorGUILayout.PropertyField(controlSceneExposure, true, null);
                            if (myTarget.lightSettings.controlSceneExposure)
                                EditorGUILayout.PropertyField(exposure, true, null);
                            EditorGUILayout.PropertyField(lightIntensityLuxMult, true, null);
                            EditorGUILayout.PropertyField(skyboxExposure, true, null);
                             GUILayout.Space(10);
                            EditorGUILayout.LabelField("Current Lighting Settings:", headerStyle);
                            EditorGUILayout.LabelField("Current Sun light intensity in LUX: " + (myTarget.lightSettings.directLightSunIntensity.Evaluate(myTarget.GameTime.solarTime) * myTarget.lightSettings.lightIntensityLuxMult * 250f).ToString(), wrapStyle);
                            EditorGUILayout.LabelField("Current Moon light intensity in LUX: " + ((myTarget.lightSettings.directLightMoonIntensity.Evaluate(myTarget.GameTime.lunarTime) * myTarget.lightSettings.lightIntensityLuxMult * 250f) * (1 - myTarget.GameTime.solarTime)).ToString(), wrapStyle);
                            GUILayout.Space(5);
                            EditorGUILayout.LabelField("Current scene exposure: " + myTarget.lightSettings.exposure.ToString(), wrapStyle);
                            EditorGUILayout.LabelField("Current sky exposure: " + myTarget.lightSettings.skyExposure.ToString(), wrapStyle);
                        } 
#else
                        EditorGUILayout.PropertyField(directionalLightMode, true, null);
                        EditorGUILayout.PropertyField(lightColorGradient, true, null);
                        EditorGUILayout.PropertyField(lightIntensityCurveSun, true, null);
                        EditorGUILayout.PropertyField(lightIntensityCurveMoon, true, null);
                        EditorGUILayout.PropertyField(shadowStrength, true, null);
                        EditorGUILayout.PropertyField(lightIntensityTransitionSpeed, true, null);
#endif
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField("Misc", headerStyle);
                        EditorGUILayout.PropertyField(angleOffset, true, null);
                        EditorGUILayout.PropertyField(stopRotationAtHigh, true, null);
                        if (myTarget.lightSettings.stopRotationAtHigh)
                            EditorGUILayout.PropertyField(rotationStopHigh, true, null);
#if ENVIRO_HDRP
#else
                        EditorGUILayout.PropertyField(ambientMode, true, null);
                        EditorGUILayout.PropertyField(ambientIntensityCurve, true, null);
                        EditorGUILayout.PropertyField(ambientSkyGradient, true, null);
                        EditorGUILayout.PropertyField(ambientEquatorGradient, true, null);
                        EditorGUILayout.PropertyField(ambientGroundGradient, true, null);
#endif
                        ApplyChanges();
                        GUILayout.EndVertical();
                        break;
                    case EnviroProfile.settingsMode.Sky:
                        GUILayout.BeginVertical("", boxStyleModified);
                        EditorGUI.BeginChangeCheck();
#if !ENVIRO_HDRP
                        EditorGUILayout.PropertyField(skyboxMode, true, null);
                        if (myTarget.skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.CustomSkybox)
                            EditorGUILayout.PropertyField(customSkyboxMaterial, true, null);
                        if (myTarget.skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.CustomColor)
                            EditorGUILayout.PropertyField(customSkyboxColor, true, null);
                        if (myTarget.skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.Default)
                            EditorGUILayout.PropertyField(blackGroundMode, true, null);
                        GUILayout.Space(10);
#else
                        if (myTarget.skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.Default)
                            EditorGUILayout.PropertyField(blackGroundMode, true, null);

                        EditorGUILayout.PropertyField(setEnviroSkybox, true, null);   
                        GUILayout.Space(10);                     
#endif
                        if (myTarget.skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.Default)
                        {
                            EditorGUILayout.LabelField("Scattering", headerStyle);
                            myTarget.skySettings.waveLength = EditorGUILayout.Vector3Field("Wave Length", myTarget.skySettings.waveLength);
                            EditorGUILayout.PropertyField(rayleigh, true, null);
                            EditorGUILayout.PropertyField(g, true, null);
                            EditorGUILayout.PropertyField(mie, true, null);
                            EditorGUILayout.PropertyField(scatteringCurve, true, null);
                            EditorGUILayout.PropertyField(scatteringColor, true, null);
                        }
                        else if (myTarget.skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.Simple)
                        {
                            EditorGUILayout.LabelField("Color", headerStyle);
                            EditorGUILayout.PropertyField(simpleSkyColor, true, null);
                            EditorGUILayout.PropertyField(simpleHorizonColor, true, null);
                            EditorGUILayout.PropertyField(simpleHorizonBackColor, true, null);
                            EditorGUILayout.PropertyField(simpleSunColor, true, null);
                            EditorGUILayout.PropertyField(simpleGroundColor, true, null);
                        }

                        EditorGUILayout.PropertyField(sunMoonPos, true, null);

                        if (myTarget.skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.Default)
                        {
                            EditorGUILayout.PropertyField(sunIntensity, true, null);
                            EditorGUILayout.PropertyField(sunDiskScale, true, null);
                            EditorGUILayout.PropertyField(sunDiskIntensity, true, null);
                            EditorGUILayout.PropertyField(sunDiskColor, true, null);
                        }
                        else if (myTarget.skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.Simple)
                            EditorGUILayout.PropertyField(sunDiskSizeSimple, true, null);

                        EditorGUILayout.PropertyField(renderMoon, true, null);
                        EditorGUILayout.PropertyField(moonPhaseMode, true, null);
                        EditorGUILayout.PropertyField(moonTexture, true, null);
                        EditorGUILayout.PropertyField(moonGlowTexture, true, null);
                        EditorGUILayout.PropertyField(moonColor, true, null);
                        EditorGUILayout.PropertyField(moonSize, true, null);
                        EditorGUILayout.PropertyField(moonGlowSize, true, null);
                        EditorGUILayout.PropertyField(moonGlow, true, null);
                        EditorGUILayout.PropertyField(moonGlowColor, true, null);

                        if (myTarget.skySettings.moonPhaseMode == EnviroSkySettings.MoonPhases.Custom)
                        {
                            EditorGUILayout.PropertyField(currentMoonPhase, true, null);
                        }
                        if (myTarget.skySettings.skyboxMode == EnviroSkySettings.SkyboxModi.Default)
                        {
                            EditorGUILayout.PropertyField(skyLuminance, true, null);
                            EditorGUILayout.PropertyField(skyColorPower, true, null);
                            EditorGUILayout.PropertyField(skyExposure, true, null);
                            EditorGUILayout.PropertyField(skyDithering, true, null);
                        }
                        EditorGUILayout.PropertyField(starsCubemap, true, null);
                        EditorGUILayout.PropertyField(starsIntensity, true, null);
                        EditorGUILayout.PropertyField(starsTwinklingRate, true, null);

                        EditorGUILayout.PropertyField(galaxyCubeMap, true, null);
                        EditorGUILayout.PropertyField(galaxyIntensity, true, null);
                        ApplyChanges();
                        GUILayout.EndVertical();
                        break;
                    // CLouds Category
                    case EnviroProfile.settingsMode.Clouds:
                        EditorGUI.BeginChangeCheck();
                        GUILayout.BeginVertical("Volume Clouds", boxStyleModified);
                        GUILayout.Space(20);

                        ///
                        GUILayout.BeginVertical("Clouds Quality", boxStyleModified);

                        myTarget.PopulateCloudsQualityList();

                        if (myTarget.cloudsQualityList.Count > 0)
                        {
                            cloudsQualityPresetsNames = new string[myTarget.cloudsQualityList.Count];

                            for (int i = 0; i < myTarget.cloudsQualityList.Count; i++)
                            {
                                cloudsQualityPresetsNames[i] = myTarget.cloudsQualityList[i].name;
                            }

                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.BeginHorizontal();
                            GUILayout.Label("", headerStyle, GUILayout.MaxWidth(96));
                            myTarget.selectedCloudsQuality = EditorGUILayout.Popup(myTarget.selectedCloudsQuality, cloudsQualityPresetsNames);
                            EditorGUILayout.EndHorizontal();
                            if (EditorGUI.EndChangeCheck())
                            {
                                myTarget.ApplyVolumeCloudsQualityPreset(myTarget.cloudsQualityList[myTarget.selectedCloudsQuality]);
                            }
                            GUILayout.Space(5);

                            if (GUILayout.Button("Edit current quality preset"))
                            {
                                Selection.activeObject = myTarget.cloudsQualityList[myTarget.selectedCloudsQuality];
                            }
                            if (GUILayout.Button("Create new quality preset"))
                            {
                                EnviroCloudsQualityCreation.CreateNewEnviroCloudsQualityPreset();
                            }
                        }

                        EditorGUILayout.EndVertical();
#if !ENVIRO_HDRP
                        GUILayout.BeginVertical("Clouds Blending (WIP)", boxStyleModified);
                        GUILayout.Space(20);
                        EditorGUILayout.PropertyField(depthBlending, true, null);
                        EditorGUILayout.PropertyField(bilateralUpsampling, true, null);
                        EditorGUILayout.EndVertical();
#endif
                        GUILayout.BeginVertical("Clouds Optimizations (WIP)", boxStyleModified);
                        GUILayout.Space(20);
                        EditorGUILayout.LabelField("Use the HaltonSequence based raymarch offset to help with undersampling. Use this together with TAA to improve visuals and/or lower you raymarch steps in your clouds quality settings to improve performance.", wrapStyle);
                        EditorGUILayout.PropertyField(useHaltonRaymarchOffset, true, null);
                        if (myTarget.cloudsSettings.useHaltonRaymarchOffset)
                            EditorGUILayout.PropertyField(useLessSteps, true, null);

                        EditorGUILayout.EndVertical();

                        GUILayout.BeginVertical("Clouds Area", boxStyleModified);
                        GUILayout.Space(20);
                        EditorGUILayout.PropertyField(cloudsWorldScale, true, null);
                        EditorGUILayout.PropertyField(cloudsHeightMod, true, null);
                        EditorGUILayout.PropertyField(cloudsSkyFogHeightBlending, true, null);                        
                        EditorGUILayout.EndVertical();

                        GUILayout.BeginVertical("Clouds Lighting", boxStyleModified);
                        GUILayout.Space(20);
                        EditorGUILayout.PropertyField(attenuationClamp, true, null);
                        EditorGUILayout.PropertyField(hgPhase, true, null);
                        EditorGUILayout.PropertyField(silverLiningIntensity, true, null);
                        EditorGUILayout.PropertyField(silverLiningSpread, true, null);
                        EditorGUILayout.PropertyField(volumeCloudsColor, true, null);
                        EditorGUILayout.PropertyField(volumeCloudsMoonColor, true, null);
                        EditorGUILayout.PropertyField(volumeCloudsAmbientColor, true, null);
                        EditorGUILayout.PropertyField(LightIntensity, true, null);
                        EditorGUILayout.PropertyField(ambientLightIntensity, true, null);
                        EditorGUILayout.PropertyField(lightingVarianceTiling, true, null);
                        GUILayout.Space(5);
                        EditorGUILayout.PropertyField(cloudsExposure, true, null);
                        EditorGUILayout.EndVertical();

                        GUILayout.BeginVertical("Weather Map", boxStyleModified);
                        GUILayout.Space(20);
                        EditorGUILayout.PropertyField(weatherMapTiling, true, null);
                        EditorGUILayout.PropertyField(customWeatherMap, true, null);
                        myTarget.cloudsSettings.locationOffset = EditorGUILayout.Vector2Field("Location Offset", myTarget.cloudsSettings.locationOffset);
                        EditorGUILayout.PropertyField(weatherAnimSpeedScale, true, null);
                        GUILayout.Space(5);
                        showWeatherMap = GUILayout.Toggle(showWeatherMap, "Show Weather Map Preview", headerFoldout);

                        if (myTarget.weatherMap != null && showWeatherMap)
                            EditorGUI.DrawPreviewTexture(GUILayoutUtility.GetAspectRect(1f), myTarget.weatherMap);

                        EditorGUILayout.PropertyField(globalCloudCoverage, true, null);
                        EditorGUILayout.EndVertical();

#if !ENVIRO_HDRP
                        GUILayout.BeginVertical("Clouds Shadows", boxStyleModified);
                        GUILayout.Space(20);
                        EditorGUILayout.PropertyField(shadowIntensity, true, null);
                        EditorGUILayout.PropertyField(shadowCookieSize, true, null);
                        EditorGUILayout.EndVertical();
#endif
                        EditorGUILayout.EndVertical();

                        GUILayout.BeginVertical("2D Clouds", boxStyleModified);
                        GUILayout.Space(20);
                        EditorGUILayout.PropertyField(flatCloudsBaseTexture, true, null);
                        EditorGUILayout.PropertyField(flatCloudsBaseTextureTiling, true, null);
                        EditorGUILayout.PropertyField(flatCloudsDetailTexture, true, null);
                        EditorGUILayout.PropertyField(flatCloudsDetailTextureTiling, true, null);
                        EditorGUILayout.PropertyField(flatCloudsDirectLightColor, true, null);
                        EditorGUILayout.PropertyField(flatCloudsAmbientLightColor, true, null);
                        EditorGUILayout.PropertyField(flatCloudsAltitude, true, null);
                        EditorGUILayout.EndVertical();

                        GUILayout.BeginVertical("Wind Settings", boxStyleModified);
                        GUILayout.Space(20);
                        EditorGUILayout.PropertyField(useWindZoneDirection, true, null);
                        EditorGUILayout.PropertyField(windTimeScale, true, null);
                        EditorGUILayout.PropertyField(windIntensity, true, null);
                        EditorGUILayout.PropertyField(cloudsDetailWindIntensity, true, null);
                        EditorGUILayout.PropertyField(windUpwardsIntensity, true, null);
                        EditorGUILayout.PropertyField(cirrusWindIntensity, true, null);
                        if (useWindZoneDirection.boolValue == false)
                        {
                            EditorGUILayout.PropertyField(windDirectionX, true, null);
                            EditorGUILayout.PropertyField(windDirectionY, true, null);
                        }
                        EditorGUILayout.EndVertical();

                        GUILayout.BeginVertical("Particle Clouds", boxStyleModified);
                        GUILayout.Space(20);
                        EditorGUILayout.PropertyField(dualLayerParticleClouds, true, null);
                        EditorGUILayout.LabelField("Layer 1:", headerStyle);
                        EditorGUILayout.PropertyField(particleCloudsHeight1, true, null);
                        EditorGUILayout.PropertyField(particleCloudsColor1, true, null);
                        if (myTarget.cloudsSettings.dualLayerParticleClouds)
                        {
                            GUILayout.Space(10);
                            EditorGUILayout.LabelField("Layer 2", headerStyle);
                            EditorGUILayout.PropertyField(particleCloudsHeight2, true, null);
                            EditorGUILayout.PropertyField(particleCloudsColor2, true, null);
                        }
                        EditorGUILayout.EndVertical();

                        GUILayout.BeginVertical("Cirrus Clouds", boxStyleModified);
                        GUILayout.Space(20);
                        EditorGUILayout.PropertyField(cirrusCloudsTexture, true, null);
                        EditorGUILayout.PropertyField(cirrusCloudsColor, true, null);
                        EditorGUILayout.PropertyField(cirrusCloudsAltitude, true, null);
                        EditorGUILayout.EndVertical();

                        ApplyChanges();
                        break;

                    case EnviroProfile.settingsMode.Weather:
                        GUILayout.BeginVertical("", boxStyleModified);
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(useTag, true, null);
                        EditorGUILayout.PropertyField(wetnessAccumulationSpeed, true, null);
                        EditorGUILayout.PropertyField(wetnessDryingSpeed, true, null);
                        EditorGUILayout.PropertyField(snowAccumulationSpeed, true, null);
                        EditorGUILayout.PropertyField(snowMeltingSpeed, true, null);
                        EditorGUILayout.PropertyField(snowMeltingTresholdTemperature, true, null);
                        GUILayout.Space(10);
                        EditorGUILayout.PropertyField(cloudTransitionSpeed, true, null);
                        EditorGUILayout.PropertyField(fogTransitionSpeed, true, null);
                        EditorGUILayout.PropertyField(windIntensityTransitionSpeed, true, null);
                        EditorGUILayout.PropertyField(effectTransitionSpeed, true, null);
                        EditorGUILayout.PropertyField(audioTransitionSpeed, true, null);
                        EditorGUILayout.PropertyField(lightningEffect, true, null);
                        EditorGUILayout.PropertyField(lightningRange, true, null);
                        EditorGUILayout.PropertyField(lightningHeight, true, null);
                        EditorGUILayout.PropertyField(temperatureChangingSpeed, true, null);
                        ApplyChanges();
                        GUILayout.EndVertical();
                        break;

                    case EnviroProfile.settingsMode.Reflections:
                        GUILayout.BeginVertical("", boxStyleModified);
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(reflectionBool, true, null);
                        if (myTarget.reflectionSettings.globalReflections)
                        {                     
#if !ENVIRO_HDRP
                            EditorGUILayout.PropertyField(updateDefaultEnvironmentReflections, true, null);    
                            GUILayout.Space(5);     
                            EditorGUILayout.PropertyField(globalReflectionClouds, true, null);
                            if (myTarget.reflectionSettings.globalReflectionCustomRendering)
                            {
                                EditorGUILayout.PropertyField(globalReflectionTimeSlicing, true, null);
                                EditorGUILayout.PropertyField(reflectionCloudsQuality, true, null);
                                EditorGUILayout.PropertyField(useFogInReflection, true, null);

                            }
#endif
                            GUILayout.Space(5);
                            EditorGUILayout.PropertyField(globalReflectionsUpdateOnGameTime, true, null);
                            if (myTarget.reflectionSettings.globalReflectionsUpdateOnGameTime)
                                EditorGUILayout.PropertyField(reflectionUpdate, true, null);
                            EditorGUILayout.PropertyField(globalReflectionsUpdateOnPosition, true, null);
                            if (myTarget.reflectionSettings.globalReflectionsUpdateOnPosition)
                                EditorGUILayout.PropertyField(reflectionUpdatePos, true, null);
                            GUILayout.Space(5);
                            EditorGUILayout.PropertyField(reflectionIntensity, true, null);
                            EditorGUILayout.PropertyField(globalReflectionsScale, true, null);
#if !ENVIRO_HDRP
                            EditorGUILayout.PropertyField(globalReflectionResolution, true, null);
#endif
                            EditorGUILayout.PropertyField(globalReflectionLayers, true, null);

                        }
                        ApplyChanges();
                        GUILayout.EndVertical();
                        break;

                    case EnviroProfile.settingsMode.Season:
                        GUILayout.BeginVertical("", boxStyleModified);
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(SpringStart, true, null);
                        EditorGUILayout.PropertyField(SpringEnd, true, null);
                        EditorGUILayout.PropertyField(springBaseTemperature, true, null);
                        EditorGUILayout.PropertyField(SummerStart, true, null);
                        EditorGUILayout.PropertyField(SummerEnd, true, null);
                        EditorGUILayout.PropertyField(summerBaseTemperature, true, null);
                        EditorGUILayout.PropertyField(AutumnStart, true, null);
                        EditorGUILayout.PropertyField(AutumnEnd, true, null);
                        EditorGUILayout.PropertyField(autumnBaseTemperature, true, null);
                        EditorGUILayout.PropertyField(WinterStart, true, null);
                        EditorGUILayout.PropertyField(WinterEnd, true, null);
                        EditorGUILayout.PropertyField(winterBaseTemperature, true, null);
                        ApplyChanges();
                        GUILayout.EndVertical();
                        break;

                    case EnviroProfile.settingsMode.Fog:
                        GUILayout.BeginVertical("", boxStyleModified);
                        EditorGUI.BeginChangeCheck();
#if !ENVIRO_HDRP
                        EditorGUILayout.PropertyField(useSimpleFog, true, null);
                        EditorGUILayout.PropertyField(fogmode, true, null);
                        EditorGUILayout.PropertyField(distanceFog, true, null);
                        EditorGUILayout.PropertyField(startDistance, true, null);
                        EditorGUILayout.PropertyField(distanceFogIntensity, true, null);
                        EditorGUILayout.PropertyField(maximumFogIntensity, true, null);
                        EditorGUILayout.PropertyField(heightFog, true, null);
                        EditorGUILayout.PropertyField(height, true, null);
                        EditorGUILayout.PropertyField(heightFogIntensity, true, null);

                        if (!myTarget.fogSettings.useSimpleFog)
                        {
        
                            EditorGUILayout.PropertyField(noiseIntensity, true, null);
                            EditorGUILayout.PropertyField(noiseIntensityOffset, true, null);
                            EditorGUILayout.PropertyField(noiseScale, true, null);
                            GUILayout.Space(10);
                            EditorGUILayout.LabelField("Tonemapping", headerStyle);
                            EditorGUILayout.PropertyField(fogExposure, true, null);
                            GUILayout.Space(10);
                            EditorGUILayout.LabelField("Fog Scattering", headerStyle);
                            EditorGUILayout.PropertyField(fogMie, true, null);
                            EditorGUILayout.PropertyField(fogG, true, null);
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(simpleFogColor, true, null);
                        }

#else
                        GUILayout.BeginVertical("", boxStyleModified);
                        EditorGUILayout.PropertyField(useEnviroFog, true, null);
                        EditorGUILayout.PropertyField(useHDRPFog, true, null);
                       
                        GUILayout.EndVertical();
                         GUILayout.Space(10);
                        if (myTarget.fogSettings.useEnviroGroundFog)
                        {
                            GUILayout.BeginVertical("", boxStyleModified);
                            EditorGUILayout.LabelField("Enviro Ground Fog", headerStyle);
                            EditorGUILayout.PropertyField(fogmode, true, null);
                            EditorGUILayout.PropertyField(distanceFog, true, null);
                            EditorGUILayout.PropertyField(startDistance, true, null);
                            EditorGUILayout.PropertyField(distanceFogIntensity, true, null);
                            EditorGUILayout.PropertyField(maximumFogIntensity, true, null);
                            EditorGUILayout.PropertyField(heightFog, true, null);
                            EditorGUILayout.PropertyField(height, true, null);
                            EditorGUILayout.PropertyField(heightFogIntensity, true, null);
                            GUILayout.EndVertical();
                        }

                        if (myTarget.fogSettings.useHDRPFog)
                        {
                            
                            GUILayout.BeginVertical("", boxStyleModified);
                            EditorGUILayout.LabelField("HDRP Fog", headerStyle);
                            EditorGUILayout.PropertyField(fogColorTint, true, null);  
                            GUILayout.EndVertical();                     
                        }
#endif
                        GUILayout.BeginVertical("", boxStyleModified);
                        EditorGUILayout.PropertyField(fogDithering, true, null);
                        GUILayout.EndVertical();       
                        ApplyChanges();
                        GUILayout.EndVertical();
                        break;


                    case EnviroProfile.settingsMode.VolumeLighting:
                        GUILayout.BeginVertical("", boxStyleModified);
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(VolumeLightingResolution, true, null);
                        EditorGUILayout.LabelField("Directional Volume Lighting", headerStyle);
                        EditorGUILayout.PropertyField(volumeLighting, true, null);
                        EditorGUILayout.PropertyField(SampleCount, true, null);
                        EditorGUILayout.PropertyField(ScatteringCoef, true, null);
                        EditorGUILayout.PropertyField(ExtinctionCoef, true, null);
                        EditorGUILayout.PropertyField(Anistropy, true, null);
                        EditorGUILayout.PropertyField(MaxRayLength, true, null);
                        EditorGUILayout.PropertyField(useNoise, true, null);
                        EditorGUILayout.PropertyField(volumeNoiseIntensity, true, null);
                        EditorGUILayout.PropertyField(volumeNoiseIntensityOffset, true, null);
                        EditorGUILayout.PropertyField(volumeNoiseScale, true, null);
                        //myTarget.volumeLightSettings.noiseVelocity = EditorGUILayout.Vector2Field("Noise Velocity", myTarget.volumeLightSettings.noiseVelocity);
                        ApplyChanges();
                        GUILayout.EndVertical();
                        break;


                    case EnviroProfile.settingsMode.Aurora:
                        GUILayout.BeginVertical("", boxStyleModified);
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(auroraIntensity, true, null);
                        EditorGUILayout.PropertyField(auroraColor, true, null);
                        EditorGUILayout.PropertyField(auroraBrightness, true, null);
                        EditorGUILayout.PropertyField(auroraContrast, true, null);
                        EditorGUILayout.PropertyField(auroraHeight, true, null);
                        EditorGUILayout.PropertyField(auroraScale, true, null);
                        EditorGUILayout.PropertyField(auroraSteps, true, null);
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField("Aurora Modelling and Animation Settings", headerStyle);
                        myTarget.auroraSettings.auroraLayer1Settings = EditorGUILayout.Vector4Field("Aurora Layer 1 Settings", myTarget.auroraSettings.auroraLayer1Settings);
                        myTarget.auroraSettings.auroraLayer2Settings = EditorGUILayout.Vector4Field("Aurora Layer 2 Settings", myTarget.auroraSettings.auroraLayer2Settings);
                        myTarget.auroraSettings.auroraColorshiftSettings = EditorGUILayout.Vector4Field("Aurora Colorshift Settings", myTarget.auroraSettings.auroraColorshiftSettings);
                        EditorGUILayout.PropertyField(auroraSpeed, true, null);
                        ApplyChanges();
                        GUILayout.EndVertical();
                        break;


                    case EnviroProfile.settingsMode.Lightshafts:
                        GUILayout.BeginVertical("", boxStyleModified);
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(resolution, true, null);
                        EditorGUILayout.PropertyField(screenBlendMode, true, null);
                        EditorGUILayout.PropertyField(useDepthTexture, true, null);
                        EditorGUILayout.PropertyField(lightShaftsColorSun, true, null);
                        EditorGUILayout.PropertyField(lightShaftsColorMoon, true, null);
                        EditorGUILayout.PropertyField(treshholdColorSun, true, null);
                        EditorGUILayout.PropertyField(treshholdColorMoon, true, null);
                        EditorGUILayout.PropertyField(blurRadius, true, null);
                        EditorGUILayout.PropertyField(shaftsIntensity, true, null);
                        EditorGUILayout.PropertyField(maxRadius, true, null);
                        ApplyChanges();
                        GUILayout.EndVertical();
                        break;

                    case EnviroProfile.settingsMode.DistanceBlur:
                        GUILayout.BeginVertical("", boxStyleModified);
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(antiFlicker, true, null);
                        EditorGUILayout.PropertyField(highQuality, true, null);
                        EditorGUILayout.PropertyField(radius, true, null);
                        ApplyChanges();
                        GUILayout.EndVertical();
                        break;

                    case EnviroProfile.settingsMode.Audio:
                        GUILayout.BeginVertical("", boxStyleModified);
                        myTarget.Audio.SFXHolderPrefab = (GameObject)EditorGUILayout.ObjectField("SFX Prefab:", myTarget.Audio.SFXHolderPrefab, typeof(GameObject), false);
                        serializedObject.Update();
                        thunderSFX.DoLayoutList();
                        serializedObject.ApplyModifiedProperties();
                        GUILayout.EndVertical();
                        break;

                    case EnviroProfile.settingsMode.Satellites:
                        GUILayout.BeginVertical(" Layer Setup", boxStyleModified);
                        GUILayout.Space(20);
                        if (GUILayout.Button("Add Satellite"))
                        {
                            myTarget.satelliteSettings.additionalSatellites.Add(new EnviroSatellite());
                        }

                        if (GUILayout.Button("Apply Changes"))
                        {
                            myTarget.CheckSatellites();
                        }
                        for (int i = 0; i < myTarget.satelliteSettings.additionalSatellites.Count; i++)
                        {
                            GUILayout.BeginVertical("", boxStyle);
                            GUILayout.Space(10);
                            myTarget.satelliteSettings.additionalSatellites[i].name = EditorGUILayout.TextField("Name", myTarget.satelliteSettings.additionalSatellites[i].name);
                            GUILayout.Space(10);
                            myTarget.satelliteSettings.additionalSatellites[i].prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", myTarget.satelliteSettings.additionalSatellites[i].prefab, typeof(GameObject), false);
                            myTarget.satelliteSettings.additionalSatellites[i].orbit = EditorGUILayout.Slider("OrbitDistance", myTarget.satelliteSettings.additionalSatellites[i].orbit, 0.01f, 1f);
                            myTarget.satelliteSettings.additionalSatellites[i].xRot = EditorGUILayout.Slider("XRot", myTarget.satelliteSettings.additionalSatellites[i].xRot, 0f, 360f);
                            myTarget.satelliteSettings.additionalSatellites[i].yRot = EditorGUILayout.Slider("YRot", myTarget.satelliteSettings.additionalSatellites[i].yRot, 0f, 360f);
                            if (GUILayout.Button("Remove"))
                            {
                                myTarget.satelliteSettings.additionalSatellites.Remove(myTarget.satelliteSettings.additionalSatellites[i]);
                                myTarget.CheckSatellites();
                            }
                            GUILayout.EndVertical();
                        }
                        serializedObj.Update();
                        GUILayout.EndVertical();
                        break;

                    case EnviroProfile.settingsMode.Quality:
                        GUILayout.BeginVertical("", boxStyleModified);
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(effectQuality, true, null);
                        EditorGUILayout.PropertyField(updateInterval, true, null);
                        ApplyChanges();
                        GUILayout.EndVertical();
                        break;
                }

            }
            GUILayout.EndVertical();
            //EditorGUILayout.EndToggleGroup ();
        }
        GUILayout.EndVertical();

        if (myTarget.profile != null)
        {
            EditorGUI.BeginChangeCheck();
            // Begin Setup
            GUILayout.BeginVertical("", boxStyle);
            // Player Setup
            if ((myTarget.Player == null || myTarget.PlayerCamera == null) && !myTarget.AssignInRuntime)
                GUI.backgroundColor = modifiedColor;
            else if ((myTarget.Player != null && myTarget.PlayerCamera != null) || myTarget.AssignInRuntime)
                GUI.backgroundColor = greenColor;

            GUILayout.BeginVertical("", boxStyleModified);
            GUI.backgroundColor = Color.white;
            // myTarget.profile.showPlayerSetup = EditorGUILayout.BeginToggleGroup ("Player & Camera Setup", myTarget.profile.showPlayerSetup);
            myTarget.profile.showPlayerSetup = GUILayout.Toggle(myTarget.profile.showPlayerSetup, "Player & Camera Setup", headerFoldout);
            if (myTarget.profile.showPlayerSetup)
            {
                GUILayout.BeginVertical("", boxStyleModified);
                EditorGUILayout.PropertyField(Player, true, null);
                EditorGUILayout.PropertyField(Camera, true, null);
                EditorGUILayout.PropertyField(startMode, true, null);
                
#if ENVIRO_HDRP || ENVIRO_LWRP
                GUILayout.BeginVertical("Additional Cameras", boxStyleModified);
                GUILayout.Space(20);
                if (GUILayout.Button ("Add")) 
                {
                        myTarget.additionalCameras.Add (null);
                }
                 GUILayout.Space(5);
                for (int i = 0; i < myTarget.additionalCameras.Count; i++)
                {     
                    GUILayout.BeginVertical("", boxStyleModified);
                    myTarget.additionalCameras[i] = (Camera)EditorGUILayout.ObjectField ("Camera", myTarget.additionalCameras[i], typeof(Camera), true);
                    if (GUILayout.Button ("Remove")) 
                    {
                        myTarget.additionalCameras.RemoveAt (i);
                    }
                       
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();
#endif

                GUILayout.EndVertical();
                GUILayout.BeginVertical("", boxStyleModified);
                AssignOnRuntime.boolValue = EditorGUILayout.BeginToggleGroup("Assign On Runtime", AssignOnRuntime.boolValue);
                PlayerTag.stringValue = EditorGUILayout.TagField("Player Tag", PlayerTag.stringValue);
                CameraTag.stringValue = EditorGUILayout.TagField("Camera Tag", CameraTag.stringValue);
                EditorGUILayout.EndToggleGroup();
                GUILayout.EndVertical();
            }
            //EditorGUILayout.EndToggleGroup ();
            GUILayout.EndVertical();


            /// Render Setup                     
             if(myTarget.tonemapping)
            GUI.backgroundColor = modifiedColor;
             else
            GUI.backgroundColor = boxColor1;
            GUILayout.BeginVertical("", boxStyleModified);
            GUI.backgroundColor = Color.white;
            //myTarget.profile.showRenderingSetup = EditorGUILayout.BeginToggleGroup ("Rendering Setup", myTarget.profile.showRenderingSetup);
            myTarget.profile.showRenderingSetup = GUILayout.Toggle(myTarget.profile.showRenderingSetup, "Rendering Setup", headerFoldout);
            if (myTarget.profile.showRenderingSetup)
            {
                GUILayout.BeginVertical("", boxStyleModified);
                EditorGUILayout.LabelField("Tonemapping Settings", headerStyle, null);
                EditorGUILayout.PropertyField(tonemapping, true, null);
                if (myTarget.tonemapping)
                {
                    GUILayout.BeginVertical("", boxStyleModified);
                    EditorGUILayout.LabelField("Please consider to deactivate tonemapping option here and use post processing tonemapping instead for best results!", wrapStyle, null);
                    GUILayout.EndVertical();
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical("", boxStyleModified);
                EditorGUILayout.LabelField("Camera Settings", headerStyle, null);
                EditorGUILayout.PropertyField(setCameraClearFlags, true, null);
                GUILayout.EndVertical();

                GUILayout.BeginVertical("", boxStyleModified);
                EditorGUILayout.LabelField("Layer Setup", headerStyle, null);
                SatTag.intValue = EditorGUILayout.LayerField("Satellites Layers", SatTag.intValue);
                GUILayout.EndVertical();

                GUILayout.BeginVertical("", boxStyleModified);
                EditorGUILayout.LabelField("Floating Point Origin", headerStyle, null);
                EditorGUILayout.PropertyField(floatingPointOriginAnchor, true, null);
                GUILayout.EndVertical();

                GUILayout.BeginVertical("", boxStyleModified);
                EditorGUILayout.LabelField("Virtual Reality", headerStyle, null);
                EditorGUILayout.PropertyField(singlePassInstancedVR, true, null);
                GUILayout.EndVertical();
            }
            //EditorGUILayout.EndToggleGroup ();
            GUILayout.EndVertical();

            /// Components Setup
            GUI.backgroundColor = boxColor1;
            GUILayout.BeginVertical("", boxStyleModified);
            GUI.backgroundColor = Color.white;
            //myTarget.profile.showComponentsSetup = EditorGUILayout.BeginToggleGroup ("Component Setup", myTarget.profile.showComponentsSetup);
            myTarget.profile.showComponentsSetup = GUILayout.Toggle(myTarget.profile.showComponentsSetup, "Component Setup", headerFoldout);
            if (myTarget.profile.showComponentsSetup)
            {
                GUILayout.BeginVertical("", boxStyleModified);
                EditorGUILayout.PropertyField(Sun, true, null);
                EditorGUILayout.PropertyField(Moon, true, null);
                EditorGUILayout.PropertyField(DirectLight, true, null);
                EditorGUILayout.PropertyField(AdditionalDirectLight, true, null);
                EditorGUILayout.PropertyField(LightningGenerator, true, null);
                EditorGUILayout.PropertyField(windZone, true, null);
                EditorGUILayout.PropertyField(GlobalReflectionProbe, true, null);
                EditorGUILayout.PropertyField(satellites, true, null);
                EditorGUILayout.PropertyField(starsRotation, true, null);
                EditorGUILayout.PropertyField(particleClouds, true, null);
                GUILayout.EndVertical();
            }
            //EditorGUILayout.EndToggleGroup ();
            GUILayout.EndVertical();
            GUILayout.EndVertical();



            ////////////
            // Begin Controls
            GUILayout.BeginVertical("", boxStyle);
            // Time Control
            GUI.backgroundColor = boxColor1;
            GUILayout.BeginVertical("", boxStyleModified);
            GUI.backgroundColor = Color.white;
            //myTarget.profile.showTimeUI = EditorGUILayout.BeginToggleGroup ("Time and Location Controls", myTarget.profile.showTimeUI);
            myTarget.profile.showTimeUI = GUILayout.Toggle(myTarget.profile.showTimeUI, "Time and Location Controls", headerFoldout);
            if (myTarget.profile.showTimeUI)
            {
                //GUILayout.Space (20);
                GUILayout.BeginVertical("Time", boxStyleModified);
                GUILayout.Space(20);
                EditorGUILayout.PropertyField(ProgressMode, true, null);
                GUILayout.Space(20);
                EditorGUILayout.PropertyField(Seconds, true, null);
                EditorGUILayout.PropertyField(Minutes, true, null);
                EditorGUILayout.PropertyField(Hours, true, null);
                EditorGUILayout.PropertyField(Days, true, null);
                EditorGUILayout.PropertyField(Years, true, null);
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(DaysInYear, true, null);
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(cycleLenghtInMinutes, true, null);
                EditorGUILayout.PropertyField(DayLength, true, null);
                EditorGUILayout.PropertyField(NightLength, true, null);
                EditorGUILayout.PropertyField(dayNightSwitch, true, null);
                GUILayout.EndVertical();
                GUILayout.BeginVertical("Season", boxStyleModified);
                GUILayout.Space(20);
                EditorGUILayout.PropertyField(UpdateSeason, true, null);
                EditorGUILayout.PropertyField(CurrentSeason, true, null);
                GUILayout.EndVertical();
                GUILayout.BeginVertical("Location", boxStyleModified);
                GUILayout.Space(20);
                EditorGUILayout.PropertyField(UTC, true, null);
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(Latitude, true, null);
                EditorGUILayout.PropertyField(Longitude, true, null);
                GUILayout.EndVertical();
            }
            //EditorGUILayout.EndToggleGroup ();
            GUILayout.EndVertical();
            // Time End
            // Weather Control
            GUI.backgroundColor = boxColor1;
            GUILayout.BeginVertical("", boxStyleModified);
            GUI.backgroundColor = Color.white;
            //myTarget.profile.showWeatherUI = EditorGUILayout.BeginToggleGroup ("Weather Controls", myTarget.profile.showWeatherUI);
            myTarget.profile.showWeatherUI = GUILayout.Toggle(myTarget.profile.showWeatherUI, "Weather Controls", headerFoldout);
            if (myTarget.profile.showWeatherUI)
            {

                GUILayout.BeginVertical("Weather", boxStyleModified);
                GUILayout.Space(20);
                EditorGUILayout.PropertyField(UpdateWeather, true, null);
                EditorGUILayout.PropertyField(StartWeather, true, null);
                GUILayout.Space(15);
                if (Application.isPlaying)
                {
                    if (myTarget.Weather.weatherPresets.Count > 0)
                    {
                        GUIContent[] zonePrefabs = new GUIContent[myTarget.Weather.weatherPresets.Count];
                        for (int idx = 0; idx < zonePrefabs.Length; idx++)
                        {
                            zonePrefabs[idx] = new GUIContent(myTarget.Weather.weatherPresets[idx].Name);
                        }
                        int weatherID = EditorGUILayout.Popup(new GUIContent("Current Weather"), myTarget.GetActiveWeatherID(), zonePrefabs);
                        myTarget.ChangeWeather(weatherID);
                    }
                }
                else
                    EditorGUILayout.LabelField("Weather can only be changed in runtime!");

                if (GUILayout.Button("Edit current Weather Preset"))
                {
                    if (myTarget.Weather.currentActiveWeatherPreset != null)
                        Selection.activeObject = myTarget.Weather.currentActiveWeatherPreset;
                    else if (myTarget.Weather.startWeatherPreset != null)
                        Selection.activeObject = myTarget.Weather.startWeatherPreset;
                }
                GUILayout.EndVertical();
                GUILayout.BeginVertical("Zones", boxStyleModified);
                GUILayout.Space(20);
                myTarget.Weather.currentActiveZone = (EnviroZone)EditorGUILayout.ObjectField("Current Zone", myTarget.Weather.currentActiveZone, typeof(EnviroZone), true);
                GUILayout.EndVertical();
            }
            //EditorGUILayout.EndToggleGroup ();
            GUILayout.EndVertical();
            // Weather End
            // Effects Control
            GUI.backgroundColor = boxColor1;
            GUILayout.BeginVertical("", boxStyleModified);
            GUI.backgroundColor = Color.white;
            //myTarget.profile.showEffectsUI = EditorGUILayout.BeginToggleGroup ("Feature Controls", myTarget.profile.showEffectsUI);
            myTarget.profile.showEffectsUI = GUILayout.Toggle(myTarget.profile.showEffectsUI, "Feature Controls", headerFoldout);
            if (myTarget.profile.showEffectsUI)
            {
                GUILayout.BeginVertical("Features", boxStyleModified);
                GUILayout.Space(20);
#if !ENVIRO_HDRP && !ENVIRO_LWRP
                EditorGUILayout.PropertyField(EnableVolumeLighting, true, null);
#endif
                EditorGUILayout.PropertyField(useVolumeClouds, true, null);
                EditorGUILayout.PropertyField(useFog, true, null);
                EditorGUILayout.PropertyField(useFlatClouds, true, null);
                EditorGUILayout.PropertyField(useParticleClouds, true, null);
                EditorGUILayout.PropertyField(useDistanceBlur, true, null);
                EditorGUILayout.PropertyField(useAurora, true, null);
                EditorGUILayout.PropertyField(EnableSunShafts, true, null);
                EditorGUILayout.PropertyField(EnableMoonShafts, true, null);
                GUILayout.EndVertical();
                GUILayout.BeginVertical("Scene View", boxStyleModified);
                GUILayout.Space(20);
#if !ENVIRO_HDRP && !ENVIRO_LWRP
                EditorGUILayout.PropertyField(showVolumeLightingEditor, true, null);
#endif
                EditorGUILayout.PropertyField(showVolumeCloudsEditor, true, null);
                EditorGUILayout.PropertyField(showFlatCloudsEditor, true, null);
                EditorGUILayout.PropertyField(showFogEditor, true, null);
#if !ENVIRO_HDRP
                EditorGUILayout.PropertyField(showDistanceBlurInEditor, true, null);
#endif
                GUILayout.EndVertical();
            }
            //EditorGUILayout.EndToggleGroup ();
            GUILayout.EndVertical();
            // Effects End
            // Audio Control
            GUI.backgroundColor = boxColor1;
            GUILayout.BeginVertical("", boxStyleModified);
            GUI.backgroundColor = Color.white;
            //myTarget.profile.showAudioUI = EditorGUILayout.BeginToggleGroup ("Audio Controls", myTarget.profile.showAudioUI);
            myTarget.profile.showAudioUI = GUILayout.Toggle(myTarget.profile.showAudioUI, "Audio Controls", headerFoldout);
            if (myTarget.profile.showAudioUI)
            {
                GUILayout.BeginVertical("", boxStyleModified);
                EditorGUILayout.PropertyField(AmbientVolume, true, null);
                EditorGUILayout.PropertyField(WeatherVolume, true, null);
                GUILayout.EndVertical();
            }
            //EditorGUILayout.EndToggleGroup ();
            GUILayout.EndVertical();
            // Audio End
            /////////////

            EditorGUILayout.EndVertical();
            if (EditorGUI.EndChangeCheck())
            {
                serializedObj.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }
        }
        else
        {
            GUILayout.BeginVertical("", boxStyle);
            EditorGUILayout.LabelField("No profile assigned!");
            if (GUILayout.Button("Create and assign new profile!"))
            {
                myTarget.profile = EnviroProfileCreation.CreateNewEnviroProfile();
                myTarget.ApplyProfile(myTarget.profile);
                myTarget.ReInit();
            }
            GUILayout.EndVertical();
        }


    }
#endif
}
