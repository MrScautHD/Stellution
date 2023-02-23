using System.Collections.Generic;
using Easel.Content;
using Easel.GUI;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientFontRegistry : Registry, IRegistry {
    
    public static readonly Dictionary<string, Font> Fonts = new();
    
    public static Font Fontoe { get; private set; }
    
    public void InitializePre(ContentManager content) {
        Fontoe = this.RegisterLoad("fontoe", Fonts, content, "font/fontoe.ttf");
    }
}