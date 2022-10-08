using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    public abstract class AbstractMassiveCloudsEditor : Editor
    {
        protected void ProfileGUI(SerializedProperty parameter)
        {
            var horizontal = parameter.FindPropertyRelative("Horizontal").boolValue;
            var authentic = parameter.FindPropertyRelative("Renderer").intValue == (int) MassiveCloudsParameter.RendererType.Authentic;

                Section("Volume Texture Parameters", () =>
            {
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("VolumeTexture"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Tiling"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Renderer"));
            });
            Section("Shape Parameters", () =>
            {
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Horizontal")); 
                if (horizontal)
                {
                    EditorGUILayout.PropertyField(parameter.FindPropertyRelative("RelativeHeight"));

                    var from = parameter.FindPropertyRelative("FromHeight").floatValue;
                    var to = parameter.FindPropertyRelative("ToHeight").floatValue;

                    EditorGUILayout.LabelField("Height", GUILayout.Width(100));
                    EditorGUILayout.MinMaxSlider(ref from, ref to, -10000, 10000);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        from = EditorGUILayout.FloatField(from);
                        EditorGUILayout.LabelField("m -", GUILayout.Width(45));
                        to = EditorGUILayout.FloatField(to);
                        EditorGUILayout.LabelField("m", GUILayout.Width(30));
                    }
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField("Thickness", GUILayout.Width(100));
                        EditorGUILayout.LabelField(string.Format("{0}m", to - from), EditorStyles.helpBox);
                    }

                    parameter.FindPropertyRelative("FromHeight").floatValue = from;
                    parameter.FindPropertyRelative("ToHeight").floatValue = to;

                    EditorGUILayout.PropertyField(parameter.FindPropertyRelative("HorizontalSoftnessTop"), new GUIContent("SoftnessTop"));
                    EditorGUILayout.PropertyField(parameter.FindPropertyRelative("HorizontalSoftnessBottom"), new GUIContent("SoftnessBottom"));
                    EditorGUILayout.PropertyField(parameter.FindPropertyRelative("HorizontalSoftnessFigure"), new GUIContent("Figure"));
                }
                else
                {
                    EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Thickness"));
                    EditorGUILayout.PropertyField(parameter.FindPropertyRelative("FromDistance"));
                }
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("MaxDistance"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Octave"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("DetailDistance"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Sculpture"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Sculpture2"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Density"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Scale"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Softness"));
            });
            Section("Ray Marching Parameters", () =>
            {
                if (!authentic)
                    EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Iteration"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Dissolve"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("FarDissolve"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Fade"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Optimize"));
            });
            Section("Animation Parameters",
                () =>
                {
                    EditorGUILayout.PropertyField(parameter.FindPropertyRelative("ScrollVelocity"));
                    EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Phase"));
                });
            Section("Lighting Parameters", () =>
            {
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Lighting"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("DirectLight"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Ambient"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("LightingQuality"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("LightScattering"));
                if (!authentic)
                    EditorGUILayout.PropertyField(parameter.FindPropertyRelative("LightSmoothness"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Shading"));
            });
            Section("Global Lighting Parameters", () =>
            {
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("GlobalLighting"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("GlobalLightingRange"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("EdgeLighting"));
            });
            Section("Ramp Parameters", () =>
            {
                var ramp = parameter.FindPropertyRelative("Ramp").boolValue;
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Ramp"));
                if (ramp)
                {
                    EditorGUILayout.PropertyField(parameter.FindPropertyRelative("RampTexture"));
                    EditorGUILayout.PropertyField(parameter.FindPropertyRelative("RampScale"));
                    EditorGUILayout.PropertyField(parameter.FindPropertyRelative("RampOffset"));
                    EditorGUILayout.PropertyField(parameter.FindPropertyRelative("RampStrength"));
                }
            });
            if (horizontal)
            {
                var shadow = parameter.FindPropertyRelative("Shadow").boolValue;
                Section("Shadow Parameters", () =>
                {
                    EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Shadow"));
                    if (shadow)
                    {
                        EditorGUILayout.PropertyField(parameter.FindPropertyRelative("ShadowSoftness"));
                        EditorGUILayout.PropertyField(parameter.FindPropertyRelative("ShadowQuality"));
                        EditorGUILayout.PropertyField(parameter.FindPropertyRelative("ShadowStrength"));
                        EditorGUILayout.PropertyField(parameter.FindPropertyRelative("ShadowThreshold"));
                    }
                });
                Section("Volumetric Shadow Parameters", () =>
                {
                    var volumetricShadow = parameter.FindPropertyRelative("VolumetricShadow").boolValue;
                    EditorGUILayout.PropertyField(parameter.FindPropertyRelative("VolumetricShadow"));
                    if (volumetricShadow)
                    {
                        EditorGUILayout.PropertyField(parameter.FindPropertyRelative("VolumetricShadowDensity"));
                        EditorGUILayout.PropertyField(parameter.FindPropertyRelative("VolumetricShadowStrength"));
                    }
                });
            }
            Section("Post Process Parameters", () =>
            {
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Brightness"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Contrast"));
                EditorGUILayout.PropertyField(parameter.FindPropertyRelative("Transparency"));
            });

            serializedObject.ApplyModifiedProperties();
        }

        private static readonly Dictionary<string, bool> folding = new Dictionary<string, bool>();

        private void Section(string label, Action action)
        {
            if (!folding.ContainsKey(label)) folding[label] = true;
            using (new EditorGUILayout.VerticalScope())
            {
                EditorGUI.indentLevel++;
                folding[label] = Foldout(label, folding[label]);
                if (folding[label])
                {
                    action();
                    EditorGUILayout.Space();
                }

                EditorGUI.indentLevel--;
            }
        }

        private static bool Foldout(string title, bool display)
        {
            var style = new GUIStyle("ShurikenModuleTitle");
            style.font = new GUIStyle(EditorStyles.label).font;
            style.border = new RectOffset(15, 7, 4, 4);
            style.fixedHeight = 22;
            style.contentOffset = new Vector2(20f, -2f);

            var rect = GUILayoutUtility.GetRect(16f, 22f, style);
            GUI.Box(rect, title, style);

            var currentEvent = Event.current;
            var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);

            if (currentEvent.type == EventType.Repaint)
                EditorStyles.foldout.Draw(toggleRect, false, false, display, false);

            if (currentEvent.type == EventType.MouseDown && rect.Contains(currentEvent.mousePosition))
            {
                display = !display;
                currentEvent.Use();
            }

            return display;
        }
    }
}