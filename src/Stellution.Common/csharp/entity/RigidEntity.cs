using Easel.Entities;
using Easel.Entities.Components;
using Easel.Physics.Shapes;
using Easel.Physics.Structs;
using JoltPhysicsSharp;

namespace Stellution.Common.csharp.entity; 

public abstract class RigidEntity : ModifiedEntity {

    public Rigidbody Rigidbody { get; }
    public BodyInterface BodyInterface => this.Simulation.BodyInterface;

    protected RigidEntity(Transform transform, string? entityName = null, int initialCapacity = 16) : base(transform, entityName, initialCapacity) {
        this.Rigidbody = new Rigidbody(this.GetCollisionShape(), new RigidbodyInitSettings() {
            BodyType = this.GetBodyType(),
            Restitution = this.GetRestitution(),
        });
        this.AddComponent(this.Rigidbody);
    }

    protected abstract float GetMass();
    
    protected abstract IShape GetCollisionShape();

    public virtual float GetRestitution() {
        return 0;
    }

    public virtual BodyType GetBodyType() {
        return BodyType.Dynamic;
    }
}