using Easel.Content;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Scenes;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientCameraInfoRegistry : Registry, IRegistry {
    
    public void InitializeLate(ContentManager content) {
        if (SceneManager.ActiveScene == null || Camera.Main == null) return;
        
        Camera.Main.Skybox = ClientSkyboxRegistry.EarthSkybox;
        Camera.Main.AddComponent(new NoClipCamera() {
            MouseSensitivity =  0.005F,
        });
    }
}