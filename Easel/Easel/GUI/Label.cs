using System.Numerics;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public class Label : UIElement
{
    public string Text;

    public uint FontSize;

    public Color Color;
    
    public Label(Position position, string text, uint fontSize, Color? color = null) : base(position, Size<int>.Zero)
    {
        Size = Theme.Font.MeasureStringBBCode(fontSize, text);
        Text = text;
        FontSize = fontSize;
        Color = color ?? Color.White;
    }
    
    protected internal override void Draw(SpriteRenderer renderer)
    {
        Size = Theme.Font.MeasureStringBBCode(FontSize, Text);
        
        Theme.Font.DrawBBCode(renderer, FontSize, Text, CalculatedScreenPos, Color);
    }
}