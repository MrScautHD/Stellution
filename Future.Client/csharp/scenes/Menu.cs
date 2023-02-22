using System.Numerics;
using Easel;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Scenes;
using Future.Client.csharp.registry;
using Future.Common.csharp.entity;

namespace Future.Client.csharp.scenes;

public class Menu : Scene {
    
    protected override void Initialize() {
        base.Initialize();
        
        if (!EaselGame.Instance.IsServer) {
            Camera.Main.Skybox = ClientSkyboxRegistry.EarthSkybox;
            Camera.Main.AddComponent(new NoClipCamera() {
                MouseSensitivity =  0.005F,
            });
        }

        ModifiedEntity cyberCar = new ModifiedEntity("cyber_car");
        this.AddEntity(cyberCar);

        Transform transform = new Transform();
        transform.Position = new Vector3(10, 0, 0);
        
        ModifiedEntity player = new ModifiedEntity(transform, "player");
        this.AddEntity(player);
    }
}