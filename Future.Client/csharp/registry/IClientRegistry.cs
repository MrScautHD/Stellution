using System.Collections.Generic;
using Future.Client.csharp.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.registry; 

public interface IClientRegistry {

     public static readonly List<IClientRegistry> Registries = new();

     public virtual void Initialize(GraphicsDevice graphicsDevice, GameWindow window, Camera camera) {
          
     }
     
     public virtual void LoadContent(GraphicsDevice graphicsDevice, ContentManager content) {
          
     }

     public virtual void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime time) {
          
     }

     public virtual void Update(GameTime gameTime) {
          
     }
}