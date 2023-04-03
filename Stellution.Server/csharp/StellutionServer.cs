using Easel;
using Easel.Core;
using Easel.Scenes;
using Stellution.Common.csharp.file;
using Stellution.Common.csharp.registry;
using Stellution.Server.csharp.network;
using Stellution.Server.csharp.registry;

namespace Stellution.Server.csharp; 

public class StellutionServer : EaselGame {
    
    protected ServerNetworkManager ServerNetworkManager;

    public StellutionServer(GameSettings settings, Scene scene) : base(settings, scene) {

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