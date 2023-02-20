using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector2<T> : IEquatable<Vector2<T>> where T : INumber<T>
{
    public static Vector2<T> Zero => new Vector2<T>(T.Zero);

    public static Vector2<T> One => new Vector2<T>(T.One);

    public static Vector2<T> UnitX => new Vector2<T>(T.One, T.Zero);

    public static Vector2<T> UnitY => new Vector2<T>(T.Zero, T.One);

    [XmlAttribute]
    public T X;

    [XmlAttribute]
    public T Y;

    public Vector2(T scalar)
    {
        X = scalar;
        Y = scalar;
    }

    public Vector2(T x, T y)
    {
        X = x;
        Y = y;
    }
    
    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator +(Vector2<T> left, Vector2<T> right) =>
        new Vector2<T>(left.X + right.X, left.Y + right.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator -(Vector2<T> left, Vector2<T> right) =>
        new Vector2<T>(left.X - right.X, left.Y - right.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator -(Vector2<T> negate)
    {
        return new Vector2<T>(-negate.X, -negate.Y);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator *(Vector2<T> left, Vector2<T> right) =>
        new Vector2<T>(left.X * right.X, left.Y * right.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator *(Vector2<T> left, T right) =>
        new Vector2<T>(left.X * right, left.Y * right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator *(T left, Vector2<T> right) =>
        new Vector2<T>(left * right.X, left * right.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator /(Vector2<T> left, Vector2<T> right) =>
        new Vector2<T>(left.X / right.X, left.Y / right.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<T> operator /(Vector2<T> left, T right) =>
        new Vector2<T>(left.X / right, left.Y / right);

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2<TOther> As<TOther>() where TOther : INumber<TOther> =>
        Vector2.Cast<T, TOther>(this);

    public bool Equals(Vector2<T> other)
    {
        return EqualityComparer<T>.Default.Equals(X, other.X) && EqualityComparer<T>.Default.Equals(Y, other.Y);
    }

    public override bool Equals(object obj)
    {
        return obj is Vector2<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Vector2<T> left, Vector2<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector2<T> left, Vector2<T> right)
    {
        return !left.Equals(right);
    }

    public static explicit operator System.Numerics.Vector2(Vector2<T> vector)
    {
        float x = Convert.ToSingle(vector.X);
        float y = Convert.ToSingle(vector.Y);
        return new System.Numerics.Vector2(x, y);
    }
    
    public static explicit operator Vector2<T>(System.Numerics.Vector2 vector)
    {
        return new Vector2<T>(T.CreateChecked(vector.X), T.CreateChecked(vector.Y));
    }
    
    #region Quick conversions

    public static explicit operator Vector2<float>(Vector2<T> value)
    {
        return Vector2.Cast<T, float>(value);
    }

    public static explicit operator Vector2<int>(Vector2<T> value)
    {
        return Vector2.Cast<T, int>(value);
    }

    public static explicit operator Vector2<double>(Vector2<T> value)
    {
        return Vector2.Cast<T, double>(value);
    }
    
    #endregion
}

public static class Vector2
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector2<TOther> Cast<T, TOther>(Vector2<T> vector) where T : INumber<T> where TOther : INumber<TOther>
    {
        return new Vector2<TOther>(TOther.CreateChecked(vector.X), TOther.CreateChecked(vector.Y));
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Magnitude<T>(Vector2<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        return T.Sqrt(MagnitudeSquared(vector));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T MagnitudeSquared<T>(Vector2<T> vector) where T : INumber<T>
    {
        return Dot(vector, vector);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(Vector2<T> left, Vector2<T> right) where T : INumber<T>
    {
        return T.CreateChecked(left.X * right.X + left.Y * right.Y);
    }
}