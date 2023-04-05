using System.Numerics;
using System.Reflection;
using BulletSharp;
using Easel;
using Easel.Core;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Math;
using Easel.Physics;
using Stellution.Common.csharp.entity;

namespace Stellution.Common.csharp.scenes; 

public class Earth : ModifiedScene {

    public Earth(int initialCapacity = 128) : base("earth", initialCapacity) {
    }

    protected override void Initialize() {
        base.Initialize();
        Physics.Initialize(new PhysicsInitializeSettings());
        
        Transform carPos = new Transform() { Position = new Vector3(0, 9, 0) };
        RigidEntity cyberCar = new RigidEntity(carPos, 2, new BoxShape(3), "cyber_car", "cyber_car");
        this.AddEntity(cyberCar);
        
        RigidEntity groundEntity = new RigidEntity(0, new BoxShape(10000, 1, 10000), "ground", "ground");
        this.AddEntity(groundEntity);
        
        Transform playPos = new Transform() { Position = new Vector3(10, 0, 0) };
        ModifiedEntity player = new ModifiedEntity(playPos, "player");
        this.AddEntity(player);

        Entity light = this.GetEntity("Sun");
        light.GetComponent<DirectionalLight>().Color = Color.Blue;
    }
    
    protected override void Update() {
        Physics.Timestep(Time.DeltaTime);

        RigidEntity cyberCar = (RigidEntity) this.GetEntity("cyber_car");
        Vector3 pos = cyberCar.Transform.Position;
        
        this.Hover(cyberCar, new Vector3(pos.X + 3, pos.Y - 3, pos.Z), 4);
        this.Hover(cyberCar, new Vector3(pos.X - 3, pos.Y - 3, pos.Z), 4);
        this.Hover(cyberCar, new Vector3(pos.X, pos.Y - 3, pos.Z + 3), 4);
        this.Hover(cyberCar, new Vector3(pos.X, pos.Y - 3, pos.Z - 3), 4);

        base.Update();
    }

    public void Hover(RigidEntity entity, Vector3 pos, float hoverHeight) {
        if (Physics.Raycast(pos, -Vector3.UnitY, 4, out RayHit hit)) {
            entity.GetBulletRigidBody().ApplyForce(new Vector3(0, 9.85F, 0), pos);
        }
    }
}