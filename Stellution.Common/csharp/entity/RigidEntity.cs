using System.Numerics;
using Easel;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Physics;
using Easel.Physics.Shapes;
using Easel.Physics.Structs;

namespace Stellution.Common.csharp.entity; 

public abstract class RigidEntity : ModifiedEntity {

    public Rigidbody Rigidbody { get; private set; }
    
    public RigidEntity(string key, string? entityName = null, int initialCapacity = 16) : base(key, entityName, initialCapacity) {
        this.Rigidbody = new Rigidbody(this.GetCollisionShape(), new RigidbodyInitSettings());
        this.AddComponent(this.Rigidbody);
    }
    
    public RigidEntity(Vector3 position, string key, string? entityName = null, int initialCapacity = 16) : base(position, key, entityName, initialCapacity) {
        this.Rigidbody = new Rigidbody(this.GetCollisionShape(), new RigidbodyInitSettings());
        this.AddComponent(this.Rigidbody);
    }

    public RigidEntity(Transform transform, string key, string? entityName = null, int initialCapacity = 16) : base(transform, key, entityName, initialCapacity) {
        this.Rigidbody = new Rigidbody(this.GetCollisionShape(), new RigidbodyInitSettings());
        this.AddComponent(this.Rigidbody);
    }

    protected abstract float GetMass();
    
    protected abstract IShape GetCollisionShape();
}