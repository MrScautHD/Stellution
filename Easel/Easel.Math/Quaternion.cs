using System.Numerics;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct QuaternionT<T> where T : INumber<T>
{
    public static QuaternionT<T> Identity => new QuaternionT<T>(T.Zero, T.Zero, T.Zero, T.One);

    [XmlAttribute]
    public T X;

    [XmlAttribute]
    public T Y;

    [XmlAttribute]
    public T Z;

    [XmlAttribute]
    public T W;

    public QuaternionT(T x, T y, T z, T w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public override string ToString()
    {
        return "QuaternionT<" + typeof(T) + ">(X: " + X + ", Y: " + Y + ", Z: " + Z + ", W: " + W + ")";
    }
}

public static class QuaternionT
{
    public static QuaternionT<T> FromEuler<T>(Vector3T<T> euler) where T : INumber<T>, ITrigonometricFunctions<T>
    {
        return FromEuler(euler.X, euler.Y, euler.Z);
    }

    public static QuaternionT<T> FromEuler<T>(T yaw, T pitch, T roll) where T : INumber<T>, ITrigonometricFunctions<T>
    {
        T half = T.CreateChecked(0.5);
        T cy = T.Cos(yaw * half);
        T sy = T.Sin(yaw * half);
        T cp = T.Cos(pitch * half);
        T sp = T.Sin(pitch * half);
        T cr = T.Cos(roll * half);
        T sr = T.Sin(roll * half);

        T x = cr * sp * cy + sr * cp * sy;
        T y = cr * cp * sy - sr * sp * cy;
        T z = sr * cp * cy - cr * sp * sy;
        T w = cr * cp * cy + sr * sp * sy;
        return new QuaternionT<T>(x, y, z, w);
    }
}