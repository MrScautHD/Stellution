using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
[System.Serializable]
public class EnviroVolumeCloudsQualitySettings
{
    // Reprojection
    public enum ReprojectionPixelSize
    {
        Off,
        Low,
        Medium,
        High
    }

    public enum CloudDetailQuality
    {
        Low,
        High
    }

    [Header("Clouds Height Settings")]
    [Tooltip("Clouds start height.")]
    public float bottomCloudHeight = 3000f;
    [Tooltip("Clouds end height.")]
    public float topCloudHeight = 7000f;

    [Header("Raymarch Step Settings")]
    [Range(32, 256)]
    [Tooltip("Number of raymarching samples.")]
    public int raymarchSteps = 150;
    [Tooltip("Increase performance by using less steps when clouds are hidden by objects.")]
    [Range(0.1f, 1f)]
    public float stepsInDepthModificator = 0.75f;
    [Tooltip("Increase performance by using early exit expensive raymarching. Higher values = more performant but less accurate lighting.")]
    [Range(0.0f, 0.5f)]
    public float transmissionToExit = 0.05f;
    [Range(1, 8)]
    [Header("Resolution, Upsample and Reprojection")]
    [Tooltip("Downsampling of clouds rendering. 1 = full res, 2 = half Res, ...")]
    public int cloudsRenderResolution = 1;
    public ReprojectionPixelSize reprojectionPixelSize;

    [Header("Clouds Modelling")]
    [Tooltip("LOD Distance for using lower res 3d texture for far away clouds. ")]
    [Range(0f, 1f)]
    public float lodDistance = 0.5f;
    [Tooltip("The UV scale of base noise. High Values = Low performance!")]
    [Range(2f, 100f)]
    public float baseNoiseUV = 20f;
    [Tooltip("The UV scale of detail noise. High Values = Low performance!")]
    [Range(2f, 100f)]
    public float detailNoiseUV = 50f;
    [Tooltip("Enable to use a curl noise to further enhance the detail erode.")]
    public bool useCurlNoise = false;
    [Tooltip("Resolution of Base Noise Texture.")]
    public CloudDetailQuality baseQuality;
    [Tooltip("Resolution of Detail Noise Texture.")]
    public CloudDetailQuality detailQuality;
}



[System.Serializable]
public class EnviroVolumeCloudsQuality : ScriptableObject 
{
    public EnviroVolumeCloudsQualitySettings qualitySettings;
}

public static class EnviroCloudsQualityCreation
{
#if UNITY_EDITOR
	[MenuItem("Assets/Create/Enviro/Volume Clouds Quality")]
	public static EnviroVolumeCloudsQuality CreateNewEnviroCloudsQualityPreset()
	{
		EnviroVolumeCloudsQuality preset = ScriptableObject.CreateInstance<EnviroVolumeCloudsQuality>();

		// Create and save the new profile with unique name
		string path = "Assets/Enviro - Sky and Weather/Enviro Standard/Profiles/Clouds Quality";
		string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/New " + "Clouds Quality" + ".asset");
		AssetDatabase.CreateAsset (preset, assetPathAndName);
		AssetDatabase.SaveAssets ();
		AssetDatabase.Refresh();
		return preset;
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
		Gradient nG = new Gradient ();
		GradientColorKey[] gClr = new GradientColorKey[2];
		GradientAlphaKey[] gAlpha = new GradientAlphaKey[2];

		gClr [0].color = clr1;
		gClr [0].time = time1;
		gClr [1].color = clr2;
		gClr [1].time = time2;

		gAlpha [0].alpha = 1f;
		gAlpha [0].time = 0f;
		gAlpha [1].alpha = 1f;
		gAlpha [1].time = 1f;

		nG.SetKeys (gClr, gAlpha);

		return nG;
	}

	public static Gradient CreateGradient(List<Color> clrs,List<float>times)
	{
		Gradient nG = new Gradient ();

		GradientColorKey[] gClr = new GradientColorKey[clrs.Count];
		GradientAlphaKey[] gAlpha = new GradientAlphaKey[2];

		for (int i = 0; i < clrs.Count; i++) {
			gClr [i].color = clrs [i];
			gClr [i].time = times[i];
		}

		gAlpha [0].alpha = 1f;
		gAlpha [0].time = 0f;
		gAlpha [1].alpha = 1f;
		gAlpha [1].time = 1f;

		nG.SetKeys (gClr, gAlpha);
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
