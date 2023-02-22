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

    private double _timer;
    private readonly double _delay = 1.0 / 60.0;
    
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
    }

    protected override void Initialize() {
        // GAME PROPERTIES
        this.Content.ContentRootDir = "content";
        Input.MouseState = MouseState.Locked;
        
        // INIT REGISTRY
        foreach (IRegistry registry in IRegistry.RegistryTypes) {
            registry.Initialize(this.Content);
        }
        
        // BASE INITIALIZE
        base.Initialize();
    }

    protected override void Update() {
        base.Update();

        this._timer += Time.DeltaTime;

        // UPDATE REGISTRY
        if (this._timer >= this._delay) {
            foreach (IRegistry registry in IRegistry.RegistryTypes) {
                registry.FixedUpdate();
                this._timer -= this._delay;
            }
        }
    }

    protected override void Draw() {
        base.Draw();
        
        // DRAW REGISTRY
        foreach (IRegistry registry in IRegistry.RegistryTypes) {
            registry.Draw();
        }
    }
}