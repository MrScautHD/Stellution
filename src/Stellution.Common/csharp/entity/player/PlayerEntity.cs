using System.Numerics;
using Easel.Entities;
using Easel.Physics.Shapes;

namespace Stellution.Common.csharp.entity.player; 

public class PlayerEntity : RigidEntity {

    public PlayerEntity(Transform transform, string? entityName = null, int initialCapacity = 16) : base(transform, entityName, initialCapacity) {
    }

    protected override void Initialize() {
        base.Initialize();

        //this.Rigidbody.LockX = true;
        //this.Rigidbody.LockZ = true;
    }
    
    public override string GetKey() {
        return "player";
    }

    protected override IShape GetCollisionShape() {
        return new BoxShape(new Vector3(0.5F, 1.5F, 0.5F));
    }
}