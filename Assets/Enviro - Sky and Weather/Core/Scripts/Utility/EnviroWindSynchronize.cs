//UNCOMMENT IF YOU HAVE NATUREMANUFACTOR ASSET WITH NW_WIND COMPONENT IN YOUR PROJECT AND WANT TO SYNCHRONIZE WITH ENVIRO!
//#define NW_WIND

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class EnviroWindSynchronize : MonoBehaviour {
    [Header("Terrain Grass")]
    public bool syncTerrainGrassWind = true;
    public List<Terrain> terrains = new List<Terrain>();

#if NW_WIND
    [Header("NatureManufactor Wind")]
    public bool syncNatureManufactureWind = true;
    public NM_Wind natureManufacturWind;
    [Range(1f,100f)]
    public float natureManufacturWindMult = 50f;
#endif
    [Header("Speed")]
    [Range(0f, 10f)]
    public float windChangingSpeed = 1f;

    void Start ()
    {
        if (syncTerrainGrassWind && terrains.Count > 0)
        {
            Debug.Log("Please assign Terrain, or deactivate 'syncTerrainGrassWind'!");
            this.enabled = false;
        }
#if NW_WIND
        if (syncNatureManufactureWind && natureManufacturWind == null)
        {
            Debug.Log("Please assign the NatureManufacture Wind Component, or deactivate 'syncNatureManufactureWind'!");
            this.enabled = false;
        }
#endif
    }


    void Update ()
    {
        if (EnviroSkyMgr.instance == null)
            return;

        if (syncTerrainGrassWind)
        {
            for (int i = 0; i < terrains.Count; i++)
            {
                terrains[i].terrainData.wavingGrassStrength = Mathf.Lerp(terrains[i].terrainData.wavingGrassStrength, EnviroSkyMgr.instance.Components.windZone.windMain, Time.deltaTime * windChangingSpeed);
            }
        }
#if NW_WIND
        if (syncNatureManufactureWind)
        {
            natureManufacturWind.WindSpeed = Mathf.Lerp(natureManufacturWind.WindSpeed, EnviroSkyMgr.instance.Components.windZone.windMain * natureManufacturWindMult, Time.deltaTime * windChangingSpeed);
        }
#endif
    }
}
