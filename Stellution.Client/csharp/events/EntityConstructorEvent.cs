using System;
using Easel.Entities.Components;
using Stellution.Client.csharp.registry;
using Stellution.Common.csharp.entity;

namespace Stellution.Client.csharp.events; 

public class EntityConstructorEvent {

    public EntityConstructorEvent() {
        ModifiedEntity.Constructing += (obj, args) => this.Event(args.Entity);
    }
    
    protected void Event(ModifiedEntity entity) {
        this.AddComponent(entity, "cyber_car", () => new ModelRenderer(ModelRegistry.CyberCar));
        this.AddComponent(entity, "player", () => new ModelRenderer(ModelRegistry.Female));
    }

    protected void AddComponent(ModifiedEntity entity, string entityKey, Func<Component> component) {
        if (entity.Key.Equals(entityKey)) {
            entity.AddComponent(component.Invoke());
        }
    }
}