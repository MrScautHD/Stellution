using Future.Client.csharp.config;
using Future.Common.csharp.file;

namespace Future.Client.csharp.registry.types; 

public class ClientConfigRegistry : IClientRegistry {

    public static readonly GraphicConfig GraphicConfig = Register("graphic_config", new GraphicConfig("config", "graphic-config.json"));

    private static T Register<T>(string name, T config) where T : AbstractConfig {
        ClientRegistryTypes.Configs.Add(name, config);
        
        return config;
    }
}