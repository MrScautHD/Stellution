using Easel;
using Easel.Content;
using Easel.Content.Builder;
using Easel.GUI;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry; 

public class FontRegistry : Registry, IRegistry {
    
    public static readonly string DefinitionName = "content/font";
    public static ContentManager Content => EaselGame.Instance.Content;
    
    public static Font Fontoe => Content.Load<Font>(DefinitionName, "fontoe");

    public void Initialize() {
        ContentDefinition definition = new ContentBuilder(DefinitionName)
            .Add(new FontContent("fontoe.ttf", new FontOptions() { IsAntialiased = false}))
            .Build();

        Content.AddContent(definition);
    }
}