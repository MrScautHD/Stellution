using Stellution.Client.csharp.config;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry.types; 

public class ClientConfigRegistry : Registry {
    
    public static GraphicConfig? Graphic { get; private set; }
    
    public override void Initialize() {
        Graphic = new GraphicConfig("config", "graphic-config");
    }
}