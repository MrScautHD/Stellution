using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Vector4T<T> where T : INumber<T>
{
    public static Vector4T<T> Zero => new Vector4T<T>(T.Zero);

    public static Vector4T<T> One => new Vector4T<T>(T.One);

    public static Vector4T<T> UnitX =>
        new Vector4T<T>(T.One, T.Zero, T.Zero, T.Zero);
    
    public static Vector4T<T> UnitY =>
        new Vector4T<T>(T.Zero, T.One, T.Zero, T.Zero);
    
    public static Vector4T<T> UnitZ =>
        new Vector4T<T>(T.Zero, T.Zero, T.One, T.Zero);
    
    public static Vector4T<T> UnitW =>
        new Vector4T<T>(T.Zero, T.Zero, T.Zero, T.One);

    [XmlAttribute]
    public T X;

    [XmlAttribute]
    public T Y;

    [XmlAttribute]
    public T Z;

    [XmlAttribute]
    public T W;

    public Vector4T(T scalar)
    {
        X = scalar;
        Y = scalar;
        Z = scalar;
        W = scalar;
    }

    public Vector4T(Vector2<T> xy, T z, T w)
    {
        X = xy.X;
        Y = xy.Y;
        Z = z;
        W = w;
    }
    
    public Vector4T(Vector3T<T> xyz, T w)
    {
        X = xyz.X;
        Y = xyz.Y;
        Z = xyz.Z;
        W = w;
    }

    public Vector4T(T x, T y, T z, T w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }
    
    #region Swizzle

    public Vector3T<T> XYZ => new Vector3T<T>(X, Y, Z);
    
    #endregion

    public override string ToString()
    {
        return "Vector4T<" + typeof(T) + ">(X: " + X + ", Y: " + Y + ", Z: " + Z + ", W: " + W + ")";
    }
    
    public static implicit operator Vector4(Vector4T<T> vector)
    {
        float x = Convert.ToSingle(vector.X);
        float y = Convert.ToSingle(vector.Y);
        float z = Convert.ToSingle(vector.Z);
        float w = Convert.ToSingle(vector.W);
        return new Vector4(x, y, z, w);
    }

    public static implicit operator Vector4T<T>(Vector4 vector)
    {
        return new Vector4T<T>(T.CreateChecked(vector.X), T.CreateChecked(vector.Y), T.CreateChecked(vector.Z),
            T.CreateChecked(vector.W));
    }
}

public static class Vector4T
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T Dot<T>(Vector4T<T> left, Vector4T<T> right) where T : INumber<T>
    {
        return T.CreateChecked(left.X * right.X + left.Y * right.Y + left.Z * right.Z + left.W * right.W);
    }
}