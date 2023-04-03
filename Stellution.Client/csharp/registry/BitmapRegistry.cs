using Easel.Content;
using Easel.Content.Builder;
using Easel.Graphics;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry; 

public class BitmapRegistry : Registry, IRegistry {

    public static readonly string DefinitionName = "content/bitmaps";
    
    public static Bitmap SkyEarthTop => Content.Load<Bitmap>(DefinitionName, "sky/skybox/earth/earth_top");
    public static Bitmap SkyEarthSide => Content.Load<Bitmap>(DefinitionName, "sky/skybox/earth/earth_side");
    public static Bitmap SkyEarthBottom => Content.Load<Bitmap>(DefinitionName, "sky/skybox/earth/earth_bottom");
    
    public void Initialize(ContentManager content) {
        ContentDefinition definition = new ContentBuilder(DefinitionName)
            .Add(new ImageContent("sky/skybox/earth/earth_top.bmp"))
            .Add(new ImageContent("sky/skybox/earth/earth_side.bmp"))
            .Add(new ImageContent("sky/skybox/earth/earth_bottom.bmp"))
            .Build();

        content.AddContent(definition);
    }
}