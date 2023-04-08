using System.Numerics;
using BulletSharp;
using Easel.Entities;

namespace Stellution.Common.csharp.entity.player; 

public class PlayerEntity : RigidEntity {
    
    public PlayerEntity(string? entityName = null, int initialCapacity = 16) : base("player", entityName, initialCapacity) {
    }

    public PlayerEntity(Vector3 position, string? entityName = null, int initialCapacity = 16) : base(position, "player", entityName, initialCapacity) {
    }

    public PlayerEntity(Transform transform, string? entityName = null, int initialCapacity = 16) : base(transform, "player", entityName, initialCapacity) {
    }

    protected override float GetMass() {
        return 2;
    }

    protected override CollisionShape GetCollisionShape() {
        return new CapsuleShape(1, 2.5F);
    }

    protected override void Update() {
        base.Update();
    }
}