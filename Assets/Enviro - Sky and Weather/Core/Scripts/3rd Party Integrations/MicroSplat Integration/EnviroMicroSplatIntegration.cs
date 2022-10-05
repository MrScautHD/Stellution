using UnityEngine;
using System.Collections;
#if ENVIRO_MICROSPLAT_SUPPORT
[AddComponentMenu("Enviro/Integration/MicroSplat Integration")]
#endif

[ExecuteInEditMode]
public class EnviroMicroSplatIntegration : MonoBehaviour {
#if ENVIRO_MICROSPLAT_SUPPORT
    [Header("Wetness")]
    public bool UpdateWetness = true;
    [Range(0f, 1f)]
    public float minWetness = 0f;
    [Range(0f, 1f)]
    public float maxWetness = 1f;
    [Header("Rain Ripples")]
    public bool UpdateRainRipples = true;
    [Header("Puddle Settings")]
    public bool UpdatePuddles = true;
    [Header("Stream Settings")]
    public bool UpdateStreams = true;
    [Header("Snow Settings")]
    public bool UpdateSnow = true;
    [Header("Wind Settings")]
    public bool UpdateWindStrength = true;
    public bool UpdateWindRotation = true;

    void Update () 
	{
    	if (EnviroSkyMgr.instance == null)
			return;

		if (UpdateSnow){
            Shader.SetGlobalFloat ("_Global_SnowLevel", EnviroSkyMgr.instance.GetSnowIntensity());
		}

		if (UpdateWetness) {
            float currWetness = Mathf.Clamp(EnviroSkyMgr.instance.GetWetnessIntensity(), minWetness, maxWetness);
            Shader.SetGlobalVector("_Global_WetnessParams", new Vector2(minWetness, currWetness));
        }
			
		if (UpdatePuddles) {
            Shader.SetGlobalFloat("_Global_PuddleParams", EnviroSkyMgr.instance.GetWetnessIntensity());
		}

		if (UpdateRainRipples) {
            Shader.SetGlobalFloat("_Global_RainIntensity", EnviroSkyMgr.instance.GetWetnessIntensity());
        }

        if (UpdateStreams) {
            Shader.SetGlobalFloat("_Global_StreamMax", EnviroSkyMgr.instance.GetWetnessIntensity());
        }
        //Sync all MicroSplat Values
        //MicroSplatTerrain.SyncAll();
    }
#endif
}
