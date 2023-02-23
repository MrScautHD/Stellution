using Easel;
using Easel.Core;
using Easel.Scenes;
using Future.Common.csharp.registry;
using Future.Server.csharp.network;
using Future.Server.csharp.registry;

namespace Future.Server.csharp; 

public class FutureServer : EaselGame {
    
    private ServerNetworkManager networkManager;

    public FutureServer(GameSettings settings, Scene scene) : base(settings, scene) {
        // NETWORK
        this.networkManager = new ServerNetworkManager();
        
        // LOGGER
        Logger.InitializeLogFile("logs");
        Logger.UseConsoleLogs();
        
        // REGISTRY
        IRegistry.RegistryTypes.Add(new ServerConfigRegistry());
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

    public new void Run() {
        base.Run();
        
        Logger.Info("Server Starting!");
        this.networkManager.Start("localhost", "");
    }

    public new void Close() {
        base.Close();
        
        Logger.Info("Server Closed!");
        this.networkManager.Stop();
    }
}