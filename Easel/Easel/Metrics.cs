using System;
using Pie;

namespace Easel;

public static class Metrics
{
    private static ulong _totalFrames;
    private static float _secondsCounter;
    private static int _framesInSecond;
    private static int _fps;

    public static ulong TotalFrames => _totalFrames;

    // hec u rider
    // ReSharper disable once InconsistentNaming
    public static int FPS => _fps;

    public static string GetString()
    {
        return $"FPS: {FPS} (dt: {MathF.Round((Time.DeltaTime) * 1000, 1)}ms)\nFrame: {TotalFrames}\nTotal VBuffers: {PieMetrics.VertexBufferCount}\nTotal IBuffers: {PieMetrics.IndexBufferCount}\nTotal CBuffers: {PieMetrics.UniformBufferCount}\nDraws: {PieMetrics.DrawCalls}\nTris: {PieMetrics.TriCount}\nBackend: {EaselGame.Instance.GraphicsInternal.PieGraphics.Api.ToFriendlyString()}\nPie {PieMetrics.Version}\nEasel {EaselGame.Version}";
    }

    internal static void Update()
    {
        _totalFrames++;
        _secondsCounter += Time.DeltaTime;
        _framesInSecond++;
        if (_secondsCounter >= 1)
        {
            _secondsCounter = 0;
            _fps = _framesInSecond;
            _framesInSecond = 0;
        }
    }
}