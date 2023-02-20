using System.Numerics;
using System.Runtime.InteropServices;
using Easel.Math;

namespace Easel.Graphics.Renderers.Structs;

/// <summary>
/// Contains all the possible material parameters that can be sent to the shader.
/// Some shaders will completely ignore certain parameters.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct ShaderMaterial
{
    public Vector4 Tiling;
    
    public Color Albedo;

    public float Metallic;

    public float Roughness;

    public float Ao;

    private float _padding;
}