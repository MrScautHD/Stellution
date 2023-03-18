using Easel.Content;
using Easel.Entities.Components;
using Future.Common.csharp.entity;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientEntityRendererRegistry : Registry, IRegistry {

    public void Register(ContentManager content) {
        ModifiedEntity.Constructing += (obj, args) => this.Event(args.Entity);
    }

    protected void Event(ModifiedEntity entity) {
        this.AddComponent(entity, "cyber_car", new ModelRenderer(ClientModelRegistry.Get(ClientModelRegistry.CyberCarModel, ClientMaterialRegistry.CyberCarMaterial)));
        this.AddComponent(entity, "player", new ModelRenderer(ClientModelRegistry.Get(ClientModelRegistry.FemaleModel, ClientMaterialRegistry.FemaleMaterial)));
    }

    protected void AddComponent(ModifiedEntity entity, string entityKey, Component component) {
        if (entity.Key.Equals(entityKey)) {
            entity.AddComponent(component);
        }
    }
}