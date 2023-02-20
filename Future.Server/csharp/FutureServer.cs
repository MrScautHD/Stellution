using Easel;
using Easel.Core;
using Easel.Scenes;
using Future.Common.csharp.registry;
using LiteNetLib;

namespace Future.Server.csharp; 

public class FutureServer : EaselGame {
    
    private NetManager _netManager;
    protected EventBasedNetListener _listener;
    
    public FutureServer(GameSettings settings, Scene scene) : base(settings, scene) {
        // NETWORK
        this._listener = new EventBasedNetListener();
        this._netManager = new NetManager(this._listener);
        
        // LOGGER
        Logger.InitializeLogFile("logs");
        Logger.UseConsoleLogs();
    }

    protected override void Initialize() {
        base.Initialize();
        
        // INIT REGISTRY
        foreach (IRegistry registry in IRegistry.RegistryTypes) {
            registry.Initialize();
        }
    }

    protected override void Update() {
        base.Update();
        
        // UPDATE REGISTRY
        foreach (IRegistry registry in IRegistry.RegistryTypes) {
            registry.FixedUpdate();
        }
    }

    /**
     * Don't use "Run"
     */
    public void StartServer() {
        Logger.Info("Server Starting!");
        
        this._netManager.Start();
        this.Run();
    }

    /**
     * Don't use "Close"
     */
    public void StopServer() {
        Logger.Info("Server Closed!");
        
        this._netManager.Stop();
        this.Close();
    }
}