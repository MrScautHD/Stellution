using System.Numerics;
using BulletSharp;
using Easel.Entities;
using Easel.Physics;

namespace Stellution.Common.csharp.entity; 

public abstract class RigidEntity : ModifiedEntity {

    public Rigidbody Rigidbody { get; private set; }
    public RigidBody BulletBody => this.Rigidbody.BulletBody;

    public RigidEntity(string key, string? entityName = null, int initialCapacity = 16) : base(key, entityName, initialCapacity) {
        this.Rigidbody = new Rigidbody(this.GetMass(), this.GetCollisionShape());
        this.AddComponent(this.Rigidbody);
    }
    
    public RigidEntity(Vector3 position, string key, string? entityName = null, int initialCapacity = 16) : base(position, key, entityName, initialCapacity) {
        this.Rigidbody = new Rigidbody(this.GetMass(), this.GetCollisionShape());
        this.AddComponent(this.Rigidbody);
    }

    public RigidEntity(Transform transform, string key, string? entityName = null, int initialCapacity = 16) : base(transform, key, entityName, initialCapacity) {
        this.Rigidbody = new Rigidbody(this.GetMass(), this.GetCollisionShape());
        this.AddComponent(this.Rigidbody);
    }

    protected abstract float GetMass();
    
    protected abstract CollisionShape GetCollisionShape();
}