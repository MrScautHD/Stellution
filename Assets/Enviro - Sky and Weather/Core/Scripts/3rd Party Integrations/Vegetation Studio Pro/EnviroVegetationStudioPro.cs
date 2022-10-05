// Vegetation Studio Pro Integration v0.2
// Thanks to Meishin for optimizations!

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if VEGETATION_STUDIO_PRO
using AwesomeTechnologies.VegetationSystem;
using AwesomeTechnologies.VegetationStudio;
#endif
[AddComponentMenu("Enviro/Integration/VS Pro Integration")]
public class EnviroVegetationStudioPro : MonoBehaviour
{

#if VEGETATION_STUDIO_PRO
    private const float updatePrecision = 0.01f;
    public bool setWindZone = true;
    public bool syncRain = true;
    public bool syncSnow = true;

    void Start()
    {
        if (VegetationStudioManager.Instance == null || EnviroSkyMgr.instance == null)
            return;
   
        if (setWindZone)
        {
            for (int i = 0; i < VegetationStudioManager.Instance.VegetationSystemList.Count; i++)
            {
                VegetationStudioManager.Instance.VegetationSystemList[i].SelectedWindZone = EnviroSkyMgr.instance.Components.windZone;
            }
        }
    }

    void Update()
    {
        if(VegetationStudioManager.Instance == null || EnviroSkyMgr.instance == null)
            return;

        //Update Vegetation Systems
        foreach(VegetationSystemPro vegetationSystem in VegetationStudioManager.Instance.VegetationSystemList)
        {
            if (!vegetationSystem.enabled)
                continue;

            EnviroWeather Enviro = EnviroSkyMgr.instance.Weather;
            EnvironmentSettings VSPro = vegetationSystem.EnvironmentSettings;

            // Should we refresh VSPro ?
            bool updateVSPro = false;

            if (syncRain && Mathf.Abs(VSPro.RainAmount - Enviro.wetness) >= updatePrecision) 
                updateVSPro = true;
            if (syncSnow && Mathf.Abs(VSPro.SnowAmount - Enviro.snowStrength) >= updatePrecision) 
                updateVSPro = true;
                      
            if (syncRain && updateVSPro) 
                VSPro.RainAmount = Enviro.wetness;

            if (syncSnow && updateVSPro) 
                VSPro.SnowAmount = Enviro.snowStrength;

            if (updateVSPro)
                vegetationSystem.RefreshMaterials();
                
        }
    }
#endif
}
