using System.Numerics;

namespace Easel.Math;

public struct Plane<T> where T : INumber<T>
{
    public Vector3T<T> Normal;
    public T D;

    public Plane(T a, T b, T c, T d)
    {
        Normal = new Vector3T<T>(a, b, c);
        D = d;
    }
}

public static class Plane
{
    // TODO: Ref normalization for vector2, 3, 4
    public static Plane<T> Normalize<T>(Plane<T> plane) where T : INumber<T>, IRootFunctions<T>
    {
        Normalize(ref plane);
        return plane;
    }

    public static void Normalize<T>(ref Plane<T> plane) where T : INumber<T>, IRootFunctions<T>
    {
        T magnitude = Vector3T.Magnitude(plane.Normal);
        plane.Normal /= magnitude;
        plane.D /= magnitude;
    }
}