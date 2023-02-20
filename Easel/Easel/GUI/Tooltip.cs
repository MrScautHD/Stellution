using System.Numerics;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public class Tooltip
{
    public UITheme Theme;

    public string Text;
    public uint FontSize;
    
    public Tooltip(string text, uint fontSize = 24, UITheme? theme = null)
    {
        Text = text;
        FontSize = fontSize;
        Theme = theme ?? UI.Theme;
    }

    public void Draw(SpriteRenderer renderer)
    {
        Vector2<int> size = (Vector2<int>) Theme.Font.MeasureStringBBCode(FontSize, Text);
        Vector2<float> pos = Input.MousePosition + new Vector2<float>(0, -20);
        renderer.DrawRectangle(pos, (Size<int>) (size + new Vector2<int>(0, 10)), Theme.BorderWidth, Theme.BorderRadius,
            Theme.BackgroundColor, Theme.BorderColor, 0, Vector2<float>.Zero);
        Theme.Font.DrawBBCode(renderer, FontSize, Text, (Vector2<int>) pos + size / 2 - size / 2, Theme.FontColor);
    }
}