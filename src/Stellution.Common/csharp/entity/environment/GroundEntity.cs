using System.Numerics;
using Easel.Entities;
using Easel.Physics.Shapes;
using Easel.Physics.Structs;

namespace Stellution.Common.csharp.entity.environment; 

public class GroundEntity : RigidEntity {

    public GroundEntity(Transform transform, string? entityName = null, int initialCapacity = 16) : base(transform, entityName, initialCapacity) {
        
    }
    
    public override string GetKey() {
        return "ground";
    }

    protected override float GetMass() {
        return 0;
    }

    protected override IShape GetCollisionShape() {
        return new BoxShape(new Vector3(100000, 1, 100000));
    }
    
    public override BodyType GetBodyType() {
        return BodyType.Static;
    }
}