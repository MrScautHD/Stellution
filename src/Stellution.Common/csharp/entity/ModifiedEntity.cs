using System.Numerics;
using Easel.Entities;

namespace Stellution.Common.csharp.entity; 

public abstract class ModifiedEntity : Entity {
    
    public delegate void OnEntityConstruction(ModifiedEntity entity, string key);
    public static event OnEntityConstruction? Constructing;

    public Vector3 Position {
        get => this.Transform.Position;
        set => this.Transform.Position = value;
    }

    public Quaternion Rotation {
        get => this.Transform.Rotation;
        set => this.Transform.Rotation = value;
    }
    
    public Vector3 Scale {
        get => this.Transform.Scale;
        set => this.Transform.Scale = value;
    }
    
    public Vector3 Origin {
        get => this.Transform.Origin;
        set => this.Transform.Origin = value;
    }

    protected ModifiedEntity(Transform transform, string? entityName = null, int initialCapacity = 16) : base(entityName, transform, initialCapacity) {
        Constructing?.Invoke(this, this.GetKey());
    }

    public abstract string GetKey();
}