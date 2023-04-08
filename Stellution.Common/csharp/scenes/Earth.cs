using System.Numerics;
using Easel.Entities;
using Easel.Entities.Components;
using Easel.Math;
using Stellution.Common.csharp.entity;
using Stellution.Common.csharp.entity.environment;
using Stellution.Common.csharp.entity.player;
using Stellution.Common.csharp.entity.vehicle;

namespace Stellution.Common.csharp.scenes; 

public class Earth : ModifiedScene {

    public Earth() : base("earth", true) {
    }

    protected override void Initialize() {
        base.Initialize();

        // GROUND
        RigidEntity groundEntity = new GroundEntity();
        this.AddEntity(groundEntity);
        
        // PLAYER
        PlayerEntity player = new PlayerEntity(new Vector3(3, 16, 0));
        this.AddEntity(player);
        
        // CYBER CAR
        CyberCarEntity cyberCar = new CyberCarEntity(new Vector3(0, 16, 0));
        this.AddEntity(cyberCar);

        // LIGHT COLOR
        Entity light = this.GetEntity("Sun");
        light.GetComponent<DirectionalLight>().Color = Color.Blue;
    }
}