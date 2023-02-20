using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Easel.Core;
using Easel.Graphics.Primitives;
using Silk.NET.Assimp;
using Material = Easel.Graphics.Materials.Material;

namespace Easel.Graphics;

public struct Mesh
{
    public VertexPositionTextureNormalTangent[] Vertices;
    public uint[] Indices;
    public Material Material;

    public Mesh(VertexPositionTextureNormalTangent[] vertices, uint[] indices, Material material)
    {
        Vertices = vertices;
        Indices = indices;
        Material = material;
    }
}