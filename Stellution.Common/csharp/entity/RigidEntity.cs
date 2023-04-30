using System.Numerics;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Physics.Shapes;
using Easel.Physics.Structs;
using JoltPhysicsSharp;

namespace Stellution.Common.csharp.entity; 

public abstract class RigidEntity : ModifiedEntity {

    public Rigidbody Rigidbody { get; }
    public BodyInterface BodyInterface { get; }

    protected RigidEntity(string key, string? entityName = null, int initialCapacity = 16) : this(new Transform(), key, entityName, initialCapacity) {
    }
    
    protected RigidEntity(Vector3 position, string key, string? entityName = null, int initialCapacity = 16) : this(new Transform() { Position =  position }, key, entityName, initialCapacity) {
    }

    protected RigidEntity(Transform transform, string key, string? entityName = null, int initialCapacity = 16) : base(transform, key, entityName, initialCapacity) {
        this.Rigidbody = new Rigidbody(this.GetCollisionShape(), new RigidbodyInitSettings());
        this.BodyInterface = this.Simulation.BodyInterface;
        this.AddComponent(this.Rigidbody);
    }

    protected abstract float GetMass();
    
    protected abstract IShape GetCollisionShape();
}