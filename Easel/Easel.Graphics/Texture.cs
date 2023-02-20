using System;
using Easel.Core;
using Easel.Math;

namespace Easel.Graphics;

/// <summary>
/// The base texture class, for objects that can be textured.
/// </summary>
public abstract class Texture : IDisposable
{
    /// <summary>
    /// Returns <see langword="true"/> if this <see cref="Texture"/> has been disposed.
    /// </summary>
    public bool IsDisposed { get; protected set; }
    
    /// <summary>
    /// The native Pie <see cref="Pie.Texture"/>.
    /// </summary>
    public Pie.Texture PieTexture { get; protected set; }
    
    public SamplerState SamplerState { get; set; }

    /// <summary>
    /// The size (resolution), in pixels of the texture.
    /// </summary>
    public Size<int> Size => (Size<int>) PieTexture.Size;

    protected Texture(SamplerState state, bool autoDispose)
    {
        SamplerState = state;
        
        if (autoDispose)
            EaselGraphics.Instance.Disposables.Add(this);
    }

    public virtual void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        PieTexture.Dispose();
        Logger.Debug("Texture disposed.");
    }
}