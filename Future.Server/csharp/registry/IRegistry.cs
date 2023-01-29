using Microsoft.Xna.Framework;

namespace Future.Common.csharp.registry; 

public interface IRegistry {

    public static readonly List<IRegistry> Registries = new();

    public virtual void FixedUpdate(GameTime gameTime) {
          
    }
}