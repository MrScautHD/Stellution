using System.Numerics;
using Easel.Entities;
using Stellution.Common.csharp.entity.environment;
using Stellution.Common.csharp.entity.vehicle;

namespace Stellution.Common.csharp.scenes; 

public class Earth : ModifiedScene {

    public Earth() : base("earth") {
    }

    protected override void Initialize() {
        base.Initialize();
        
        // GROUND
        GroundEntity groundEntity = new GroundEntity(new Transform() { Position = new Vector3(0, 0, 0) });
        this.AddEntity(groundEntity);

        // CYBER CAR
        CyberCarEntity cyberCar = new CyberCarEntity(new Transform() { Position = new Vector3(0, 100, 0) });
        this.AddEntity(cyberCar);

        // TODO ADD LIGHT COLOR BACK DirectionalLight
    }
}