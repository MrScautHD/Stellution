using Future.Common.csharp.file;

namespace Future.Common.csharp.registry.types; 

public class ConfigRegistry : IRegistry {

    //public static readonly GraphicConfig GraphicConfig = Register("client_ticker", new GraphicConfig("config", "graphic-config.json"));

    private static T Register<T>(string name, T config) where T : AbstractConfig {
        RegistryTypes.Configs.Add(name, config);
        
        return config;
    }
}