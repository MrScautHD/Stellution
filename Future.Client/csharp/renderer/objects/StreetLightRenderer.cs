using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.renderer.objects;

public class StreetLightRenderer : DefaultRenderer {

    private Model _model;
    private Texture2D _texture;

    public override void LoadContent(GraphicsDevice graphicsDevice, ContentManager content) {
        base.LoadContent(graphicsDevice, content);

        this._model = this.LoadModel(content, "models/objects/street_light");
        this._texture = this.LoadTexture(content, "textures/objects/street_light");
    }

    protected override void DrawInWorld(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Matrix view, Matrix projection, GameTime time) {
        Matrix world = this.CreateMatrixPos(new Vector3(0, 0, 0));

        //this.DefaultBegin(spriteBatch, RasterizerState.CullNone, view);
        spriteBatch.Begin();
        this.DrawModel(this._model, this._texture, world, view, projection);
        spriteBatch.End();
        //this.DefaultEnd(spriteBatch);
    }
}
