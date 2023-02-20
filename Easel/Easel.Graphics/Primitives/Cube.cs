using System.Numerics;

namespace Easel.Graphics.Primitives;

/// <summary>
/// A cube primitive, containing 6 sides, texture coordinates, and normals.
/// </summary>
public struct Cube : IPrimitive
{
    public VertexPositionTextureNormalTangent[] Vertices => new[]
    {
        new VertexPositionTextureNormalTangent(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(0, 0), new Vector3(0, 1, 0), new Vector3(-1, 0, 0)),
        new VertexPositionTextureNormalTangent(new Vector3(0.5f, 0.5f, -0.5f), new Vector2(1, 0), new Vector3(0, 1, 0), new Vector3(-1, 0, 0)),
        new VertexPositionTextureNormalTangent(new Vector3(0.5f, 0.5f, 0.5f), new Vector2(1, 1), new Vector3(0, 1, 0), new Vector3(-1, 0, 0)),
        new VertexPositionTextureNormalTangent(new Vector3(-0.5f, 0.5f, 0.5f), new Vector2(0, 1), new Vector3(0, 1, 0), new Vector3(-1, 0, 0)),

        new VertexPositionTextureNormalTangent(new Vector3(-0.5f, -0.5f, 0.5f), new Vector2(0, 0), new Vector3(0, -1, 0), new Vector3(1, 0, 0)),
        new VertexPositionTextureNormalTangent(new Vector3(0.5f, -0.5f, 0.5f), new Vector2(1, 0), new Vector3(0, -1, 0), new Vector3(1, 0, 0)),
        new VertexPositionTextureNormalTangent(new Vector3(0.5f, -0.5f, -0.5f), new Vector2(1, 1), new Vector3(0, -1, 0), new Vector3(1, 0, 0)),
        new VertexPositionTextureNormalTangent(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0, 1), new Vector3(0, -1, 0), new Vector3(1, 0, 0)),

        new VertexPositionTextureNormalTangent(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(0, 0), new Vector3(-1, 0, 0), new Vector3(0, 0, 1)),
        new VertexPositionTextureNormalTangent(new Vector3(-0.5f, 0.5f, 0.5f), new Vector2(1, 0), new Vector3(-1, 0, 0), new Vector3(0, 0, 1)),
        new VertexPositionTextureNormalTangent(new Vector3(-0.5f, -0.5f, 0.5f), new Vector2(1, 1), new Vector3(-1, 0, 0), new Vector3(0, 0, 1)),
        new VertexPositionTextureNormalTangent(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0, 1), new Vector3(-1, 0, 0), new Vector3(0, 0, 1)),

        new VertexPositionTextureNormalTangent(new Vector3(0.5f, 0.5f, 0.5f), new Vector2(0, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1)),
        new VertexPositionTextureNormalTangent(new Vector3(0.5f, 0.5f, -0.5f), new Vector2(1, 0), new Vector3(1, 0, 0), new Vector3(0, 0, 1)),
        new VertexPositionTextureNormalTangent(new Vector3(0.5f, -0.5f, -0.5f), new Vector2(1, 1), new Vector3(1, 0, 0), new Vector3(0, 0, 1)),
        new VertexPositionTextureNormalTangent(new Vector3(0.5f, -0.5f, 0.5f), new Vector2(0, 1), new Vector3(1, 0, 0), new Vector3(0, 0, 1)),

        new VertexPositionTextureNormalTangent(new Vector3(0.5f, 0.5f, -0.5f), new Vector2(0, 0), new Vector3(0, 0, -1), new Vector3(-1, 0, 0)),
        new VertexPositionTextureNormalTangent(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(1, 0), new Vector3(0, 0, -1), new Vector3(-1, 0, 0)),
        new VertexPositionTextureNormalTangent(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(1, 1), new Vector3(0, 0, -1), new Vector3(-1, 0, 0)),
        new VertexPositionTextureNormalTangent(new Vector3(0.5f, -0.5f, -0.5f), new Vector2(0, 1), new Vector3(0, 0, -1), new Vector3(-1, 0, 0)),

        new VertexPositionTextureNormalTangent(new Vector3(-0.5f, 0.5f, 0.5f), new Vector2(0, 0), new Vector3(0, 0, 1), new Vector3(-1, 0, 0)),
        new VertexPositionTextureNormalTangent(new Vector3(0.5f, 0.5f, 0.5f), new Vector2(1, 0), new Vector3(0, 0, 1), new Vector3(-1, 0, 0)),
        new VertexPositionTextureNormalTangent(new Vector3(0.5f, -0.5f, 0.5f), new Vector2(1, 1), new Vector3(0, 0, 1), new Vector3(-1, 0, 0)),
        new VertexPositionTextureNormalTangent(new Vector3(-0.5f, -0.5f, 0.5f), new Vector2(0, 1), new Vector3(0, 0, 1), new Vector3(-1, 0, 0))
    };

    public uint[] Indices => new uint[]
    {
        0, 1, 2, 0, 2, 3,
        4, 5, 6, 4, 6, 7,
        8, 9, 10, 8, 10, 11,
        12, 13, 14, 12, 14, 15,
        16, 17, 18, 16, 18, 19,
        20, 21, 22, 20, 22, 23
    };
}