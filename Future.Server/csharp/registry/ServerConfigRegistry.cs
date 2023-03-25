using Easel.Content;
using Future.Common.csharp.registry;
using Future.Server.csharp.config;

namespace Future.Server.csharp.registry; 

public class ServerConfigRegistry : Registry, IRegistry {
    
    public static ServerPropertyConfig ServerProperty { get; private set; }

    public void Initialize(ContentManager content) {
        ServerProperty = new ServerPropertyConfig("config", "server-properties");
    }
}