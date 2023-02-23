using Easel.Content;
using Easel.Entities;
using Easel.Entities.Components;
using Future.Common.csharp.registry;
using Future.Common.csharp.scenes;

namespace Future.Client.csharp.registry; 

public class ClientCameraInfoRegistry : Registry, IRegistry {
    
    public void InitializeLate(ContentManager content) {
        ModifiedScene.Initializing += (obj, args) => this.Event(args.Scene);
    }
    
    protected void Event(ModifiedScene scene) {
        Camera.Main.Skybox = ClientSkyboxRegistry.EarthSkybox;
        Camera.Main.AddComponent(new NoClipCamera() {
            MouseSensitivity =  0.005F,
        });
    }
}