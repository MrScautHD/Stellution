using System.Numerics;
using Easel.Entities;

namespace Stellution.Common.csharp.entity; 

public class ModifiedEntity : Entity {
    
    public delegate void OnEntityConstruction(ModifiedEntity entity, string key);
    public static event OnEntityConstruction? Constructing;
    
    public string Key { get; }

    public Vector3 Position {
        get => this.Transform.Position;
        set => this.Transform.Position = value;
    }

    public ModifiedEntity(string key, string? entityName = null, int initialCapacity = 16) : this(new Transform(), key, entityName, initialCapacity) {
        
    }
    
    public ModifiedEntity(Vector3 position, string key, string? entityName = null, int initialCapacity = 16) : this(new Transform() { Position = position }, key, entityName, initialCapacity) {
    }

    public ModifiedEntity(Transform transform, string key, string? entityName = null, int initialCapacity = 16) : base(entityName, transform, initialCapacity) {
        this.Key = key;
        Constructing?.Invoke(this, this.Key);
    }
}