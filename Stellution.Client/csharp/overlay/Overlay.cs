using System.Numerics;
using Easel;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.GUI;
using Easel.Math;
using Pie.Windowing;

namespace Stellution.Client.csharp.overlay; 

public abstract class Overlay {

    public bool Enabled;

    protected Rectangle<int> Viewport;
    
    protected EaselGraphics Graphics => EaselGame.Instance.Graphics;
    protected SpriteRenderer SpriteRenderer => EaselGame.Instance.Graphics.SpriteRenderer;

    protected Overlay() {
        Input.NewKeyDown += this.OnKeyPress;
    }

    public abstract void Draw();

    public virtual void Update() {
        this.Viewport = new Rectangle<int>(Vector2T<int>.Zero, this.Graphics.MainTarget.Size);
    }

    protected virtual void OnKeyPress(Key key) {
        
    }

    public void DrawImage(Texture2D texture, Position position, Size<int>? size = null, Color? color = null) {
        Size<int> imageSize = size ?? texture.Size;
        Vector2T<float> scale = new Vector2T<float>(imageSize.Width / (float) texture.Size.Width, imageSize.Height / (float) texture.Size.Height);
        
        this.SpriteRenderer.Draw(texture, (Vector2) position.CalculatePosition(this.Viewport, imageSize), null, color ?? Color.White, 0, (Vector2) Vector2T<float>.Zero, (Vector2) scale);
    }
    
    // TODO: Easel need to fix the memory issues!
    public void DrawText(Font font, string text, Position position, uint fontSize, Color? color = null) {
        Size<int> size = font.MeasureString(fontSize, text);
        Vector2T<int> calculatedScreenPos = position.CalculatePosition(this.Viewport, size);
        
        // TODO: Add Shadow back!
        font.DrawBBCode(this.SpriteRenderer, fontSize, text, calculatedScreenPos, color ?? Color.White);
    }
}