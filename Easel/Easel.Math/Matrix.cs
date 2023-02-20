using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Matrix<T> where T : INumber<T>
{
    public static Matrix<T> Identity =>
        new Matrix<T>(Vector4T<T>.UnitX, Vector4T<T>.UnitY, Vector4T<T>.UnitZ, Vector4T<T>.UnitW);
    
    public Vector4T<T> Row0;

    public Vector4T<T> Row1;

    public Vector4T<T> Row2;

    public Vector4T<T> Row3;

    public Matrix(Vector4T<T> row0, Vector4T<T> row1, Vector4T<T> row2, Vector4T<T> row3)
    {
        Row0 = row0;
        Row1 = row1;
        Row2 = row2;
        Row3 = row3;
    }

    #region Operators

    public static Matrix<T> operator *(Matrix<T> left, Matrix<T> right)
    {
        Vector4T<T> rightColumn0 = new Vector4T<T>(right.Row0.X, right.Row1.X, right.Row2.X, right.Row3.X);
        Vector4T<T> rightColumn1 = new Vector4T<T>(right.Row0.Y, right.Row1.Y, right.Row2.Y, right.Row3.Y);
        Vector4T<T> rightColumn2 = new Vector4T<T>(right.Row0.Z, right.Row1.Z, right.Row2.Z, right.Row3.Z);
        Vector4T<T> rightColumn3 = new Vector4T<T>(right.Row0.W, right.Row1.W, right.Row2.W, right.Row3.W);
        
        Vector4T<T> row0 = new Vector4T<T>(Vector4T.Dot(left.Row0, rightColumn0), Vector4T.Dot(left.Row0, rightColumn1), Vector4T.Dot(left.Row0, rightColumn2), Vector4T.Dot(left.Row0, rightColumn3));
        Vector4T<T> row1 = new Vector4T<T>(Vector4T.Dot(left.Row1, rightColumn0), Vector4T.Dot(left.Row1, rightColumn1), Vector4T.Dot(left.Row1, rightColumn2), Vector4T.Dot(left.Row1, rightColumn3));
        Vector4T<T> row2 = new Vector4T<T>(Vector4T.Dot(left.Row2, rightColumn0), Vector4T.Dot(left.Row2, rightColumn1), Vector4T.Dot(left.Row2, rightColumn2), Vector4T.Dot(left.Row2, rightColumn3));
        Vector4T<T> row3 = new Vector4T<T>(Vector4T.Dot(left.Row3, rightColumn0), Vector4T.Dot(left.Row3, rightColumn1), Vector4T.Dot(left.Row3, rightColumn2), Vector4T.Dot(left.Row3, rightColumn3));

        return new Matrix<T>(row0, row1, row2, row3);
    }

    #endregion

    public Vector3T<T> Translation => Row3.XYZ;
    
    public static explicit operator Matrix<T>(Matrix4x4 matrix)
    {
        Vector4T<T> row0 = new Vector4T<T>(T.CreateChecked(matrix.M11), T.CreateChecked(matrix.M12), T.CreateChecked(matrix.M13), T.CreateChecked(matrix.M14));
        Vector4T<T> row1 = new Vector4T<T>(T.CreateChecked(matrix.M21), T.CreateChecked(matrix.M22), T.CreateChecked(matrix.M23), T.CreateChecked(matrix.M14));
        Vector4T<T> row2 = new Vector4T<T>(T.CreateChecked(matrix.M31), T.CreateChecked(matrix.M32), T.CreateChecked(matrix.M13), T.CreateChecked(matrix.M14));
        Vector4T<T> row3 = new Vector4T<T>(T.CreateChecked(matrix.M41), T.CreateChecked(matrix.M42), T.CreateChecked(matrix.M13), T.CreateChecked(matrix.M14));
        
        return new Matrix<T>(row0, row1, row2, row3);
    }

    public override string ToString()
    {
        return "Matrix<" + typeof(T) + ">(Row0: " + Row0 + ", Row1: " + Row1 + ", Row2: " + Row2 + ", Row3: " + Row3 + ")";
    }
}

public static class Matrix
{
    public static Matrix<T> Translate<T>(Vector3T<T> translation) where T : INumber<T>
    {
        return Matrix<T>.Identity with { Row3 = new Vector4T<T>(translation, T.CreateChecked(1)) };
    }
    
    public static Matrix<T> Scale<T>(Vector3T<T> scale) where T : INumber<T>
    {
        return Scale(scale.X, scale.Y, scale.Z);
    }

    public static Matrix<T> Scale<T>(T width, T height, T depth) where T : INumber<T>
    {
        Matrix<T> result = Matrix<T>.Identity;
        result.Row0.X = width;
        result.Row1.Y = height;
        result.Row2.Z = depth;
        return result;
    }
    
    public static Matrix<TFloat> RotateX<TFloat>(TFloat rotation)
        where TFloat : INumber<TFloat>, ITrigonometricFunctions<TFloat>
    {
        TFloat sinTheta = TFloat.Sin(rotation);
        TFloat cosTheta = TFloat.Cos(rotation);
        
        Matrix<TFloat> result = Matrix<TFloat>.Identity;
        result.Row1.Y = cosTheta;
        result.Row1.Z = sinTheta;
        result.Row2.Y = -sinTheta;
        result.Row2.Z = cosTheta;
        return result;
    }
    
    public static Matrix<TFloat> RotateY<TFloat>(TFloat rotation)
        where TFloat : INumber<TFloat>, ITrigonometricFunctions<TFloat>
    {
        TFloat sinTheta = TFloat.Sin(rotation);
        TFloat cosTheta = TFloat.Cos(rotation);
        
        Matrix<TFloat> result = Matrix<TFloat>.Identity;
        result.Row0.X = cosTheta;
        result.Row0.Z = -sinTheta;
        result.Row2.X = sinTheta;
        result.Row2.Z = cosTheta;
        return result;
    }
    
    public static Matrix<TFloat> RotateZ<TFloat>(TFloat rotation) 
        where TFloat : INumber<TFloat>, ITrigonometricFunctions<TFloat>
    {
        TFloat sinTheta = TFloat.Sin(rotation);
        TFloat cosTheta = TFloat.Cos(rotation);

        Matrix<TFloat> result = Matrix<TFloat>.Identity;
        result.Row0.X = cosTheta;
        result.Row0.Y = sinTheta;
        result.Row1.X = -sinTheta;
        result.Row1.Y = cosTheta;
        return result;
    }

    public static Matrix<T> FromQuaternion<T>(QuaternionT<T> quaternion) where T : INumber<T>
    {
        T x = quaternion.X;
        T y = quaternion.Y;
        T z = quaternion.Z;
        T w = quaternion.W;

        T one = T.One;
        T two = T.One + T.One;

        T xx = x * x;
        T xy = x * y;
        T xz = x * z;
        T xw = x * w;
        T yy = y * y;
        T yz = y * z;
        T yw = y * w;
        T zz = z * z;
        T zw = z * w;

        Vector4T<T> row0 = new Vector4T<T>(one - two * yy - two * zz, two * xy + two * zw, two * xz - two * yw, T.Zero);
        Vector4T<T> row1 = new Vector4T<T>(two * xy - two * zw, one - two * xx - two * zz, two * yz + two * xw, T.Zero);
        Vector4T<T> row2 = new Vector4T<T>(two * xz + two * yw, two * yz - two * xw, one - two * xx - two * yy, T.Zero);
        Vector4T<T> row3 = Vector4T<T>.UnitW;

        return new Matrix<T>(row0, row1, row2, row3);
    }

    public static Matrix<T> FromEuler<T>(Vector3T<T> euler) where T : INumber<T>, ITrigonometricFunctions<T>
    {
        return FromEuler(euler.X, euler.Y, euler.Z);
    }

    public static Matrix<T> FromEuler<T>(T yaw, T pitch, T roll) where T : INumber<T>, ITrigonometricFunctions<T>
    {
        // y = yaw, p = pitch, r = roll
        T sy = T.Sin(yaw);
        T cy = T.Cos(yaw);
        T sp = T.Sin(pitch);
        T cp = T.Cos(pitch);
        T sr = T.Sin(roll);
        T cr = T.Cos(roll);

        T zero = T.Zero;
        T one = T.One;

        return new Matrix<T>(
            new Vector4T<T>(cy * cp, sp, -sy * cp, zero),
            new Vector4T<T>(-cy * sp * cr + sy * sr, cp * cr, sy * sp * cr + cy * sr, zero),
            new Vector4T<T>(cy * sp * cr + sy * sr, -cp * cr, -sy * sp * cr + cy * sr, zero),
            new Vector4T<T>(zero, zero, zero, one)
        );
    }

    /*public static Matrix<T> Invert<T>(Matrix<T> matrix) where T : INumber<T>
    {
        T determinant = matrix.Row0.X * (matrix.Row1.Y * matrix.Row2.Z * matrix.Row3.W + matrix.Row1.Z * matrix.Row2.W * matrix.Row3.Y + matrix.Row1.W * matrix.Row2.Y * matrix.Row3.Z - matrix.Row1.W * matrix.Row2.Z * matrix.Row3.Y - matrix.Row1.Z * matrix.Row2.Y * matrix.Row3.W - matrix.Row1.Y * matrix.Row2.W * matrix.Row3.Z) - 
                        matrix.Row1.X * (matrix.Row0.Y * matrix.Row2.Z * matrix.Row3.W + matrix.Row0.Z * matrix.Row2.W * matrix.Row3.W + matrix.Row0.W * matrix.Row2.Y * matrix.Row3.Z - matrix.Row0.W * matrix.Row2.Z * matrix.Row3.Y - matrix.Row0.Z * matrix.Row2.Y * matrix.Row3.W - matrix.Row0.Y * matrix.Row2.W * matrix.Row3.Z) - 
                        
    }*/
}