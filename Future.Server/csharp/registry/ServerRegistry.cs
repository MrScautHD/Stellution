using Easel.Content;
using Future.Common.csharp.registry;
using Future.Server.csharp.config;

namespace Future.Server.csharp.registry; 

public class ServerRegistry : IRegistry {
    
    // CONFIGS
    public static ServerPropertyConfig ServerPropertyConfig { get; private set; }

    public void Initialize(ContentManager content) {
        
        // CONFIGS
        ServerPropertyConfig = new ServerPropertyConfig("config", "server-properties");
    }
}