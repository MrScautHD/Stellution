using System;
using UnityEditor;
using UnityEditor.Experimental.UIElements.GraphView;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    public static class MassiveCloudsEditorGUI
    {
        public static void Title(string title)
        {
            var texture = LoadTexture(MassiveCloudsEditorTextures.MassiveCloudsAtmosLogoGuid);
            using (new EditorGUILayout.VerticalScope(GradientStyle()))
            {
                EditorGUILayout.LabelField(new GUIContent(texture), CenterStyle(), GUILayout.Height(48f));
                EditorGUILayout.LabelField(title, CenterStyle(), GUILayout.Height(32f));
            }
        }

        public static void Header(string title, int level = 1)
        {
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (level == 1)
                        EditorGUILayout.LabelField(title, GradientStyle(), GUILayout.Height(48f));
                    else if (level == 3)
                        EditorGUILayout.LabelField(title, SubSectionStyle(12), GUILayout.Height(16f));
                    else
                        EditorGUILayout.LabelField(title, GradientStyle(12), GUILayout.Height(24f));
                }
            }
        }

        public static void Space()
        {
            GUILayout.Space(8f);
        }

        public static void SectionSpace()
        {
            GUILayout.Space(24f);
        }

        public static GUIStyle GradientStyle(int fontSize = 16)
        {
            var texture = LoadTexture(MassiveCloudsEditorTextures.GradientBackgroundGuid);
            var style = new GUIStyle()
            {
                padding = new RectOffset(0, 0, 0, 0),
                alignment = TextAnchor.MiddleCenter,
                fontSize = fontSize,
            };
            var styleState = new GUIStyleState()
            {
                background = texture,
                textColor = new Color(0.2f, 0.25f, 0.3f)
            };
            style.normal = styleState;
            return style;
        }

        public static GUIStyle PadStyle()
        {
            var texture = LoadTexture(MassiveCloudsEditorTextures.AtmosPadBackgroundGuid);
            var style = new GUIStyle()
            {
                padding = new RectOffset(0, 0, 0, 0),
                alignment = TextAnchor.MiddleCenter,
            };
            var styleState = new GUIStyleState()
            {
                background = texture,
                textColor = new Color(0.2f, 0.25f, 0.3f)
            };
            style.normal = styleState;
            return style;
        }

        public static GUIStyle CenterStyle(int fontSize = 16)
        {
            var style = new GUIStyle()
            {
                padding = new RectOffset(0, 0, 0, 0),
                alignment = TextAnchor.MiddleCenter,
                fontSize = fontSize,
            };
            var styleState = new GUIStyleState()
            {
                textColor = new Color(0.2f, 0.25f, 0.3f)
            };
            style.normal = styleState;
            return style;
        }

        public static GUIStyle GroupStyle(int fontSize = 16)
        {
            var texture = LoadTexture(MassiveCloudsEditorTextures.SectionBackgroundGuid);
            var style = new GUIStyle()
            {
                padding = new RectOffset(0, 0, 0, 0),
                alignment = TextAnchor.MiddleLeft,
                fontSize = fontSize,
            };
            var styleState = new GUIStyleState()
            {
                background = texture,
                textColor = new Color(0.8f, 0.75f, 0.7f)
            };
            style.normal = styleState;
            return style;
        }

        public static GUIStyle SubSectionStyle(int fontSize = 16)
        {
            var texture = LoadTexture(MassiveCloudsEditorTextures.SectionBackgroundGuid);
            var style = new GUIStyle()
            {
                padding = new RectOffset(2, 0, 0, 0),
                alignment = TextAnchor.MiddleLeft,
                fontSize = fontSize,
            };
            var styleState = new GUIStyleState()
            {
                background = texture,
                textColor = new Color(0.8f, 0.75f, 0.7f)
            };
            style.normal = styleState;
            return style;
        }

        public static Texture2D LoadTexture(string guid)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            return sprite.texture;
        }
        
        public static void BoxLabel(string text)
        {
            EditorGUILayout.LabelField(text, GUI.skin.textField);
        }

        
        public class Scope : IDisposable
        {
            private Color originalTextColor;
            private Color originalFocusedTextColor;
            private Color originalActiveTextColor;
            public Scope()
            {
                originalTextColor = EditorStyles.label.normal.textColor;
                originalFocusedTextColor = EditorStyles.label.focused.textColor;
                originalActiveTextColor = EditorStyles.label.active.textColor;
                EditorStyles.label.normal.textColor  = Color.white;
                EditorStyles.label.focused.textColor  = Color.white;
                EditorStyles.label.active.textColor  = new Color(0.7f, 0.7f, 1f);
            }

            public void Dispose()
            {
                EditorStyles.label.normal.textColor  = originalTextColor;
                EditorStyles.label.focused.textColor  = originalFocusedTextColor;
                EditorStyles.label.active.textColor  = originalActiveTextColor;
            }
        }

    }
}