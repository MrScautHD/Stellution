using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;


[CustomEditor(typeof(EnviroWeatherPreset))]
public class EnviroWeatherPresetEditor : Editor {

	GUIStyle boxStyle;
    GUIStyle boxStyleModified;
    GUIStyle wrapStyle;
    GUIStyle wrapStyle2;
    GUIStyle clearStyle;
    GUIStyle headerFoldout;
    GUIStyle headerStyle;

    EnviroWeatherPreset myTarget;

	public bool showAudio = false;
	public bool showFog = false;
	public bool showSeason = false;
	public bool showClouds = false;
	public bool showGeneral = false;
    public bool showPostProcessing = false;
    public bool showThirdParty = false;

    SerializedObject serializedObj;
	SerializedProperty fogMod;
	SerializedProperty skyMod;
	SerializedProperty lightMod;

    private Color boxColor1;

    void OnEnable()
	{
		myTarget = (EnviroWeatherPreset)target;

		serializedObj = new SerializedObject (myTarget);
		fogMod = serializedObj.FindProperty ("weatherFogMod");
		skyMod = serializedObj.FindProperty ("weatherSkyMod");
		lightMod = serializedObj.FindProperty ("weatherLightMod");

#if UNITY_2019_3_OR_NEWER
        boxColor1 = new Color(0.95f, 0.95f, 0.95f,1f);
#else
        boxColor1 = new Color(0.85f, 0.85f, 0.85f, 1f);
#endif
    }

    public override void OnInspectorGUI ()
	{

		myTarget = (EnviroWeatherPreset)target;
		#if UNITY_5_6_OR_NEWER
		serializedObj.UpdateIfRequiredOrScript ();
		#else
		serializedObj.UpdateIfDirtyOrScript ();
		#endif
		//Set up the box style
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

        //Setup the wrap style
        if (wrapStyle == null)
		{
			wrapStyle = new GUIStyle(GUI.skin.label);
			wrapStyle.fontStyle = FontStyle.Normal;
			wrapStyle.wordWrap = true;
		}

        if (wrapStyle2 == null)
        {
            wrapStyle2 = new GUIStyle(GUI.skin.label);
            wrapStyle2.fontStyle = FontStyle.Italic;
            wrapStyle2.wordWrap = true;
        }

        if (clearStyle == null) {
			clearStyle = new GUIStyle(GUI.skin.label);
			clearStyle.normal.textColor = GUI.skin.label.normal.textColor;
			clearStyle.fontStyle = FontStyle.Bold;
			clearStyle.alignment = TextAnchor.UpperRight;
		}

        if (headerFoldout == null)
        {
            headerFoldout = new GUIStyle(EditorStyles.foldout);
            headerFoldout.fontStyle = FontStyle.Bold;
        }

        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.alignment = TextAnchor.UpperLeft;
        }

        // Begin
        GUILayout.BeginVertical("", boxStyle);
        EditorGUILayout.LabelField("Enviro - Weather Preset", headerStyle);
        EditorGUILayout.LabelField("Setup a new weather type here. Change clouds, lighting, audio and so on and add this preset to one of your enviro weather zones to make it usable in enviro weather system. Do not forget to set a unique name in 'General Configs'!", wrapStyle);
        GUILayout.Space(10);

        // General Setup
        GUI.backgroundColor = boxColor1;
        GUILayout.BeginVertical("", boxStyleModified);
        GUI.backgroundColor = Color.white;
        showGeneral = GUILayout.Toggle(showGeneral, "General Configs", headerFoldout);
        if (showGeneral) {
            EditorGUI.BeginChangeCheck();
            GUILayout.BeginVertical("Unique Name", boxStyleModified);
            GUILayout.Space(20);
            myTarget.Name = EditorGUILayout.TextField("Name", myTarget.Name);
            EditorGUILayout.EndVertical();
            GUILayout.BeginVertical("Sky and Light Color", boxStyleModified);
			GUILayout.Space(15);		
			EditorGUILayout.PropertyField(skyMod, true, null);


			EditorGUILayout.PropertyField(lightMod, true, null);

            myTarget.volumeLightIntensity = EditorGUILayout.Slider("Volume Light Intensity", myTarget.volumeLightIntensity, 0, 2);

            myTarget.shadowIntensityMod = EditorGUILayout.Slider("Shadow Intensity Mod", myTarget.shadowIntensityMod, -1f, 1f);
#if ENVIRO_HDRP
            if (EnviroSkyMgr.instance == null || EnviroSkyMgr.instance.LightSettings.usePhysicalBasedLighting)
            {     
                myTarget.sceneExposureMod = EditorGUILayout.Slider("Scene Exposure Modification", myTarget.sceneExposureMod, 0, 2);
                myTarget.skyExposureMod = EditorGUILayout.Slider("Sky Exposure Modification", myTarget.skyExposureMod, 0, 2);
                myTarget.lightIntensityMod = EditorGUILayout.Slider("Light Intensity Modification", myTarget.lightIntensityMod, 0, 2);
            }
#endif
            if (EditorGUI.EndChangeCheck())
			{
				serializedObj.ApplyModifiedProperties();
			}
			EditorGUILayout.EndVertical ();
			GUILayout.BeginVertical("Weather Condition", boxStyleModified);
			GUILayout.Space(15);
			myTarget.WindStrenght = EditorGUILayout.Slider("Wind Intensity",myTarget.WindStrenght,0f,1f);
			myTarget.wetnessLevel = EditorGUILayout.Slider("Maximum Wetness",myTarget.wetnessLevel,0f,1f);
			myTarget.snowLevel = EditorGUILayout.Slider("Maximum Snow",myTarget.snowLevel,0f,1f);
            myTarget.temperatureLevel = EditorGUILayout.Slider("Temperature Modification", myTarget.temperatureLevel, -50f, 50f);
            myTarget.isLightningStorm = EditorGUILayout.Toggle ("Lightning Storm", myTarget.isLightningStorm);
			myTarget.lightningInterval = EditorGUILayout.Slider("Lightning Interval",myTarget.lightningInterval,2f,60f);
			EditorGUILayout.EndVertical ();
			GUILayout.BeginVertical("Particle Effects", boxStyleModified);
			GUILayout.Space(15);
			if (!Application.isPlaying) {
				if (GUILayout.Button ("Add")) {
					myTarget.effectSystems.Add (new EnviroWeatherEffects ());
				}
			} else
				EditorGUILayout.LabelField ("Can't add effects in runtime!");

			for (int i = 0; i < myTarget.effectSystems.Count; i++) {
				GUILayout.BeginVertical ("Effect " + (i+1), boxStyleModified);
				GUILayout.Space(15);
				myTarget.effectSystems[i].prefab = (GameObject)EditorGUILayout.ObjectField ("Effect Prefab", myTarget.effectSystems[i].prefab, typeof(GameObject), true);
				myTarget.effectSystems [i].localPositionOffset = EditorGUILayout.Vector3Field ("Position Offset", myTarget.effectSystems [i].localPositionOffset);
				myTarget.effectSystems [i].localRotationOffset = EditorGUILayout.Vector3Field ("Rotation Offset", myTarget.effectSystems [i].localRotationOffset);
				if (GUILayout.Button ("Remove")) {
					myTarget.effectSystems.Remove (myTarget.effectSystems[i]);
				}
				GUILayout.EndVertical ();
			}

            EditorGUILayout.EndVertical();

#if ENVIRO_HD
            if (EnviroSkyMgr.instance == null || EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
            {               
                GUILayout.BeginVertical("Aurora Settings", boxStyleModified);
                GUILayout.Space(15);
                myTarget.auroraIntensity = EditorGUILayout.Slider("Aurora Intensity", myTarget.auroraIntensity, 0f, 1f);
                EditorGUILayout.EndVertical();
            }
#endif



		}
		EditorGUILayout.EndVertical ();


        // Season Setup
        GUI.backgroundColor = boxColor1;
        GUILayout.BeginVertical("", boxStyleModified);
        GUI.backgroundColor = Color.white;
        showSeason = GUILayout.Toggle(showSeason, "Season Configs", headerFoldout);
        if (showSeason) {
            GUILayout.BeginVertical("", boxStyleModified);
            myTarget.Spring = EditorGUILayout.Toggle ("Spring",myTarget.Spring);
			if (myTarget.Spring)
				myTarget.possibiltyInSpring = EditorGUILayout.Slider ("Spring Possibility",myTarget.possibiltyInSpring, 0, 100);
			myTarget.Summer = EditorGUILayout.Toggle ("Summer",myTarget.Summer);
			if (myTarget.Summer)
				myTarget.possibiltyInSummer = EditorGUILayout.Slider ("Summer Possibility",myTarget.possibiltyInSummer, 0, 100);
			myTarget.Autumn = EditorGUILayout.Toggle ("Autumn",myTarget.Autumn);
			if (myTarget.Autumn)
				myTarget.possibiltyInAutumn = EditorGUILayout.Slider ("Autumn Possibility",myTarget.possibiltyInAutumn, 0, 100);
			myTarget.winter = EditorGUILayout.Toggle ("Winter",myTarget.winter);
			if (myTarget.winter)
				myTarget.possibiltyInWinter = EditorGUILayout.Slider ("Winter Possibility",myTarget.possibiltyInWinter, 0, 100);
            EditorGUILayout.EndVertical();
        }
		EditorGUILayout.EndVertical ();



        // Clouds Setup
        GUI.backgroundColor = boxColor1;
        GUILayout.BeginVertical("", boxStyleModified);
        GUI.backgroundColor = Color.white;
        showClouds = GUILayout.Toggle(showClouds, "Clouds Configs", headerFoldout);
		if (showClouds) {
            //Add Cloud Stuff
#if ENVIRO_HD
            if (EnviroSkyMgr.instance == null || EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
            { 
                GUILayout.BeginVertical("Volume Clouds", boxStyleModified);
                GUILayout.Space(20);
                myTarget.cloudsConfig.scatteringCoef = EditorGUILayout.Slider("Direct Light Intensity", myTarget.cloudsConfig.scatteringCoef, 0f, 2f);
                myTarget.cloudsConfig.ambientSkyColorIntensity = EditorGUILayout.Slider("Ambient Color Intensity", myTarget.cloudsConfig.ambientSkyColorIntensity, 0f, 2f);
                GUILayout.Space(10);
                myTarget.cloudsConfig.edgeDarkness = EditorGUILayout.Slider("Powder Term Intensity", myTarget.cloudsConfig.edgeDarkness, 0.4f, 0.99f);
                myTarget.cloudsConfig.lightAbsorbtion = EditorGUILayout.Slider("Light Absorbtion", myTarget.cloudsConfig.lightAbsorbtion, 0.0f, 1f);                
                myTarget.cloudsConfig.lightStepModifier = EditorGUILayout.Slider("Light Step Modifier", myTarget.cloudsConfig.lightStepModifier, 0.01f, 2.0f);
                myTarget.cloudsConfig.lightVariance = EditorGUILayout.Slider("Light Variance Intensity", myTarget.cloudsConfig.lightVariance, 0.0f, 1.0f); 
                GUILayout.Space(10);
                myTarget.cloudsConfig.density = EditorGUILayout.Slider("Density", myTarget.cloudsConfig.density, 0.1f, 4f);               
                GUILayout.Space(10);
                myTarget.cloudsConfig.baseErosionIntensity = EditorGUILayout.Slider("Base Erosion Intensity", myTarget.cloudsConfig.baseErosionIntensity, 0f, 1f);
                myTarget.cloudsConfig.detailErosionIntensity = EditorGUILayout.Slider("Detail Erosion Intensity", myTarget.cloudsConfig.detailErosionIntensity, 0f, 1f);
                GUILayout.Space(10);
                myTarget.cloudsConfig.coverage = EditorGUILayout.Slider("Coverage", myTarget.cloudsConfig.coverage, -1f, 1f);            
                myTarget.cloudsConfig.coverageType = EditorGUILayout.Slider("Coverage Type", myTarget.cloudsConfig.coverageType, 0f, 1f);             
                myTarget.cloudsConfig.cloudType = EditorGUILayout.Slider("Cloud Type", myTarget.cloudsConfig.cloudType, 0f, 1f);
                GUILayout.Space(10);              
                myTarget.cloudsConfig.raymarchingScale = EditorGUILayout.Slider("Raymarch Step Modifier", myTarget.cloudsConfig.raymarchingScale, 0.25f, 2f);
                EditorGUILayout.EndVertical();
            }
#endif
                GUILayout.BeginVertical("Flat Clouds", boxStyleModified);
                GUILayout.Space(20);
                myTarget.cloudsConfig.flatCoverage = EditorGUILayout.Slider("Flat Clouds Coverage", myTarget.cloudsConfig.flatCoverage, 0f, 2f);
                GUILayout.Space(5);
                myTarget.cloudsConfig.flatCloudsDensity = EditorGUILayout.Slider("Flat Clouds Density", myTarget.cloudsConfig.flatCloudsDensity, 0f, 5f);
                myTarget.cloudsConfig.flatCloudsAbsorbtion = EditorGUILayout.Slider("Flat Clouds Absorbtion", myTarget.cloudsConfig.flatCloudsAbsorbtion, 0f, 10f);
                myTarget.cloudsConfig.flatCloudsDirectLightIntensity = EditorGUILayout.Slider("Flat Clouds Direct Light Intensity", myTarget.cloudsConfig.flatCloudsDirectLightIntensity, 0f, 100f);
                myTarget.cloudsConfig.flatCloudsAmbientLightIntensity = EditorGUILayout.Slider("Flat Clouds Ambient Light Intensity", myTarget.cloudsConfig.flatCloudsAmbientLightIntensity, 0.0f, 2f);
                myTarget.cloudsConfig.flatCloudsHGPhase = EditorGUILayout.Slider("Flat Clouds HG Phase", myTarget.cloudsConfig.flatCloudsHGPhase, 0.0f, 1f);
                EditorGUILayout.EndVertical();
           

                GUILayout.BeginVertical("Particle Clouds", boxStyleModified);
                GUILayout.Space(20);
#if ENVIRO_HD
            if (EnviroSkyMgr.instance == null || EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
            {
                myTarget.cloudsConfig.particleCloudsOverwrite = EditorGUILayout.Toggle("Enable Particle Clouds Overwrite", myTarget.cloudsConfig.particleCloudsOverwrite);
                GUILayout.Space(10);
            }
#endif
                myTarget.cloudsConfig.particleLayer1Alpha = EditorGUILayout.Slider("Layer 1 Alpha", myTarget.cloudsConfig.particleLayer1Alpha, 0f, 1f);
                myTarget.cloudsConfig.particleLayer1Brightness = EditorGUILayout.Slider("Layer 1 Brightness", myTarget.cloudsConfig.particleLayer1Brightness, 0f, 1f);
                myTarget.cloudsConfig.particleLayer1ColorPow = EditorGUILayout.Slider("Layer 1 Color Power", myTarget.cloudsConfig.particleLayer1ColorPow, 0.1f, 5f);
                GUILayout.Space(20);
                myTarget.cloudsConfig.particleLayer2Alpha = EditorGUILayout.Slider("Layer 2 Alpha", myTarget.cloudsConfig.particleLayer2Alpha, 0f, 1f);
                myTarget.cloudsConfig.particleLayer2Brightness = EditorGUILayout.Slider("Layer 2 Brightness", myTarget.cloudsConfig.particleLayer2Brightness, 0f, 1f);
                myTarget.cloudsConfig.particleLayer2ColorPow = EditorGUILayout.Slider("Layer 2 Color Power", myTarget.cloudsConfig.particleLayer2ColorPow, 0.1f, 5f);
                EditorGUILayout.EndVertical();

            GUILayout.BeginVertical("Cirrus Clouds", boxStyleModified);
			GUILayout.Space(20);
			myTarget.cloudsConfig.cirrusAlpha = EditorGUILayout.Slider ("Cirrus Clouds Alpha", myTarget.cloudsConfig.cirrusAlpha,0f,1f);
			myTarget.cloudsConfig.cirrusCoverage = EditorGUILayout.Slider ("Cirrus Clouds Coverage", myTarget.cloudsConfig.cirrusCoverage,0f,1f);
			myTarget.cloudsConfig.cirrusColorPow = EditorGUILayout.Slider ("Cirrus Clouds Color Power", myTarget.cloudsConfig.cirrusColorPow,0.1f,5f);
			EditorGUILayout.EndVertical ();


		}
		EditorGUILayout.EndVertical ();
         
        // Fog Setup
        GUI.backgroundColor = boxColor1;
        GUILayout.BeginVertical("", boxStyleModified);
        GUI.backgroundColor = Color.white;
        showFog = GUILayout.Toggle(showFog, "Fog Configs", headerFoldout);
        if (showFog) {
			GUILayout.BeginVertical ("Fog Intensity", boxStyleModified);
			GUILayout.Space(15);
			GUILayout.BeginVertical ("Linear", boxStyleModified);
			GUILayout.Space(15);
			myTarget.fogStartDistance = EditorGUILayout.FloatField ("Start Distance", myTarget.fogStartDistance);
			myTarget.fogDistance = EditorGUILayout.FloatField ("End Distance", myTarget.fogDistance);
			EditorGUILayout.EndVertical ();
			GUILayout.BeginVertical ("EXP", boxStyleModified);
			GUILayout.Space(15);
			myTarget.fogDensity = EditorGUILayout.FloatField ("Density", myTarget.fogDensity);
			EditorGUILayout.EndVertical ();
			GUILayout.BeginVertical ("Height", boxStyleModified);
			GUILayout.Space(15);
			myTarget.heightFogDensity = EditorGUILayout.Slider ("Height Fog Density", myTarget.heightFogDensity,0f,10f);
			EditorGUILayout.EndVertical ();

#if ENVIRO_HDRP
            GUILayout.BeginVertical("HDRP", boxStyleModified);
            GUILayout.Space(15);
            myTarget.fogAttenuationDistance = EditorGUILayout.Slider("Attenuation Distance", myTarget.fogAttenuationDistance, 1f, 10000f);
            myTarget.fogRelativeFogHeight = EditorGUILayout.Slider("Relative Fog Height", myTarget.fogRelativeFogHeight, -1000f, 1000f);
            EditorGUILayout.EndVertical();
#endif

            EditorGUILayout.EndVertical ();


            GUILayout.BeginVertical ("Advanced", boxStyleModified);
			GUILayout.Space(15);
            GUILayout.BeginVertical("", boxStyleModified);
            EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(fogMod, true, null);
			if(EditorGUI.EndChangeCheck())
			{
				serializedObj.ApplyModifiedProperties();
			}
			myTarget.FogScatteringIntensity = EditorGUILayout.Slider ("Scattering Intensity", myTarget.FogScatteringIntensity,1f,10f);
			myTarget.fogSunBlocking = EditorGUILayout.Slider ("Sundisk Intensity", myTarget.fogSunBlocking,0f,1f);
            EditorGUILayout.EndVertical();
            GUILayout.BeginVertical ("Sky Fog", boxStyleModified);
			GUILayout.Space(15);
            myTarget.skyFogStart = EditorGUILayout.Slider("Sky Fog Start", myTarget.skyFogStart, 0f, 0.2f);
            myTarget.SkyFogHeight = EditorGUILayout.Slider ("Sky Fog Height", myTarget.SkyFogHeight,0f,5f);
			myTarget.SkyFogIntensity = EditorGUILayout.Slider ("Sky Fog Intensity", myTarget.SkyFogIntensity,0f,1f);
#if ENVIRO_LW
            if (EnviroSkyMgr.instance == null || EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.LW)
            {
                myTarget.moonIntensity = EditorGUILayout.Slider("Moon Intensity", myTarget.moonIntensity, 0.0f, 1f);
            }
#endif
            EditorGUILayout.EndVertical ();
			EditorGUILayout.EndVertical ();
		}
		EditorGUILayout.EndVertical ();


#if ENVIRO_HD
        if (EnviroSkyMgr.instance == null || EnviroSkyMgr.instance.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
        {
            GUI.backgroundColor = boxColor1;
            GUILayout.BeginVertical("", boxStyleModified);
            GUI.backgroundColor = Color.white;
            showPostProcessing = GUILayout.Toggle(showPostProcessing, "Distance Blur Configs", headerFoldout);
            if (showPostProcessing)
            {
                GUILayout.BeginVertical("", boxStyleModified);
                myTarget.blurDistance = EditorGUILayout.Slider("Blur Distance", myTarget.blurDistance, 0f, 5000f);
                myTarget.blurIntensity = EditorGUILayout.Slider("Blur Intensity", myTarget.blurIntensity, 0f, 1f);
                myTarget.blurSkyIntensity = EditorGUILayout.Slider("Sky Blur Intensity", myTarget.blurSkyIntensity, 0f, 5f);
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }
#endif

        // Audio Setup
        GUI.backgroundColor = boxColor1;
        GUILayout.BeginVertical("", boxStyleModified);
        GUI.backgroundColor = Color.white;
        showAudio = GUILayout.Toggle(showAudio, "Audio Configs", headerFoldout);
		if (showAudio) {
            GUILayout.BeginVertical("", boxStyleModified);
            myTarget.weatherSFX = (AudioClip)EditorGUILayout.ObjectField ("Weather Soundeffect",myTarget.weatherSFX, typeof(AudioClip), true);
			GUILayout.Space(10);
			myTarget.SpringDayAmbient = (AudioClip)EditorGUILayout.ObjectField ("Spring Day Ambient",myTarget.SpringDayAmbient, typeof(AudioClip), true);
			myTarget.SpringNightAmbient = (AudioClip)EditorGUILayout.ObjectField ("Spring Night Ambient",myTarget.SpringNightAmbient, typeof(AudioClip), true);
			GUILayout.Space(10);
			myTarget.SummerDayAmbient = (AudioClip)EditorGUILayout.ObjectField ("Summer Day Ambient",myTarget.SummerDayAmbient, typeof(AudioClip), true);
			myTarget.SummerNightAmbient = (AudioClip)EditorGUILayout.ObjectField ("Summer Night Ambient",myTarget.SummerNightAmbient, typeof(AudioClip), true);
			GUILayout.Space(10);
			myTarget.AutumnDayAmbient = (AudioClip)EditorGUILayout.ObjectField ("Autumn Day Ambient",myTarget.AutumnDayAmbient, typeof(AudioClip), true);
			myTarget.AutumnNightAmbient = (AudioClip)EditorGUILayout.ObjectField ("Autumn Night Ambient",myTarget.AutumnNightAmbient, typeof(AudioClip), true);
			GUILayout.Space(10);
			myTarget.WinterDayAmbient = (AudioClip)EditorGUILayout.ObjectField ("Winter Day Ambient",myTarget.WinterDayAmbient, typeof(AudioClip), true);
			myTarget.WinterNightAmbient = (AudioClip)EditorGUILayout.ObjectField ("Winter Night Ambient",myTarget.WinterNightAmbient, typeof(AudioClip), true);
            EditorGUILayout.EndVertical();
        }
		EditorGUILayout.EndVertical ();

#if AURA_IN_PROJECT
        // Third Party Setup
            GUI.backgroundColor = boxColor1;
        GUILayout.BeginVertical("", boxStyleModified);
        GUI.backgroundColor = Color.white; 
        showThirdParty = GUILayout.Toggle(showThirdParty, "Third Party Integrations", headerFoldout);
        if (showThirdParty)
        {      
            GUILayout.BeginVertical("Aura 2", boxStyleModified);
            GUILayout.Space(20);
            GUILayout.BeginVertical("", boxStyleModified);
            EditorGUILayout.LabelField("Set Aura2 settings for each weather preset here. Please also activate Aura 2 Integration in Enviro Sky Manager -> Third Party Integration!", wrapStyle2);
            //myTarget.auraPreset = (Aura2API.AuraBaseSettings)EditorGUILayout.ObjectField("Aura 2 Preset", myTarget.auraPreset, typeof(Aura2API.AuraBaseSettings), true);
            myTarget.enviroAura2Config.aura2GlobalDensity = EditorGUILayout.Slider("Global Density", myTarget.enviroAura2Config.aura2GlobalDensity, 0f, 10f);
            myTarget.enviroAura2Config.aura2GlobalScattering = EditorGUILayout.Slider("Global Scattering", myTarget.enviroAura2Config.aura2GlobalScattering, 0f, 1f);
            myTarget.enviroAura2Config.aura2GlobalAmbientLight = EditorGUILayout.Slider("Global Ambient Light", myTarget.enviroAura2Config.aura2GlobalAmbientLight, 0f, 10f);
            myTarget.enviroAura2Config.aura2GlobalExtinction = EditorGUILayout.Slider("Global Extiction", myTarget.enviroAura2Config.aura2GlobalExtinction, 0f, 1f);
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
#endif

 
        // END
        EditorGUILayout.EndVertical ();
        serializedObj.ApplyModifiedProperties();
		EditorUtility.SetDirty (target);
	}
}
