using Easel.Content;
using Future.Client.csharp.config;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientConfigRegistry : Registry, IRegistry {
    
    public static GraphicConfig Graphic { get; private set; }
    
    public void Initialize(ContentManager content) {
        Graphic = new GraphicConfig("config", "graphic-config");
    }
}