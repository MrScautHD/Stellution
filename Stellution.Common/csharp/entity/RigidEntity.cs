using System.Reflection;
using BulletSharp;
using Easel.Entities;
using Easel.Physics;

namespace Stellution.Common.csharp.entity; 

public class RigidEntity : ModifiedEntity {

    public Rigidbody Rigidbody { get; private set; }
    
    public RigidEntity(Rigidbody rigidbody, string entityKey, string? entityName = null, int initialCapacity = 16) : base(entityKey, entityName, initialCapacity) {
        this.Rigidbody = rigidbody;
        this.AddComponent(rigidbody);
    }
    
    public RigidEntity(Transform transform, Rigidbody rigidbody, string entityKey, string? entityName = null, int initialCapacity = 16) : base(transform, entityKey, entityName, initialCapacity) {
        this.Rigidbody = rigidbody;
        this.AddComponent(rigidbody);
    }
    
    public RigidEntity(float mass, CollisionShape collisionShape, string entityKey, string? entityName = null, int initialCapacity = 16) : base(entityKey, entityName, initialCapacity) {
        this.Rigidbody = new Rigidbody(mass, collisionShape);
        this.AddComponent(this.Rigidbody);
    }

    public RigidEntity(Transform transform, float mass, CollisionShape collisionShape, string key, string? entityName = null, int initialCapacity = 16) : base(transform, key, entityName, initialCapacity) {
        this.Rigidbody = new Rigidbody(mass, collisionShape);
        this.AddComponent(this.Rigidbody);
    }
    
    public RigidBody GetBulletRigidBody() {
        return (RigidBody) typeof(Rigidbody).GetField("_rb", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(this.Rigidbody);
    }
}