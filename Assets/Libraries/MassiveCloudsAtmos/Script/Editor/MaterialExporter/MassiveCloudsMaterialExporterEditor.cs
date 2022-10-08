using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [CustomEditor(typeof(MassiveCloudsMaterialExporter))]
    public class MassiveCloudsMaterialExporterEditor : AbstractMassiveCloudsEditor
    {
        public override void OnInspectorGUI()
        {
            var exporter = target as MassiveCloudsMaterialExporter;
            var profile = serializedObject.FindProperty("stylizedCloudProfile");
            EditorGUILayout.PropertyField(profile);
            if (GUILayout.Button("Export to Material"))
            {
                var path = EditorUtility.SaveFilePanel( "Export Current Profile to Material", Application.dataPath, "MassiveClouds", "mat" );                
                exporter.SaveToMaterial(path);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}