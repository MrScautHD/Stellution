using Easel.Content;
using Stellution.Client.csharp.config;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry; 

public class ClientConfigRegistry : Registry, IRegistry {
    
    public static GraphicConfig Graphic { get; private set; }
    
    public void Initialize(ContentManager content) {
        Graphic = new GraphicConfig("config", "graphic-config");
    }
}