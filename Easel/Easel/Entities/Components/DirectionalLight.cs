using System.Numerics;
using Easel.Math;

namespace Easel.Entities.Components;

public class DirectionalLight : Component
{
    internal Graphics.Lighting.DirectionalLight InternalLight;

    public Vector2<float> Direction
    {
        get => InternalLight.Direction;
        set => InternalLight.Direction = value;
    }

    public Color Color
    {
        get => InternalLight.Color;
        set => InternalLight.Color = value;
    }

    public DirectionalLight(Vector2<float> direction, Color color)
    {
        InternalLight = new Graphics.Lighting.DirectionalLight(direction, color);
    }
}