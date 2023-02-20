namespace Easel.Graphics.Primitives;

public struct Quad : IPrimitive
{
    public VertexPositionTextureNormalTangent[] Vertices { get; }
    public uint[] Indices { get; }
}