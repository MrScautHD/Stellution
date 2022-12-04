using Future.Common.csharp.world;

namespace Future.Common.csharp.entity;

public abstract class Entity {
    
    private AbstractWorld _abstractWorld;

    public bool IsRemoved { get; private set; }

    public Entity(AbstractWorld abstractWorld) {
        this._abstractWorld = abstractWorld;
    }
    
    public virtual void Update() {
        
    }

    public virtual void FixedUpdate() {
        
    }

    public void Remove() {
        this._abstractWorld.RemoveEntity(this);
        this.IsRemoved = true;
    }
}