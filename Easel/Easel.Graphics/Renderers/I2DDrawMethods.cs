using System.Numerics;
using Easel.Math;

namespace Easel.Graphics.Renderers;

public interface I2DDrawMethods
{
    public void Draw(Texture texture, Rectangle<float> destination, Color tint, float z = 0);

    public void Draw(Texture texture, Rectangle<float> destination, Rectangle<int>? source, Color tint, float z = 0);

    public void Draw(Texture texture, Rectangle<float> destination, Rectangle<int>? source, Color tint, float rotation,
        Vector2<float> origin, SpriteFlip flip = SpriteFlip.None, float z = 0);

    public void Draw(Texture texture, Vector3 position);

    public void Draw(Texture texture, Vector3 position, Color tint);

    public void Draw(Texture texture, Vector3 position, Rectangle<int>? source, Color tint);

    public void Draw(Texture texture, Vector3 position, Rectangle<float>? source, Color tint, float rotation,
        Vector2<float> origin, float scale, SpriteFlip flip = SpriteFlip.None);

    public void Draw(Texture texture, Vector3 position, Rectangle<float>? source, Color tint, float rotation, Vector2<float> origin,
        Vector2<float> scale, SpriteFlip flip = SpriteFlip.None);

    public void DrawRectangle(Vector3 position, Size<float> size, int borderWidth, float radius, Color color,
        Color borderColor, float rotation, Vector2<float> origin);
    
    public void DrawRectangle(Texture texture, Vector3 position, Size<float> size, int borderWidth, float radius,
        Color color, Color borderColor, float rotation, Vector2<float> origin);
}