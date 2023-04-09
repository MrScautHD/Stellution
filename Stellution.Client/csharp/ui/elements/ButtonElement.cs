using System;
using Easel;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.GUI;
using Easel.Math;

namespace Stellution.Client.csharp.ui.elements; 

public class ButtonElement : UIElement {

    public Texture2D Texture;
    public string Text;
    public uint FontSize;
    public bool FontShadow;
    public Color Color;
    protected Color HoverColor;
    protected Color FontColor;
    protected Color FontShadowColor;
    protected Vector2T<int> FontPos;
    protected Vector2T<int> FontShadowPos;

    public ButtonElement(string name, Texture2D texture, string text, uint fontSize, Position position, Size<int> size, bool fontShadow = false, Color? color = null, Color? fontColor = null) : base(name, position, size) {
        this.Texture = texture;
        this.Text = text;
        this.FontSize = fontSize;
        this.FontShadow = fontShadow;
        this.Color = color ?? Color.White;
        this.SetFontColor(fontColor ?? Color.White);
    }

    protected override void Draw(SpriteRenderer renderer) {
        this.CalculateHoverColor();
        this.DrawTexture(renderer);
        this.DrawText(renderer);
    }

    protected void DrawTexture(SpriteRenderer renderer) {
        Vector2T<float> scale = new Vector2T<float>(this.Size.Width / (float) this.Texture.Size.Width, this.Size.Height / (float) this.Texture.Size.Height);
        renderer.Draw(this.Texture, (Vector2T<float>) this.CalculatedScreenPos, null, this.HoverColor, 0, Vector2T<float>.Zero, scale);
    }

    protected void DrawText(SpriteRenderer renderer) {
        Size<int> size = UI.DefaultStyle.Font.MeasureStringBBCode(this.FontSize, this.Text);
        this.FontPos.X = this.CalculatedScreenPos.X + this.Size.Width / 2 - size.Width / 2;
        this.FontPos.Y = this.CalculatedScreenPos.Y + this.Size.Height / 2 - size.Height / 2;
        
        if (this.FontShadow) {
            int shadowOffset = (int) Math.Round(Math.Max(size.Width, size.Height) * 0.028F);
            this.FontShadowPos.X = FontPos.X + shadowOffset;
            this.FontShadowPos.Y = FontPos.Y + shadowOffset;

            UI.DefaultStyle.Font.DrawBBCode(renderer, this.FontSize, this.Text, this.FontShadowPos, this.FontShadowColor);
        }
        
        UI.DefaultStyle.Font.DrawBBCode(renderer, this.FontSize, this.Text, new Vector2T<int>(this.FontPos.X, this.FontPos.Y), this.FontColor);
    }

    protected void CalculateHoverColor() {
        this.HoverColor = this.Color;
        
        if (this.IsHovered) {
            this.HoverColor = new Color(this.Color.R * 1.6F , this.Color.G * 1.6F, this.Color.B * 1.6F, this.Color.A * 1.6F);
        }
        
        if (this.IsClicked) {
            this.HoverColor = new Color(this.Color.R * 2F , this.Color.G * 2F, this.Color.B * 2F, this.Color.A * 2F);
        }
    }
    
    public void SetFontColor(Color color) {
        this.FontColor = color;
        this.FontShadowColor = new Color(color.R * 0.4F , color.G * 0.4F, color.B * 0.4F, color.A * 0.4F);
    }
}