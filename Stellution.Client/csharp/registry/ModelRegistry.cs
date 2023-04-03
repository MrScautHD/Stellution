using Easel.Content;
using Easel.Content.Builder;
using Easel.Graphics;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry; 

public class ModelRegistry : Registry, IRegistry {
    
    public static readonly string DefinitionName = "content/models";

    public static Model CyberCar => ModelGetter(DefinitionName, "entity/vehicle/cyber_car", MaterialRegistry.CyberCar);
    public static Model Female => ModelGetter(DefinitionName, "entity/player/female", MaterialRegistry.Female);

    public void Initialize(ContentManager content) {
        ContentDefinition definition = new ContentBuilder(DefinitionName)
            .Add(new ModelContent("entity/vehicle/cyber_car.glb", false))
            .Add(new ModelContent("entity/player/female.glb", false))
            .Build();

        content.AddContent(definition);
    }
}