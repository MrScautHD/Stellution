using System;
using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;
using Vector2 = Easel.Math.Vector2;

namespace Easel.GUI;

public class Button : UIElement
{
    public string Text;
    
    public uint FontSize;

    public Vector2<int> TextOffset;

    public Justification Justification;

    public Texture Image;

    public Button(Position position, Size<int> size, string text, uint fontSize = 24, Justification justification = Justification.Center) : base(position, size)
    {
        Text = text;
        FontSize = fontSize;
        Justification = justification;
    }
    
    protected internal override void Draw(SpriteRenderer renderer)
    {
        base.Draw(renderer);
        
        Color color = Theme.BackgroundColor;
        if (IsHovering)
            color = Theme.HoverColor;
        if (IsMouseButtonHeld)
            color = Theme.ClickedColor;

        if (Theme.DropShadow.HasValue)
        {
            DropShadow shadow = Theme.DropShadow.Value;
            renderer.DrawRectangle((Vector2<float>) CalculatedScreenPos + shadow.Offset, Size, 0, Theme.BorderRadius, shadow.Color, Color.Transparent, 0, Vector2<float>.Zero);
        }

        if (BlurTexture != null)
            renderer.DrawRectangle(BlurTexture, (Vector2<float>) CalculatedScreenPos, Size, 0, Theme.BorderRadius, Color.White, Color.Transparent, 0, Vector2<float>.Zero);

        renderer.DrawRectangle(Image ?? Texture2D.White, (Vector2<float>) CalculatedScreenPos, Size, Theme.BorderWidth, Theme.BorderRadius, color, Theme.BorderColor, 0, Vector2<float>.Zero);
        Size<int> size = Theme.Font.MeasureStringBBCode(FontSize, Text);

        int posX = Justification switch
        {
            Justification.Left => CalculatedScreenPos.X,
            Justification.Center => CalculatedScreenPos.X + Size.Width / 2 - size.Width / 2,
            Justification.Right => CalculatedScreenPos.X + Size.Width - size.Width,
            _ => throw new ArgumentOutOfRangeException()
        };
        Theme.Font.DrawBBCode(renderer, FontSize, Text, new Vector2<int>(posX, CalculatedScreenPos.Y + Size.Height / 2 - size.Height / 2) + TextOffset, Theme.FontColor);
    }
}