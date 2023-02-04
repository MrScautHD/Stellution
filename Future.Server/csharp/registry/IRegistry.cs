using Microsoft.Xna.Framework;

namespace Future.Server.csharp.registry; 

public interface IRegistry {

    public static readonly List<IRegistry> Registries = new();

    public virtual void FixedUpdate(GameTime gameTime) {
          
    }
}