using Future.Common.csharp.ticker;
using Future.Common.csharp.world;
using Microsoft.Xna.Framework;

namespace Future.Common.csharp.entity;

public abstract class Entity : ITicker {
    
    private AbstractWorld _abstractWorld;

    public bool IsRemoved { get; private set; }

    public Entity(AbstractWorld abstractWorld) {
        this._abstractWorld = abstractWorld;
    }

    public virtual void FixedUpdate(GameTime time) {
        
    }

    public void Remove() {
        this._abstractWorld.RemoveEntity(this);
        this.IsRemoved = true;
    }
}