using Easel.Content;
using Future.Client.csharp.events;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientEventRegistry : Registry, IRegistry {
    
    public static EntityConstructorEvent EntityConstructor { get; private set; }
    public static SceneEvent Scene { get; private set; }
    
    public void Initialize(ContentManager content) {
        EntityConstructor = new EntityConstructorEvent();
        Scene = new SceneEvent();
    }
}