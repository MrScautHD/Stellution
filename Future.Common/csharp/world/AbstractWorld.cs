using Future.Common.csharp.entity;
using Future.Common.csharp.network;

namespace Future.Common.csharp.world; 

public abstract class AbstractWorld {

    private List<Entity> Entities;

    private NetworkHandler _network;

    public AbstractWorld(NetworkHandler network) {
        this.Entities = new();
        this._network = network;
    }

    public void SpawnEntity(Entity entity) {
        Entities.Add(entity);
        // Spawn Point
        // Rot
    }

    public void RemoveEntity(Entity entity) {
        Entities.Remove(entity);
    }
}