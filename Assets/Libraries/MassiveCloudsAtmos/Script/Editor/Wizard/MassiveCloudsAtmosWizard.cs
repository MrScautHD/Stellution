using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Mewlist.MassiveClouds
{
    public class MassiveCloudsAtmosWizard : ScriptableWizard
    {
        private int currentTab = 0;
        private int currentSceneSetupTab = 0;
        private MassiveCloudsEditorTab massiveCloudsEditorTab;
        private MassiveCloudsEditorTab sceneSetupTab;
        internal static PackageInfoFinder URPPackageFinder;
        internal static PackageInfoFinder HDRPPackageFinder;

        [MenuItem("Window/Massive Clouds Atmos/Setup Wizard")]
        public static void CreateWizard()
        {
            DisplayWizard<MassiveCloudsAtmosWizard>("Massive Clouds Atmos Setup Wizard", "Close");
        }

        protected override bool DrawWizardGUI()
        {
            if (URPPackageFinder == null)
            {
                URPPackageFinder = new PackageInfoFinder();
                URPPackageFinder.Find("com.unity.render-pipelines.universal", info =>
                {
                    Repaint();
                });
            }
            
            if (HDRPPackageFinder == null)
            {
                HDRPPackageFinder = new PackageInfoFinder();
                HDRPPackageFinder.Find("com.unity.render-pipelines.high-definition", info =>
                {
                    Repaint();
                });
            }
            
            var result = base.DrawWizardGUI();
            if (massiveCloudsEditorTab == null)
            {
                massiveCloudsEditorTab = new MassiveCloudsEditorTab();
                massiveCloudsEditorTab.AddSection("1. Project Setting", ProjectTab);
                massiveCloudsEditorTab.AddSection("2. Scene Setup", SceneTab);
            }

            MassiveCloudsEditorGUI.Title("Massive Clouds Atmos Setup");
            using (new MassiveCloudsEditorGUI.Scope())
            {
                currentTab = massiveCloudsEditorTab.Inspector(currentTab, null);
            }
            MassiveCloudsEditorGUI.Space();
            return result;
        }

        private void ProjectTab(SerializedObject obj)
        {
            MassiveCloudsAtmosWizardCommon.CommonInfo();
            GraphicsSettings(obj);
        }

        private void SceneTab(SerializedObject obj)
        {
            MassiveCloudsAtmosWizardCommon.CommonInfo();
            
            var info = new EnvironmentDetector();
            sceneSetupTab = new MassiveCloudsEditorTab();
            if (info.CameraEffectModeSupported)
                sceneSetupTab.AddSection("Camera Effect Setup", MassiveCloudsAtmosWizardCameraEffectSetup.CameraEffectSetup);
            if (info.PPSv2ModeSupported)
                sceneSetupTab.AddSection("PPSv2 Setup", MassiveCloudsAtmosWizardPPSv2Setup.PPSv2Setup);
            if (info.RendererFeatureSupported)
                sceneSetupTab.AddSection("Scriptable Renderer Feature Setup", MassiveCloudsAtmosWizardScriptableRendererFeatureSetup.Setup);
            if (info.HDRPSkySupported)
                sceneSetupTab.AddSection("HDRP Sky Setup", MassiveCloudsAtmosWizardHDRPSkySetup.Setup);
            if (info.HDRPCustomPassSupported)
                sceneSetupTab.AddSection("HDRP CustomPass Setup", MassiveCloudsAtmosWizardHDRPCustomPassSetup.Setup);
            
            if (sceneSetupTab.Count > 1)
                MassiveCloudsEditorGUI.Header("Select Setup Mode");
            currentSceneSetupTab = sceneSetupTab.Inspector(currentSceneSetupTab);
        }

        private static void GraphicsSettings(SerializedObject obj)
        {
            var info = new EnvironmentDetector();
            MassiveCloudsEditorGUI.Header("Graphics Settings", 2);
            MassiveCloudsEditorGUI.Space();
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel("Preloaded Shaders");
                if (!info.ValidPreloadedShaders)
                {
                    Ng("Setup Required");
                    if (FixButton("Fix Now")) FixShaderVariantCollection();
                }
                else
                {
                    Ok("Ok");
                }
            }
        }

        private static void FixShaderVariantCollection()
        {
            var graphicsSettingsObj = AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
            var serializedObject = new SerializedObject(graphicsSettingsObj);
            var arrayProp = serializedObject.FindProperty("m_PreloadedShaders");

            var shaderVariantCollectionPath = AssetDatabase.GUIDToAssetPath(EnvironmentDetector.ShaderVariantCollectionGuid);
            var shaderVariantCollection = AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(shaderVariantCollectionPath);

            var index = arrayProp.arraySize;
            arrayProp.InsertArrayElementAtIndex(index);
            var item = arrayProp.GetArrayElementAtIndex(index);

            item.objectReferenceValue = shaderVariantCollection;

            serializedObject.ApplyModifiedProperties();
        }

        internal static void Ok(string text)
        {
            var color = GUI.color;
            GUI.color = Color.green;
            EditorGUILayout.LabelField(text);
            GUI.color = color;
        }

        internal static void Ng(string text)
        {
            var color = GUI.color;
            GUI.color = new Color(1f, 0.6f, 0.6f, 1f);
            EditorGUILayout.LabelField(text);
            GUI.color = color;
        }

        internal static void Warn(string text)
        {
            var color = GUI.color;
            GUI.color = new Color(1f, 1f, 0.4f, 1f);
            EditorGUILayout.LabelField(text);
            GUI.color = color;
        }

        internal static bool FixButton(string text)
        {
            var color = GUI.backgroundColor;
            GUI.backgroundColor = new Color(1f, 0.6f, 0.6f, 1f);
            var result = GUILayout.Button(text);
            GUI.backgroundColor = color;
            return result;
        }

        internal static bool SetupButton(string text)
        {
            var color = GUI.backgroundColor;
            GUI.backgroundColor = new Color(0f, 1f, 0f, 1f);
            var result = GUILayout.Button(text);
            GUI.backgroundColor = color;
            return result;
        }

        public void OnWizardCreate()
        {
            
        }

        public void OnWizardUpdate()
        {

        }
    }
}