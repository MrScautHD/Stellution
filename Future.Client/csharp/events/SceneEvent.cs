using Easel.Entities;
using Easel.Entities.Components;
using Future.Client.csharp.registry;
using Future.Common.csharp.scenes;

namespace Future.Client.csharp.events; 

public class SceneEvent {

    public SceneEvent() {
        ModifiedScene.Initializing += (obj, args) => this.Event(args.Scene);
    }

    protected void Event(ModifiedScene scene) {
        switch (scene.Name) {
            
            case "earth":
                Camera.Main.Skybox = SkyboxRegistry.EarthSkybox;
                Camera.Main.AddComponent(new NoClipCamera() {
                    MouseSensitivity =  0.005F,
                }); 
                break;
        }
    }
}