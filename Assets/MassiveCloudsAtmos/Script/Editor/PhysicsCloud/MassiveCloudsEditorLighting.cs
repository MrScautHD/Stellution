using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mewlist.MassiveClouds
{
    public static class MassiveCloudsEditorLighting
    {
        public static void Inspector(SerializedObject serializedObject)
        {
            MassiveCloudsEditorGUI.SectionSpace();
            MassiveCloudsEditorGUI.Header("Cloud Intensity", 2);
            MassiveCloudsEditorGUI.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudIntensityAdjustment"));
            MassiveCloudsEditorGUI.Space();
            
            MassiveCloudsEditorGUI.SectionSpace();
            MassiveCloudsEditorGUI.Header("Sun Lighting", 2);
            Sun(serializedObject.FindProperty("sun"));
            MassiveCloudsEditorGUI.Space();

            MassiveCloudsEditorGUI.SectionSpace();

            MassiveCloudsEditorGUI.Header("Moon Lighting", 2);
            Moon(serializedObject.FindProperty("moon"));
            MassiveCloudsEditorGUI.Space();

            MassiveCloudsEditorGUI.SectionSpace();

            MassiveCloudsEditorGUI.Header("Ambient Lighting", 2);
            AmbientLighting(serializedObject.FindProperty("ambient"));
            MassiveCloudsEditorGUI.Space();
        }

        public static void Sun(SerializedProperty property)
        {
            var modeProperty      = property.FindPropertyRelative("mode");
            var rotationProperty  = property.FindPropertyRelative("rotation");
            var intensityProperty = property.FindPropertyRelative("intensity");
            var colorProperty     = property.FindPropertyRelative("color");

            modeProperty.intValue = GUILayout.Toolbar(
                modeProperty.intValue,
                new []{"Auto", "Manual"},
                GUILayout.Height(24f));
            EditorGUILayout.Space();

            var mode   = (MassiveCloudsLight.LightSourceMode)modeProperty.intValue;
            var isAuto = mode == MassiveCloudsLight.LightSourceMode.Auto;

            if (!isAuto)
            {
                using (new EditorGUI.DisabledGroupScope(isAuto))
                {
                    EditorGUILayout.PropertyField(rotationProperty);
                    EditorGUILayout.PropertyField(intensityProperty);
                    EditorGUILayout.PropertyField(colorProperty);
                }
            }
        }

        public static void Moon(SerializedProperty property)
        {
            var modeProperty      = property.FindPropertyRelative("mode");
            var rotationProperty  = property.FindPropertyRelative("rotation");
            var intensityProperty = property.FindPropertyRelative("intensity");
            var colorProperty     = property.FindPropertyRelative("color");

            modeProperty.intValue = GUILayout.Toolbar(
                modeProperty.intValue,
                new []{"Auto", "Manual"},
                GUILayout.Height(24f));
            EditorGUILayout.Space();

            var mode   = (MassiveCloudsLight.LightSourceMode)modeProperty.intValue;
            var isAuto = mode == MassiveCloudsLight.LightSourceMode.Auto;

            if (!isAuto)
            {
                using (new EditorGUI.DisabledGroupScope(isAuto))
                {
                    EditorGUILayout.PropertyField(rotationProperty);
                }
            }
            EditorGUILayout.PropertyField(intensityProperty);
            EditorGUILayout.PropertyField(colorProperty);
            EditorGUILayout.HelpBox("Intensity and color are required to set manually even if AutoMode selected.", MessageType.Info);
        }

        public static void AmbientLighting(SerializedProperty property)
        {
            var modeProperty         = property.FindPropertyRelative("mode");
            var skyColorProperty     = property.FindPropertyRelative("skyColor");
            var equatorColorProperty = property.FindPropertyRelative("equatorColor");
            var groundColorProperty  = property.FindPropertyRelative("groundColor");
            var luminanceFixProperty = property.FindPropertyRelative("luminanceFix");
            var pivot = property.FindPropertyRelative("pivot");
            
            modeProperty.intValue = GUILayout.Toolbar(
                modeProperty.intValue,
                new []{"Auto", "Manual"},
                GUILayout.Height(24f));

            var mode   = (MassiveCloudsAmbient.AmbientMode)modeProperty.intValue;
            var isAuto = mode == MassiveCloudsAmbient.AmbientMode.Auto;

            if (isAuto)
                EditorGUILayout.HelpBox("Lighting Mode is set as " + RenderSettings.ambientMode + " in Lighting Settings", MessageType.None);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(pivot);
            EditorGUILayout.PropertyField(luminanceFixProperty);
            if (!isAuto)
            {
                EditorGUILayout.PropertyField(skyColorProperty);
                EditorGUILayout.PropertyField(equatorColorProperty);
                EditorGUILayout.PropertyField(groundColorProperty);
            }

            if (isAuto)
            {
                switch (RenderSettings.ambientMode)
                {
                    case AmbientMode.Skybox:
                        EditorGUILayout.HelpBox("Colors are determined from AmbientProbe. If you want update AmbientProbe in EditMode press Manually Update GI Button.", MessageType.Info);
                        if (GUILayout.Button("Manually Update GI"))
                            DynamicGI.UpdateEnvironment();
                        break;
                    case AmbientMode.Trilight:
                        EditorGUILayout.HelpBox("Colors are determined from Trilight colors in Lighting Settings.", MessageType.Info);
                        break;
                    case AmbientMode.Flat:
                        EditorGUILayout.HelpBox("Colors are determined from Ambient Color in Lighting Settings.", MessageType.Info);
                        break;
                    case AmbientMode.Custom:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static void StylizedInspector(SerializedObject serializedObject)
        {
            MassiveCloudsEditorGUI.SectionSpace();
            
            MassiveCloudsEditorGUI.Header("Sun Lighting", 2);
            Sun(serializedObject.FindProperty("sun"));
            MassiveCloudsEditorGUI.Space();

            MassiveCloudsEditorGUI.SectionSpace();

            MassiveCloudsEditorGUI.Header("Moon Lighting", 2);
            Moon(serializedObject.FindProperty("moon"));
            MassiveCloudsEditorGUI.Space();

            MassiveCloudsEditorGUI.SectionSpace();

            MassiveCloudsEditorGUI.Header("Ambient Lighting", 2);
            StylizedAmbientLighting(serializedObject.FindProperty("ambient"));
            MassiveCloudsEditorGUI.Space();
        }

        public static void StylizedAmbientLighting(SerializedProperty property)
        {
            var modeProperty         = property.FindPropertyRelative("mode");
            var skyColorProperty     = property.FindPropertyRelative("skyColor");
            var equatorColorProperty = property.FindPropertyRelative("equatorColor");
            var groundColorProperty  = property.FindPropertyRelative("groundColor");
            var luminanceFixProperty = property.FindPropertyRelative("luminanceFix");
            
            modeProperty.intValue = GUILayout.Toolbar(
                modeProperty.intValue,
                new []{"Auto", "Manual"},
                GUILayout.Height(24f));

            var mode   = (MassiveCloudsAmbient.AmbientMode)modeProperty.intValue;
            var isAuto = mode == MassiveCloudsAmbient.AmbientMode.Auto;

            if (isAuto)
                EditorGUILayout.HelpBox("Lighting Mode is set as " + RenderSettings.ambientMode + " in Lighting Settings", MessageType.None);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(luminanceFixProperty);
            if (!isAuto)
            {
                EditorGUILayout.PropertyField(skyColorProperty);
                EditorGUILayout.PropertyField(equatorColorProperty);
                EditorGUILayout.PropertyField(groundColorProperty);
            }

            if (isAuto)
            {
                switch (RenderSettings.ambientMode)
                {
                    case AmbientMode.Skybox:
                        EditorGUILayout.HelpBox("Colors are determined from AmbientProbe. If you want update AmbientProbe in EditMode press Manually Update GI Button.", MessageType.Info);
                        if (GUILayout.Button("Manually Update GI"))
                            DynamicGI.UpdateEnvironment();
                        break;
                    case AmbientMode.Trilight:
                        EditorGUILayout.HelpBox("Colors are determined from Trilight colors in Lighting Settings.", MessageType.Info);
                        break;
                    case AmbientMode.Flat:
                        EditorGUILayout.HelpBox("Colors are determined from Ambient Color in Lighting Settings.", MessageType.Info);
                        break;
                    case AmbientMode.Custom:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}