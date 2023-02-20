using System;
using System.Numerics;
using Easel.Graphics.Lighting;
using Easel.Graphics.Renderers.Structs;

namespace Easel.Graphics.Renderers;

public interface IRenderer : IDisposable
{
    public DirectionalLight? DirectionalLight { get; set; }
    
    public RenderTarget MainTarget { get; set; }
    
    /// <summary>
    /// Draw an object instance that will be rendered in the scene.
    /// </summary>
    /// <param name="renderable">The renderable instance to draw.</param>
    /// <param name="world">Its world transform.</param>
    public void Draw(in Renderable renderable, in Matrix4x4 world);

    /// <summary>
    /// Draw a sprite that will be batched and rendered in the scene.
    /// </summary>
    /// <param name="sprite">The sprite to render.</param>
    public void DrawSprite(in Sprite sprite);

    /// <summary>
    /// Prepare the renderer for a new frame of objects.
    /// </summary>
    public void NewFrame();

    public void DoneFrame();
    
    /// <summary>
    /// Render all objects added to the current render frame.
    /// </summary>
    public void Perform3DPass(CameraInfo cameraInfo);

    public void Perform2DPass(CameraInfo cameraInfo);
}