using Easel;
using Easel.Core;
using Easel.Scenes;
using Future.Client.csharp.network;
using Future.Client.csharp.registry;
using Future.Common.csharp.registry;
using Pie.Windowing;

namespace Future.Client.csharp; 

public class FutureClient : EaselGame {

    protected GameSettings _settings;

    private ClientNetworkManager networkManager;
    
    public FutureClient(GameSettings settings, Scene scene) : base(settings, scene) {
        this._settings = settings;
        
        // LOGGER
        Logger.InitializeLogFile("logs");
        Logger.UseConsoleLogs();

        // REGISTRY
        IRegistry.RegistryTypes.Add(new ClientConfigRegistry());
        IRegistry.RegistryTypes.Add(new ClientFontRegistry());
        IRegistry.RegistryTypes.Add(new ClientTranslationRegistry());
        IRegistry.RegistryTypes.Add(new ClientBitmapRegistry());
        IRegistry.RegistryTypes.Add(new ClientTextureRegistry());
        IRegistry.RegistryTypes.Add(new ClientModelRegistry());
        IRegistry.RegistryTypes.Add(new ClientSkyboxRegistry());
        IRegistry.RegistryTypes.Add(new ClientEntityRendererRegistry());
        IRegistry.RegistryTypes.Add(new ClientCameraInfoRegistry());
    }

    protected override void Initialize() {
        // GAME PROPERTIES
        this.Content.ContentRootDir = "content";
        Input.MouseState = MouseState.Visible;
        
        // INIT REGISTRY
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