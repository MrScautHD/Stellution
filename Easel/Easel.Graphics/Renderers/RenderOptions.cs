namespace Easel.Graphics.Renderers;

public struct RenderOptions
{
    /// <summary>
    /// If enabled, Easel will use a Deferred pipeline. Otherwise, a Forward+ pipeline will be used.
    /// </summary>
    public bool Deferred;

    public static RenderOptions Default => new RenderOptions()
    {
        Deferred = false
    };
}