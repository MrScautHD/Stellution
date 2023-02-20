using System;
using System.IO;
using System.IO.Compression;
using System.Numerics;
using System.Reflection;
using System.Text;
using Easel.Math;
using Vector3 = System.Numerics.Vector3;
using Mth = System.Math;

namespace Easel.Core;

/// <summary>
/// Provides certain utilities and extension methods.
/// </summary>
public static class Utils
{
    /// <summary>
    /// Get a random value between the min (inclusive) and max (exclusive) value.
    /// </summary>
    /// <param name="random">The random instance.</param>
    /// <param name="min">The minimum value (inclusive).</param>
    /// <param name="max">The maximum value (exclusive).</param>
    /// <returns></returns>
    public static float NextFloat(this Random random, float min, float max)
    {
        return EaselMath.Lerp(min, max, random.NextSingle());
    }
    
    public static Vector3 ToEulerAngles(this Quaternion quat)
    {
        double test = quat.X * quat.Y + quat.Z * quat.W;

        double yaw, pitch, roll;
        
        const double accuracy = 0.4999;
        
        if (test > accuracy)
        {
            yaw = 2 * Mth.Atan2(quat.X, quat.W);
            pitch = Mth.PI / 2;
            roll = 0;
        }
        else if (test < -accuracy)
        {
            yaw = -2 * Mth.Atan2(quat.X, quat.W);
            pitch = -Mth.PI / 2;
            roll = 0;
        }
        else
        {
            double xx = quat.X * quat.X;
            double yy = quat.Y * quat.Y;
            double zz = quat.Z * quat.Z;

            yaw = Mth.Atan2(2 * quat.Y * quat.W - 2 * quat.X * quat.Z, 1 - 2 * yy - 2 * zz);
            pitch = Mth.Asin(2 * test);
            roll = Mth.Atan2(2 * quat.X * quat.W - 2 * quat.Y * quat.Z, 1 - 2 * xx - 2 * zz);
        }

        return new Vector3((float) yaw, (float) pitch, (float) roll);
    }

    public static System.Numerics.Vector2 ToVector2(this Vector3 vector3)
    {
        return new System.Numerics.Vector2(vector3.X, vector3.Y);
    }

    public static Matrix4x4 To3x3Matrix(this Matrix4x4 matrix)
    {
        return new Matrix4x4(
            matrix.M11, matrix.M12, matrix.M13, 0,
            matrix.M21, matrix.M22, matrix.M23, 0,
            matrix.M31, matrix.M32, matrix.M33, 0,
            0, 0, 0, 1);
    }

    /// <summary>
    /// Load an embedded resource with the given name.
    /// </summary>
    /// <param name="assemblyName">The assembly name of the resource to load.</param>
    /// <returns>The loaded resource.</returns>
    public static byte[] LoadEmbeddedResource(Assembly assembly, string assemblyName)
    {
        using Stream stream = assembly.GetManifestResourceStream(assemblyName);
        using MemoryStream memoryStream = new MemoryStream();
        stream!.CopyTo(memoryStream);
        return memoryStream.ToArray();
    }

    public static string LoadEmbeddedString(Assembly assembly, string assemblyName, Encoding encoding)
    {
        return encoding.GetString(LoadEmbeddedResource(assembly, assemblyName));
    }

    public static string LoadEmbeddedString(Assembly assembly, string assemblyName) => LoadEmbeddedString(assembly, assemblyName, Encoding.UTF8);

    public static byte[] Compress(byte[] data, CompressionLevel level = CompressionLevel.Optimal)
    {
        using MemoryStream stream = new MemoryStream();
        using DeflateStream deflate = new DeflateStream(stream, level);
        deflate.Write(data, 0, data.Length);
        return stream.ToArray();
    }
    
    public static byte[] Decompress(byte[] data)
    {
        using MemoryStream stream = new MemoryStream(data);
        using MemoryStream output = new MemoryStream();
        using DeflateStream deflate = new DeflateStream(stream, CompressionMode.Decompress);
        deflate.CopyTo(output);
        return output.ToArray();
    }
}