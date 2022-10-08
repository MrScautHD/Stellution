using UnityEditor;

namespace Mewlist.MassiveClouds
{
    [CustomEditor(typeof(MassiveCloudsCameraEffect))]
    public class MassiveCloudsCameraEffectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            MassiveCloudsEditorGUI.Title("Camera Effect");
            MassiveCloudsEditorGUI.Space();
            using (new EditorGUILayout.VerticalScope(MassiveCloudsEditorGUI.GroupStyle()))
            {
                using (new MassiveCloudsEditorGUI.Scope())
                {
                    base.OnInspectorGUI();
                }
            }
        }
    }
}