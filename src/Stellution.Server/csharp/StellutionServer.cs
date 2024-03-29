using System.Text.Json.Nodes;
using Easel;
using Easel.Core;
using Easel.Scenes;
using Stellution.Common.csharp.file;
using Stellution.Common.csharp.registry;
using Stellution.Common.csharp.registry.types;
using Stellution.Server.csharp.network;
using Stellution.Server.csharp.registry;

namespace Stellution.Server.csharp; 

public class StellutionServer : EaselGame {
    
    public new static StellutionServer Instance { get; private set; }
    
    public static ServerNetworkManager NetworkManager { get; private set; }

    // TRY TO START THE SERVER WITH: Process.Start("D:/projects/Stellution/src/Stellution.Server/bin/Debug/net7.0/Stellution.Server.exe");
    public StellutionServer(GameSettings settings, Scene scene) : base(settings, scene) {
        Instance = this;
        NetworkManager = new ServerNetworkManager();
        GameLogger.Initialize("logs", "log");

        // REGISTER
        Registry.RegistryTypes.Add(new ServerConfigRegistry());
        Registry.RegistryTypes.Add(new PrefabRegistry());
    }

    protected override void Initialize() {
        Logger.Debug("Initializing Registries...");
        foreach (Registry registry in Registry.RegistryTypes) {
            registry.Initialize();
        }
        
        base.Initialize();
        
        // START NETWORK
        JsonNode serverProperty = ServerConfigRegistry.ServerProperty.ReadJsonAsNode();
        NetworkManager.Start(serverProperty["port"].GetValue<ushort>(), serverProperty["max_client_count"].GetValue<ushort>());
    }

    protected override void FixedUpdate() {
        base.FixedUpdate();
        NetworkManager.Update();
    }

    public new void Close() {
        NetworkManager.Stop();
        base.Close();
    }
}