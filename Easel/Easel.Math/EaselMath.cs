using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Easel.Math;

/// <summary>
/// Provides helpful math functions useful in game development, such as linear interpolation and clamping.
/// </summary>
public static class EaselMath
{
    /// <summary>
    /// Convert degrees to radians.
    /// </summary>
    /// <param name="degrees">The value in degrees.</param>
    /// <returns>The converted value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToRadians(float degrees) => degrees * (MathF.PI / 180);

    /// <summary>
    /// Convert radians to degrees.
    /// </summary>
    /// <param name="radians">The value in radians.</param>
    /// <returns>The converted value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToDegrees(float radians) => radians * (180 / MathF.PI);

    /// <summary>
    /// <b>L</b>inearly int<b>erp</b>olate between two <see langword="float"/> values, from the given <b>normalized</b>
    /// multiplier value (aka 0 = min, 1 = max)
    /// </summary>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <param name="multiplier">The normalized multiplier</param>
    /// <returns>The interpolated value.</returns>
    /// <remarks>The <paramref name="multiplier"/> value can be outside of the 0-1 range, you will just get numbers larger
    /// or smaller than the max/min values respectively.</remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Lerp<T>(T min, T max, T multiplier) where T : INumber<T> => multiplier * (max - min) + min;

    /// <summary>
    /// Clamp the given value between the min and max values.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The clamped value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Clamp<T>(T value, T min, T max) where T : INumber<T> => value <= min ? min : value >= max ? max : value;

    /// <summary>
    /// Similar to <see cref="Clamp{T}"/>, the given value cannot exceed the bounds of the min and max values.
    /// If it does, it will wrap around back to the other value. For example, if max is exceeded, the value will wrap
    /// around back to min. This works both ways.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    /// <param name="min">The minimum value (inclusive).</param>
    /// <param name="max">The maximum value (inclusive).</param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Wrap<T>(T value, T min, T max) where T : INumber<T> => value < min ? max : value > max ? min : value;

    /// <summary>
    /// Calculates a position along a quadratic bezier curve that outputs a value based on the given variable for t
    /// along the given control points, taken as <see cref="Vector2"/>.
    /// </summary>
    /// <param name="t">A normalized value representing the change in time (elapsed position) of the bezier curve.</param>
    /// <remarks>
    /// Use <see cref="InverseLerp"/> to get a normalized value from any point from a defined
    /// interval, if your changing parameter is not normalized.
    /// </remarks>
    /// <param name="p0">A <see cref="Vector2"/> representing the first control point of the curve.</param>
    /// <param name="p1">A <see cref="Vector2"/> representing the second control point of the curve.</param>
    /// <param name="p2">A <see cref="Vector2"/> representing the third control point of the curve.</param>
    /// <returns>A <see cref="Vector2"/> result position along the curve, based on t.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> QuadraticBezierCurve<T>(T t, Vector2<T> p0, Vector2<T> p1, Vector2<T> p2) where T : INumber<T> 
    {
        return p1 + ((T.One - t) * (T.One - t)) * (p0 - p1) + (t * t) * (p2 - p1);
    }

    /// <summary>
    /// Calculates a position along a quadratic bezier curve that outputs a value based on the given variable for t
    /// along the given control points, taken as <see cref="Vector2"/>.
    /// </summary>
    /// <param name="t">A normalized value representing the change in time (elapsed position) of the bezier curve.
    /// </param>
    /// <remarks>
    /// Use <see cref="InverseLerp"/> to get a normalized value from any point from a defined
    /// interval, if your changing parameter is not normalized.
    /// </remarks>
    /// <param name="p0">A <see cref="Vector2"/> representing the first control point of the curve.</param>
    /// <param name="p1">A <see cref="Vector2"/> representing the second control point of the curve.</param>
    /// <param name="p2">A <see cref="Vector2"/> representing the third control point of the curve.</param>
    /// <param name="p3">A <see cref="Vector2"/> representing the fourth control point of the curve.</param>
    /// <returns>A <see cref="Vector2"/> result position along the curve, based on t.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> CubicBezierCurve<T>(T t, Vector2<T> p0, Vector2<T> p1, Vector2<T> p2, Vector2<T> p3) where T : INumber<T>
    {
        return (T.One - t) * QuadraticBezierCurve(t, p0, p1, p2) + t * QuadraticBezierCurve(t, p1, p2, p3);
    }

    /// <summary>
    /// Same thing as <see cref="CubicBezierCurve"/>, except the formula is calculated directly, rather than through
    /// calling <see cref="QuadraticBezierCurve"/>. Should be identical in functionality.
    /// </summary>
    /// <param name="t">A normalized value representing the change in time (elapsed position) of the bezier curve.</param>
    /// <remarks>
    /// Use <see cref="InverseLerp"/> to get a normalized value from any point from a defined
    /// interval, if your changing parameter is not normalized.
    /// </remarks>
    /// <param name="p0">A <see cref="Vector2"/> representing the first control point of the curve.</param>
    /// <param name="p1">A <see cref="Vector2"/> representing the second control point of the curve.</param>
    /// <param name="p2">A <see cref="Vector2"/> representing the third control point of the curve.</param>
    /// <param name="p3">A <see cref="Vector2"/> representing the fourth control point of the curve.</param>
    /// <returns>A <see cref="Vector2"/> result position along the curve, based on t.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> ExplicitCubicBezierCurve<T>(T t, Vector2<T> p0, Vector2<T> p1, Vector2<T> p2, Vector2<T> p3) where T : INumber<T>
    {
        return ((T.One - t) * (T.One - t) * (T.One - t)) * p0 + T.CreateChecked(3) * ((T.One - t) * (T.One - t)) * t * p1 + T.CreateChecked(3) * ((T.One - t) * (T.One - t)) * p2 + (t * t * t) * p3;
    }

    /// <summary>
    /// An extremely cheap, simple way to estimate the arc length, taken as the average between the chord and control net.
    /// </summary>
    /// <remarks>See here: https://stackoverflow.com/questions/29438398/cheap-way-of-calculating-cubic-bezier-length</remarks>
    /// <param name="p0">A <see cref="Vector2"/> representing the first control point of the curve.</param>
    /// <param name="p1">A <see cref="Vector2"/> representing the second control point of the curve.</param>
    /// <param name="p2">A <see cref="Vector2"/> representing the third control point of the curve.</param>
    /// <param name="p3">A <see cref="Vector2"/> representing the fourth control point of the curve.</param>
    /// <returns>An estimated arclength of the bezier curve.</returns>
    public static T GetBezierArcLength<T>(Vector2<T> p0, Vector2<T> p1, Vector2<T> p2, Vector2<T> p3) where T : INumber<T>, IRootFunctions<T>
    {
        T chord = Vector2.Magnitude(p3 - p0);
        T controlNet = Vector2.Magnitude(p0 - p1) + Vector2.Magnitude(p2 - p1) + Vector2.Magnitude(p3 - p2);

        return (controlNet + chord) / T.CreateChecked(2);
    }

    /// <summary>
    /// Returns a normalized point (0 - 1) representative of a given interval.
    /// </summary>
    /// <example>
    /// For example, an interval from 200 to 300, given the point 250, would return the normalized value .5.
    /// </example>
    /// <param name="min">The minimum value of the interval.</param>
    /// <param name="max">The maximum value of the interval.</param>
    /// <param name="point">The given point within the interval that is to be normalized.</param>
    /// <returns>A normalized point representing the given point in the interval.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T InverseLerp<T>(T min, T max, T point) where T : INumber<T>
    {
        return (point - min) / (max - min);
    }
    
}