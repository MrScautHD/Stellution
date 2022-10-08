using UnityEditor;

namespace Mewlist.MassiveClouds
{
    [CustomEditor(typeof(MassiveCloudsStylizedCloudProfile))]
    public class MassiveCloudsProfileEditor : AbstractMassiveCloudsEditor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var parameter = serializedObject.FindProperty("Parameter");

            ProfileGUI(parameter);
        }
    }
}