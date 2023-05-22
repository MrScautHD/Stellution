using System.Numerics;
using Easel.Entities;
using Stellution.Common.csharp.entity;
using Stellution.Common.csharp.entity.environment;
using Stellution.Common.csharp.entity.player;
using Stellution.Common.csharp.entity.vehicle;

namespace Stellution.Common.csharp.scenes; 

public class Earth : ModifiedScene {

    public Earth() : base("earth") {
    }

    protected override void Initialize() {
        base.Initialize();
        
        // GROUND
        RigidEntity groundEntity = new GroundEntity(new Transform() { Position = new Vector3(0, 0, 0) });
        this.AddEntity(groundEntity);

        // PLAYER
        for (int i = 0; i < 900; i++) {
            PlayerEntity player = new PlayerEntity(new Transform() { Position = new Vector3(0, i, 0) });
            this.AddEntity(player);
        }
        
        PlayerEntity player2 = new PlayerEntity(new Transform() { Position = new Vector3(8, 16, 0) });
        this.AddEntity(player2);

        // CYBER CAR
        CyberCarEntity cyberCar = new CyberCarEntity(new Transform() { Position = new Vector3(0, 16, 0) });
        this.AddEntity(cyberCar);

        // TODO ADD LIGHT COLOR BACK DirectionalLight
    }
}