using System.Numerics;
using Easel.Entities;
using Easel.Physics.Shapes;

namespace Stellution.Common.csharp.entity.player; 

public class PlayerEntity : RigidEntity {
    
    public PlayerEntity(string? entityName = null, int initialCapacity = 16) : base("player", entityName, initialCapacity) {
    }

    public PlayerEntity(Vector3 position, string? entityName = null, int initialCapacity = 16) : base(position, "player", entityName, initialCapacity) {
    }

    public PlayerEntity(Transform transform, string? entityName = null, int initialCapacity = 16) : base(transform, "player", entityName, initialCapacity) {
    }

    protected override void Initialize() {
        base.Initialize();
        
        //this.Rigidbody.LockX = true;
        //this.Rigidbody.LockZ = true;
    }

    protected override float GetMass() {
        return 2;
    }

    protected override IShape GetCollisionShape() {
        return new BoxShape(new Vector3(1, 2.5F, 1));
    }

    protected override void Update() {
        base.Update();
    }
}