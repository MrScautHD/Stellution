using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [CustomEditor(typeof(AtmosPad))]
    public class AtmosPadEditor : Editor
    {
        private AtmosPadController atmosPadController;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            using (new MassiveCloudsEditorGUI.Scope())
            {
                DrawPadSetting();
                DrawAtmosPad();
            }

            serializedObject.ApplyModifiedProperties();
            var atmosPad = target as AtmosPad;
            if (atmosPad != null) atmosPad.UpdateParameters();
        }

        private void DrawPadSetting()
        {
            var massiveClouds = serializedObject.FindProperty("massiveClouds");
            var sun = serializedObject.FindProperty("sun");
            var earthTilt = serializedObject.FindProperty("earthTilt");
            var earthAxis = serializedObject.FindProperty("earthAxis");
            var enableSunControl = serializedObject.FindProperty("enableSunControl");
            var enableSkyControl = serializedObject.FindProperty("enableSkyControl");
            var updateEnvironmentOnChange = serializedObject.FindProperty("updateEnvironmentOnChange");
            using (new EditorGUILayout.VerticalScope(MassiveCloudsEditorGUI.GroupStyle(), GUILayout.Height(36f)))
            {
                GUI.color = Color.white;
                MassiveCloudsEditorGUI.Title("Atmos Pad");
                EditorGUILayout.PropertyField(massiveClouds);
                EditorGUILayout.PropertyField(updateEnvironmentOnChange);
                EditorGUILayout.PropertyField(enableSkyControl);
                EditorGUILayout.PropertyField(enableSunControl);
                if (enableSunControl.boolValue)
                {
                    EditorGUILayout.PropertyField(sun);
                    if (sun.objectReferenceValue == null)
                        EditorGUILayout.HelpBox("Set scene directional light", MessageType.Error);
                    EditorGUILayout.PropertyField(earthAxis);
                    EditorGUILayout.PropertyField(earthTilt);
                }
                MassiveCloudsEditorGUI.Space();
            }
        }

        private void DrawAtmosPad()
        {
            if (atmosPadController == null) atmosPadController = new AtmosPadController(this);
            atmosPadController.Update(serializedObject);
            atmosPadController.Inspector(serializedObject);
        }
    }
}