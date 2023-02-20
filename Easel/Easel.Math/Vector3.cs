using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector3T<T> : IEquatable<Vector3T<T>> where T : INumber<T>
{
    public static Vector3T<T> Zero => new Vector3T<T>(T.Zero);

    public static Vector3T<T> One => new Vector3T<T>(T.One);

    public static Vector3T<T> UnitX => new Vector3T<T>(T.One, T.Zero, T.Zero);

    public static Vector3T<T> UnitY => new Vector3T<T>(T.Zero, T.One, T.Zero);

    public static Vector3T<T> UnitZ => new Vector3T<T>(T.Zero, T.Zero, T.One);
    
    [XmlAttribute]
    public T X;
    
    [XmlAttribute]
    public T Y;
    
    [XmlAttribute]
    public T Z;

    public Vector3T(T scalar)
    {
        X = scalar;
        Y = scalar;
        Z = scalar;
    }

    public Vector3T(Vector2<T> xy, T z)
    {
        X = xy.X;
        Y = xy.Y;
        Z = z;
    }

    public Vector3T(T x, T y, T z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    #region Operators

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3T<T> operator +(Vector3T<T> left, Vector3T<T> right) =>
        new Vector3T<T>(left.X + right.X, left.Y + right.Y, left.Z + right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3T<T> operator -(Vector3T<T> left, Vector3T<T> right) =>
        new Vector3T<T>(left.X - right.X, left.Y - right.Y, left.Z - right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3T<T> operator -(Vector3T<T> negate)
    {
        return new Vector3T<T>(-negate.X, -negate.Y, -negate.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3T<T> operator *(Vector3T<T> left, Vector3T<T> right) =>
        new Vector3T<T>(left.X * right.X, left.Y * right.Y, left.Z * right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3T<T> operator *(Vector3T<T> left, T right) =>
        new Vector3T<T>(left.X * right, left.Y * right, left.Z * right);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3T<T> operator *(T left, Vector3T<T> right) =>
        new Vector3T<T>(left * right.X, left * right.Y, left * right.Z);

    public static Vector3T<T> operator *(Vector3T<T> left, Matrix<T> right)
    {
        return new Vector3T<T>(
            (left.X * right.Row0.X) + (left.Y * right.Row1.X) + (left.Z * right.Row2.X) + right.Row3.X,
            (left.X * right.Row0.Y) + (left.Y * right.Row1.Y) + (left.Z * right.Row2.Y) + right.Row3.Y,
            (left.X * right.Row0.Z) + (left.Y * right.Row1.Z) + (left.Z * right.Row2.Z) + right.Row3.Z
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3T<T> operator /(Vector3T<T> left, Vector3T<T> right) =>
        new Vector3T<T>(left.X / right.X, left.Y / right.Y, left.Z / right.Z);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3T<T> operator /(Vector3T<T> left, T right) =>
        new Vector3T<T>(left.X / right, left.Y / right, left.Z / right);

    #endregion

    #region Swizzle
    
    // TODO: Swizzle auto generator

    #endregion

    public bool Equals(Vector3T<T> other)
    {
        return EqualityComparer<T>.Default.Equals(X, other.X) && EqualityComparer<T>.Default.Equals(Y, other.Y) && EqualityComparer<T>.Default.Equals(Z, other.Z);
    }

    public override bool Equals(object obj)
    {
        return obj is Vector3T<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public static bool operator ==(Vector3T<T> left, Vector3T<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Vector3T<T> left, Vector3T<T> right)
    {
        return !left.Equals(right);
    }

    public override string ToString()
    {
        return "Vector3T<" + typeof(T) + ">(X: " + X + ", Y: " + Y + ", Z: " + Z + ")";
    }
    
    public static explicit operator System.Numerics.Vector3(Vector3T<T> vector)
    {
        float x = Convert.ToSingle(vector.X);
        float y = Convert.ToSingle(vector.Y);
        float z = Convert.ToSingle(vector.Z);
        return new System.Numerics.Vector3(x, y, z);
    }
    
    public static explicit operator Vector3T<T>(System.Numerics.Vector3 vector)
    {
        return new Vector3T<T>(T.CreateChecked(vector.X), T.CreateChecked(vector.Y), T.CreateChecked(vector.Z));
    }
}

public static class Vector3T
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Magnitude<T>(Vector3T<T> value) where T : INumber<T>, IRootFunctions<T>
    {
        return T.Sqrt(MagnitudeSquared(value));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T MagnitudeSquared<T>(Vector3T<T> value) where T : INumber<T>
    {
        return Dot(value, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(Vector3T<T> left, Vector3T<T> right) where T : INumber<T>
    {
        return T.CreateChecked(left.X * right.X + left.Y * right.Y + left.Z * right.Z);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3T<T> Normalize<T>(Vector3T<T> vector) where T : INumber<T>, IRootFunctions<T>
    {
        T magnitude = Magnitude(vector);
        return new Vector3T<T>(vector.X / magnitude, vector.Y / magnitude, vector.Z / magnitude);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Distance<T>(Vector3T<T> vector1, Vector3T<T> vector2) where T : INumber<T>, IRootFunctions<T>
    {
        return T.Sqrt(DistanceSquared(vector1, vector2));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T DistanceSquared<T>(Vector3T<T> vector1, Vector3T<T> vector2) where T : INumber<T>
    {
        Vector3T<T> dist = vector2 - vector1;
        return Dot(dist, dist);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Vector3T<T> Abs<T>(Vector3T<T> vector) where T : INumber<T>
    {
        return new Vector3T<T>(T.Abs(vector.X), T.Abs(vector.Y), T.Abs(vector.Z));
    }
}