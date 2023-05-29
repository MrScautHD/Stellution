using Easel.Entities;
using Stellution.Common.csharp.entity.environment;
using Stellution.Common.csharp.entity.player;
using Stellution.Common.csharp.entity.vehicle;

namespace Stellution.Common.csharp.registry.types; 

public class EntityPrefabRegistry : Registry {
    
    public static readonly Dictionary<string, Func<Entity>> Entities = new();
    
    public static Func<GroundEntity>? GroundEntity { get; private set; }
    public static Func<PlayerEntity>? PlayerEntity { get; private set; }
    public static Func<CyberCarEntity>? CyberCarEntity { get; private set; }

    public override void Initialize() {
        GroundEntity = this.Register("ground", Entities, () => new GroundEntity(new Transform()));
        PlayerEntity = this.Register("player", Entities, () => new PlayerEntity(new Transform()));
        CyberCarEntity = this.Register("cyber_car", Entities, () => new CyberCarEntity(new Transform()));
    }
}