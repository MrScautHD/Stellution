using Easel.Entities.Components;
using Future.Client.csharp.registry;
using Future.Common.csharp.entity;

namespace Future.Client.csharp.events; 

public class EntityConstructorEvent {

    public EntityConstructorEvent() {
        ModifiedEntity.Constructing += (obj, args) => this.Event(args.Entity);
    }
    
    protected void Event(ModifiedEntity entity) {
        this.AddComponent(entity, "cyber_car", new ModelRenderer(ClientRegistry.CyberCarModel));
        //this.AddComponent(entity, "player", new ModelRenderer(ClientModelRegistry.Get(ClientModelRegistry.FemaleModel, ClientMaterialRegistry.FemaleMaterial)));
    }

    protected void AddComponent(ModifiedEntity entity, string entityKey, Component component) {
        if (entity.Key.Equals(entityKey)) {
            entity.AddComponent(component);
        }
    }
}