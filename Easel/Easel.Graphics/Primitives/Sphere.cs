using System;
using System.Collections.Generic;
using System.Numerics;

namespace Easel.Graphics.Primitives;

public struct Sphere : IPrimitive
{
    private VertexPositionTextureNormalTangent[] _vertices;

    private uint[] _indices;
    
    public VertexPositionTextureNormalTangent[] Vertices => _vertices;
    public uint[] Indices => _indices;

    public Sphere(int latitude, int longitude, float radius)
    {
        List<VertexPositionTextureNormalTangent> vertices = new List<VertexPositionTextureNormalTangent>();
        List<uint> indices = new List<uint>();

        VertexPositionTextureNormalTangent vptnt = new VertexPositionTextureNormalTangent();
        
        for (int lat = 0; lat <= latitude; lat++)
        {
            float theta = lat * MathF.PI / latitude;
            float sinTheta = MathF.Sin(theta);
            float cosTheta = MathF.Cos(theta);

            for (int lng = 0; lng <= longitude; lng++)
            {
                float phi = lng * 2 * MathF.PI / longitude;
                float sinPhi = MathF.Sin(phi);
                float cosPhi = MathF.Cos(phi);
                
                // TODO: U component of tex coords is seemingly reversed.
                vptnt.Normals = new Vector3(cosPhi * sinTheta, cosTheta, sinPhi * sinTheta);
                vptnt.TexCoords = new Vector2((lng / (float) longitude), (lat / (float) latitude));
                vptnt.Position = new Vector3(radius * vptnt.Normals.X, radius * vptnt.Normals.Y, radius * vptnt.Normals.Z);
                
                vertices.Add(vptnt);
            }
        }

        for (int lat = 0; lat < latitude; lat++)
        {
            for (int lng = 0; lng < longitude; lng++)
            {
                uint first = (uint) ((lat * (longitude + 1)) + lng);
                uint second = (uint) (first + longitude + 1);
                
                indices.Add(first);
                indices.Add(second);
                indices.Add(first + 1);
                
                indices.Add(second);
                indices.Add(second + 1);
                indices.Add(first + 1);
            }
        }

        _vertices = vertices.ToArray();
        _indices = indices.ToArray();
        
        for (int i = 0; i < indices.Count; i += 3)
        {
            ref VertexPositionTextureNormalTangent v0 = ref _vertices[_indices[i]];
            ref VertexPositionTextureNormalTangent v1 = ref _vertices[_indices[i + 1]];
            ref VertexPositionTextureNormalTangent v2 = ref _vertices[_indices[i + 2]];

            Vector3 edge1 = v1.Position - v0.Position;
            Vector3 edge2 = v2.Position - v0.Position;

            float deltaU1 = v1.TexCoords.X - v0.TexCoords.X;
            float deltaV1 = v1.TexCoords.Y - v0.TexCoords.Y;
            float deltaU2 = v2.TexCoords.X - v0.TexCoords.X;
            float deltaV2 = v2.TexCoords.Y - v0.TexCoords.Y;

            float f = 1f / (deltaU1 * deltaV2 - deltaU2 * deltaV1);

            Vector3 tangent = new Vector3(
                f * (deltaV2 * edge1.X - deltaV1 * edge2.X),
                f * (deltaV2 * edge1.Y - deltaV1 * edge2.Y),
                f * (deltaV2 * edge1.Z - deltaV1 * edge2.Z));

            v0.Tangents += tangent;
            v1.Tangents += tangent;
            v2.Tangents += tangent;
        }

        for (int i = 0; i < _vertices.Length; i++)
            _vertices[i].Tangents = Vector3.Normalize(_vertices[i].Tangents);
    }
}