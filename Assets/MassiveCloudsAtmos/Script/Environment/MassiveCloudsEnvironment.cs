using UnityEngine;

namespace Mewlist.MassiveClouds
{
    public class MassiveCloudsEnvironment : MonoBehaviour
    {
        [SerializeField] private Camera referenceCamera = null;
        [SerializeField] private Transform rotRoot = null;
        [SerializeField] private Light directionalLight = null;

        public void ChangeTime(float t)
        {
            var east = Quaternion.AngleAxis(-90f, Vector3.up);
            var selfRot = Quaternion.AngleAxis(t / 24f * 360f - 90, Vector3.forward);
            var earthAxis = Quaternion.AngleAxis(24f, Vector3.left);
            rotRoot.rotation = earthAxis * selfRot * east;
            directionalLight.color = GenerateSunColor(t / 24f);
            UpdateFogColor();
            DynamicGI.UpdateEnvironment();
        }

        private void UpdateFogColor()
        {
            float h, s, v;
            Color.RGBToHSV(directionalLight.color, out h, out s, out v);
            RenderSettings.fogColor =
                Color.HSVToRGB(h, s / 5f, v * 0.6f);
        }

        private void LateUpdate()
        {
            transform.position = referenceCamera.transform.position;
        }

        private static Color GenerateSunColor(float t)
        {
            var n = NormalDistribution(t * 5f - 2.5f);
            var scaledN = n / 0.4f;
            const float blueThreshold = 0.75f;
            const float redThreshold = 0.4f;
            const float nightThreshold = 0.3f;

            var nightFactor = Mathf.Lerp(1, 0, (scaledN - nightThreshold) / (redThreshold - nightThreshold));
            var dayFactor = Mathf.Lerp(0, 1, (scaledN - redThreshold) / (blueThreshold - redThreshold));
            Color col = Color.Lerp(
                new Color(0.9f,0.5f,0.5f),
                new Color(0.85f,0.9f,1f),
                dayFactor);
            col = Color.Lerp(
                col,
                new Color(0.04f,0.04f,0.2f),
                nightFactor);

            return col;
        }

        private static float NormalDistribution(float t)
        {
            return 1 / Mathf.Pow(2 * Mathf.PI, 0.5f) * Mathf.Exp(-t * t / 2);
        }
    }
}