using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    public static class MassiveCloudsEditorHeightFog
    {
        public static void Inspector(SerializedObject serializedObject)
        {
            MassiveCloudsEditorGUI.SectionSpace();
            HeightFog(serializedObject.FindProperty("atmospherePass").FindPropertyRelative("fog"));
            MassiveCloudsEditorGUI.SectionSpace();
        }

        public static void HeightFog(SerializedProperty property)
        {
            var groundHeight = property.FindPropertyRelative("groundHeight");
            var heightFogRange = property.FindPropertyRelative("range");
            var heightFogDensity = property.FindPropertyRelative("density");
            var heightFogColoring = property.FindPropertyRelative("coloring");
            var heightFogColor = property.FindPropertyRelative("color");
            var heightFogScattering = property.FindPropertyRelative("scattering");

            MassiveCloudsEditorGUI.Header("Shape", 2);
            MassiveCloudsEditorGUI.Space();
            EditorGUILayout.PropertyField(groundHeight);
            EditorGUILayout.PropertyField(heightFogRange);
            EditorGUILayout.PropertyField(heightFogDensity);

            MassiveCloudsEditorGUI.SectionSpace();

            MassiveCloudsEditorGUI.Header("Coloring", 2);
            MassiveCloudsEditorGUI.Space();
            EditorGUILayout.PropertyField(heightFogColoring);
            EditorGUILayout.PropertyField(heightFogColor);
            EditorGUILayout.PropertyField(heightFogScattering);
        }
    }
}