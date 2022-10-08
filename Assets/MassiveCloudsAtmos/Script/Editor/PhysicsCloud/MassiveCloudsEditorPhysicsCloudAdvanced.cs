using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    public static class MassiveCloudsEditorPhysicsCloudAdvanced
    {
        public static void Inspector(SerializedObject serializedObject)
        {
            MassiveCloudsEditorGUI.SectionSpace();

            MassiveCloudsEditorGUI.Header("Buffer Texture Format", 2);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("bufferFormat"));
            EditorGUILayout.HelpBox("Higher bit format reduces Mach Band", MessageType.Info);

            MassiveCloudsEditorGUI.SectionSpace();

            MassiveCloudsEditorGUI.Header("Sun Intensity Scale", 2);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sunIntensityScale"));

            MassiveCloudsEditorGUI.SectionSpace();

            MassiveCloudsEditorGUI.Header("Progressive Rendering of Atmosphere", 2);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("progressive"));

            MassiveCloudsEditorGUI.SectionSpace();

            MassiveCloudsEditorGUI.Header("Adaptive Sampling of Atmosphere", 2);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("adaptiveSampling"));
            EditorGUILayout.HelpBox("If you have artifact on the edge of scene object, increase this value", MessageType.Info);

            MassiveCloudsEditorGUI.SectionSpace();

            MassiveCloudsEditorGUI.Header("Quality", 2);
            EditorGUILayout.Space();
            EditorGUI.indentLevel += 1;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("unityQualities"), true);
            EditorGUI.indentLevel -= 1;
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("forcingFullQuality"));
            EditorGUILayout.HelpBox(
                "All quality settings are forcefully set to the highest quality.",
                MessageType.Info,
                true);

            MassiveCloudsEditorGUI.SectionSpace();
        }
    }
}