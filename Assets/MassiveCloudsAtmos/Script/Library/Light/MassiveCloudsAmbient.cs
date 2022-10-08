using System;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [Serializable]
    public class MassiveCloudsAmbient
    {
        public enum AmbientMode
        {
            Auto,
            Manual,
        }
        [SerializeField] private AmbientMode mode = AmbientMode.Auto;
        [ColorUsage(false, true)]
        [SerializeField] private Color skyColor = Color.blue;
        [ColorUsage(false, true)]
        [SerializeField] private Color equatorColor = Color.cyan;
        [ColorUsage(false, true)]
        [SerializeField] private Color groundColor = Color.gray;
        [Range(0f, 1f), SerializeField] private float pivot = 0.3f;
        [SerializeField] private float luminanceFix = 0f;

        private static readonly Vector3[] probeDirections = new[] {Vector3.up, Vector3.back, Vector3.down};
        
        private float ambientOverride = 0f;
        private Color skyColorOverride = Color.blue;
        private Color equatorColorOverride = Color.cyan;
        private Color groundColorOverride = Color.gray;

        private Color Fix(Color col)
        {
            var factor = Mathf.Pow(2, -luminanceFix);
            return col / factor;
        }

        public AmbientMode Mode { get { return mode; } }

        public Color SkyColor
        {
            get { return Fix(skyColor); }
            set { skyColor = value; }
        }

        public Color EquatorColor
        {
            get { return Fix(equatorColor); }
            set { equatorColor = value; }
        }

        public Color GroundColor
        {
            get { return Fix(groundColor); }
            set { groundColor = value; }
        }

        public float LuminanceFix
        {
            get { return luminanceFix; }
        }

        public void ApplyShaderParameters(Material material)
        {
            material.SetColor("_AmbientTopColor", SkyColor);
            material.SetColor("_AmbientMidColor", EquatorColor);
            material.SetColor("_AmbientBottomColor", GroundColor);
            material.SetFloat("_AmbientPivot", pivot);
        }

        public void Collect(MassiveCloudsLight sun, MassiveCloudsLight moon)
        {
            var colors = new Color[3];

            switch (RenderSettings.ambientMode)
            {
                case UnityEngine.Rendering.AmbientMode.Skybox:
                    RenderSettings.ambientProbe.Evaluate(probeDirections, colors);
                    break;
                case UnityEngine.Rendering.AmbientMode.Trilight:
                    colors[0] = RenderSettings.ambientSkyColor;
                    colors[1] = RenderSettings.ambientEquatorColor;
                    colors[2] = RenderSettings.ambientGroundColor;
                    break;
                case UnityEngine.Rendering.AmbientMode.Flat:
                    colors[0] = RenderSettings.ambientLight;
                    colors[1] = RenderSettings.ambientLight;
                    colors[2] = RenderSettings.ambientLight;
                    break;
                case UnityEngine.Rendering.AmbientMode.Custom:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var nightColor = moon.Color;
            var factor = 1f - Mathf.Clamp01(-sun.LightDirection.y * 10f);
            SkyColor = Color.Lerp(SafeColor(colors[0]), nightColor * 0.02f, factor);
            EquatorColor = Color.Lerp(SafeColor(colors[1]), nightColor * 0.03f, factor);
            GroundColor = Color.Lerp(SafeColor((colors[0]) / 2f), nightColor * 0.01f, factor);

            EquatorColor = SkyColor;
            GroundColor = SkyColor * 0.25f;
            SkyColor = sun.Color * sun.Intensity * 0.5f;

            SkyColor = Color.Lerp(SkyColor, skyColorOverride, ambientOverride);
            EquatorColor = Color.Lerp(EquatorColor, equatorColorOverride, ambientOverride);
            GroundColor = Color.Lerp(GroundColor, groundColorOverride, ambientOverride);
        }

        private Color SafeColor(Color c)
        {
            float r, g, b;
            r = float.IsNaN(c.r) ? 0 : c.r;
            g = float.IsNaN(c.g) ? 0 : c.g;
            b = float.IsNaN(c.b) ? 0 : c.b;
            return new Color(r, g, b);
        }

        public void SetAmbientColor(float ambientOverride, Color skyColor, Color equatorColor, Color groundColor)
        {
            this.ambientOverride = ambientOverride;
            skyColorOverride = skyColor;
            equatorColorOverride = equatorColor;
            groundColorOverride = groundColor;
        }

        public void SetLuminanceFix(float luminanceFix)
        {
            this.luminanceFix = luminanceFix;
        }
    }
}