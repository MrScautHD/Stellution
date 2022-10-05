using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class EnviroCloudsAnimateWithTime : MonoBehaviour {

	void Update () 
	{		
		if(EnviroSky.instance == null)
		return;

		float timeToAnimation = EnviroSky.instance.Remap(EnviroSky.instance.GetTimeOfDay(),0f,24f,-1f,1f);
		float timeOfYearInHours =  EnviroSky.instance.GetTimeOfDay() + (EnviroSky.instance.GameTime.Days * 24f);

		EnviroSky.instance.cloudAnim = new Vector3(timeToAnimation * EnviroSky.instance.cloudsSettings.cloudsWindDirectionX,timeToAnimation * -1f,timeToAnimation * EnviroSky.instance.cloudsSettings.cloudsWindDirectionY);
		EnviroSky.instance.cloudAnimNonScaled = new Vector2(timeOfYearInHours * EnviroSky.instance.cloudsSettings.cloudsWindDirectionX,timeOfYearInHours * EnviroSky.instance.cloudsSettings.cloudsWindDirectionY);
	}
}
