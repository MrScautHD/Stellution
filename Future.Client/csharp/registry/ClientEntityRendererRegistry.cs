using Easel.Content;
using Easel.Entities.Components;
using Future.Common.csharp.entity;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientEntityRendererRegistry : Registry, IRegistry {

    public void InitializePre(ContentManager content) {
        ModifiedEntity.Constructing += (obj, args) => this.Event(args.Entity);
    }

    protected void Event(ModifiedEntity entity) {
        this.AddComponent(entity, "cyber_car", new ModelRenderer(ClientModelRegistry.CyberCarModel));
        this.AddComponent(entity, "player", new ModelRenderer(ClientModelRegistry.FemaleModel));
    }

    protected void AddComponent(ModifiedEntity entity, string entityKey, Component component) {
        if (entity.Name.Equals(entityKey)) {
            entity.AddComponent(component);
        }
    }
}