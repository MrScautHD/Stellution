using System.Numerics;
using Easel.Entities;
using Easel.Scenes;
using Future.Common.csharp.entity;

namespace Future.Common.csharp.scenes; 

public class Earth : Scene {
    
    protected override void Initialize() {
        base.Initialize();

        ModifiedEntity cyberCar = new ModifiedEntity("cyber_car");
        this.AddEntity(cyberCar);

        Transform transform = new Transform();
        transform.Position = new Vector3(10, 0, 0);
        
        ModifiedEntity player = new ModifiedEntity(transform, "player");
        this.AddEntity(player);
    }
}