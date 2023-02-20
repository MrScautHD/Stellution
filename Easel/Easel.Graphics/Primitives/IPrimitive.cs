namespace Easel.Graphics.Primitives;

/// <summary>
/// The base interface for in-built engine primitives.
/// </summary>
public interface IPrimitive
{
    /// <summary>
    /// The vertices of this primitive.
    /// </summary>
    public VertexPositionTextureNormalTangent[] Vertices { get; }
    
    /// <summary>
    /// The indices of this primitive.
    /// </summary>
    public uint[] Indices { get; }
}