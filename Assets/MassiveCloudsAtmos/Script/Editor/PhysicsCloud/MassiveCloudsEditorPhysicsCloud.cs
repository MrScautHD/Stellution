using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    public class MassiveCloudsEditorPhysicsCloud
    {
        private int currentSelection = 0;
        public void Inspector(SerializedObject serializedObject)
        {
            var tab = new MassiveCloudsEditorTab(24f);
            tab.AddSection("Main Cloud", MainCloud);
            tab.AddSection("Layered Cloud", LayeredCloud);
            MassiveCloudsEditorGUI.SectionSpace();
            MassiveCloudsEditorGUI.Header("Layer", 2);
            currentSelection = tab.Inspector(currentSelection, serializedObject);
            MassiveCloudsEditorGUI.SectionSpace();
        }

        private static void MainCloud(SerializedObject serializedObject)
        {
            CloudPass(serializedObject.FindProperty("physicsCloudPass"));
        }

        private static void LayeredCloud(SerializedObject serializedObject)
        {
            EditorGUILayout.HelpBox("Layered cloud has no interaction with volumetric light and shaft.", MessageType.None);
            CloudPass(serializedObject.FindProperty("layeredCloudPass"));
        }

        private static void CloudPass(SerializedProperty property)
        {
            var profile = property.FindPropertyRelative("profile");
            var densityAdjustment = property.FindPropertyRelative("densityAdjustment");
            var scattering = property.FindPropertyRelative("scattering");
            var shading = property.FindPropertyRelative("shading");
            var shadingDistance = property.FindPropertyRelative("shadingDistance");
            var resolution = property.FindPropertyRelative("resolution");
            var animation = property.FindPropertyRelative("Animation");

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(profile);
            if (!profile.objectReferenceValue)
            {
                EditorGUILayout.HelpBox("Profile is not set. This layer is disabled.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(densityAdjustment);
                EditorGUILayout.PropertyField(scattering, true);
                EditorGUILayout.PropertyField(shading, true);
                EditorGUILayout.PropertyField(shadingDistance, true);

                MassiveCloudsEditorGUI.SectionSpace();

                Animation(animation);

                MassiveCloudsEditorGUI.SectionSpace();

                MassiveCloudsEditorGUI.Header("Quality", 3);
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (GUILayout.Button("VeryLow"))
                        resolution.floatValue = 0.2f;
                    if (GUILayout.Button("Low"))
                        resolution.floatValue = 1f / 4f;
                    if (GUILayout.Button("Middle"))
                        resolution.floatValue = 1f / 3f;
                    if (GUILayout.Button("High"))
                        resolution.floatValue = 0.5f;
                    if (GUILayout.Button("VeryHigh"))
                        resolution.floatValue = 1f;
                }
                EditorGUILayout.Space();
                EditorGUILayout.PropertyField(resolution);
            }
        }

        private static void Animation(SerializedProperty property)
        {
            var scrollVelocity = property.FindPropertyRelative("ScrollVelocity");
            var phase = property.FindPropertyRelative("Phase");

            MassiveCloudsEditorGUI.Header("Animation", 3);
            MassiveCloudsEditorGUI.Space();
            EditorGUILayout.PropertyField(scrollVelocity);
            EditorGUILayout.PropertyField(phase);
        }
    }
}