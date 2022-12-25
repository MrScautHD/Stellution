using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.registry; 

public abstract class ClientRegistry {

     public static readonly List<ClientRegistry> Registries = new();

     public virtual void Initialize(GraphicsDevice graphicsDevice, GameWindow window) {
          
     }
     
     public virtual void LoadContent(GraphicsDevice graphicsDevice, ContentManager content) {
          
     }

     public virtual void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime time) {
          
     }

     public virtual void Update(GameTime gameTime) {
          
     }
}