using Easel;
using Easel.Core;
using Easel.Scenes;
using Future.Client.csharp.network;
using Future.Client.csharp.registry;
using Future.Common.csharp.file;
using Future.Common.csharp.registry;

namespace Future.Client.csharp; 

public class FutureClient : EaselGame {
    
    protected GameSettings Settings;
    protected ClientNetworkManager NetworkManager;
    
    public FutureClient(GameSettings settings, Scene scene) : base(settings, scene) {
        this.Settings = settings;
        
        // LOGGER
        GameLogger.Initialize("logs", "log");
        
        // REGISTER
        Registry.RegistryTypes.Add(new TranslationRegistry());
        Registry.RegistryTypes.Add(new FontRegistry());
        Registry.RegistryTypes.Add(new BitmapRegistry());
        Registry.RegistryTypes.Add(new TextureRegistry());
        Registry.RegistryTypes.Add(new ClientConfigRegistry());
        Registry.RegistryTypes.Add(new MaterialRegistry());
        Registry.RegistryTypes.Add(new ModelRegistry());
        Registry.RegistryTypes.Add(new SkyboxRegistry());
        Registry.RegistryTypes.Add(new ClientEventRegistry());
    }

    protected override void Initialize() {
        
        // REGISTRY
        Logger.Debug("Initializing Registries...");
        foreach (IRegistry registry in Registry.RegistryTypes) {
            registry.Initialize(this.Content);
        }
        
        base.Initialize();
        
        //this.NetworkManager.Connect("localhost", 9050);
    }
}