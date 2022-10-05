using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if ENVIRO_GLOBALSNOW_SUPPORT
using GlobalSnowEffect;
#endif
[AddComponentMenu("Enviro/Integration/Global Snow")]
public class GlobalSnowIntegration : MonoBehaviour
{
#if ENVIRO_GLOBALSNOW_SUPPORT
    public float snowIntensityMult = 1f;

    void Start()
    {
        
    }

    void Update()
    {
        if(GlobalSnow.instance != null)
           GlobalSnow.instance.snowAmount = Mathf.Clamp(EnviroSkyMgr.instance.GetSnowIntensity() * snowIntensityMult, 0f,2f);        
    }
#endif
}
