#if ENVIRO_HDRP
using UnityEditor.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEditor.Rendering.HighDefinition
{
    [CanEditMultipleObjects]
    [VolumeComponentEditor(typeof(EnviroSkybox))]
    class EnviroSkySettingsEditor : SkySettingsEditor
    {

        public override void OnEnable()
        {
            base.OnEnable();

            m_CommonUIElementsMask = (uint)SkySettingsUIElement.UpdateMode | (uint)SkySettingsUIElement.SkyIntensity;

            var o = new PropertyFetcher<EnviroSkybox>(serializedObject);
        }

        public override void OnInspectorGUI()
        {
            base.CommonSkySettingsGUI();
        }
    }
}
#endif
