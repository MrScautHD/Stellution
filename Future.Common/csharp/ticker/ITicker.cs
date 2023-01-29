using Microsoft.Xna.Framework;

namespace Future.Common.csharp.ticker; 

public interface ITicker {
    
    void FixedUpdate(GameTime gameTime);
}