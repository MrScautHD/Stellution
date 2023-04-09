using System;
using Easel.Graphics.Renderers;
using Easel.GUI;
using Easel.Math;

namespace Stellution.Client.csharp.ui.elements; 

public class LabelElement : UIElement {

    public string Text;
    public uint FontSize;
    public bool Shadow;
    protected Color Color;
    protected Color ShadowColor;
    protected Vector2T<int> ShadowPos;
    
    // EASEL wait on adding Font Options to disable MSAA on text
    public LabelElement(string name, string text, Position position, uint fontSize, bool shadow = false, Color? color = null) : base(name, position, Size<int>.Zero) {
        this.Text = text;
        this.FontSize = fontSize;
        this.Shadow = shadow;
        this.SetColor(color ?? Color.White);
        this.Size = UI.DefaultStyle.Font.MeasureStringBBCode(fontSize, text);
    }
    
    protected override void Draw(SpriteRenderer renderer) {
        this.Size = UI.DefaultStyle.Font.MeasureStringBBCode(this.FontSize, this.Text);
        
        if (this.Shadow) {
            int shadowOffset = (int) Math.Round(Math.Max(Size.Width, Size.Height) * 0.0215f);
            
            this.ShadowPos.X = this.CalculatedScreenPos.X + shadowOffset;
            this.ShadowPos.Y = this.CalculatedScreenPos.Y + shadowOffset;

            UI.DefaultStyle.Font.DrawBBCode(renderer, this.FontSize, this.Text, this.ShadowPos, this.ShadowColor);
        }
        
        UI.DefaultStyle.Font.DrawBBCode(renderer, this.FontSize, this.Text, this.CalculatedScreenPos, this.Color);
    }

    public void SetColor(Color color) {
        this.Color = color;
        this.ShadowColor = new Color(color.R * 0.4F , color.G * 0.4F, color.B * 0.4F, color.A * 0.4F);
    }
}