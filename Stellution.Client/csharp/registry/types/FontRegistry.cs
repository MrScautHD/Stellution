using System;
using Easel.Content.Builder;
using Easel.GUI;

namespace Stellution.Client.csharp.registry.types; 

public class FontRegistry : ContentRegistry {
    
    public static readonly string DefinitionName = "content/font";
    
    public static readonly Lazy<Font> Fontoe = Load<Font>(DefinitionName, "fontoe");

    public override void Initialize() {
        ContentDefinition definition = new ContentBuilder(DefinitionName)
            .Add(new FontContent("fontoe.ttf", new FontOptions() { IsAntialiased = false}))
            .Build();

        Content.AddContent(definition);
    }
}