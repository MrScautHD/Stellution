using System.Numerics;

namespace Easel.Graphics.Renderers.Structs;

public struct TransformedRenderable
{
    public Renderable Renderable;
    public Matrix4x4 Transform;

    public TransformedRenderable(Renderable renderable, Matrix4x4 transform)
    {
        Renderable = renderable;
        Transform = transform;
    }
}