using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [CustomPropertyDrawer(typeof(MassiveCloudsPhysicsCloudPass.OverrideFloat))]
    public class OverrideFloatPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            var isOverride = property.FindPropertyRelative("IsOverride");
            var value = property.FindPropertyRelative("Value");

            var labelRect = position;
            labelRect.width = 145;
            position.x += labelRect.width;
            position.width -= labelRect.width;
            GUI.Label(labelRect, property.displayName, EditorStyles.label);

            var checkRect = position;
            checkRect.width = 24;
            position.x += checkRect.width;
            position.width -= checkRect.width;
            isOverride.boolValue = GUI.Toggle(checkRect, isOverride.boolValue, "");

            using (new EditorGUI.DisabledScope(!isOverride.boolValue))
            {
                EditorGUI.PropertyField(position, value, GUIContent.none);
            }
        }
    }
}