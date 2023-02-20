using Easel;
using Easel.Core;
using Easel.Scenes;
using Future.Client.csharp.registry;
using Future.Common.csharp.registry;
using Pie.Windowing;

namespace Future.Client.csharp; 

public class FutureClient : EaselGame {

    protected GameSettings _settings;
    
    public FutureClient(GameSettings settings, Scene scene) : base(settings, scene) {
        this._settings = settings;
        
        // REGISTRY
        IRegistry.RegistryTypes.Add(new ClientConfigRegistry());
    }

    protected override void Initialize() {
        base.Initialize();

        // GAME PROPERTIES
        this.Content.ContentRootDir = "content";
        Input.MouseState = MouseState.Visible;
        
        // LOGGER
        Logger.InitializeLogFile("logs");
        Logger.UseConsoleLogs();
    }
}