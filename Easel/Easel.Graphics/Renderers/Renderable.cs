using System;
using System.Collections.Generic;
using Easel.Graphics.Materials;
using Pie;

namespace Easel.Graphics.Renderers;

public struct Renderable : IDisposable
{
    public GraphicsBuffer VertexBuffer;
    public GraphicsBuffer IndexBuffer;

    public uint NumIndices;
    public Material Material;

    public Renderable(GraphicsBuffer vertexBuffer, GraphicsBuffer indexBuffer, uint numIndices, Material material)
    {
        VertexBuffer = vertexBuffer;
        IndexBuffer = indexBuffer;
        NumIndices = numIndices;
        Material = material;
    }

    public static Renderable CreateFromMesh(in Mesh mesh)
    {
        GraphicsDevice device = EaselGraphics.Instance.PieGraphics;

        GraphicsBuffer vertexBuffer = device.CreateBuffer(BufferType.VertexBuffer, mesh.Vertices);
        GraphicsBuffer indexBuffer = device.CreateBuffer(BufferType.IndexBuffer, mesh.Indices);

        return new Renderable(vertexBuffer, indexBuffer, (uint) mesh.Indices.Length, mesh.Material);
    }

    public static Renderable[] CreateFromMeshes(in Mesh[] meshes)
    {
        List<Renderable> renderables = new List<Renderable>(meshes.Length);
        
        for (int i = 0; i < meshes.Length; i++)
            renderables.Add(CreateFromMesh(meshes[i]));

        return renderables.ToArray();
    }

    public void Dispose()
    {
        VertexBuffer.Dispose();
        IndexBuffer.Dispose();
    }
}