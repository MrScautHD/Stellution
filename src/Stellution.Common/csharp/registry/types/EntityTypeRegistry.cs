using Stellution.Common.csharp.entity;
using Stellution.Common.csharp.entity.environment;
using Stellution.Common.csharp.entity.player;
using Stellution.Common.csharp.entity.vehicle;

namespace Stellution.Common.csharp.registry.types; 

public class EntityTypeRegistry : Registry {
    
    public static readonly Dictionary<string, EntityType> Entities = new();
    
    public static EntityType GroundEntity { get; private set; }
    public static EntityType PlayerEntity { get; private set; }
    public static EntityType CyberCarEntity { get; private set; }

    public override void Initialize() {
        GroundEntity = this.Register("cyber_car", Entities, new EntityType(typeof(GroundEntity)));
        PlayerEntity = this.Register("cyber_car", Entities, new EntityType(typeof(PlayerEntity)));
        CyberCarEntity = this.Register("cyber_car", Entities, new EntityType(typeof(CyberCarEntity)));
    }
}