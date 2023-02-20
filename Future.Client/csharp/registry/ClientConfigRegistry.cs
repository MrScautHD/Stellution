using System.Collections.Generic;
using Future.Client.csharp.config;
using Future.Common.csharp.file;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientConfigRegistry : Registry {
    
    // REGISTRY LIST
    public static readonly Dictionary<string, AbstractConfig> Configs = new();

    // REGISTRIES
    public static GraphicConfig GraphicConfig { get; private set; }

    public void Initialize() {
        GraphicConfig = this.Register<>("graphic_config", Configs, new GraphicConfig("config", "graphic-config"));
    }
}