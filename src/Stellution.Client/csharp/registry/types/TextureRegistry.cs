using System;
using Easel.Content.Builder;
using Easel.Graphics;

namespace Stellution.Client.csharp.registry.types;

public class TextureRegistry : ContentRegistry {

    public static readonly string DefinitionName = "content/textures";

    // LOGO
    public static readonly Lazy<Texture2D> Logo = Get(DefinitionName, "logo/logo");
    public static readonly Lazy<Texture2D> LogoBanner = Get(DefinitionName, "logo/logo_banner");
    public static readonly Lazy<Texture2D> CarBanner = Get(DefinitionName, "logo/car_banner");

    // ENTITY
    public static readonly Lazy<Texture2D> CyberCar = Get(DefinitionName, "entity/vehicle/cyber_car");
    public static readonly Lazy<Texture2D> Female = Get(DefinitionName, "entity/player/female");

    // OVERLAY
    public static readonly Lazy<Texture2D> Crosshair = Get(DefinitionName, "overlay/crosshair");
    
    public override void Initialize() {
        ContentDefinition definition = new ContentBuilder(DefinitionName)
            .Add(new ImageContent("logo/logo.png"))
            .Add(new ImageContent("logo/logo_banner.png"))
            .Add(new ImageContent("logo/car_banner.png"))
            .Add(new ImageContent("entity/vehicle/cyber_car.png"))
            .Add(new ImageContent("entity/player/female.png"))
            .Add(new ImageContent("overlay/crosshair.png"))
            .Build();

        Content.AddContent(definition);
    }
    
    /**
     * Use this to get the "TEXTURE" with the right "Sample-State".
     */
    protected static Lazy<Texture2D> Get(string definition, string path, SamplerState? state = null) {
        Lazy<Texture2D> lazy = new Lazy<Texture2D>(() => {
            Texture2D texture = Content.Load<Texture2D>(definition, path);
            texture.SamplerState = state ?? SamplerState.PointClamp;

            return texture;
        });

        return lazy;
    }
}