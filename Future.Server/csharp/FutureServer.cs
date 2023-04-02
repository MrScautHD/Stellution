using Easel;
using Easel.Core;
using Easel.Scenes;
using Future.Common.csharp.file;
using Future.Common.csharp.registry;
using Future.Server.csharp.network;
using Future.Server.csharp.registry;

namespace Future.Server.csharp; 

public class FutureServer : EaselGame {
    
    protected ServerNetworkManager ServerNetworkManager;

    public FutureServer(GameSettings settings, Scene scene) : base(settings, scene) {
        
        // NETWORK
        this.ServerNetworkManager = new ServerNetworkManager();
        
        // LOGGER
        GameLogger.Initialize("logs", "log");
        
        // REGISTER
        Registry.RegistryTypes.Add(new ServerConfigRegistry());
    }

    protected override void Initialize() {
        
        // REGISTRY
        Logger.Debug("Initializing Registries...");
        foreach (IRegistry registry in Registry.RegistryTypes) {
            registry.Initialize(this.Content);
        }

        base.Initialize();
    }

    protected override void Update() {
        base.Update();

        if (this.ServerNetworkManager != null) {
            this.ServerNetworkManager.Update();
        }
    }

    public new void Run() {
        base.Run();
        
        Logger.Info("Server Starting!");
        this.ServerNetworkManager.Start(9050);
    }

    public new void Close() {
        base.Close();
        
        Logger.Info("Server Closed!");
        this.ServerNetworkManager.Stop();
    }
}