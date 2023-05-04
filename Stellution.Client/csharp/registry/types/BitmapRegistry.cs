using System;
using Easel.Content.Builder;
using Easel.Graphics;

namespace Stellution.Client.csharp.registry.types; 

public class BitmapRegistry : ClientRegistry {

    public static readonly string DefinitionName = "content/bitmaps";
    
    public static readonly Lazy<Bitmap> SkyEarthTop = Load<Bitmap>(DefinitionName, "sky/skybox/earth/earth_top");
    public static readonly Lazy<Bitmap> SkyEarthSide = Load<Bitmap>(DefinitionName, "sky/skybox/earth/earth_side");
    public static readonly Lazy<Bitmap> SkyEarthBottom = Load<Bitmap>(DefinitionName, "sky/skybox/earth/earth_bottom");
    
    public override void Initialize() {
        ContentDefinition definition = new ContentBuilder(DefinitionName)
            .Add(new ImageContent("sky/skybox/earth/earth_top.bmp"))
            .Add(new ImageContent("sky/skybox/earth/earth_side.bmp"))
            .Add(new ImageContent("sky/skybox/earth/earth_bottom.bmp"))
            .Build();

        Content.AddContent(definition);
    }
}