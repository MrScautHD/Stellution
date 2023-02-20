using System;
using System.Numerics;
using Easel.Graphics.Renderers.Structs;
using Easel.Math;
using Vector3 = System.Numerics.Vector3;
using Vector4 = System.Numerics.Vector4;

namespace Easel.Graphics.Lighting;

public struct DirectionalLight
{
    private Vector2<float> _direction;
    private Vector3 _position;

    public Vector2<float> Direction
    {
        get => _direction;
        set
        {
            _direction = value;
            float theta = -value.Y;
            float phi = value.X;
            _position = new Vector3(MathF.Cos(phi) * MathF.Cos(theta), MathF.Cos(phi) * MathF.Sin(theta),
                MathF.Sin(phi));
        }
    }
    
    public Color Color;

    //public ShadowMap

    public DirectionalLight(Vector2<float> direction, Color color, bool castShadows = true)
    {
        Direction = direction;
        Color = color;
    }

    public ShaderDirLight ShaderDirLight => new ShaderDirLight()
    {
        Direction = new Vector4(_position, 0),
        Color = Color
    };
}