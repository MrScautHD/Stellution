using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [CustomEditor(typeof(MassiveCloudsPhysicsCloudProfile))]
    public class MassiveCloudsPhysicsCloudProfileEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            MassiveCloudsEditorGUI.Title("Physics Cloud Profile");
            var sampler = serializedObject.FindProperty("Sampler");
            var shape = serializedObject.FindProperty("HorizontalShape");
            var lighting = serializedObject.FindProperty("Lighting");
            using (new EditorGUILayout.VerticalScope(MassiveCloudsEditorGUI.GroupStyle(), GUILayout.Height(36f)))
            {
                using (new MassiveCloudsEditorGUI.Scope())
                {
                    MassiveCloudsEditorGUI.SectionSpace();
                    Sampler(sampler);
                    MassiveCloudsEditorGUI.SectionSpace();
                    Shape(shape);
                    MassiveCloudsEditorGUI.SectionSpace();
                    Lighting(lighting);
                    MassiveCloudsEditorGUI.SectionSpace();
                }
            }

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private static void Sampler(SerializedProperty property)
        {
            var volumeTexture = property.FindPropertyRelative("VolumeTexture");
            var tiling = property.FindPropertyRelative("Tiling");
            var octave = property.FindPropertyRelative("Octave");
            var sculpture = property.FindPropertyRelative("Sculpture");
            var sculpture2 = property.FindPropertyRelative("Sculpture2");
            var sculpture3 = property.FindPropertyRelative("Sculpture3");
            var sculpture4 = property.FindPropertyRelative("Sculpture4");
            var warp = property.FindPropertyRelative("Warp");
            var softness = property.FindPropertyRelative("Softness");
            var nearSoftnessScale = property.FindPropertyRelative("NearSoftnessScale");
            var density = property.FindPropertyRelative("Density");
            var scale = property.FindPropertyRelative("Scale");

            MassiveCloudsEditorGUI.Header("Sampler", 2);
            MassiveCloudsEditorGUI.Space();
            EditorGUILayout.PropertyField(volumeTexture);
            EditorGUILayout.PropertyField(tiling);
            EditorGUILayout.PropertyField(octave);
            EditorGUILayout.PropertyField(sculpture, new GUIContent("Octave Multiplier"));
            EditorGUILayout.PropertyField(sculpture2, new GUIContent("Sculpture A"));
            EditorGUILayout.PropertyField(sculpture3, new GUIContent("Sculpture B"));
            EditorGUILayout.PropertyField(sculpture4, new GUIContent("Sculpture C"));
            EditorGUILayout.PropertyField(warp);
            EditorGUILayout.PropertyField(softness);
            EditorGUILayout.PropertyField(nearSoftnessScale);
            EditorGUILayout.PropertyField(density);
            EditorGUILayout.PropertyField(scale);
        }

        private static void Shape(SerializedProperty property)
        {
            var fromHeight = property.FindPropertyRelative("FromHeight");
            var toHeight = property.FindPropertyRelative("ToHeight");
            var softnessTop = property.FindPropertyRelative("SoftnessTop");
            var softnessBottom = property.FindPropertyRelative("SoftnessBottom");
            var figure = property.FindPropertyRelative("Figure");
            var maxDistance = property.FindPropertyRelative("MaxDistance");

            MassiveCloudsEditorGUI.Header("Shape", 2);
            MassiveCloudsEditorGUI.Space();
            var from = fromHeight.floatValue;
            var to = toHeight.floatValue;

            EditorGUILayout.LabelField("Height", GUILayout.Width(100));
            EditorGUILayout.MinMaxSlider(ref from, ref to, -10000, 10000);
            using (new EditorGUILayout.HorizontalScope())
            {
                from = EditorGUILayout.FloatField(from);
                EditorGUILayout.LabelField("m -", GUILayout.Width(45));
                to = EditorGUILayout.FloatField(to);
                EditorGUILayout.LabelField("m", GUILayout.Width(30));
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("Thickness", GUILayout.Width(100));
                EditorGUILayout.LabelField(string.Format("{0}m", to - from), EditorStyles.helpBox);
            }

            fromHeight.floatValue = from;
            toHeight.floatValue = to;

            EditorGUILayout.PropertyField(softnessTop);
            EditorGUILayout.PropertyField(softnessBottom);
            EditorGUILayout.PropertyField(figure);
            EditorGUILayout.PropertyField(maxDistance);
        }

        private static void Lighting(SerializedProperty property)
        {
            var intensity = property.FindPropertyRelative("Intensity");
            var quality = property.FindPropertyRelative("Quality");
            var scattering = property.FindPropertyRelative("Scattering");
            var shading = property.FindPropertyRelative("Shading");
            var shadingDistance = property.FindPropertyRelative("ShadingDistance");
            var transparency = property.FindPropertyRelative("Transparency");
            var mie = property.FindPropertyRelative("Mie");

            MassiveCloudsEditorGUI.Header("Lighting", 2);
            MassiveCloudsEditorGUI.Space();
            EditorGUILayout.PropertyField(intensity);
            EditorGUILayout.PropertyField(quality);
            EditorGUILayout.PropertyField(scattering);
            EditorGUILayout.PropertyField(shading);
            EditorGUILayout.PropertyField(shadingDistance);
            EditorGUILayout.PropertyField(transparency);
            EditorGUILayout.PropertyField(mie);
        }
    }
}