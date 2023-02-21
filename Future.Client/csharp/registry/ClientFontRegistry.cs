using System.Collections.Generic;
using Easel.Content;
using Easel.GUI;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientFontRegistry : Registry, IRegistry {
    
    // REGISTRY LIST
    public static readonly Dictionary<string, Font> Fonts = new();

    // REGISTRIES
    public static Font Fontoe { get; private set; }
    
    public void Initialize(ContentManager content) {
        Fontoe = this.RegisterLoad("fontoe", Fonts, content, "font/fontoe.ttf");
    }
}