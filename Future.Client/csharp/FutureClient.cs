using Easel;
using Easel.Content.Builder;
using Easel.Scenes;
using Future.Client.csharp.network;
using Future.Client.csharp.registry;
using Future.Common.csharp.file;
using Future.Common.csharp.registry;

namespace Future.Client.csharp; 

public class FutureClient : EaselGame {

    public static readonly ContentBuilder ContentBuilder = new("content");

    protected GameSettings Settings;
    protected ClientNetworkManager NetworkManager;
    
    public FutureClient(GameSettings settings, Scene scene) : base(settings, scene) {
        this.Settings = settings;
        
        // LOGGER
        GameLogger.Initialize("logs", "log");

        // REGISTRY
        IRegistry.RegistryTypes.Add(new ClientConfigRegistry());
        IRegistry.RegistryTypes.Add(new ClientEntityRendererRegistry());
        IRegistry.RegistryTypes.Add(new ClientCameraInfoRegistry());
    }

    protected override void Initialize() {

        // REGISTRY
        foreach (IRegistry registry in IRegistry.RegistryTypes) {
            registry.InitializePre(this.Content);
        }

        // BASE INITIALIZE
        base.Initialize();
        
        // DELAYED REGISTRY
        foreach (IRegistry registry in IRegistry.RegistryTypes) {
            registry.InitializeLate(this.Content);
        }
    }
}