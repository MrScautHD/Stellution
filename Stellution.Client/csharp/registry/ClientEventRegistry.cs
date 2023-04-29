using Stellution.Client.csharp.events;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry; 

public class ClientEventRegistry : Registry, IRegistry {
    
    public static EntityConstructorEvent EntityConstructor { get; private set; }
    public static SceneEvent Scene { get; private set; }
    
    public void Initialize() {
        EntityConstructor = new EntityConstructorEvent();
        Scene = new SceneEvent();
    }
}