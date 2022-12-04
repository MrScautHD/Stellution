using Future.Common.csharp.world;

namespace Future.Common.csharp.registry.types; 

public class WorldRegistry {
    
    public static RegistryObject<Earth> Earth = RegistryObject<Earth>.Create(RegistryTypes.WorldType, "earth", new Earth(null));
}