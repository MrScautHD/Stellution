using Easel;
using Easel.Core;
using Easel.Scenes;
using Future.Client.csharp.config;
using Pie.Windowing;

namespace Future.Client.csharp; 

public class FutureClient : EaselGame {

    protected GameSettings _settings;
    
    public FutureClient(GameSettings settings, Scene scene) : base(settings, scene) {
        this._settings = settings;
    }

    protected override void Initialize() {
        base.Initialize();

        // GAME PROPERTIES
        this.Content.ContentRootDir = "content";
        Input.MouseState = MouseState.Visible;
        
        // LOGGER
        Logger.InitializeLogFile("logs");
        Logger.UseConsoleLogs();

        // REGISTER SYSTEM (NOT DONE)
        GraphicConfig graphicConfig = new GraphicConfig("config", "graphic-config.json");
    }
}