using Easel.Content;
using Easel.Content.Builder;
using Easel.Graphics;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry;

public class TextureRegistry : Registry, IRegistry {

    public static readonly string DefinitionName = "content/textures";
    
    public static Texture2D CyberCar => TextureGetter(DefinitionName, "entity/vehicle/cyber_car");
    public static Texture2D Female => TextureGetter(DefinitionName, "entity/player/female");

    public void Initialize(ContentManager content) {
        ContentDefinition definition = new ContentBuilder(DefinitionName)
            .Add(new ImageContent("entity/vehicle/cyber_car.png"))
            .Add(new ImageContent("entity/player/female.png"))
            .Build();

        content.AddContent(definition);
    }
}