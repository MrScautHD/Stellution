using System.Numerics;

namespace Easel.Math;

public struct Frustum<T> where T : INumber<T>, IRootFunctions<T>
{
    public Plane<T> Left;

    public Plane<T> Right;

    public Plane<T> Bottom;

    public Plane<T> Top;

    public Plane<T> Near;

    public Plane<T> Far;

    public Frustum(Matrix<T> projView)
    {
        Left = new Plane<T>(
            projView.Row0.W + projView.Row0.X,
            projView.Row1.W + projView.Row1.X,
            projView.Row2.W + projView.Row2.X,
            projView.Row3.W + projView.Row3.X
        );
        
        Right = new Plane<T>(
            projView.Row0.W - projView.Row0.X, 
            projView.Row1.W - projView.Row1.X,
            projView.Row2.W - projView.Row2.X, 
            projView.Row3.W - projView.Row3.X
        );
        
        Top = new Plane<T>(
            projView.Row0.W - projView.Row0.Y, 
            projView.Row1.W - projView.Row1.Y,
            projView.Row2.W - projView.Row2.Y, 
            projView.Row3.W - projView.Row3.Y
        );
        
        Bottom = new Plane<T>(
            projView.Row0.W + projView.Row0.Y, 
            projView.Row1.W + projView.Row1.Y,
            projView.Row2.W + projView.Row2.Y, 
            projView.Row3.W + projView.Row3.Y
        );

        Near = new Plane<T>(
            projView.Row0.Z,
            projView.Row1.Z,
            projView.Row2.Z,
            projView.Row3.Z
        );
        
        Far = new Plane<T>(
            projView.Row0.W - projView.Row0.Z, 
            projView.Row1.W - projView.Row1.Z,
            projView.Row2.W - projView.Row2.Z, 
            projView.Row3.W - projView.Row3.Z
        );

        Plane.Normalize(ref Left);
        Plane.Normalize(ref Right);
        Plane.Normalize(ref Top);
        Plane.Normalize(ref Bottom);
        Plane.Normalize(ref Near);
        Plane.Normalize(ref Far);
    }
}