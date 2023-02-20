using Easel.Graphics.Renderers;
using Easel.Math;

namespace Easel.GUI;

public class Panel : UIElement
{
    public Panel(Position position, Size<int> size) : base(position, size) { }
    
    protected internal override void Draw(SpriteRenderer renderer)
    {
        base.Draw(renderer);

        renderer.DrawRectangle(BlurTexture, (Vector2<float>) CalculatedScreenPos, Size, 0, Theme.BorderRadius,
            Color.White, Color.White, 0, Vector2<float>.Zero);
    }
}