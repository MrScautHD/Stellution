using Easel;
using Easel.Scenes;
using Future.Client.csharp.network;
using Future.Client.csharp.registry;
using Future.Common.csharp.file;

namespace Future.Client.csharp; 

public class FutureClient : EaselGame {
    
    protected GameSettings Settings;
    protected ClientNetworkManager NetworkManager;
    
    public FutureClient(GameSettings settings, Scene scene) : base(settings, scene) {
        this.Settings = settings;
        
        // LOGGER
        GameLogger.Initialize("logs", "log");
    }

    protected override void Initialize() {
        
        // REGISTRY
        ClientRegistry registry = new ClientRegistry();
        registry.Initialize(this.Content);

        base.Initialize();
    }
}