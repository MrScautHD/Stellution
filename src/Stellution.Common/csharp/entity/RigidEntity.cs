using System.Numerics;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Physics.Shapes;
using Easel.Physics.Structs;
using JoltPhysicsSharp;

namespace Stellution.Common.csharp.entity; 

public abstract class RigidEntity : ModifiedEntity {

    public Rigidbody Rigidbody { get; }
    public BodyID Handle => this.Rigidbody.Handle;
    public BodyInterface BodyInterface => this.Simulation.BodyInterface;
    public PhysicsSystem PhysicsSystem => this.Simulation.PhysicsSystem;
    public NarrowPhaseQuery NarrowPhaseQuery => this.PhysicsSystem.NarrowPhaseQuery;

    protected RigidEntity(Transform transform, string? entityName = null, int initialCapacity = 16) : base(transform, entityName, initialCapacity) {
        this.Rigidbody = new Rigidbody(this.GetCollisionShape(), new RigidbodyInitSettings() {
            BodyType = this.GetBodyType(),
            Restitution = this.GetRestitution(),
        });
        this.AddComponent(this.Rigidbody);

        this.PhysicsSystem.OnContactAdded += this.OnContactAdded;
        this.PhysicsSystem.OnContactRemoved += this.OnContactRemoved;
    }
    
    protected override void FixedUpdate() {
        base.FixedUpdate();
        if (this.Position.Y < -64) {
            this.Position = new Vector3(this.Position.X, 10, this.Position.Z);
            // TODO CALCULATE THE GROUND POS AND TELEPORT THE ENTITY TO IT
        }
    }

    protected virtual void OnContactAdded(PhysicsSystem system, in Body body1, in Body body2) { }
    
    protected virtual void OnContactRemoved(PhysicsSystem system, ref SubShapeIDPair subShapePair) { }

    protected abstract IShape GetCollisionShape();

    public virtual float GetRestitution() {
        return 0;
    }

    public virtual BodyType GetBodyType() {
        return BodyType.Dynamic;
    }
}