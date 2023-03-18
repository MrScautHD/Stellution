using System.Collections.Generic;
using Easel.Content;
using Future.Client.csharp.config;
using Future.Common.csharp.file;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientConfigRegistry : Registry, IRegistry {
    
    public static readonly Dictionary<string, AbstractConfig> Configs = new();
    
    public static GraphicConfig GraphicConfig { get; private set; }

    public void Register(ContentManager content) {
        GraphicConfig = Register("graphic_config", Configs, new GraphicConfig("config", "graphic-config"));
    }
}