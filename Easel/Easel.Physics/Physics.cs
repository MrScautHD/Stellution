using System;
using System.Numerics;
using BulletSharp;
using Easel.Math;

namespace Easel.Physics;

public static class Physics
{
    public static readonly DiscreteDynamicsWorld World;
    public static readonly CollisionDispatcher Dispatcher;
    public static readonly DbvtBroadphase Broadphase;
    public static readonly CollisionConfiguration Configuration;

    public static float SimulationSpeed = 1;

    static Physics()
    {
        Configuration = new DefaultCollisionConfiguration();
        Dispatcher = new CollisionDispatcher(Configuration);
        Broadphase = new DbvtBroadphase();
        World = new DiscreteDynamicsWorld(Dispatcher, Broadphase, null, Configuration);
        World.Gravity = new Vector3(0, -9.81f, 0);

        World.PairCache.SetInternalGhostPairCallback(new GhostPairCallback());
    }

    public static bool Raycast(Vector3 position, Vector3 direction, float distance, out RayHit hit)
    {
        Vector3 dir = position + direction * distance;
        using CompoundClosestRayResultCallback cb = new CompoundClosestRayResultCallback(ref position, ref dir);
        cb.Flags |= (uint) TriangleRaycastCallback.EFlags.KeepUnflippedNormal;
        World.RayTest(position, dir, cb);

        if (!cb.HasHit)
        {
            hit = default;
            return false;
        }

        Matrix4x4.Decompose(cb.CollisionObject.WorldTransform, out _, out Quaternion rotation, out _);
        Quaternion invert = Quaternion.Inverse(rotation);
        Vector3 normal = Vector3.Transform(cb.HitNormalWorld, Matrix4x4.CreateFromQuaternion(invert));
        Vector3 normalAbs = Vector3.Abs(normal);
        if (normalAbs.X > normalAbs.Y && normalAbs.X > normalAbs.Z)
            normal = new Vector3(1 * MathF.Sign(normal.X), 0, 0);
        else if (normalAbs.Y > normalAbs.X && normalAbs.Y > normalAbs.Z)
            normal = new Vector3(0, 1 * MathF.Sign(normal.Y), 0);
        else if (normalAbs.Z > normalAbs.Y && normalAbs.Z > normalAbs.Y)
            normal = new Vector3(0, 0, 1 * MathF.Sign(normal.Z));
        else
            normal = Vector3.Zero;
        
        //Console.WriteLine(Vector3.Normalize(normal));
        //normal = new Vector3((int) MathF.Round(normal.X), (int) MathF.Round(normal.Y), (int) MathF.Round(normal.Z));
        //Vector3 normal = cb.HitNormalWorld;
        //Vector3 normal = cb.HitNormalWorld;
        //Vector3 normal = Vector3.TransformNormal(cb.HitNormalWorld, Matrix4x4.CreateFromQuaternion(invert));

        hit.WorldPosition = (Vector3T<float>) cb.CollisionObject.WorldTransform.Translation;
        hit.HitPosition = (Vector3T<float>) cb.HitPointWorld;
        hit.CubeNormal = (Vector3T<int>) normal;
        hit.RealNormal = (Vector3T<float>) cb.HitNormalWorld;
        hit.CollisionObject = cb.CollisionObject;
        hit.Rotation = rotation;
        hit.ChildIndex = cb.ChildIndex;

        return true;
    }

    public static RigidBody AddRigidBody(float mass, CollisionShape shape, Matrix4x4 startTransform)
    {
        Vector3 localInertia = shape.CalculateLocalInertia(mass);
        using RigidBodyConstructionInfo info =
            new RigidBodyConstructionInfo(mass, new DefaultMotionState(startTransform), shape, localInertia);
        RigidBody rb = new RigidBody(info);
        World.AddRigidBody(rb);
        return rb;
    }

    public static RigidBody AddStaticBody(CollisionShape shape, Matrix4x4 startTransform)
    {
        using RigidBodyConstructionInfo info =
            new RigidBodyConstructionInfo(0, new DefaultMotionState(startTransform), shape);
        RigidBody rb = new RigidBody(info);
        rb.CollisionFlags |= CollisionFlags.StaticObject | CollisionFlags.CustomMaterialCallback;
        World.AddRigidBody(rb);
        return rb;
    }

    public static GhostObject AddTrigger(CollisionShape shape, Matrix4x4 startTransform)
    {
        GhostObject obj = new PairCachingGhostObject();
        obj.CollisionShape = shape;
        obj.WorldTransform = startTransform;
        obj.CollisionFlags |= CollisionFlags.NoContactResponse;
        World.AddCollisionObject(obj);
        return obj;
    }

    public static void Update()
    {
        World.StepSimulation(Time.DeltaTime * SimulationSpeed, fixedTimeStep: (1 / 60f) * SimulationSpeed);
    }
}