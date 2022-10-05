using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class EnviroCopyLightSettings : MonoBehaviour {

    public Light myLight;
    private Light enviroLight;

	void OnEnable ()
    {
        if (EnviroSkyMgr.instance != null)
            enviroLight = EnviroSkyMgr.instance.Components.DirectLight.GetComponent<Light>();

        if (myLight == null)
            myLight = GetComponent<Light>();
    }
	 
	void Update ()
    {
	    if(EnviroSkyMgr.instance != null && enviroLight != null && myLight != null)
        {
            myLight.transform.position = EnviroSkyMgr.instance.Components.DirectLight.position;
            myLight.transform.rotation = EnviroSkyMgr.instance.Components.DirectLight.rotation;

            myLight.color = enviroLight.color;
            myLight.intensity = enviroLight.intensity;
            myLight.shadowStrength = enviroLight.shadowStrength;
        }
	}
}
