using Easel.Entities;
using Future.Common.csharp.args;

namespace Future.Common.csharp.entity; 

public class ModifiedEntity : Entity {
    
    public string Key { get; }
    
    public static event EventHandler<EntityConstructorArgs>? Constructing;

    public ModifiedEntity(string entityKey, string? entityName = null, int initialCapacity = 16) : this(new Transform(), entityKey, entityName, initialCapacity) {
    }

    public ModifiedEntity(Transform transform, string key, string? entityName = null, int initialCapacity = 16) : base(entityName, transform, initialCapacity) {
        this.Key = key;

        Constructing?.Invoke(null, new EntityConstructorArgs(this));
    }
}