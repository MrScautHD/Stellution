using Easel.Content;
using Easel.Content.Builder;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientFontRegistry : ContentRegistry, IContentRegistry {

    public static FontContent Fontoe { get; private set; }
    
    public void Load(ContentManager content) {
        Fontoe = this.Register(FutureClient.ContentBuilder, new FontContent("font/fontoe.ttf"));
    }
}