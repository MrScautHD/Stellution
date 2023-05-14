using Easel.Entities;
using Easel.Entities.Components;
using Easel.Math;
using Stellution.Client.csharp.registry.types;
using Stellution.Common.csharp.scenes;

namespace Stellution.Client.csharp.events; 

public class SceneEvent {

    public SceneEvent() {
        ModifiedScene.Initializing += this.OnInitializing;
    }

    private void OnInitializing(ModifiedScene scene, string name) {
        switch (scene.Name) {
            
            case "earth":
                OverlayRegistry.CrosshairOverlay.Enabled = true;
                
                // LIGHT
                Entity sun = scene.GetEntity("Sun");
                sun.GetComponent<DirectionalLight>().Color = Color.Blue;
                
                // CAMERA
                Camera.Main.Skybox = SkyboxRegistry.EarthSkybox;
                Camera.Main.AddComponent(new NoClipCamera() {
                    MouseSensitivity =  0.005F,
                });
                break;
        }
    }
}