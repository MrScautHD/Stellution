using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.GUI;
using Easel.Math;

namespace Stellution.Client.csharp.ui.elements; 

public class ImageElement : UIElement {

    private Texture2D _texture;
    private Color _color;
    
    public ImageElement(string name, Texture2D texture, Position position, Size<int>? size, Color? color = null) : base(name, position, size ?? texture.Size) {
        this._texture = texture;
        this._color = color ?? Color.White;
    }

    protected override void Draw(SpriteRenderer renderer) {
        Vector2T<float> scale = new Vector2T<float>(this.Size.Width / (float) this._texture.Size.Width, this.Size.Height / (float) this._texture.Size.Height);
        renderer.Draw(this._texture, (Vector2T<float>) this.CalculatedScreenPos, null, this._color, 0, Vector2T<float>.Zero, scale);
    }
}