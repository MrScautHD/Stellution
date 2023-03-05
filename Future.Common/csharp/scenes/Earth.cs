using System.Numerics;
using BulletSharp;
using Easel.Entities;
using Easel.Physics;
using Future.Common.csharp.entity;

namespace Future.Common.csharp.scenes; 

public class Earth : ModifiedScene {

    public Earth(int initialCapacity = 128) : base("earth", initialCapacity) {
    }

    protected override void Initialize() {
        base.Initialize();
        
        Rigidbody rigidbody = new Rigidbody(1, new BoxShape(3));

        ModifiedEntity cyberCar = new ModifiedEntity("cyber_car", "test");
        cyberCar.AddComponent(rigidbody);
        this.AddEntity(cyberCar);

        Transform transform = new Transform();
        transform.Position = new Vector3(10, 0, 0);
        
        ModifiedEntity player = new ModifiedEntity(transform, "player");
        this.AddEntity(player);
    }
    
    protected override void Update() {
        base.Update();
        
    }
}