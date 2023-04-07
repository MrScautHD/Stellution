using Easel;
using Easel.Core;
using Easel.Scenes;
using Stellution.Client.csharp.network;
using Stellution.Client.csharp.registry;
using Stellution.Common.csharp.file;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp; 

public class StellutionClient : EaselGame {
    
    public new static StellutionClient Instance { get; private set; }
    
    public ClientNetworkManager NetworkManager { get; private set; }

    private float _timer;
    private readonly float _delay = 1.0F / 60.0F;

    public StellutionClient(GameSettings settings, Scene scene) : base(settings, scene) {
        Instance = this;
        this.NetworkManager = new ClientNetworkManager();
        GameLogger.Initialize("logs", "log");
        
        // REGISTER
        Registry.RegistryTypes.Add(new TranslationRegistry());
        Registry.RegistryTypes.Add(new FontRegistry());
        Registry.RegistryTypes.Add(new BitmapRegistry());
        Registry.RegistryTypes.Add(new TextureRegistry());
        Registry.RegistryTypes.Add(new ClientConfigRegistry());
        Registry.RegistryTypes.Add(new MaterialRegistry());
        Registry.RegistryTypes.Add(new ModelRegistry());
        Registry.RegistryTypes.Add(new SkyboxRegistry());
        Registry.RegistryTypes.Add(new ClientEventRegistry());
    }

    protected override void Initialize() {
        Logger.Debug("Initializing Registries...");
        foreach (IRegistry registry in Registry.RegistryTypes) {
            registry.Initialize(this.Content);
        }
        
        base.Initialize();
        
        //TODO CREATE UI FOR IT
        this.NetworkManager.Connect("127.0.0.1:7777");
    }

    protected override void Update() {
        base.Update();
        this.FixedUpdateCalculator();
    }
    
    protected void FixedUpdate() {
        this.NetworkManager.FixedUpdate();
    }
    
    private void FixedUpdateCalculator() {
        this._timer += Time.DeltaTime;
        
        if (this._timer >= this._delay) {
            this.FixedUpdate();
            this._timer -= this._delay;
        }
    }
}