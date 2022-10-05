using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using UnityEditorInternal;
using System;
using System.Linq;

[CustomEditor(typeof(EnviroSkyMgr))]
public class EnviroSkyMgrEditor : Editor {

    private GUIStyle boxStyle;
    private GUIStyle boxStyleModified;
    private GUIStyle wrapStyle;
    private GUIStyle headerStyle;
    private GUIStyle headerFoldout;
    EnviroSkyMgr myTarget;

    private Color modifiedColor;
    private Color greenColor;
    private Color boxColor1;

    void OnEnable()
	{
		myTarget = (EnviroSkyMgr)target;
        modifiedColor = Color.red;
        modifiedColor.a = 0.5f;

        greenColor = Color.green;
        greenColor.a = 0.5f;

#if UNITY_2019_3_OR_NEWER
        boxColor1 = new Color(0.95f, 0.95f, 0.95f,1f);
#else
        boxColor1 = new Color(0.85f, 0.85f, 0.85f, 1f);
#endif

    }

    public override void OnInspectorGUI ()
	{
		myTarget = (EnviroSkyMgr)target;

		if (boxStyle == null) {
			boxStyle = new GUIStyle (GUI.skin.box);
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
			wrapStyle.alignment = TextAnchor.UpperLeft;
		}

        if (headerStyle == null)
        {
            headerStyle = new GUIStyle(GUI.skin.label);
            headerStyle.fontStyle = FontStyle.Bold;
            headerStyle.wordWrap = true;
            headerStyle.alignment = TextAnchor.UpperLeft;
        }

        if (headerFoldout == null)
        {
            headerFoldout = new GUIStyle(EditorStyles.foldout);
            headerFoldout.fontStyle = FontStyle.Bold;
        }


        GUILayout.BeginVertical("Enviro - Sky Manager 2.4.2", boxStyle);
		GUILayout.Space(20);
		EditorGUILayout.LabelField("Welcome to the Enviro Sky Manager! Add Lite and Standard Enviro instances and switch between those. Add third party support components or choose your render pipeline.", wrapStyle);
	

        GUI.backgroundColor = boxColor1;
        GUILayout.BeginVertical("", boxStyleModified);
        GUI.backgroundColor = Color.white;
        myTarget.showSetup = GUILayout.Toggle(myTarget.showSetup, "Setup", headerFoldout);
        if (myTarget.showSetup)
        {
            //  
     
            GUILayout.BeginVertical("General", boxStyleModified);
           
            GUILayout.Space(20);
            myTarget.dontDestroy = EditorGUILayout.ToggleLeft("  Don't Destroy On Load", myTarget.dontDestroy);

            GUILayout.EndVertical();

            GUILayout.BeginVertical("Render Pipeline", boxStyleModified);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Please set your render pipeline here and choose between 'Built-In', 'URP' and HDRP.", wrapStyle);
            GUILayout.Space(10);


#if ENVIRO_HDRP
             GUILayout.Label("Current Render Pipeline:   HDRP", headerStyle);
#elif ENVIRO_LWRP
             GUILayout.Label("Current Render Pipeline:   URP", headerStyle);
#else
            GUILayout.Label("Current Render Pipeline:   Built-In", headerStyle);
#endif

            GUILayout.Space(10);
#if !ENVIRO_HDRP
            if (GUILayout.Button("Activate HDRP Support"))
                {
                    AddDefineSymbol("ENVIRO_HDRP");
                    RemoveDefineSymbol("ENVIRO_LWRP");
                }
#endif

#if !ENVIRO_LWRP
                if (GUILayout.Button("Activate URP Support"))
                {
                     AddDefineSymbol("ENVIRO_LWRP");
                     RemoveDefineSymbol("ENVIRO_HDRP");
                }
#endif

#if ENVIRO_LWRP || ENVIRO_HDRP
            if (GUILayout.Button("Activate Built-In Support"))
                { 
                    RemoveDefineSymbol("ENVIRO_LWRP");
                    RemoveDefineSymbol("ENVIRO_HDRP");
                }
#endif

#if ENVIRO_LWRP
                GUILayout.BeginVertical("URP Setup", boxStyleModified);
                GUILayout.Space(20);
                EditorGUILayout.LabelField("Please assign the Enviro URP Renderer in your URP Assets -> Renderer List and activate the 'Depth' option.", wrapStyle);
                GUILayout.EndVertical();
#endif

#if ENVIRO_HDRP
                GUILayout.BeginVertical("HDRP Setup", boxStyleModified);
                GUILayout.Space(20);
                EditorGUILayout.LabelField("Please assign the Enviro Post Processing Effects in your HDRP Default Settings  -> Quality Settings", wrapStyle);
                GUILayout.EndVertical();
#endif


            GUILayout.EndVertical();

            GUILayout.Space(10);

#if ENVIRO_HD
            GUILayout.BeginVertical("Standard Version", boxStyleModified);
            GUILayout.Space(20);
            if (myTarget.enviroHDInstance == null)
            {
                if (GUILayout.Button("Create Standard Instance"))
                {
                    myTarget.CreateEnviroHDInstance();
                }
                if (GUILayout.Button("Create Standard VR Instance"))
                {
                    myTarget.CreateEnviroHDVRInstance();
                }
            }
            else
            {
                GUILayout.Label("Current Instance found!", headerStyle);
                GUILayout.Label("Delete " + myTarget.enviroHDInstance.gameObject.name + " if you want to add other prefab!");
            }
            GUILayout.EndVertical();
#endif

#if ENVIRO_LW
            GUILayout.BeginVertical("Lite Version", boxStyleModified);
            GUILayout.Space(20);
            if (myTarget.enviroLWInstance == null)
            {
                if (GUILayout.Button("Create Lite Instance"))
                {
                    myTarget.CreateEnviroLWInstance();
                }
                if (GUILayout.Button("Create Lite Mobile Instance"))
                {
                    myTarget.CreateEnviroLWMobileInstance();
                }
            }
            else
            {
                GUILayout.Label("Current Instance found!", headerStyle);
                GUILayout.Label("Delete " + myTarget.enviroLWInstance.gameObject.name + " if you want to add other prefab!");
            }
            GUILayout.EndVertical();
#endif
        }
        GUILayout.EndVertical();

        //EditorGUILayout.EndToggleGroup();
        GUI.backgroundColor = boxColor1;
        GUILayout.BeginVertical("", boxStyleModified);
        GUI.backgroundColor = Color.white;
        //myTarget.showInstances = EditorGUILayout.BeginToggleGroup(" Instances", myTarget.showInstances);
        myTarget.showInstances = GUILayout.Toggle(myTarget.showInstances, "Instances", headerFoldout);
        if (myTarget.showInstances)
        {
          //  GUILayout.Space(10);
#if ENVIRO_HD
            if (myTarget.enviroHDInstance != null)
            {
                if (myTarget.currentEnviroSkyVersion != EnviroSkyMgr.EnviroSkyVersion.HD)
                    GUI.backgroundColor = modifiedColor;
                else
                {
                    if (myTarget.enviroHDInstance.Player == null || myTarget.enviroHDInstance.PlayerCamera == null)
                        GUI.backgroundColor = modifiedColor;
                    else
                        GUI.backgroundColor = greenColor;
                }

                GUILayout.BeginVertical(myTarget.enviroHDInstance.gameObject.name, boxStyle);
                GUI.backgroundColor = Color.white;
                GUILayout.Space(20);
                if (myTarget.currentEnviroSkyVersion != EnviroSkyMgr.EnviroSkyVersion.HD)
                {
                    if (GUILayout.Button("Activate"))
                    {
                        myTarget.ActivateHDInstance();
                    }
                }
                else if (myTarget.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.HD)
                {
                    if (myTarget.enviroHDInstance.Player == null || myTarget.enviroHDInstance.PlayerCamera == null)
                    {
                        GUILayout.Label("Player and/or camera assignment is missing!");

                        if (GUILayout.Button("Auto Assign"))
                        {
                            myTarget.enviroHDInstance.AssignAndStart(Camera.main.gameObject, Camera.main);
                        }
                    }
                    else
                    {
                        if (Application.isPlaying)
                        {
                            if (!myTarget.enviroHDInstance.started)
                            {
                                if (GUILayout.Button("Play"))
                                {
                                    myTarget.enviroHDInstance.Play(myTarget.enviroHDInstance.GameTime.ProgressTime);
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("Stop"))
                                {
                                    myTarget.enviroHDInstance.Stop(false, true);
                                }

                            }
                        }

                        if (GUILayout.Button("Deactivate"))
                        {
                            myTarget.DeactivateHDInstance();
                        }
           
                    }
                }

                if (GUILayout.Button("Show"))
                {                  
                    Selection.activeObject = myTarget.enviroHDInstance;
                }
                if (GUILayout.Button("Delete"))
                {
                    if (EditorUtility.DisplayDialog("Delete Instance?", "Are you sure that you want to delete this instance?", "Delete", "Cancel"))
                        myTarget.DeleteHDInstance();
                }

                GUILayout.EndVertical();
            }
#endif

#if ENVIRO_LW

            if (myTarget.enviroLWInstance != null)
            {
                if (myTarget.currentEnviroSkyVersion != EnviroSkyMgr.EnviroSkyVersion.LW)
                    GUI.backgroundColor = modifiedColor;
                else
                {
                    if (myTarget.enviroLWInstance.Player == null || myTarget.enviroLWInstance.PlayerCamera == null)
                        GUI.backgroundColor = modifiedColor;
                    else
                        GUI.backgroundColor = greenColor;
                }

                GUILayout.BeginVertical(myTarget.enviroLWInstance.gameObject.name, boxStyle);
                GUI.backgroundColor = Color.white;
                GUILayout.Space(20);
                if (myTarget.currentEnviroSkyVersion != EnviroSkyMgr.EnviroSkyVersion.LW)
                {
                    if (GUILayout.Button("Activate"))
                    {
                        myTarget.ActivateLWInstance();
                    }
                }
                else if (myTarget.currentEnviroSkyVersion == EnviroSkyMgr.EnviroSkyVersion.LW)
                {
                    if (myTarget.enviroLWInstance.Player == null || myTarget.enviroLWInstance.PlayerCamera == null)
                    {
                        GUILayout.Label("Player and/or camera assignment is missing!");

                        if (GUILayout.Button("Auto Assign"))
                        {
                            if (Camera.main != null)
                                myTarget.enviroLWInstance.AssignAndStart(Camera.main.gameObject, Camera.main);
                        }
                    }
                    else
                    {
                        if (Application.isPlaying)
                        {
                            if (!myTarget.enviroLWInstance.started)
                            {
                                if (GUILayout.Button("Play"))
                                {
                                    myTarget.enviroLWInstance.Play(myTarget.enviroLWInstance.GameTime.ProgressTime);
                                }
                            }
                            else
                            {
                                if (GUILayout.Button("Stop"))
                                {
                                    myTarget.enviroLWInstance.Stop(false, true);
                                }
                            }
                        }
                        if (GUILayout.Button("Deactivate"))
                        {
                            myTarget.DeactivateLWInstance();
                        }

           
                    }
                }
                 
                if (GUILayout.Button("Show"))
                {
             
                    Selection.activeObject = myTarget.enviroLWInstance;
                }

                if (GUILayout.Button("Delete"))
                {
                    if (EditorUtility.DisplayDialog("Delete Instance?", "Are you sure that you want to delete this instance?", "Delete", "Cancel"))
                        myTarget.DeleteLWInstance();
                }

                GUILayout.EndVertical();
            }
#endif
        }
        GUILayout.EndVertical();
        //EditorGUILayout.EndToggleGroup();
        GUI.backgroundColor = boxColor1;
        GUILayout.BeginVertical("", boxStyleModified);
        GUI.backgroundColor = Color.white;
        // myTarget.showThirdParty = EditorGUILayout.BeginToggleGroup(" Third Party Support", myTarget.showThirdParty);
        myTarget.showThirdParty = GUILayout.Toggle(myTarget.showThirdParty, "Third Party Support", headerFoldout);
        if (myTarget.showThirdParty)
        {
            GUILayout.BeginVertical("", boxStyleModified);
            //myTarget.showThirdPartyMisc = EditorGUILayout.BeginToggleGroup(" Miscellaneous", myTarget.showThirdPartyMisc);
            myTarget.showThirdPartyMisc = GUILayout.Toggle(myTarget.showThirdPartyMisc, "Miscellaneous", headerFoldout);

            if (myTarget.showThirdPartyMisc)
            {
                //WAPI
                GUILayout.BeginVertical("World Manager API", boxStyleModified);
                GUILayout.Space(20);
#if WORLDAPI_PRESENT
                if (myTarget.gameObject.GetComponent<EnviroWorldAPIIntegration>() == null)
                {
                    if (GUILayout.Button("Add WAPI Support"))
                    {
                        myTarget.gameObject.AddComponent<EnviroWorldAPIIntegration>();
                    }
                }
                else
                {
                    if (GUILayout.Button("Remove WAPI Support"))
                    {
                        DestroyImmediate(myTarget.gameObject.GetComponent<EnviroWorldAPIIntegration>());
                    }

                }
#else
            EditorGUILayout.LabelField("World Manager API no found!", wrapStyle);
#endif
                GUILayout.EndVertical();

                //Vegetation Studio Pro
                GUILayout.BeginVertical("Vegetation Studio Pro", boxStyleModified);
                GUILayout.Space(20);
#if VEGETATION_STUDIO_PRO
                if (myTarget.gameObject.GetComponent<EnviroVegetationStudioPro>() == null)
                {
                    if (GUILayout.Button("Add Vegetation Studio Pro Support"))
                    {
                        myTarget.gameObject.AddComponent<EnviroVegetationStudioPro>();
                    }
                }
                else
                {
                    if (GUILayout.Button("Remove Vegetation Studio Pro Support"))
                    {
                        DestroyImmediate(myTarget.gameObject.GetComponent<EnviroVegetationStudioPro>());
                    }

                }
#else
                EditorGUILayout.LabelField("Vegetation Studio Pro not found in project!", wrapStyle);
#endif
                GUILayout.EndVertical();


                //PEGASUS
                GUILayout.BeginVertical("Pegasus", boxStyleModified);
                GUILayout.Space(20);
#if ENVIRO_PEGASUS_SUPPORT
            EditorGUILayout.LabelField("Pegasus support is activated! Please use the new enviro trigger to drive enviro settings with Pegasus.");
            GUILayout.Space(20);
            if (GUILayout.Button("Deactivate Pegasus Support"))
            {
                RemoveDefineSymbol("ENVIRO_PEGASUS_SUPPORT");
            }
#else
                EditorGUILayout.LabelField("Pegasus support not activated! Please activate if you have Pegasus in your project.");
                GUILayout.Space(10);
                if (GUILayout.Button("Activate Pegasus Support"))
                {
                    AddDefineSymbol("ENVIRO_PEGASUS_SUPPORT");
                }
                if (GUILayout.Button("Deactivate Pegasus Support"))
                {
                    RemoveDefineSymbol("ENVIRO_PEGASUS_SUPPORT");
                }
#endif
                GUILayout.EndVertical();
                //////////


                //FogVolume
                GUILayout.BeginVertical("FogVolume 3", boxStyleModified);
                GUILayout.Space(20);
#if ENVIRO_FV3_SUPPORT

            if (myTarget.gameObject.GetComponent<EnviroFogVolumeIntegration>() == null)
            {
                if (GUILayout.Button("Add FogVolume Support"))
                {
                    myTarget.gameObject.AddComponent<EnviroFogVolumeIntegration>();
                }
            }
            else
            {
                if (GUILayout.Button("Remove FogVolume Support"))
                {
                    DestroyImmediate(myTarget.gameObject.GetComponent<EnviroFogVolumeIntegration>());
                }
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Deactivate FogVolume Support"))
            {
                RemoveDefineSymbol("ENVIRO_FV3_SUPPORT");
            }
#else
                EditorGUILayout.LabelField("FogVolume3 support not activated! Please activate if you have FogVolume3 package in your project.");
                GUILayout.Space(10);
                if (GUILayout.Button("Activate FogVolume Support"))
                {
                    AddDefineSymbol("ENVIRO_FV3_SUPPORT");
                }
                if (GUILayout.Button("Deactivate FogVolume Support"))
                {
                    RemoveDefineSymbol("ENVIRO_FV3_SUPPORT");
                }
                
#endif
                GUILayout.EndVertical();
                //////////
                //Aura 2
                GUILayout.BeginVertical("Aura 2", boxStyleModified);
                GUILayout.Space(20);
#if AURA_IN_PROJECT
                if (!myTarget.aura2Support)
                {
                    EditorGUILayout.LabelField("Aura 2 support is deactivated!", wrapStyle);
                    if (GUILayout.Button("Activate Aura 2 Support"))
                    {
                        myTarget.aura2Support = true;
                    }
                }
                else
                {
                    EditorGUILayout.LabelField("Aura 2 is active! Please assign your Aura 2 presets in your weather presets! Enviro will change aura preset for you on weather changes then.", wrapStyle);
                    GUILayout.Space(20);
                    GUILayout.BeginVertical("", boxStyle);
                    GUILayout.Space(5);
                    if (myTarget.IsAvailable() && myTarget.Components != null && myTarget.Components.DirectLight.GetComponent<Aura2API.AuraLight>() == null)
                    {
                        if (GUILayout.Button("Convert Enviro Directional Light to Aura2"))
                        {
                             myTarget.Components.DirectLight.gameObject.AddComponent<Aura2API.AuraLight>();

                             if(myTarget.Components.AdditionalDirectLight != null && myTarget.Components.AdditionalDirectLight.GetComponent<Aura2API.AuraLight>() == null)
                                myTarget.Components.AdditionalDirectLight.gameObject.AddComponent<Aura2API.AuraLight>();
                        }
                    }
                    else
                    {
                        myTarget.aura2DirectionalLightIntensity = EditorGUILayout.CurveField("Aura2 Directional Light Strength",myTarget.aura2DirectionalLightIntensity);     
                       
                        if(myTarget.IsAvailable() && myTarget.Components.AdditionalDirectLight != null)
                           myTarget.aura2DirectionalLightIntensityMoon = EditorGUILayout.CurveField("Aura2 Directional Light Moon Strength",myTarget.aura2DirectionalLightIntensityMoon);     
                    }
                    if (myTarget.IsAvailable() && myTarget.Camera != null && myTarget.Camera.GetComponent<Aura2API.AuraCamera>() == null)
                    {
                        if (GUILayout.Button("Add Aura2 Camera component"))
                        {
                            Aura2API.Aura.AddAuraToCameras();
                        }
                    }
                    else
                    {
                        myTarget.aura2TransitionSpeed = EditorGUILayout.FloatField("Aura2 Transition Speed", myTarget.aura2TransitionSpeed);
                    }
                    GUILayout.Space(5);
                    GUILayout.EndVertical();
                    GUILayout.Space(20);


                    if (GUILayout.Button("Deactivate Aura 2 Support"))
                    {
                        myTarget.aura2Support = false;
                    }

                }
#else
                EditorGUILayout.LabelField("Aura 2 not found in your project.", wrapStyle);
#endif
                GUILayout.EndVertical();






            }
            GUILayout.EndVertical();
             //   EditorGUILayout.EndToggleGroup();


            GUILayout.BeginVertical("", boxStyleModified);
            //myTarget.showThirdPartyShaders = EditorGUILayout.BeginToggleGroup(" Shaders", myTarget.showThirdPartyShaders);
            myTarget.showThirdPartyShaders = GUILayout.Toggle(myTarget.showThirdPartyShaders, "Shaders", headerFoldout);
            if (myTarget.showThirdPartyShaders)
            {

                //CTS
                GUILayout.BeginVertical("Complete Terrain Shader", boxStyleModified);
                GUILayout.Space(20);
#if CTS_PRESENT
            if(myTarget.gameObject.GetComponent<EnviroCTSIntegration>() == null)
            {     
                if (GUILayout.Button("Add CTS Support"))
                {
                    myTarget.gameObject.AddComponent<EnviroCTSIntegration>();
                }
            }
            else
            {
                if (GUILayout.Button("Remove WAPI Support"))
                {
                    DestroyImmediate(myTarget.gameObject.GetComponent<EnviroCTSIntegration>());
                }

            }
#else
                EditorGUILayout.LabelField("CTS not found in project!", wrapStyle);
#endif
                GUILayout.EndVertical();


                //MicroSplat
                GUILayout.BeginVertical("MicroSplat", boxStyleModified);
                GUILayout.Space(20);

#if ENVIRO_MICROSPLAT_SUPPORT

            if (myTarget.gameObject.GetComponent<EnviroMicroSplatIntegration>() == null)
            {
                if (GUILayout.Button("Add MicroSplat Support"))
                {
                    myTarget.gameObject.AddComponent<EnviroMicroSplatIntegration>();
                }
            }
            else
            {
                if (GUILayout.Button("Remove MicroSplat Support"))
                {
                    DestroyImmediate(myTarget.gameObject.GetComponent<EnviroMicroSplatIntegration>());
                }
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Deactivate MicroSplat Support"))
            {
                RemoveDefineSymbol("ENVIRO_MICROSPLAT_SUPPORT");
            }
#else
                EditorGUILayout.LabelField("MicroSplat support not activated! Please activate if you have Microsplat in your project.");
                GUILayout.Space(10);
                if (GUILayout.Button("Activate MicroSplat Support"))
                {
                    AddDefineSymbol("ENVIRO_MICROSPLAT_SUPPORT");
                }
                if (GUILayout.Button("Deactivate MicroSplat Support"))
                {
                    RemoveDefineSymbol("ENVIRO_MICROSPLAT_SUPPORT");
                }
#endif
                GUILayout.EndVertical();
                //////////

                //MegaSplat
                GUILayout.BeginVertical("MegaSplat", boxStyleModified);
                GUILayout.Space(20);
#if ENVIRO_MEGASPLAT_SUPPORT

            if (myTarget.gameObject.GetComponent<EnviroMegaSplatIntegration>() == null)
            {
                if (GUILayout.Button("Add MegaSplat Support"))
                {
                    myTarget.gameObject.AddComponent<EnviroMegaSplatIntegration>();
                }
            }
            else
            {
                if (GUILayout.Button("Remove MegaSplat Support"))
                {
                    DestroyImmediate(myTarget.gameObject.GetComponent<EnviroMegaSplatIntegration>());
                }
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Deactivate MegaSplat Support"))
            {
                RemoveDefineSymbol("ENVIRO_MEGASPLAT_SUPPORT");
            }
#else
                EditorGUILayout.LabelField("MegaSplat support not activated! Please activate if you have MegaSplat in your project.");
                GUILayout.Space(10);
                if (GUILayout.Button("Activate MegaSplat Support"))
                {
                    AddDefineSymbol("ENVIRO_MEGASPLAT_SUPPORT");
                }
                if (GUILayout.Button("Deactivate MegaSplat Support"))
                {
                    RemoveDefineSymbol("ENVIRO_MEGASPLAT_SUPPORT");
                }
#endif
                GUILayout.EndVertical();
                //////////

                //RTP
                GUILayout.BeginVertical("Relief Terrain Shader", boxStyleModified);
                GUILayout.Space(20);

#if ENVIRO_RTP_SUPPORT
            if (myTarget.gameObject.GetComponent<EnviroRTPIntegration>() == null)
            {
                if (GUILayout.Button("Add RTP Support"))
                {
                    myTarget.gameObject.AddComponent<EnviroRTPIntegration>();
                }
            }
            else
            {
                if (GUILayout.Button("Remove RTP Support"))
                {
                    DestroyImmediate(myTarget.gameObject.GetComponent<EnviroRTPIntegration>());
                }
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Deactivate RTP Support"))
            {
                RemoveDefineSymbol("ENVIRO_RTP_SUPPORT");
            }
#else
                EditorGUILayout.LabelField("Relief Terrain Shader support not activated! Please activate if you have Relief Terrain Shader package in your project.");
                GUILayout.Space(10);
                if (GUILayout.Button("Activate RTP Support"))
                {
                    AddDefineSymbol("ENVIRO_RTP_SUPPORT");
                }
                if (GUILayout.Button("Deactivate RTP Support"))
                {
                    RemoveDefineSymbol("ENVIRO_RTP_SUPPORT");
                }
#endif
                GUILayout.EndVertical();
                //////////

                //UBER
                GUILayout.BeginVertical("UBER Shaderframework", boxStyleModified);
                GUILayout.Space(20);

#if ENVIRO_UBER_SUPPORT
            if (myTarget.gameObject.GetComponent<EnviroRTPIntegration>() == null)
            {
                if (GUILayout.Button("Add UBER Support"))
                {
                    myTarget.gameObject.AddComponent<EnviroRTPIntegration>();
                }
            }
            else
            {
                if (GUILayout.Button("Remove UBER Support"))
                {
                    DestroyImmediate(myTarget.gameObject.GetComponent<EnviroRTPIntegration>());
                }
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Deactivate UBER Support"))
            {
                RemoveDefineSymbol("ENVIRO_UBER_SUPPORT");
            }
#else
                EditorGUILayout.LabelField("UBER Shader support not activated! Please activate if you have UBER Shader package in your project.");
                GUILayout.Space(10);
                if (GUILayout.Button("Activate UBER Support"))
                {
                    AddDefineSymbol("ENVIRO_UBER_SUPPORT");
                }
                if (GUILayout.Button("Deactivate UBER Support"))
                {
                    RemoveDefineSymbol("ENVIRO_UBER_SUPPORT");
                }
#endif
                GUILayout.EndVertical();
                //////////

                //LUX
                GUILayout.BeginVertical("LUX Shaderframework", boxStyleModified);
                GUILayout.Space(20);

#if ENVIRO_LUX_SUPPORT
            if (myTarget.gameObject.GetComponent<EnviroLUXIntegration>() == null)
            {
                if (GUILayout.Button("Add LUX Support"))
                {
                    myTarget.gameObject.AddComponent<EnviroLUXIntegration>();
                }
            }
            else
            {
                if (GUILayout.Button("Remove LUX Support"))
                {
                    DestroyImmediate(myTarget.gameObject.GetComponent<EnviroLUXIntegration>());
                }
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Deactivate LUX Support"))
            {
                RemoveDefineSymbol("ENVIRO_LUX_SUPPORT");
            }
#else
                EditorGUILayout.LabelField("LUX Shader support not activated! Please activate if you have LUX Shader package in your project.");
                GUILayout.Space(10);
                if (GUILayout.Button("Activate LUX Support"))
                {
                    AddDefineSymbol("ENVIRO_LUX_SUPPORT");
                }
                if (GUILayout.Button("Deactivate LUX Support"))
                {
                    RemoveDefineSymbol("ENVIRO_LUX_SUPPORT");
                }
#endif
                GUILayout.EndVertical();
                //////////

                //Global Snow
                GUILayout.BeginVertical("Global Snow", boxStyleModified);
                GUILayout.Space(20);

#if ENVIRO_GLOBALSNOW_SUPPORT
            if (myTarget.gameObject.GetComponent<GlobalSnowIntegration>() == null)
            {
                if (GUILayout.Button("Add Global Snow Support"))
                {
                    myTarget.gameObject.AddComponent<GlobalSnowIntegration>();
                }
            }
            else
            {
                if (GUILayout.Button("Remove Global Snow Support"))
                {
                    DestroyImmediate(myTarget.gameObject.GetComponent<GlobalSnowIntegration>());
                }
            }
            GUILayout.Space(20);
            if (GUILayout.Button("Deactivate Global Snow Support"))
            {
                RemoveDefineSymbol("ENVIRO_GLOBALSNOW_SUPPORT");
            }
#else
                EditorGUILayout.LabelField("Global Snow support not activated! Please activate if you have Global Snow package in your project.");
                GUILayout.Space(10);
                if (GUILayout.Button("Activate Global Snow Support"))
                {
                    AddDefineSymbol("ENVIRO_GLOBALSNOW_SUPPORT");
                }
                if (GUILayout.Button("Deactivate Global Snow Support"))
                {
                    RemoveDefineSymbol("ENVIRO_GLOBALSNOW_SUPPORT");
                }
#endif
                GUILayout.EndVertical();
                //////////



            }
            GUILayout.EndVertical();
        //EditorGUILayout.EndToggleGroup();


            GUILayout.BeginVertical("", boxStyleModified);
           // myTarget.showThirdPartyNetwork = EditorGUILayout.BeginToggleGroup(" Networking", myTarget.showThirdPartyNetwork);
            myTarget.showThirdPartyNetwork = GUILayout.Toggle(myTarget.showThirdPartyNetwork, "Networking", headerFoldout);
            if (myTarget.showThirdPartyNetwork)
            {

                //UNET
                GUILayout.BeginVertical("UNet Networking", boxStyleModified);
                GUILayout.Space(20);
#if ENVIRO_UNET_SUPPORT
            EditorGUILayout.LabelField("UNET support is activated! Please also add the EnviroUNetPlayer component to your players!");

            if (myTarget.gameObject.GetComponent<EnviroUNetServer>() == null)
            {
                if (GUILayout.Button("Add UNet Integration Component"))
                {
                    myTarget.gameObject.AddComponent<EnviroUNetServer>();
                }
            }
            else
            {
                if (GUILayout.Button("Remove UNet Integration Component"))
                {
                    DestroyImmediate(myTarget.gameObject.GetComponent<EnviroUNetServer>());
                }
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Deactivate UNet Support"))
            {
                RemoveDefineSymbol("ENVIRO_UNET_SUPPORT");
            }
#else
                EditorGUILayout.LabelField("UNet support not activated! Please activate if would like to use UNet with Enviro.");
                GUILayout.Space(10);
                if (GUILayout.Button("Activate UNet Support"))
                {
                    AddDefineSymbol("ENVIRO_UNET_SUPPORT");
                }
                if (GUILayout.Button("Deactivate UNet Support"))
                {
                    RemoveDefineSymbol("ENVIRO_UNET_SUPPORT");
                }
#endif
                GUILayout.EndVertical();
                //////////

                

                //Mirror
                GUILayout.BeginVertical("Mirror Networking", boxStyleModified);
                GUILayout.Space(20);
#if ENVIRO_MIRROR_SUPPORT
            EditorGUILayout.LabelField("Mirror support is activated! Please also add the EnviroMirrorPlayer component to your players!");

            if (GameObject.Find("/Enviro Mirror Server") == null)
            {
                if (GUILayout.Button("Add Mirror Integration Component"))
                {
                GameObject mServer = new GameObject();
                mServer.name = "Enviro Mirror Server";
                mServer.AddComponent<EnviroMirrorServer>();
                }
            }
            else
            {
                if (GUILayout.Button("Remove Mirror Integration Component"))
                {

                if(GameObject.Find("/Enviro Mirror Server") != null)
                    DestroyImmediate(GameObject.Find("/Enviro Mirror Server"));
                }
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Deactivate Mirror Support"))
            {
                RemoveDefineSymbol("ENVIRO_MIRROR_SUPPORT");
            }
#else
                EditorGUILayout.LabelField("Mirror support not activated! Please activate if would like to use Mirror with Enviro.");
                GUILayout.Space(10);
                if (GUILayout.Button("Activate Mirror Support"))
                {
                    AddDefineSymbol("ENVIRO_MIRROR_SUPPORT");
                }
                if (GUILayout.Button("Deactivate Mirror Support"))
                {
                    RemoveDefineSymbol("ENVIRO_MIRROR_SUPPORT");
                }
#endif
                GUILayout.EndVertical();
                //////////

                //Photon
                GUILayout.BeginVertical("Photon Networking", boxStyleModified);
            GUILayout.Space(20);
#if ENVIRO_PHOTON_SUPPORT
            EditorGUILayout.LabelField("Photon PUN 2 support is activated!");

            if (myTarget.gameObject.GetComponent<EnviroPhotonIntegration>() == null)
            {
                if (GUILayout.Button("Add Photon Integration Component"))
                {
                    myTarget.gameObject.AddComponent<EnviroPhotonIntegration>();
                }
            }
            else
            {
                if (GUILayout.Button("Remove Photon Integration Component"))
                {
                    DestroyImmediate(myTarget.gameObject.GetComponent<EnviroPhotonIntegration>());
                }
            }
            GUILayout.Space(10);
            if (GUILayout.Button("Deactivate Photon Support"))
            {
                RemoveDefineSymbol("ENVIRO_PHOTON_SUPPORT");
            }
#else
            EditorGUILayout.LabelField("Photon support not activated! Please activate if you have Photon PUN 2 in your project.");
            GUILayout.Space(10);
            if (GUILayout.Button("Activate Photon Support"))
            {
                AddDefineSymbol("ENVIRO_PHOTON_SUPPORT");
            }
            if (GUILayout.Button("Deactivate Photon Support"))
            {
                RemoveDefineSymbol("ENVIRO_PHOTON_SUPPORT");
            }
#endif
            GUILayout.EndVertical();
                //////////

            }
            GUILayout.EndVertical();
            //EditorGUILayout.EndToggleGroup();


        }
        // END THIRDPARTY



        // END Utilities

        GUILayout.EndVertical();
        //EditorGUILayout.EndToggleGroup();


#if ENVIRO_HD
        GUI.backgroundColor = boxColor1;
        GUILayout.BeginVertical("", boxStyleModified);
        GUI.backgroundColor = Color.white;
        // myTarget.showUtilities = EditorGUILayout.BeginToggleGroup(" Utilities", myTarget.showUtilities);
        myTarget.showUtilities = GUILayout.Toggle(myTarget.showUtilities, "Utilities", headerFoldout);
        if (myTarget.showUtilities)
        {
            GUILayout.BeginVertical("Export Sky to HDR Cubemap", boxStyleModified);
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Cubemap Resolution");
            myTarget.skyBaking.resolution = EditorGUILayout.IntPopup(myTarget.skyBaking.resolution, bakingResNames, bakingResValues);
            GUILayout.Space(5);
            if (GUILayout.Button("Bake to Cubemap"))
            {
                SelectPathForBaking(myTarget.skyBaking.resolution);
            }
            GUILayout.EndVertical();
        }
        GUILayout.EndVertical();
        // EditorGUILayout.EndToggleGroup();
#endif
        GUILayout.EndVertical();
    }
#if ENVIRO_HD
    private GUIContent[] bakingResNames = new GUIContent[] { new GUIContent("64"), new GUIContent("128"), new GUIContent("256"), new GUIContent("512"), new GUIContent("1024"), new GUIContent("2048") };
    private int[] bakingResValues = new int[] {64,128,256,512,1024,2048 };


    public void SelectPathForBaking(int res)
    {
        string path = EditorUtility.SaveFilePanelInProject("Save your baked sky cubemap.", "New BakedSkyCubemapHDR", "exr", "");

        if (string.IsNullOrEmpty(path))
            return;

        BakeToCubemap(res, path);
       // myTarget.StartCoroutine(BakeNextFrame(res, path));
       // SceneView.RepaintAll();
    }

    IEnumerator BakeNextFrame(int res, string path)
    {
        bool volumeLight = myTarget.useVolumeLighting;
        myTarget.useVolumeLighting = false;
        SceneView.RepaintAll();
        yield return null;
        SceneView.RepaintAll();
        yield return null;
      //  BakeToCubemap(res, path, volumeLight);
    }


    public void BakeToCubemap(int res,string path)
    {
        try
        {
            int current = 0;
            Texture2D tex = new Texture2D(res * 6, res, TextureFormat.RGBAFloat, false);
           
            RenderTexture save = RenderTexture.active;

            //Dirty workaround! Render first side for nothing. Otherwise the first side may be messed up as effects not setup correctly somehow sometimes... 
            myTarget.Components.GlobalReflectionProbe.BakeCubemapFace(0, res);

            for (int i = 0; i < 6; i++)
            {
                current++;
                RenderTexture face = myTarget.Components.GlobalReflectionProbe.BakeCubemapFace(i, res);
                RenderTexture.active = face;
                tex.ReadPixels(new Rect(0, 0, res, res), res * i, 0);
                EditorUtility.DisplayProgressBar("Baking Enviro Sky", "Currently Baking. Please Wait!", (float)(current / 6));
            }
            tex.Apply();

            RenderTexture.active = save;

            //Encode texture into the EXR
            byte[] bytes = tex.EncodeToEXR(Texture2D.EXRFlags.CompressZIP);
            System.IO.File.WriteAllBytes(path, bytes);

            AssetDatabase.Refresh();

            Texture assetTex = AssetDatabase.LoadAssetAtPath(path,typeof(Texture)) as Texture;

            if (assetTex == null)
                return;

            string assetpath = AssetDatabase.GetAssetPath(assetTex);

            TextureImporter import = AssetImporter.GetAtPath(assetpath) as TextureImporter;

            if (import != null)
            {
                import.textureShape = TextureImporterShape.TextureCube;
                import.generateCubemap = TextureImporterGenerateCubemap.AutoCubemap;
                import.sRGBTexture = false;
            }
            AssetDatabase.ImportAsset(assetpath, ImportAssetOptions.ForceUpdate);
            //  
            
        }
        finally
        {
         //   myTarget.useVolumeLighting = vLight;
            EditorUtility.ClearProgressBar();
        }

    }
#endif

    public void AddDefineSymbol(string symbol)
    {
        var targets = Enum.GetValues(typeof(BuildTargetGroup))
        .Cast<BuildTargetGroup>()
        .Where(x => x != BuildTargetGroup.Unknown)
        .Where(x => !IsObsolete(x));

        foreach (var target in targets)
        {
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Trim();

            var list = defines.Split(';', ' ')
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();

            if (list.Contains(symbol))
                continue;

            list.Add(symbol);
            defines = list.Aggregate((a, b) => a + ";" + b);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
        }
    }


    bool IsObsolete(BuildTargetGroup group)
    {
        var attrs = typeof(BuildTargetGroup)
            .GetField(group.ToString())
            .GetCustomAttributes(typeof(ObsoleteAttribute), false);

        return attrs != null && attrs.Length > 0;
    }

    public void ForceAddDefineSymbol(string symbol)
    {
        if(symbol == "ENVIRO_HD")
        {
            string[] path = AssetDatabase.FindAssets("Enviro Standard");

            if(path.Length > 0)
            {
                string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                List<string> allDefines = definesString.Split(';').ToList();
                allDefines.Add(symbol);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
            }
        }
        else if (symbol == "ENVIRO_LW")
        {
            string[] path = AssetDatabase.FindAssets("Enviro Lite");

            if (path.Length > 0)
            {
                string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
                List<string> allDefines = definesString.Split(';').ToList();
                allDefines.Add(symbol);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, string.Join(";", allDefines.ToArray()));
            }
        }
    }

    public void RemoveDefineSymbol(string symbol)
    {
        string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);



        var targets = Enum.GetValues(typeof(BuildTargetGroup))
        .Cast<BuildTargetGroup>()
        .Where(x => x != BuildTargetGroup.Unknown)
        .Where(x => !IsObsolete(x));

        foreach (var target in targets)
        {
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(target).Trim();

            if (defines.Contains(symbol))
            {
                defines = defines.Replace(symbol + "; ", "");
                defines = defines.Replace(symbol, "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(target, defines);
            }


        }

        /*
        if (symbols.Contains(symbol))
        {
            symbols = symbols.Replace(symbol + "; ", "");
            symbols = symbols.Replace(symbol, "");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
        }
        */
    }
}
