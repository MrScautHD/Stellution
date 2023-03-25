using Easel.Content;
using Easel.Content.Builder;
using Easel.GUI;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class FontRegistry : Registry, IRegistry {
    
    public static readonly string DefinitionName = "content/font";
    
    public static Font Fontoe => Content.Load<Font>(DefinitionName, "font/fontoe");

    public void Initialize(ContentManager content) {
        ContentDefinition definition = new ContentBuilder(DefinitionName)
            .Add(new FontContent("fontoe.ttf"))
            .Build();

        content.AddContent(definition);
    }
}