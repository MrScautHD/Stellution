using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    internal static class MassiveCloudsAtmosWizardCameraEffectSetup
    {
        public static void CameraEffectSetup()
        {
            MassiveCloudsEditorGUI.Space();

            var cameraEffect = Object.FindObjectOfType<MassiveCloudsCameraEffect>();

            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Camera Effect");
                if (cameraEffect)
                {
                    MassiveCloudsAtmosWizard.Ok("Ok");
                    EditorGUILayout.ObjectField(cameraEffect, typeof(MassiveCloudsCameraEffect), false);
                }
                else
                {
                    MassiveCloudsAtmosWizard.Ok("Install available");
                    if (MassiveCloudsAtmosWizard.SetupButton("Install"))
                        Camera.main.gameObject.AddComponent<MassiveCloudsCameraEffect>();
                }
            }

            if (cameraEffect)
            {
                var cameraEffectSerializedObject = new SerializedObject(cameraEffect);
                var massiveClouds = cameraEffectSerializedObject.FindProperty("massiveClouds");
                var sun = cameraEffectSerializedObject.FindProperty("sun");

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel("Renderer");

                    var renderer = massiveClouds.objectReferenceValue as AbstractMassiveClouds;
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
                    massiveClouds.objectReferenceValue = EditorGUILayout.ObjectField(renderer, typeof(AbstractMassiveClouds), false);
                    GUI.color = color;
                }

                using (new EditorGUILayout.HorizontalScope())
                {
                    EditorGUILayout.PrefixLabel("Sun Light Source");

                    var sunLight = sun.objectReferenceValue as Light;
                    if (sunLight)
                    {
                        MassiveCloudsAtmosWizard.Ok("Ok");
                        sun.objectReferenceValue = EditorGUILayout.ObjectField(sunLight, typeof(Light), true);
                    }
                    else
                    {
                        MassiveCloudsAtmosWizard.Ng("Please set Directional Light.");
                        if (MassiveCloudsAtmosWizard.FixButton("Fix Now"))
                        {
                            var lights = Object.FindObjectsOfType<Light>().Where(x => x.type == LightType.Directional)
                                .OrderByDescending(x => x.intensity).ToArray();
                            if (lights.Any()) sun.objectReferenceValue = lights.First();
                        }
                    }
                }

                MassiveCloudsEditorGUI.SectionSpace();
                MassiveCloudsEditorGUI.Header("Uninstall", 2);
                MassiveCloudsEditorGUI.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("");
                    if (GUILayout.Button("Uninstall"))
                        Object.DestroyImmediate(cameraEffect);
                }
                MassiveCloudsEditorGUI.SectionSpace();

                if (cameraEffectSerializedObject.targetObject)
                    cameraEffectSerializedObject.ApplyModifiedProperties();
            }
        }

        public static void ScriptableRendererFeatureSetup()
        {
            throw new System.NotImplementedException();
        }
    }
}