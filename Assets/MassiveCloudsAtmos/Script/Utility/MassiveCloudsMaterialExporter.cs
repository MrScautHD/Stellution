#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [HelpURL("http://massive-clouds-atmos.mewli.st/mca_material_exporter_ja.html")]
    [CreateAssetMenu(fileName = "MassiveCloudsMaterialExporter", menuName = "Mewlist/MassiveClouds/MaterialExporter", order = 50)]
    public class MassiveCloudsMaterialExporter : ScriptableObject
    {
        [SerializeField] private MassiveCloudsStylizedCloudProfile stylizedCloudProfile = null;

        public void SaveToMaterial(string path)
        {
#if UNITY_EDITOR
            var mat = stylizedCloudProfile.CreateMaterialForExport();
            mat.SetColor("_BaseColor", Color.white);
            mat.SetColor("_MCFogColor", RenderSettings.fogColor);
            mat.SetColor("_BaseColor", RenderSettings.fogColor);
            mat.SetColor("_MassiveCloudsSunLightColor", Color.white);
            mat.SetColor("_MassiveCloudsMoonLightColor", new Color(0.4f, 0.5f, 1f));
            mat.SetVector("_MassiveCloudsSunLightDirection", new Vector3(0, -0.7f, 0.7f));
            mat.SetVector("_MassiveCloudsMoonLightDirection", new Vector3(-6.1f, -0.7f, 0.7f));
            Color[] colors = new Color[3];
            RenderSettings.ambientProbe.Evaluate(
                new []{Vector3.up, Vector3.back, Vector3.down},
                colors);
            mat.SetColor("_AmbientTopColor", colors[0]);
            mat.SetColor("_AmbientMidColor", colors[1]);
            mat.SetColor("_AmbientBottomColor", colors[2]);
            
            AssetDatabase.CreateAsset(mat, path.Replace(Application.dataPath, "Assets"));
#endif
        }
    }
}