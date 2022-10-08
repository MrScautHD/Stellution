using UnityEditor;
using UnityEngine;

namespace Mewlist.MassiveClouds
{
    internal class AtmosPadUI
    {
        private const float ITEM_RADIUS = 0.03f;

        private readonly Rect controlRect;
        private readonly Rect padRect;
        private readonly Vector2 padding;
        private readonly Vector2 padSize;

        public AtmosPadUI(Rect controlRect)
        {
            this.controlRect = controlRect;
            padding = this.controlRect.size * ITEM_RADIUS;
            padSize = this.controlRect.size - 2f * padding;
            padRect = new Rect(this.controlRect.x + padding.x, this.controlRect.y + padding.y, padSize.x, padSize.y);
        }

        public static bool InRange(Vector2 position)
        {
            return -ITEM_RADIUS <= position.x && position.x <= 1f + ITEM_RADIUS && -ITEM_RADIUS <= position.y && position.y <= 1f + ITEM_RADIUS;
        }

        public void DrawPad()
        {
            EditorGUI.LabelField(controlRect, "", MassiveCloudsEditorGUI.PadStyle());
        }

        public void DrawGroup(AtmosGroup atmosGroup, Color color)
        {
            var bounds = atmosGroup.Bounds();
            var pos = ToWorldPosition(bounds.position);
            GUI.color = Color.white;
            EditorGUI.DrawRect(new Rect(pos.x - padding.x, pos.y, bounds.width * padSize.x + 2f * padding.x, bounds.height * padSize.y), color);
        }

        public void DrawProfile(AtmosProfile profile, string text, Color baseColor, Color labelColor)
        {
            Handles.color = baseColor;
            DrawCircle(profile.Position);
            DrawLabel(profile.Position, text, labelColor);
        }

        private void DrawCircle(Vector2 pos, float r = ITEM_RADIUS)
        {
            Handles.DrawSolidArc(
                padRect.position + pos * padRect.size,
                Vector3.forward,
                Vector3.right,
                360f,
                r * padRect.height);
        }

        public void DrawCursor(Vector2 padPos)
        {
            var pos = ToWorldPosition(padPos);
            Handles.color = new Color(0, 0, 0, 0.5f);
            EditorGUI.DrawRect(new Rect(controlRect.x, pos.y, controlRect.width, 1f), new Color(1f, 1f, 1f, 0.5f));
            EditorGUI.DrawRect(new Rect(pos.x, controlRect.y, 1f, controlRect.height), new Color(1f, 1f, 1f, 0.5f));
            DrawLabel(padPos - new Vector2(0f, 0.04f), padPos.x.ToTimeString(), new Color(0,0,0,0), 2f);
        }

        public Vector2 ToLocalPosition(Vector2 pos)
        {
            return new Vector2(
                (pos.x - padRect.x) / padRect.width,
                (pos.y - padRect.y) / padRect.height);
        }
        
        private Vector2 ToWorldPosition(Vector2 pos)
        {
            return new Vector2(
                padRect.x + pos.x * padRect.width,
                padRect.y + pos.y * padRect.height);
        }

        private void DrawLabel(  Vector2 position, string text, Color bgColor, float fontSize = 1.0f)
        {
            var centeredStyle = new GUIStyle(EditorStyles.label)
            {
                fontSize = Mathf.FloorToInt(fontSize * padRect.height / 40),
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(0, 0, 0, 0)
            };

            const float width = 8f;
            var pos = ToWorldPosition(position);
            var border = 1f;
            var shadowRect = new Rect(
                Mathf.RoundToInt(pos.x - centeredStyle.fontSize * 0.5f * width) + border,
                Mathf.RoundToInt(pos.y - centeredStyle.fontSize * 1.4f / 2f),
                centeredStyle.fontSize * width,
                centeredStyle.fontSize * 1.3f);
            var shadowRect2 = new Rect(
                Mathf.RoundToInt(pos.x - centeredStyle.fontSize * 0.5f * width) - border,
                Mathf.RoundToInt(pos.y - centeredStyle.fontSize * 1.4f / 2f),
                centeredStyle.fontSize * width,
                centeredStyle.fontSize * 1.3f);
            var shadowRect3 = new Rect(
                Mathf.RoundToInt(pos.x - centeredStyle.fontSize * 0.5f * width),
                Mathf.RoundToInt(pos.y - centeredStyle.fontSize * 1.4f / 2f) + border,
                centeredStyle.fontSize * width,
                centeredStyle.fontSize * 1.3f);
            var shadowRect4 = new Rect(
                Mathf.RoundToInt(pos.x - centeredStyle.fontSize * 0.5f * width),
                Mathf.RoundToInt(pos.y - centeredStyle.fontSize * 1.4f / 2f) - border,
                centeredStyle.fontSize * width,
                centeredStyle.fontSize * 1.3f);
            var rect = new Rect(
                Mathf.RoundToInt(pos.x - centeredStyle.fontSize * 0.5f * width),
                Mathf.RoundToInt(pos.y - centeredStyle.fontSize * 1.4f / 2f),
                centeredStyle.fontSize * width,
                centeredStyle.fontSize * 1.3f);

            GUI.color = Color.white;
            EditorGUI.DrawRect(rect, bgColor);

            GUI.color = bgColor.grayscale > 0.5f ? Color.white : Color.black;
            GUI.Label(shadowRect, text, centeredStyle);
            GUI.Label(shadowRect2, text, centeredStyle);
            GUI.Label(shadowRect3, text, centeredStyle);
            GUI.Label(shadowRect4, text, centeredStyle);

            GUI.color = bgColor.grayscale > 0.5f ? Color.black : Color.white;
            GUI.Label(rect, text, centeredStyle);
            GUI.color = Color.white;
        }
    }
}