using System.Collections.ObjectModel;
using UnityEditor;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mewlist.MassiveClouds
{
    public class EnvironmentDetector
    {
        public static string ShaderVariantCollectionGuid
        {
            get
            {
                if (DetectPipeline() == PipelineType.HDRP)
                    return "6db3415097d05d44296585c60b3c1955";
                else
                    return "afc3e5abc7d7844c5a851506db499008";
            }
        }

        public enum PipelineType
        {
            StandardRP,
            UniversalRP,
            HDRP,
            Unknown
        }

        public readonly UnityVersionInfo UnityVersion;
        public readonly PipelineType Pipeline;
        public readonly bool ValidPreloadedShaders;

        public readonly bool CameraEffectModeSupported;
        public readonly bool PPSv2ModeSupported;
        public readonly bool RendererFeatureSupported;
        public readonly bool HDRPSkySupported;
        public readonly bool HDRPCustomPassSupported;

        public EnvironmentDetector()
        {
            UnityVersion = new UnityVersionInfo();
            Pipeline = DetectPipeline();
            ValidPreloadedShaders = ValidateContainsShaderVariantCollection();
            CameraEffectModeSupported = Pipeline == PipelineType.StandardRP;
            PPSv2ModeSupported = Pipeline == PipelineType.StandardRP;
            RendererFeatureSupported = Pipeline == PipelineType.UniversalRP;
            HDRPSkySupported = Pipeline == PipelineType.HDRP;
            HDRPCustomPassSupported = Pipeline == PipelineType.HDRP;
        }

        private static bool ValidateContainsShaderVariantCollection()
        {
            var graphicsSettings = AssetDatabase.LoadAssetAtPath<GraphicsSettings>("ProjectSettings/GraphicsSettings.asset");
            var serializedObject = new SerializedObject(graphicsSettings);

            var preloadedShadersProperty = serializedObject.FindProperty("m_PreloadedShaders");

            for (var i = 0; i < preloadedShadersProperty.arraySize; ++i)
            {
                var arrayElem = preloadedShadersProperty.GetArrayElementAtIndex(i);
                var variantCollection = arrayElem.objectReferenceValue as ShaderVariantCollection;
                var assetPath = AssetDatabase.GetAssetPath(variantCollection);
                var assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
                if (assetGuid == ShaderVariantCollectionGuid)
                    return true;
            }

            return false;
        }

        private static PipelineType DetectPipeline()
        {
            var rp = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset;
            if (rp == null) return PipelineType.StandardRP;

            var rpType = UnityEngine.Rendering.GraphicsSettings.renderPipelineAsset.GetType();
            if (rpType.Name.Contains("HDRenderPipelineAsset"))
                return PipelineType.HDRP;
            if (rpType.Name.Contains("UniversalRenderPipelineAsset"))
                return PipelineType.UniversalRP;
            return PipelineType.Unknown;
        }
    }
}