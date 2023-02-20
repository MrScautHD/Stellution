using System.Diagnostics;
using Easel.Core;
using Pie.Windowing;

namespace Easel;

/// <summary>
/// Provides engine time functions.
/// </summary>
public static class Time
{
    private static double _deltaTime;
    
    internal static Stopwatch InternalStopwatch;
    private static Stopwatch _timerWatch;

    /// <summary>
    /// Get the amount of time passed since the previous frame. Use to create framerate independent actions.
    /// </summary>
    public static float DeltaTime => (float) _deltaTime;

    public static float TotalSeconds => (float) _timerWatch.Elapsed.TotalSeconds;

    internal static void Initialize()
    {
        InternalStopwatch = Stopwatch.StartNew();
        _timerWatch = Stopwatch.StartNew();
    }

    internal static void Update()
    { 
        _deltaTime = (float) InternalStopwatch.Elapsed.TotalSeconds;
        InternalStopwatch.Restart();

        //double time = view.Time;
        //double time = _stopwatch.Elapsed.TotalSeconds;
        //_deltaTime = time - _lastTime;
        //_lastTime = time;
    }
}