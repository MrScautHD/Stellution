using Future.Common.csharp.entity;

namespace Future.Common.csharp.events; 

public struct EntityConstructorArgs {
    
    public readonly ModifiedEntity Entity;

    public EntityConstructorArgs(ModifiedEntity entity) => this.Entity = entity;
}