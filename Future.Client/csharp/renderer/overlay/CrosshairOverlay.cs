using Future.Client.csharp.registry.types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.renderer.overlay;

public class CrosshairOverlay : DefaultRenderer {

    private Texture2D _texture;
    
    public override void LoadContent(GraphicsDevice graphicsDevice, ContentManager content) {
        base.LoadContent(graphicsDevice, content);
        this._texture = this.LoadTexture(content, "textures/overlay/crosshair");
    }

    protected override void DrawOnScreen(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Matrix view, Matrix projection, GameTime time) {
        base.DrawOnScreen(graphicsDevice, spriteBatch, view, projection, time);
        
        //this.Begin2D(graphicsDevice, spriteBatch);
        spriteBatch.DrawString(FontRegistry.Fontoe, "Test", new Vector2(100, 100), Color.White);
        //this.End2D(graphicsDevice, spriteBatch);
        
        int width = 30;
        int height = 30;

        int x = (this.GetDisplayMode(graphicsDevice).Width / 2) - (width / 2);
        int y = (this.GetDisplayMode(graphicsDevice).Height / 2) - (height / 2);

        //this.Begin2D(graphicsDevice, spriteBatch);
        spriteBatch.Draw(this._texture, new Rectangle(x, y, width, height), Color.White);
        //this.End2D(graphicsDevice, spriteBatch);
    }
}