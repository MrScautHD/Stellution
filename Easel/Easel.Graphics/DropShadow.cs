using System.Numerics;
using Easel.Math;

namespace Easel.Graphics;

public struct DropShadow
{
    public Color Color;

    public Vector2<float> Offset;

    public DropShadow(Color color, Vector2<float> offset)
    {
        Color = color;
        Offset = offset;
    }
}