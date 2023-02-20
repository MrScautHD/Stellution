using System.Numerics;
using Easel.Math;

namespace Easel.Graphics.Renderers.Structs;

public struct CameraInfo
{
    public Matrix4x4 Projection;
    public Matrix4x4 View;
    public Vector3 Position;

    public Color? ClearColor;
    public Skybox Skybox;

    /// <summary>
    /// Create new camera info. Using this constructor will set the <see cref="Position"/> value automatically.
    /// </summary>
    /// <param name="projection">The projection matrix.</param>
    /// <param name="view">The view matrix.</param>
    /// <param name="clearColor">The clear color, if any. If a clear color is not set, the scene will not be cleared.</param>
    /// <param name="skybox">The skybox, if any.</param>
    public CameraInfo(Matrix4x4 projection, Matrix4x4 view, Color? clearColor = null, Skybox skybox = null)
    {
        Projection = projection;
        View = view;
        ClearColor = clearColor;
        Skybox = skybox;

        Matrix4x4.Invert(view, out Matrix4x4 invView);
        Position = invView.Translation;
    }
}