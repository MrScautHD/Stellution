using System.Numerics;

namespace Easel.Graphics;

public class ModelMesh
{
    public Mesh[] Meshes;

    public Matrix4x4 Transform;

    public ModelMesh(Mesh[] meshes, Matrix4x4 transform)
    {
        Meshes = meshes;
        Transform = transform;
    }
}