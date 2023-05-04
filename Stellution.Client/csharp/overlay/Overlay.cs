using System.Numerics;
using Easel;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.GUI;
using Easel.Math;

namespace Stellution.Client.csharp.overlay; 

public abstract class Overlay {

    public bool Enabled;
    private readonly Font _font;

    protected EaselGraphics Graphics => EaselGame.Instance.Graphics;
    protected SpriteRenderer SpriteRenderer => EaselGame.Instance.Graphics.SpriteRenderer;

    protected Overlay(Font font) {
        this._font = font;
    }
    
    public abstract void Draw();

    public virtual void Update() {
    }

    public void DrawImage(Texture2D texture, Position position, Size<int>? size = null, Color? color = null) {
        Rectangle<int> viewport = new Rectangle<int>(Vector2T<int>.Zero, this.Graphics.MainTarget.Size);
        Size<int> imageSize = size ?? texture.Size;
        Vector2T<float> scale = new Vector2T<float>(imageSize.Width / (float) texture.Size.Width, imageSize.Height / (float) texture.Size.Height);
        
        this.SpriteRenderer.Draw(texture, (Vector2) position.CalculatePosition(viewport, imageSize), null, color ?? Color.White, 0, (Vector2) Vector2T<float>.Zero, (Vector2) scale);
    }
    
    public void DrawText(string text, Position position, uint fontSize, Color? color = null, bool shadow = true) {
        Size<int> size = this._font.MeasureStringBBCode(fontSize, text);
        Color fontColor = color ?? Color.White;
        Rectangle<int> viewport = new Rectangle<int>(Vector2T<int>.Zero, this.Graphics.MainTarget.Size);
        Vector2T<int> calculatedScreenPos = position.CalculatePosition(viewport, size);

        if (shadow) {
            Vector2T<int> shadowPos = new() {
                X = calculatedScreenPos.X,
                Y = calculatedScreenPos.Y + size.Height / 5
            };

            Color shadowColor = new Color(fontColor.R * 0.4F , fontColor.G * 0.4F, fontColor.B * 0.4F, fontColor.A * 0.4F);
            this._font.DrawBBCode(this.SpriteRenderer, fontSize, text, shadowPos, shadowColor);
        }
        
        this._font.DrawBBCode(this.SpriteRenderer, fontSize, text, calculatedScreenPos, fontColor);
    }
}