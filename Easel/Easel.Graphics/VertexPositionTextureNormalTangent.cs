using System.Numerics;

namespace Easel.Graphics;

public struct VertexPositionTextureNormalTangent
{
    public Vector3 Position;
    public Vector2 TexCoords;
    public Vector3 Normals;
    public Vector3 Tangents;

    public VertexPositionTextureNormalTangent(Vector3 position, Vector2 texCoords, Vector3 normals, Vector3 tangents)
    {
        Position = position;
        TexCoords = texCoords;
        Normals = normals;
        Tangents = tangents;
    }

    public const int SizeInBytes = 44;
}