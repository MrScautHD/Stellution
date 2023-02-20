using System;
using System.Numerics;
using BulletSharp;

namespace Easel.Physics;

public class CompoundClosestRayResultCallback : ClosestRayResultCallback
{
    public int ChildIndex;
    
    public CompoundClosestRayResultCallback(ref Vector3 rayFromWorld, ref Vector3 rayToWorld) : base(ref rayFromWorld, ref rayToWorld) { }

    public override float AddSingleResult(ref LocalRayResult rayResult, bool normalInWorldSpace)
    {
        ChildIndex = rayResult.LocalShapeInfo?.TriangleIndex ?? -1;
        
        return base.AddSingleResult(ref rayResult, normalInWorldSpace);
    }

    public override bool NeedsCollision(BroadphaseProxy proxy0)
    {
        //if (((CollisionObject) proxy0.ClientObject).UserIndex == (int) PhysicsTags.IgnoreRaycast)
        //    return false;

        return base.NeedsCollision(proxy0);
    }
}