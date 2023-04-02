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

        // LOGGER
        GameLogger.Initialize("logs", "log");

        // REGISTER
        Registry.RegistryTypes.Add(new ServerConfigRegistry());
        
        // NETWORK
        this.ServerNetworkManager = new ServerNetworkManager();
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
        this.ServerNetworkManager.FixedUpdate();
    }

    public new void Run() {
        this.ServerNetworkManager.Start(7777, 10);
        base.Run();
    }

    public new void Close() {
        this.ServerNetworkManager.Stop();
        base.Close();
    }
}