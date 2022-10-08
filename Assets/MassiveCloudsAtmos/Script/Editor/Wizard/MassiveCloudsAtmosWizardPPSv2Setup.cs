using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif

using Object = UnityEngine.Object;

namespace Mewlist.MassiveClouds
{
    internal static class MassiveCloudsAtmosWizardPPSv2Setup
    {
        public static void PPSv2Setup()
        {
            MassiveCloudsEditorGUI.Space();

#if UNITY_POST_PROCESSING_STACK_V2
            var ppsLayer  = Object.FindObjectOfType<PostProcessLayer>();
            var ppsVolume = Object.FindObjectsOfType<PostProcessVolume>()
                .FirstOrDefault(x =>
                    x.sharedProfile &&
                    (  x.sharedProfile.HasSettings<MassiveCloudsBeforeTransparentEffectSettings>() 
                       || x.sharedProfile.HasSettings<MassiveCloudsBeforeStackEffectSettings>()));

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("PostProcessLayer");
                if (ppsLayer)
                {
                    MassiveCloudsAtmosWizard.Ok("Ok");
                    EditorGUILayout.ObjectField(ppsLayer, typeof(PostProcessLayer), true);
                }
                else
                {
                    MassiveCloudsAtmosWizard.Ng("Setup required");
                    if (MassiveCloudsAtmosWizard.SetupButton("Install"))
                    {
                        ppsLayer = Camera.main.gameObject.AddComponent<PostProcessLayer>();
                        ppsLayer.volumeLayer = LayerMask.GetMask(new []{ "Default" });
                    }
                }
            }

            var validVolume = ppsVolume && ppsVolume.sharedProfile &&
                              (  ppsVolume.sharedProfile.HasSettings<MassiveCloudsBeforeTransparentEffectSettings>()
                                 || ppsVolume.sharedProfile.HasSettings<MassiveCloudsBeforeStackEffectSettings>());

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Global PostProcessVolume");
                if (validVolume)
                {
                    MassiveCloudsAtmosWizard.Ok("Ok");
                    EditorGUILayout.ObjectField(ppsVolume, typeof(PostProcessVolume), true);
                }
                else
                {
                    MassiveCloudsAtmosWizard.Ok("Install available");
                    if (MassiveCloudsAtmosWizard.SetupButton("Install"))
                    {
                        var globalVolume = Object.FindObjectsOfType<PostProcessVolume>().FirstOrDefault(x => x.isGlobal);
                        if (!globalVolume)
                        {
                            var go = new GameObject("PostProcessVolume");
                            globalVolume = go.AddComponent<PostProcessVolume>();
                            globalVolume.isGlobal = true;
                        }

                        var globalProfile = globalVolume.sharedProfile;
                        if (!globalProfile || (String.IsNullOrEmpty(globalProfile.name) && globalProfile.settings.Count == 0))
                        {
                            var path = EditorUtility.SaveFilePanel("Create PostProcessProfile", Application.dataPath, "MassiveCloudsPPSv2Profile", "asset");
                            path = path.Replace(Application.dataPath, "Assets");
                            if (!String.IsNullOrEmpty(path))
                            {
                                globalProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
                                AssetDatabase.CreateAsset(globalProfile, path);
                                AssetDatabase.Refresh();
                            }
                        }
                        globalVolume.sharedProfile = globalProfile;

                        if (globalVolume.sharedProfile)
                        {
                            if (!(globalProfile.HasSettings<MassiveCloudsBeforeTransparentEffectSettings>() ||
                                  globalProfile.HasSettings<MassiveCloudsBeforeStackEffectSettings>()))
                            {
                                var settings = ScriptableObject.CreateInstance<MassiveCloudsBeforeTransparentEffectSettings>();
                                settings.name = "MassiveCloudsBeforeTransparentEffectSettings";
                                settings.enabled.value = true;
                                globalProfile.AddSettings(settings);
                                AssetDatabase.AddObjectToAsset(settings, globalProfile);
                                settings.rendererParameter.overrideState = true;
                            }

                            globalVolume.sharedProfile = globalProfile;
                            EditorUtility.SetDirty(globalVolume);
                            EditorUtility.SetDirty(globalProfile);
                        }
                    }
                }
            }

            if (validVolume)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel("Renderer");
                    var settings = ppsVolume.sharedProfile.GetSetting<MassiveCloudsBeforeTransparentEffectSettings>();
                    var renderer = settings.rendererParameter.value;
                    var color = GUI.color;
                    if (renderer)
                    {
                        MassiveCloudsAtmosWizard.Ok("Ok");
                    }
                    else
                    {
                        GUI.color = new Color(1f, 0.6f, 0.6f, 1f);
                        MassiveCloudsAtmosWizard.Ng("Please select Renderer Object.");
                    }
                    settings.rendererParameter.value = EditorGUILayout.ObjectField(renderer, typeof(AbstractMassiveClouds), false) as AbstractMassiveClouds;
                    GUI.color = color;
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel("Sun Light Source");
                    var lightSource = GameObject.FindObjectOfType<MassiveCloudsSunLightSource>();
                    if (!lightSource)
                    {
                        MassiveCloudsAtmosWizard.Ng("Ng");
                        if (MassiveCloudsAtmosWizard.FixButton("Fix Now"))
                        {
                            var lights = Object.FindObjectsOfType<Light>().Where(x => x.type == LightType.Directional)
                                .OrderByDescending(x => x.intensity).ToArray();
                            if (lights.Any())
                            {
                                var light = lights.First();
                                light.gameObject.AddComponent<MassiveCloudsSunLightSource>();
                            }
                        }
                    }
                    else
                    {
                        MassiveCloudsAtmosWizard.Ok("Ok");
                        EditorGUILayout.ObjectField(lightSource, typeof(MassiveCloudsSunLightSource), true);
                    }
                }

                MassiveCloudsEditorGUI.SectionSpace();
                MassiveCloudsEditorGUI.Header("Uninstall", 2);
                MassiveCloudsEditorGUI.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("");
                    if (GUILayout.Button("Uninstall"))
                    {
                        ppsVolume = Object.FindObjectsOfType<PostProcessVolume>()
                            .FirstOrDefault(x =>
                                x.sharedProfile &&
                                (  x.sharedProfile.HasSettings<MassiveCloudsBeforeTransparentEffectSettings>() 
                                   || x.sharedProfile.HasSettings<MassiveCloudsBeforeStackEffectSettings>()));

                        if (ppsVolume)
                        {
                            var go = ppsVolume.gameObject;
                            if (ppsVolume.profile.settings.Count > 1)
                            {
                                ppsVolume.profile.RemoveSettings<MassiveCloudsBeforeTransparentEffectSettings>();
                                ppsVolume.profile.RemoveSettings<MassiveCloudsBeforeStackEffectSettings>();
                            }
                            else
                                Object.DestroyImmediate(ppsVolume);

                            var list = go.GetComponents<Component>();
                            if (list.Length == 1) Object.DestroyImmediate(go);
                        }
                    }
                }
            }

            MassiveCloudsEditorGUI.SectionSpace();
#else
            MassiveCloudsAtmosWizard.Ng("PostProcessingStackv2 is not installed. Install from PackageManager.");
#endif
        }
    }
}