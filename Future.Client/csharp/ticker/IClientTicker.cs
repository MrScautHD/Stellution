using Microsoft.Xna.Framework;

namespace Future.Client.csharp.ticker; 

public interface IClientTicker {

    void Update(GameTime gameTime);
}