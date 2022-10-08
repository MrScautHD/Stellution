using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    [CustomPropertyDrawer(typeof(MassiveCloudsPhysicsCloud.UnityQuality))]
    public class UnityQualityPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property,
            GUIContent label)
        {
            var level = property.FindPropertyRelative("Level");
            var scale = property.FindPropertyRelative("Scale");
            label = EditorGUI.BeginProperty(position, label, property);
//            position = EditorGUI.PrefixLabel(position, label);
            var levelRect = new Rect(position.x, position.y, position.width / 2f, position.height);
            var scaleRect = new Rect(position.x + position.width/2f, position.y, position.width / 2f, position.height);
            var originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 64f;
            level.intValue = EditorGUI.IntPopup(levelRect, level.intValue, QualitySettings.names, Enumerable.Range(0, QualitySettings.names.Length).ToArray());
            scale.floatValue = EditorGUI.FloatField(scaleRect, "Scale", scale.floatValue);
            EditorGUIUtility.labelWidth = originalLabelWidth;
            EditorGUI.EndProperty();
        }
    }
}