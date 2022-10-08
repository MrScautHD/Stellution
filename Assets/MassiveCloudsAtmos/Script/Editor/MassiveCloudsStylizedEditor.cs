using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [CustomEditor(typeof(MassiveCloudsStylizedCloud))]
    public class MassiveCloudsStylizedEditor : AbstractMassiveCloudsEditor
    {
        ReorderableList reorderableList;
        private int currentSection = 0;
        private MassiveCloudsEditorTab tab;

        void OnEnable ()
        {
            var profiles = serializedObject.FindProperty("profiles");
            reorderableList = new ReorderableList (serializedObject, profiles);
            reorderableList.drawHeaderCallback = (rect) =>
                EditorGUI.LabelField (rect, "Layers");
            reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
            {
                SerializedProperty property = profiles.GetArrayElementAtIndex(index);
                EditorGUI.PropertyField(rect, property, new GUIContent(string.Format("Layer {0}", index + 1)));
            };
            reorderableList.onAddCallback += (list) => {

                //要素を追加
                profiles.arraySize++;

                //最後の要素を選択状態にする
                list.index = profiles.arraySize - 1;
            };

            tab = new MassiveCloudsEditorTab();
            tab.AddSection("Lighting", MassiveCloudsEditorLighting.StylizedInspector);
            tab.AddSection("Cloud", Cloud);
            tab.AddSection("Fog", Fog);
        }

        public override void OnInspectorGUI()
        {
            MassiveCloudsEditorGUI.Title("Stylized Renderer");
            serializedObject.Update();
            using (new MassiveCloudsEditorGUI.Scope())
            {
                currentSection = tab.Inspector(currentSection, serializedObject);
            }
            serializedObject.ApplyModifiedProperties();
        }

        public void Cloud(SerializedObject serializedObject)
        {
            var massiveClouds = target as MassiveCloudsStylizedCloud;

            var resolutionProperty = serializedObject.FindProperty("resolution");
            EditorGUILayout.PropertyField(resolutionProperty);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("volumetricShadowPass").FindPropertyRelative("volumetricShadowResolution"));
            var durationProperty = serializedObject.FindProperty("duration");
            EditorGUILayout.PropertyField(durationProperty, new GUIContent("Switch Duration"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudColorMode"),
                new GUIContent("Cloud Color Type"));
            if (serializedObject.FindProperty("cloudColorMode").intValue == 0)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudColor"));
            if (serializedObject.FindProperty("cloudColorMode").intValue == 2)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("cloudLuminanceFix"));

            EditorGUILayout.PropertyField(serializedObject.FindProperty("shadowColorMode"),
                new GUIContent("Shadow Color Type"));
            if (serializedObject.FindProperty("shadowColorMode").intValue == 0)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("shadowColor"));

            GUILayout.Space(8f);
            reorderableList.DoLayoutList ();
            GUILayout.Space(16f);

            var parameters = serializedObject.FindProperty("parameters");
            for (var i=0; i<parameters.arraySize; i++)
            {
                if (i != reorderableList.index) continue;

                if (GUILayout.Button("↑ Save to Profile ↑"))
                    massiveClouds.SaveToProfile();
                var parameter = parameters.GetArrayElementAtIndex(i);
                ProfileGUI(parameter);
            }
        }

        public void Fog(SerializedObject serializedObject)
        {
            var fogPass = serializedObject.FindProperty("fogPass");
            EditorGUILayout.PropertyField(fogPass.FindPropertyRelative("heightFog"));
            if (fogPass.FindPropertyRelative("heightFog").boolValue)
            {
                EditorGUILayout.PropertyField(fogPass.FindPropertyRelative("fogColorMode"),
                    new GUIContent("Fog Color Type"));
                if (fogPass.FindPropertyRelative("fogColorMode").intValue == 0)
                    EditorGUILayout.PropertyField(fogPass.FindPropertyRelative("fogColor"));
                if (fogPass.FindPropertyRelative("fogColorMode").intValue == 2)
                    EditorGUILayout.PropertyField(fogPass.FindPropertyRelative("fogLuminanceFix"));
                EditorGUILayout.PropertyField(fogPass.FindPropertyRelative("groundHeight"));
                EditorGUILayout.PropertyField(fogPass.FindPropertyRelative("heightFogFromDistance"));
                EditorGUILayout.PropertyField(fogPass.FindPropertyRelative("heightFogRange"));
                EditorGUILayout.PropertyField(fogPass.FindPropertyRelative("farHeightFogRange"));
                EditorGUILayout.PropertyField(fogPass.FindPropertyRelative("heightFogDensity"));
            }
        }
    }
}