using System.Numerics;
using System.Runtime.InteropServices;

namespace Easel.Graphics.Renderers.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct SceneInfo
{
    public Vector4 CameraPos;
    public ShaderMaterial Material;
    public ShaderDirLight Sun;
}