using System.Numerics;
using System.Runtime.InteropServices;
using Easel.Math;

namespace Easel.Graphics.Renderers.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct ShaderDirLight
{
    public Vector4 Direction;
    public Color Color;
}