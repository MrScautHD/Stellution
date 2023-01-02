using Future.Client.csharp.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.renderer; 

public interface IRenderer {
    
    public virtual void Initialize(GraphicsDevice graphicsDevice, GameWindow window, Camera camera) {
        
    }

    public virtual void LoadContent(GraphicsDevice graphicsDevice, ContentManager content) {
        
    }

    void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime time);
}