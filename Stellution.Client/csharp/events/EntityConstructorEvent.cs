using System;
using Easel.Entities.Components;
using Stellution.Client.csharp.registry.types;
using Stellution.Common.csharp.entity;

namespace Stellution.Client.csharp.events; 

public class EntityConstructorEvent {

    public EntityConstructorEvent() {
        ModifiedEntity.Constructing += this.OnConstructing;
    }
    
    private void OnConstructing(ModifiedEntity entity, string key) {
        this.AddComponent(entity, "cyber_car", () => new ModelRenderer(ModelRegistry.CyberCar.Value));
        this.AddComponent(entity, "player", () => new ModelRenderer(ModelRegistry.Female.Value));
    }

    private void AddComponent(ModifiedEntity entity, string key, Func<Component> component) {
        if (entity.Key.Equals(key)) {
            entity.AddComponent(component.Invoke());
        }
    }
}