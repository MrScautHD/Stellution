using Future.Common.csharp.file;
using Future.Server.csharp.config;

namespace Future.Server.csharp.registry.types; 

public class ConfigRegistry : IRegistry {

    public readonly ServerPropertyConfig GraphicConfig = Register("server_properties", new ServerPropertyConfig("config", "server-properties.json"));

    private static T Register<T>(string name, T config) where T : AbstractConfig {
        RegistryTypes.Configs.Add(name, config);
        
        return config;
    }
}