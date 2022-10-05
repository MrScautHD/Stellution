using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


[System.Serializable]
public class EnviroWeatherCloudsConfig {
    [Tooltip("Ambient Light Intensity.")]
    [Range(0f, 1f)]
    public float ambientSkyColorIntensity = 1f;
	[Tooltip("Light extinction factor.")]
	[Range(0f,2f)]
	public float scatteringCoef = 1f;
    [Tooltip("Darkens the edges of clouds from in-out scattering.")]
    [Range(1f, 3f)]
    public float edgeDarkness = 2.0f;

    public float baseErosionIntensity = 0.0f;
    public float detailErosionIntensity = 0.2f;

    [Tooltip("Density factor of clouds.")]
	public float density = 1f;
    //  [Tooltip("Density factor of clouds lighting.")]
    //  public float densityLightning = 1f;
    [Tooltip("Light Step modifier.")]
    public float lightStepModifier = 0.5f;

    [Tooltip("Min lighting variance intensity.")]
    public float lightVariance = 0.5f;

    [Tooltip("Global coverage multiplicator of clouds.")]
	[Range(0f,1f)]
	public float coverage  = 1.0f;

    [Tooltip("Defines how much light will be absorbed from cloud particles.")]
    [Range(0.0f, 1f)]
    public float lightAbsorbtion = 0.4f;
    // [Tooltip("Add or remove Coverage at the top")]
    //  [Range(0f, 2f)]
    //  public float coverageModTop = 2.0f;
    //  [Tooltip("Add or remove Coverage at the bottom")]
    // [Range(0f, 1f)]
    // public float coverageModBottom = 1.0f;

    [Tooltip("Coverage type of clouds. 1 = more round scattered shapes , 0 = connected islands style")]
    [Range(0f, 1f)]
    public float coverageType = 1.0f;

    [Tooltip("Clouds raynarching step modifier.")]
    [Range(0.25f, 1f)]
    public float raymarchingScale = 1f;
    [Tooltip("Clouds modelling type.")]
	[Range(0f,1f)]
	public float cloudType = 1f;
	[Tooltip("Cirrus Clouds Alpha")]
	[Range(0f,1f)]
	public float cirrusAlpha = 0f;
	[Tooltip("Cirrus Clouds Coverage")]
	[Range(0f,1f)]
	public float cirrusCoverage = 0f;
	[Tooltip("Cirrus Clouds Color Power")]
	[Range(0f,1f)]
	public float cirrusColorPow = 2f;


	[Tooltip("Flat Clouds Density")]
	[Range(0f,5f)]
	public float flatCloudsDensity = 2.5f;
    [Tooltip("Flat Clouds Absorbtion")]
    [Range(0f, 5f)]
    public float flatCloudsAbsorbtion = 2.5f;
    [Tooltip("Flat Clouds Direct Light Intensity")]
    [Range(0f, 20f)]
    public float flatCloudsDirectLightIntensity = 15f;
    [Tooltip("Flat Clouds Ambient Light Intensity")]
    [Range(0f, 2f)]
    public float flatCloudsAmbientLightIntensity = 1f;
    [Tooltip("Flat Clouds HG Phase")]
    [Range(0f, 1f)]
    public float flatCloudsHGPhase = 0.9f;
    [Tooltip("Flat Clouds Coverage")]
	[Range(0f,2f)]
	public float flatCoverage = 0f;


    [Tooltip("Particle Clouds Alpha")]
    [Range(0f, 1f)]
    public float particleLayer1Alpha = 0f;
    [Tooltip("Particle Clouds Brightness")]
    [Range(0f, 1f)]
    public float particleLayer1Brightness = 0.75f;
    [Tooltip("Particle Clouds Color Power")]
    [Range(0f, 1f)]
    public float particleLayer1ColorPow = 2f;

    [Tooltip("Particle Clouds Alpha")]
    [Range(0f, 1f)]
    public float particleLayer2Alpha = 0f;
    [Tooltip("Particle Clouds Brightness")]
    [Range(0f, 1f)]
    public float particleLayer2Brightness = 0.75f;
    [Tooltip("Particle Clouds Color Power")]
    [Range(0f, 1f)]
    public float particleLayer2ColorPow = 2f;

    [Tooltip("Use particle clouds here even when it is disabled!")]
    public bool particleCloudsOverwrite = false;
}

[System.Serializable]
public class EnviroWeatherEffects {
	public GameObject prefab;
	public Vector3 localPositionOffset;
	public Vector3 localRotationOffset;
}

// Aura 2 Integration
#if AURA_IN_PROJECT
[System.Serializable]
public class EnviroAura2Config
{
    public float aura2GlobalDensity = 0.15f;
    public float aura2GlobalScattering = 0.5f;
    public float aura2GlobalAmbientLight = 1f;
    public float aura2GlobalExtinction = 0.25f;
}
#endif

[System.Serializable]
public class EnviroWeatherPreset : ScriptableObject 
{
	public string version;
	public string Name;
	[Header("Season Settings")]
	public bool Spring = true;
	[Range(1,100)]
	public float possibiltyInSpring = 50f;
	public bool Summer = true;
	[Range(1,100)]
	public float possibiltyInSummer = 50f;
	public bool Autumn = true;
	[Range(1,100)]
	public float possibiltyInAutumn = 50f;
	public bool winter = true;
	[Range(1,100)]
	public float possibiltyInWinter = 50f;

	[Header("Cloud Settings")]
	public EnviroWeatherCloudsConfig cloudsConfig;

	[Header("Linear Fog")]
	public float fogStartDistance = 0f;
	public float fogDistance = 1000f;
	[Header("Exp Fog")]
	public float fogDensity = 0.0001f;

#if ENVIRO_HDRP
    // HDRP
    [Tooltip("HDRP Fog Attenuation Distance")]
    [Range(1f, 10000f)]
    public float fogAttenuationDistance = 10000f;

    [Tooltip("HDRP Fog Maximum fog Height Camera relative.")]
    [Range(-1000f, 1000f)]
    public float fogRelativeFogHeight = 50f;
#endif

    //[Header("Advanced Fog Settings:")]
    [Tooltip("Used to modify sky, direct, ambient light and fog color. The color alpha value defines the intensity")]
	public Gradient weatherSkyMod;
	public Gradient weatherLightMod;
	public Gradient weatherFogMod;
    [Range(0f, 2.0f)]
    public float volumeLightIntensity = 1.0f;
    [Range(-1.0f, 1.0f)]
    public float shadowIntensityMod = 0.0f;

    [Range(0f,100f)][Tooltip("The density of height based fog for this weather.")]
	public float heightFogDensity = 1f;
	[Range(0,2)][Tooltip("Define the height of fog rendered in sky.")]
	public float SkyFogHeight = 0.5f;

    [Range(0f, 1f)]
    [Tooltip("Define the start height of fog rendered in sky.")]
    public float skyFogStart = 0.0f;

    [Tooltip("Define the intensity of fog rendered in sky.")]
	[Range(0,2)]
	public float SkyFogIntensity = 1f;
	[Range(1,10)][Tooltip("Define the scattering intensity of fog.")]
	public float FogScatteringIntensity = 1f;

    [Range(0,1)][Tooltip("Block the sundisk with fog.")]
	public float fogSunBlocking = 0.25f;

    [Range(0, 1)]
    [Tooltip("Block the moon with fog.")]
    public float moonIntensity = 1f;

    [Header("Weather Settings")]
	public List<EnviroWeatherEffects> effectSystems = new List<EnviroWeatherEffects>();
	[Range(0,1)][Tooltip("Wind intensity that will applied to wind zone.")]
	public float WindStrenght = 0.5f;
	[Range(0,1)][Tooltip("The maximum wetness level that can be reached.")]
	public float wetnessLevel = 0f;
	[Range(0,1)][Tooltip("The maximum snow level that can be reached.")]
	public float snowLevel = 0f;
    [Range(-50f, 50f)]
    [Tooltip("The temperature modifcation for this weather type. (Will be added or substracted)")]
    public float temperatureLevel = 0f;
    [Tooltip("Activate this to enable thunder and lightning.")]
	public bool isLightningStorm;
	[Range(0,2)][Tooltip("The Intervall of lightning in seconds. Random(lightningInterval,lightningInterval * 2). ")]
	public float lightningInterval = 10f;

#if ENVIRO_HD
    [Header("Aurora Settings")]
    [Range(0f, 1f)]
    public float auroraIntensity = 0f;
#endif

    [Header("Audio Settings - SFX")]
	[Tooltip("Define an sound effect for this weather preset.")]
	public AudioClip weatherSFX;
	[Header("Audio Settings - Ambient")]
	[Tooltip("This sound wil be played in spring at day.(looped)")]
	public AudioClip SpringDayAmbient;
	[Tooltip("This sound wil be played in spring at night.(looped)")]
	public AudioClip SpringNightAmbient;
	[Tooltip("This sound wil be played in summer at day.(looped)")]
	public AudioClip SummerDayAmbient;
	[Tooltip("This sound wil be played in summer at night.(looped)")]
	public AudioClip SummerNightAmbient;
	[Tooltip("This sound wil be played in autumn at day.(looped)")]
	public AudioClip AutumnDayAmbient;
	[Tooltip("This sound wil be played in autumn at night.(looped)")]
	public AudioClip AutumnNightAmbient;
	[Tooltip("This sound wil be played in winter at day.(looped)")]
	public AudioClip WinterDayAmbient;
	[Tooltip("This sound wil be played in winter at night.(looped)")]
	public AudioClip WinterNightAmbient;

    // Postprocessing
    public float blurDistance = 100;
    public float blurIntensity = 1f;
    public float blurSkyIntensity = 1f;

	//HDRP
	public float sceneExposureMod = 1f;
	public float skyExposureMod = 1f;
	public float lightIntensityMod = 1f;

    // Aura 2 Integration
#if AURA_IN_PROJECT
    public EnviroAura2Config enviroAura2Config;
#endif
}

public class EnviroWeatherPresetCreation {
	#if UNITY_EDITOR
	[MenuItem("Assets/Create/Enviro/WeatherPreset")]
	public static void CreateMyAsset()
	{
		EnviroWeatherPreset wpreset = ScriptableObject.CreateInstance<EnviroWeatherPreset>();
		wpreset.Name = "New Weather Preset " + UnityEngine.Random.Range (0, 999999).ToString();
		wpreset.weatherFogMod = CreateGradient ();
		wpreset.weatherSkyMod = CreateGradient ();
		wpreset.weatherLightMod = CreateGradient ();

		wpreset.version = "2.2.0";
		// Create and save the new profile with unique name
		string path = AssetDatabase.GetAssetPath (Selection.activeObject);
		if (path == "") 
		{
			path = "Assets";
		} 
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + "Weather Preset" + ".asset");
		AssetDatabase.CreateAsset (wpreset, assetPathAndName);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh();
		EditorUtility.FocusProjectWindow ();
		Selection.activeObject = wpreset;
	}
	#endif

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
		
	public static Gradient CreateGradient()
	{
		Gradient nG = new Gradient ();
		GradientColorKey[] gClr = new GradientColorKey[2];
		GradientAlphaKey[] gAlpha = new GradientAlphaKey[2];

		gClr [0].color = Color.white;
		gClr [0].time = 0f;
		gClr [1].color = Color.white;
		gClr [1].time = 0f;

		gAlpha [0].alpha = 0f;
		gAlpha [0].time = 0f;
		gAlpha [1].alpha = 0f;
		gAlpha [1].time = 1f;

		nG.SetKeys (gClr, gAlpha);

		return nG;
	}
		
	public static Color GetColor (string hex)
	{
		Color clr = new Color ();	
		ColorUtility.TryParseHtmlString (hex, out clr);
		return clr;
	}
	
	public static Keyframe CreateKey (float value, float time)
	{
		Keyframe k = new Keyframe();
		k.value = value;
		k.time = time;
		return k;
	}

	public static Keyframe CreateKey (float value, float time, float inTangent, float outTangent)
	{
		Keyframe k = new Keyframe();
		k.value = value;
		k.time = time;
		k.inTangent = inTangent;
		k.outTangent = outTangent;
		return k;
	}		
}
