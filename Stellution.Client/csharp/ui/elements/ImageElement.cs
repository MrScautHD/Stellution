using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.GUI;
using Easel.Math;

namespace Stellution.Client.csharp.ui.elements; 

public class ImageElement : UIElement {

    public Texture2D Texture;
    public Color Color;
    
    public ImageElement(string name, Texture2D texture, Position position, Size<int>? size, Color? color = null) : base(name, position, size ?? texture.Size) {
        this.Texture = texture;
        this.Color = color ?? Color.White;
    }

    protected override void Draw(SpriteRenderer renderer) {
        Vector2T<float> scale = new Vector2T<float>(this.Size.Width / (float) this.Texture.Size.Width, this.Size.Height / (float) this.Texture.Size.Height);
        renderer.Draw(this.Texture, (Vector2T<float>) this.CalculatedScreenPos, null, this.Color, 0, Vector2T<float>.Zero, scale);
    }
}