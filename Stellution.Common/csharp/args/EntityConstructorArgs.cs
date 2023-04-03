using Stellution.Common.csharp.entity;

namespace Stellution.Common.csharp.args; 

public struct EntityConstructorArgs {
    
    public readonly ModifiedEntity Entity;

    public EntityConstructorArgs(ModifiedEntity entity) => this.Entity = entity;
}