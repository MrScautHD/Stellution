using System.Numerics;
using Easel;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.GUI;
using Easel.Math;

namespace Stellution.Client.csharp.overlay; 

public abstract class Overlay {

    public bool Enabled;
    public readonly Font Font;

    public EaselGraphics Graphics => EaselGame.Instance.Graphics;
    public SpriteRenderer SpriteRenderer => EaselGame.Instance.Graphics.SpriteRenderer;

    public Overlay(Font font) {
        this.Font = font;
    }
    
    public abstract void Draw();

    public void DrawImage(Texture2D texture, Position position, Size<int> size, Color? color = null) {
        Rectangle<int> viewport = new Rectangle<int>(Vector2T<int>.Zero, this.Graphics.MainTarget.Size);
        Vector2T<float> scale = new Vector2T<float>(size.Width / (float) texture.Size.Width, size.Height / (float) texture.Size.Height);
        
        this.SpriteRenderer.Draw(texture, (Vector2) position.CalculatePosition(viewport, size), null, color ?? Color.White, 0, (Vector2) Vector2T<float>.Zero, (Vector2) scale);
    }
    
    public void DrawText(string text, Position position, uint fontSize, Color color, bool shadow = true) {
        Size<int> size = this.Font.MeasureStringBBCode(fontSize, text);
        Rectangle<int> viewport = new Rectangle<int>(Vector2T<int>.Zero, this.Graphics.MainTarget.Size);
        Vector2T<int> calculatedScreenPos = position.CalculatePosition(viewport, size);

        if (shadow) {
            Vector2T<int> shadowPos = new() {
                X = calculatedScreenPos.X,
                Y = calculatedScreenPos.Y + size.Height / 5
            };

            Color shadowColor = new Color(color.R * 0.4F , color.G * 0.4F, color.B * 0.4F, color.A * 0.4F);
            this.Font.DrawBBCode(this.SpriteRenderer, fontSize, text, shadowPos, shadowColor);
        }
        
        this.Font.DrawBBCode(this.SpriteRenderer, fontSize, text, calculatedScreenPos, color);
    }
}