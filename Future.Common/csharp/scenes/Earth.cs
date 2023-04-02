using System.Numerics;
using BulletSharp;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Math;
using Easel.Physics;
using Future.Common.csharp.entity;

namespace Future.Common.csharp.scenes; 

public class Earth : ModifiedScene {

    public Earth(int initialCapacity = 128) : base("earth", initialCapacity) {
    }

    protected override void Initialize() {
        base.Initialize();
        
        Rigidbody rigidbody = new Rigidbody(2, new BoxShape(3));
        rigidbody.Enabled = true;

        ModifiedEntity cyberCar = new ModifiedEntity("cyber_car", "cyber_car");
        cyberCar.AddComponent(rigidbody);
        this.AddEntity(cyberCar);

        Transform transform = new Transform();
        transform.Position = new Vector3(10, 0, 0);
        
        ModifiedEntity player = new ModifiedEntity(transform, "player");
        this.AddEntity(player);

        Entity light = this.GetEntity("Sun");
        light.GetComponent<DirectionalLight>().Color = Color.Blue;
    }
    
    protected override void Update() {
        Physics.Update();
        
        base.Update();
    }
}