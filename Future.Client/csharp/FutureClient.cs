using Easel;
using Easel.Content.Builder;
using Easel.Scenes;
using Future.Client.csharp.network;
using Future.Client.csharp.registry;
using Future.Common.csharp.file;
using Future.Common.csharp.registry;

namespace Future.Client.csharp; 

public class FutureClient : EaselGame {

    public static readonly ContentBuilder ContentBuilder = new("content");

    protected GameSettings Settings;
    protected ClientNetworkManager NetworkManager;
    
    public FutureClient(GameSettings settings, Scene scene) : base(settings, scene) {
        this.Settings = settings;
        
        // LOGGER
        GameLogger.Initialize("logs", "log");
        
        // CONTENT LOADER
        IContentRegistry.ContentTypes.Add(new ClientFontRegistry());
        //IRegistry.RegistryTypes.Add(new ClientBitmapRegistry());
        IContentRegistry.ContentTypes.Add(new ClientTextureRegistry());
        IContentRegistry.ContentTypes.Add(new ClientModelRegistry());
        IContentRegistry.ContentTypes.Add(new ClientSkyboxRegistry());

        // REGISTRY
        IRegistry.RegistryTypes.Add(new ClientConfigRegistry());
        IRegistry.RegistryTypes.Add(new ClientTranslationRegistry());
        IRegistry.RegistryTypes.Add(new ClientMaterialRegistry());
        IRegistry.RegistryTypes.Add(new ClientEntityRendererRegistry());
        IRegistry.RegistryTypes.Add(new ClientCameraInfoRegistry());
    }

    protected override void Initialize() {
        
        // CONTENT LOADER
        foreach (IContentRegistry contentRegistry in IContentRegistry.ContentTypes) {
            contentRegistry.Load(this.Content);
        }
        
        ContentDefinition definition = ContentBuilder.Build();
        this.Content.AddContent(definition);

        // REGISTRY
        foreach (IRegistry registry in IRegistry.RegistryTypes) {
            registry.Register(this.Content);
        }

        // BASE INITIALIZE
        base.Initialize();
    }
}