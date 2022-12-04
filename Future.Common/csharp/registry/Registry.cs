using Future.Common.csharp.entity;
using Future.Common.csharp.world;

namespace Future.Common.csharp.registry; 

public class Registry {

    public static readonly Dictionary<string, Entity> Entities = new();
    public static readonly Dictionary<string, AbstractWorld> Worlds = new();
    public static readonly Dictionary<string, object> Objcets = new();

    /**
     * With that Method you can REGISTER your Object, Entity...
     */
    public static void AddRegistry(RegistryTypes type, string key, object registryObject) {
        switch (type) {
            case RegistryTypes.EntityType:
                Entities.Add(key, (Entity) registryObject);
                break;
            
            case RegistryTypes.WorldType:
                Worlds.Add(key, (AbstractWorld) registryObject);
                break;
            
            case RegistryTypes.ObjectType:
                Objcets.Add(key, registryObject);
                break;
        }
    }
}