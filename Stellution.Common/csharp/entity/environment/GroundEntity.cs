using System.Numerics;
using BulletSharp;
using Easel.Entities;

namespace Stellution.Common.csharp.entity.environment; 

public class GroundEntity : RigidEntity {
    
    public GroundEntity(string? entityName = null, int initialCapacity = 16) : base("ground", entityName, initialCapacity) {
    }
    
    public GroundEntity(Vector3 position, string? entityName = null, int initialCapacity = 16) : base(position, "ground", entityName, initialCapacity) {
    }

    public GroundEntity(Transform transform, string? entityName = null, int initialCapacity = 16) : base(transform, "ground", entityName, initialCapacity) {
    }

    protected override float GetMass() {
        return 0;
    }

    protected override CollisionShape GetCollisionShape() {
        return new BoxShape(100000, 1, 100000);
    }
}