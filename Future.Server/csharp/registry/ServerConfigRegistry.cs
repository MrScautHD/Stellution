using Future.Common.csharp.file;
using Future.Common.csharp.registry;
using Future.Server.csharp.config;

namespace Future.Server.csharp.registry; 

public class ServerConfigRegistry : Registry {
    
    // REGISTRY LIST
    public static readonly Dictionary<string, AbstractConfig> Configs = new();

    // REGISTRIES
    public static ServerPropertyConfig ServerProperty { get; private set; }

    public void Initialize() {
        ServerProperty = this.Register("server_properties", Configs, new ServerPropertyConfig("config", "server-properties"));
    }
}