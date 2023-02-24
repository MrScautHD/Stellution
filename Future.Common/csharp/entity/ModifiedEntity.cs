using Easel.Entities;
using Future.Common.csharp.events;

namespace Future.Common.csharp.entity; 

public class ModifiedEntity : Entity {
    
    public string EntityKey { get; }
    
    public static event EventHandler<EntityConstructorArgs>? Constructing;

    public ModifiedEntity(string entityKey, string? entityName = null, int initialCapacity = 16) : this(new Transform(), entityKey, entityName, initialCapacity) {
        
    }

    public ModifiedEntity(Transform transform, string entityKey, string? entityName = null, int initialCapacity = 16) : base(entityName, transform, initialCapacity) {
        this.EntityKey = entityKey;

        Constructing?.Invoke(null, new EntityConstructorArgs(this));
    }
}