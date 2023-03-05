using Easel.Content.Builder;
using Easel.GUI;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientFontRegistry : Registry {

    public static readonly Font Fontoe = Load<Font>(FutureClient.ContentBuilder, new FontContent("font/fontoe.ttf"));
}