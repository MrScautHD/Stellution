using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AtmosProfile))]
    public class AtmosProfileEditor : Editor
    {
        private static int currentSection = 0;
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            var tab = new MassiveCloudsEditorTab();
            tab.AddSection("Common", x => Common(x, true));
            tab.AddSection("Sky", Sky);
            tab.AddSection("Cloud", Cloud);
            tab.AddSection("Atmosphere", Atmosphere);
            tab.AddSection("Fog", Fog);
            
            MassiveCloudsEditorGUI.Header("<b>"+ target.name + "</b> Setting");
            using (new EditorGUILayout.VerticalScope(MassiveCloudsEditorGUI.GroupStyle(), GUILayout.Height(500f)))
            {
                using (new MassiveCloudsEditorGUI.Scope())
                {
                    currentSection = tab.Inspector(currentSection, serializedObject);

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }


        public void OnInspectorGUIInAtmosPad(bool enableSunControl, bool enableSkyControl)
        {
            serializedObject.Update();

            var tab = new MassiveCloudsEditorTab();
            tab.AddSection("Common", x => Common(x, enableSunControl));
            if (enableSkyControl) tab.AddSection("Sky", Sky);
            tab.AddSection("Cloud", Cloud);
            tab.AddSection("Atmosphere", Atmosphere);
            tab.AddSection("Fog", Fog);
            
            MassiveCloudsEditorGUI.Header("<b>"+ target.name + "</b> Setting");
            using (new EditorGUILayout.VerticalScope(MassiveCloudsEditorGUI.GroupStyle(), GUILayout.Height(500f)))
            {
                using (new MassiveCloudsEditorGUI.Scope())
                {
                    currentSection = tab.Inspector(currentSection, serializedObject);

                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        private void Common(SerializedObject obj, bool enableSunControl)
        {
            var labelColor = serializedObject.FindProperty("labelColor");
            var displayName = obj.FindProperty("displayName");
            var position = obj.FindProperty("position");
            var intensity = obj.FindProperty("intensity");
            var lightColor = obj.FindProperty("lightColor");
            var temperature = obj.FindProperty("temperature");
            var moonIntensity = obj.FindProperty("moonIntensity");
            var moonLightColor = obj.FindProperty("moonLightColor");

            var ambientOverride = obj.FindProperty("ambientOverride");
            var ambient = obj.FindProperty("ambient");
            var skyColor = ambient.FindPropertyRelative("skyColor");
            var equatorColor = ambient.FindPropertyRelative("equatorColor");
            var groundColor = ambient.FindPropertyRelative("groundColor");
            var luminanceFix = ambient.FindPropertyRelative("luminanceFix");

            MassiveCloudsEditorGUI.Space();

            MassiveCloudsEditorGUI.Header("Pad", 2);
            EditorGUILayout.PropertyField(displayName);
            EditorGUILayout.PropertyField(labelColor);
            DrawPosition(position);
            MassiveCloudsEditorGUI.Space();

            if (enableSunControl)
            {
                MassiveCloudsEditorGUI.Header("Sun", 2);
                EditorGUILayout.PropertyField(intensity);
                EditorGUILayout.PropertyField(lightColor);
                EditorGUILayout.PropertyField(temperature);

                MassiveCloudsEditorGUI.Header("Moon", 2);
                EditorGUILayout.PropertyField(moonIntensity);
                EditorGUILayout.PropertyField(moonLightColor);
            }

            MassiveCloudsEditorGUI.Header("Ambient", 2);
            EditorGUILayout.PropertyField(luminanceFix);
            EditorGUILayout.PropertyField(ambientOverride);
            if (ambientOverride.boolValue)
            {
                EditorGUILayout.PropertyField(skyColor);
                EditorGUILayout.PropertyField(equatorColor);
                EditorGUILayout.PropertyField(groundColor);
            }
        }

        public static void Sky(SerializedObject atmosProfile)
        {
            var skyParameter = atmosProfile.FindProperty("sky");
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

            MassiveCloudsEditorGUI.Space();

            MassiveCloudsEditorGUI.Header("Sky", 2);

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
        
        private void Fog(SerializedObject obj)
        {
            var fog = serializedObject.FindProperty("fog");

            MassiveCloudsEditorHeightFog.HeightFog(fog);
            MassiveCloudsEditorGUI.Space();
        }

        private static void Atmosphere(SerializedObject obj)
        {
            var atmosphere = obj.FindProperty("atmosphere");
            MassiveCloudsEditorAtmosphere.AtmosphericScattering(atmosphere);
            MassiveCloudsEditorGUI.Space();
        }

        private static void Cloud(SerializedObject obj)
        {
            var cloudIntensityAdjustment = obj.FindProperty("cloudIntensityAdjustment");
            var mainCloud = obj.FindProperty("mainCloud");
            var layeredCloud = obj.FindProperty("layeredCloud");

            MassiveCloudsEditorGUI.Header("Lighting", 2);
            MassiveCloudsEditorGUI.Space();
            EditorGUILayout.PropertyField(cloudIntensityAdjustment);
            MassiveCloudsEditorGUI.SectionSpace();

            MassiveCloudsEditorGUI.Header("Main Cloud", 2);
            MassiveCloudsEditorGUI.Space();
            DrawCloud(mainCloud);
            MassiveCloudsEditorGUI.SectionSpace();

            MassiveCloudsEditorGUI.Header("Layered Cloud", 2);
            MassiveCloudsEditorGUI.Space();
            DrawCloud(layeredCloud);
            MassiveCloudsEditorGUI.SectionSpace();
        }

        private static void DrawCloud(SerializedProperty cloud)
        {
            var profile = cloud.FindPropertyRelative("profile");
            var densityAdjustment = cloud.FindPropertyRelative("densityAdjustment");
            var scattering = cloud.FindPropertyRelative("scattering");
            var shading = cloud.FindPropertyRelative("shading");
            var shadingDistance = cloud.FindPropertyRelative("shadingDistance");
            EditorGUILayout.PropertyField(profile);
            EditorGUILayout.PropertyField(densityAdjustment);
            EditorGUILayout.PropertyField(scattering, true);
            EditorGUILayout.PropertyField(shading, true);
            EditorGUILayout.PropertyField(shadingDistance, true);
        }

        private static void DrawPosition(SerializedProperty position)
        {
            var x = position.vector2Value.x;
            var y = position.vector2Value.y;

            using (new EditorGUILayout.HorizontalScope())
            {
                var time = 24f * x;
                var h = Mathf.Floor(time);
                var m = Mathf.Floor(60f * Mathf.Repeat(time, 1f));
                EditorGUILayout.LabelField("Time", GUILayout.Width(EditorGUIUtility.labelWidth));
                var r = EditorGUILayout.GetControlRect(false, null);
                x = GUI.HorizontalSlider(r, x, 0f, 1f);
                var timeText = h.ToString("00") + ":" + m.ToString("00");
                EditorGUILayout.LabelField(timeText, GUILayout.Width(52f));
            }
            y = EditorGUILayout.Slider("Pad Y", y, 0f, 1f);
            
            position.vector2Value = new Vector2(x, y);
        }
    }
}