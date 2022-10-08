using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    public static class MassiveCloudsEditorAtmosphere
    {
        public static void Inspector(SerializedObject serializedObject)
        {
            MassiveCloudsEditorGUI.SectionSpace();
            var pass = serializedObject.FindProperty("atmospherePass");
            AtmosphericScattering(pass.FindPropertyRelative("atmosphere"));
            Quality(pass);
            MassiveCloudsEditorGUI.SectionSpace();
        }

        public static void AtmosphericScattering(SerializedProperty property)
        {
            var atmosphere = property.FindPropertyRelative("atmosphere");
            var atmosphereColoring = property.FindPropertyRelative("atmosphereColoring");
            var atmosphereColor = property.FindPropertyRelative("atmosphereColor");
            var atmosphereHighLightColoring = property.FindPropertyRelative("atmosphereHighLightColoring");
            var atmosphereHighLightColor = property.FindPropertyRelative("atmosphereHighLightColor");
            var cloudOcclusion = property.FindPropertyRelative("cloudOcclusion");
            var cloudAtmospheric = property.FindPropertyRelative("cloudAtmospheric");
            var godRay = property.FindPropertyRelative("godRay");
            var godRayStartDistance = property.FindPropertyRelative("godRayStartDistance");
            var shadow = property.FindPropertyRelative("shadow");
            var sunShaft = property.FindPropertyRelative("sunShaft");

            MassiveCloudsEditorGUI.Header("Atmospheric Scattering", 2);
            MassiveCloudsEditorGUI.Space();
            EditorGUILayout.PropertyField(atmosphere);
            var isAtmosphereActive = atmosphere.floatValue > 0f;

            EditorGUILayout.PropertyField(shadow);
            EditorGUILayout.PropertyField(sunShaft);

            MassiveCloudsEditorGUI.SectionSpace();


            MassiveCloudsEditorGUI.Header("Coloring", 2);
            MassiveCloudsEditorGUI.Space();
            EditorGUILayout.PropertyField(atmosphereColoring);
            EditorGUILayout.PropertyField(atmosphereColor);
            EditorGUILayout.PropertyField(atmosphereHighLightColoring);
            EditorGUILayout.PropertyField(atmosphereHighLightColor);

            MassiveCloudsEditorGUI.SectionSpace();

            MassiveCloudsEditorGUI.Header("Cloud Interaction", 2);
            MassiveCloudsEditorGUI.Space();
            EditorGUILayout.PropertyField(cloudOcclusion);
            EditorGUILayout.PropertyField(cloudAtmospheric);

            MassiveCloudsEditorGUI.SectionSpace();

            MassiveCloudsEditorGUI.Header("God Ray", 2);
            MassiveCloudsEditorGUI.Space();
            EditorGUILayout.PropertyField(godRay);
            if (!isAtmosphereActive && godRay.floatValue > 0)
                EditorGUILayout.HelpBox(
                    "God Ray is rendered with atmospheric scattering. Please adjust atmosphere value",
                    MessageType.Warning);
            EditorGUILayout.PropertyField(godRayStartDistance);

            MassiveCloudsEditorGUI.SectionSpace();
        }

        private static void Quality(SerializedProperty property)
        {
            var shaftQuality = property.FindPropertyRelative("shaftQuality");
            var godRayQuality = property.FindPropertyRelative("godRayQuality");
            var resolution = property.FindPropertyRelative("resolution");
            var screenBlending = property.FindPropertyRelative("screenBlending");

            MassiveCloudsEditorGUI.Header("Quality", 2);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("VeryLow"))
                {
                    shaftQuality.floatValue = 0.01f;
                    godRayQuality.floatValue = 0.5f;
                    resolution.floatValue = 0.2f;
                }
                if (GUILayout.Button("Low"))
                {
                    shaftQuality.floatValue = 0.1f;
                    godRayQuality.floatValue = 0.5f;
                    resolution.floatValue = 1f / 4f;
                }
                if (GUILayout.Button("Middle"))
                {
                    shaftQuality.floatValue = 0.25f;
                    godRayQuality.floatValue = 0.75f;
                    resolution.floatValue = 1f / 3f;
                }
                if (GUILayout.Button("High"))
                {
                    shaftQuality.floatValue = 0.5f;
                    godRayQuality.floatValue = 0.8f;
                    resolution.floatValue = 0.5f;
                }
                if (GUILayout.Button("VeryHigh"))
                {
                    shaftQuality.floatValue = 1f;
                    godRayQuality.floatValue = 1f;
                    resolution.floatValue = 1f;
                }
            }
            MassiveCloudsEditorGUI.Space();
            EditorGUILayout.PropertyField(shaftQuality);
            EditorGUILayout.PropertyField(godRayQuality);
            EditorGUILayout.PropertyField(resolution);

            MassiveCloudsEditorGUI.Space();
            EditorGUILayout.PropertyField(screenBlending);
        }
    }
}