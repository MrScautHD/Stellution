using System.Text.Json.Nodes;
using Easel;
using Easel.Core;
using Easel.Scenes;
using Stellution.Common.csharp.file;
using Stellution.Common.csharp.registry;
using Stellution.Server.csharp.network;
using Stellution.Server.csharp.registry;

namespace Stellution.Server.csharp; 

public class StellutionServer : EaselGame {
    
    public static StellutionServer Instance { get; private set; }
    
    public ServerNetworkManager NetworkManager { get; private set; }

    public StellutionServer(GameSettings settings, Scene scene) : base(settings, scene) {
        Instance = this;
        
        // LOGGER
        GameLogger.Initialize("logs", "log");

        // REGISTER
        Registry.RegistryTypes.Add(new ServerConfigRegistry());
        
        // NETWORK
        this.NetworkManager = new ServerNetworkManager();
    }

    protected override void Initialize() {
        
        // REGISTRY
        Logger.Debug("Initializing Registries...");
        foreach (IRegistry registry in Registry.RegistryTypes) {
            registry.Initialize(this.Content);
        }
        
        base.Initialize();
        
        // START SERVER
        JsonNode serverProperty = ServerConfigRegistry.ServerProperty.ReadJsonAsNode();
        this.NetworkManager.Start(serverProperty["port"].GetValue<ushort>(), serverProperty["max_client_count"].GetValue<ushort>());
    }

    protected override void Update() {
        base.Update();
        this.NetworkManager.FixedUpdate();
    }

    public new void Close() {
        this.NetworkManager.Stop();
        base.Close();
    }
}