using Stellution.Common.csharp.registry;
using Stellution.Server.csharp.config;

namespace Stellution.Server.csharp.registry;

public class ServerConfigRegistry : Registry {
    public static ServerPropertyConfig ServerProperty { get; private set; }

    public override void Initialize() {
        ServerProperty = new ServerPropertyConfig("config", "server-properties");
    }
}