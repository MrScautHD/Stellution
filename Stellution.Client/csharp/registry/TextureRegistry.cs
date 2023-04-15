using Easel.Content;
using Easel.Content.Builder;
using Easel.Graphics;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry;

public class TextureRegistry : Registry, IRegistry {

    public static readonly string DefinitionName = "content/textures";

    public static Texture2D Logo => TextureGetter(DefinitionName, "logo/logo");
    public static Texture2D LogoBanner => TextureGetter(DefinitionName, "logo/logo_banner");
    public static Texture2D CarBanner => TextureGetter(DefinitionName, "logo/car_banner");

    
    public static Texture2D CyberCar => TextureGetter(DefinitionName, "entity/vehicle/cyber_car");
    public static Texture2D Female => TextureGetter(DefinitionName, "entity/player/female");

    public void Initialize(ContentManager content) {
        ContentDefinition definition = new ContentBuilder(DefinitionName)
            .Add(new ImageContent("logo/logo.png"))
            .Add(new ImageContent("logo/logo_banner.png"))
            .Add(new ImageContent("logo/car_banner.png"))
            .Add(new ImageContent("entity/vehicle/cyber_car.png"))
            .Add(new ImageContent("entity/player/female.png"))
            .Build();

        content.AddContent(definition);
    }
}