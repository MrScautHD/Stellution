using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public class Image : UIElement
{
    public Texture Texture;

    public Image(Position position, Texture texture, Size<int>? size = null) : base(position, size ?? texture.Size)
    {
        Texture = texture;
    }
    
    protected internal override void Draw(SpriteRenderer renderer)
    {
        renderer.Draw(Texture, (Vector2<float>) CalculatedScreenPos, null, Color.White, 0, Vector2<float>.Zero,
            new Vector2<float>(Size.Width / (float) Texture.Size.Width, Size.Height / (float) Texture.Size.Height));
    }
}