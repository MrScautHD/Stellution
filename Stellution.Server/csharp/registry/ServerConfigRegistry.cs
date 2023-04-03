using Easel.Content;
using Stellution.Server.csharp.config;
using Stellution.Common.csharp.registry;

namespace Stellution.Server.csharp.registry; 

public class ServerConfigRegistry : Registry, IRegistry {
    
    public static ServerPropertyConfig ServerProperty { get; private set; }

    public void Initialize(ContentManager content) {
        ServerProperty = new ServerPropertyConfig("config", "server-properties");
    }
}