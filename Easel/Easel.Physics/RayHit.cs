using System.Numerics;
using BulletSharp;
using Easel.Math;

namespace Easel.Physics;

public struct RayHit
{
    public Vector3T<float> WorldPosition;
    public Vector3T<float> HitPosition;
    public Vector3T<int> CubeNormal;
    public Vector3T<float> RealNormal;
    public Quaternion Rotation;
    public int ChildIndex;

    public CollisionObject CollisionObject;
}