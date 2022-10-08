using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MassiveCloudsPhysicsCloud))]
    public class MassiveCloudsPhysicsCloudEditor : UnityEditor.Editor
    {
        private const string BlueNoiseGUID = "c75056e8f2490d54cbca9a58e9dd4b1f";

        private int currentSection = 0;
        private MassiveCloudsEditorTab tab;
        private MassiveCloudsEditorPhysicsCloud massiveCloudsEditorPhysicsCloud;

        void OnEnable ()
        {
            massiveCloudsEditorPhysicsCloud = new MassiveCloudsEditorPhysicsCloud();
            tab = new MassiveCloudsEditorTab();
            tab.AddSection("Lighting", MassiveCloudsEditorLighting.Inspector);
            tab.AddSection("Sky", Sky);
            tab.AddSection("Cloud", massiveCloudsEditorPhysicsCloud.Inspector);
            tab.AddSection("Atmosphere", MassiveCloudsEditorAtmosphere.Inspector);
            tab.AddSection("Fog", MassiveCloudsEditorHeightFog.Inspector);
            tab.AddSection("Advanced", MassiveCloudsEditorPhysicsCloudAdvanced.Inspector);
        }

        public static void Sky(SerializedObject serializedObject)
        {
            var skyPass = serializedObject.FindProperty("skyPass");
            var enabled = skyPass.FindPropertyRelative("Enabled");
            var skyParameter = skyPass.FindPropertyRelative("SkyParameter");

            MassiveCloudsEditorGUI.SectionSpace();
            MassiveCloudsEditorGUI.Header("Sky", 2);
            MassiveCloudsEditorGUI.Space();
            EditorGUILayout.PropertyField(enabled);
            if (enabled.boolValue)
            {
                SkyParameter(skyParameter);
            }
            MassiveCloudsEditorGUI.Space();
        }

        public static void SkyParameter(SerializedProperty skyParameter)
        {
            var exposure = skyParameter.FindPropertyRelative("SkyExposure");
            var groundColor = skyParameter.FindPropertyRelative("GroundColor");
            var saturation = skyParameter.FindPropertyRelative("Saturation");
            var sunSize = skyParameter.FindPropertyRelative("SunSize");
            var sunSizeConvergence = skyParameter.FindPropertyRelative("SunSizeConvergence");
            var atmosphereThickness = skyParameter.FindPropertyRelative("AtmosphereThickness");
            var gradation = skyParameter.FindPropertyRelative("Gradation");
            var hdri = skyParameter.FindPropertyRelative("Hdri");
            var hdriTexture = hdri.FindPropertyRelative("texture");
            var hdriExposure = hdri.FindPropertyRelative("exposure");
            var hdriMix = hdri.FindPropertyRelative("mix");

            EditorGUILayout.PropertyField(exposure);
            EditorGUILayout.PropertyField(sunSize);
            EditorGUILayout.PropertyField(sunSizeConvergence);
            EditorGUILayout.PropertyField(groundColor);
            EditorGUILayout.PropertyField(saturation);
            EditorGUILayout.PropertyField(atmosphereThickness);
            EditorGUILayout.PropertyField(gradation);

            MassiveCloudsEditorGUI.Space();
            MassiveCloudsEditorGUI.Header("HDRI", 2);

            EditorGUILayout.PropertyField(hdriTexture);
            EditorGUILayout.PropertyField(hdriExposure);
            EditorGUILayout.PropertyField(hdriMix);
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            MassiveCloudsEditorGUI.Title("Physics Renderer");
            Setup();
            var forcingFullQuality = serializedObject.FindProperty("forcingFullQuality").boolValue;
            if (forcingFullQuality) EditorGUILayout.HelpBox("!!! Forcing Full Quality is Active !!!", MessageType.Warning);
            using (new MassiveCloudsEditorGUI.Scope())
            {
                currentSection = tab.Inspector(currentSection, serializedObject);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void Setup()
        {
            var ditheringTextureRef = serializedObject.FindProperty("ditheringTexture");
            var ditheringTexture = ditheringTextureRef.objectReferenceValue as Texture2D;
            if (ditheringTexture == null)
            {
                var path = AssetDatabase.GUIDToAssetPath(BlueNoiseGUID);
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (tex == null)
                    EditorGUILayout.HelpBox("BlueNoiseTexture is not found. Try to reinstall package.", MessageType.Error);
                ditheringTextureRef.objectReferenceValue = tex;
            }
        }
    }
}