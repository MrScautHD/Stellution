using Apos.Camera;
using Liru3D.Animations;
using Liru3D.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.renderer;

public class DefaultRenderer {

    private Camera _camera;
    private RenderTarget2D _renderTarget2D;
    
    public virtual void Initialize(GraphicsDevice graphicsDevice, GameWindow window) {
        IVirtualViewport defaultViewport = new DefaultViewport(graphicsDevice, window);
        this._camera = new Camera(defaultViewport);
    }

    public virtual void LoadContent(GraphicsDevice graphicsDevice, ContentManager content) {
        int width = this.GetDisplayMode(graphicsDevice).Width;
        int height = this.GetDisplayMode(graphicsDevice).Height;

        this._renderTarget2D = new RenderTarget2D(graphicsDevice, width, height);
    }

    public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime time) {
        this._camera.SetViewport();

        spriteBatch.Begin(transformMatrix: this._camera.GetView3D());
        this.DrawInWorld(graphicsDevice, spriteBatch, this._camera.GetView3D(), this._camera.GetProjection3D(1, 2), time);
        spriteBatch.End();
        
        this._camera.ResetViewport();

        // OVERLAYS, SCREENS...
        this.DrawOnScreen(graphicsDevice, spriteBatch, this._camera.GetView3D(), this._camera.GetProjection3D(), time);
        
        // ANIMATIONS
        this.Anim(graphicsDevice, spriteBatch, this._camera.GetView3D(), this._camera.GetProjection3D(), time);
    }
    
    protected void DrawModel(Model model, Texture2D texture, Matrix world, Matrix view, Matrix projection) {
        foreach (ModelMesh mesh in model.Meshes) {
            foreach (BasicEffect effect in mesh.Effects) {
                effect.Texture = texture;
                effect.TextureEnabled = true;
            }
        }
        
        model.Draw(world, view, projection);
    }
    
    protected void DrawSkinnedModel(SkinnedModel model, AnimationPlayer animationPlayer, SkinnedEffect effect) {
        foreach (SkinnedMesh mesh in model.Meshes) {
            animationPlayer.SetEffectBones(effect);
            effect.CurrentTechnique.Passes[0].Apply();
            mesh.Draw();
        }
    }

    protected virtual void DrawInWorld(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Matrix view, Matrix projection, GameTime time) {

    }

    protected virtual void DrawOnScreen(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Matrix view, Matrix projection, GameTime time) {

    }

    protected virtual void Anim(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Matrix view, Matrix projection, GameTime time) {
        
    }

    protected Model LoadModel(ContentManager content, string model) {
        return content.Load<Model>(model);
    }
    
    protected SkinnedModel LoadSkinnedModel(ContentManager content, string model) {
        return content.Load<SkinnedModel>(model);
    }

    protected Texture2D LoadTexture(ContentManager content, string texture) {
        return content.Load<Texture2D>(texture);
    }
    
    public void Begin2D(GraphicsDevice graphicsDevice, SpriteBatch sprite) {
        graphicsDevice.SetRenderTarget(this._renderTarget2D);
        graphicsDevice.Clear(Color.CornflowerBlue);
        sprite.Begin(samplerState: SamplerState.PointClamp, rasterizerState: RasterizerState.CullNone);
    }

    public void End2D(GraphicsDevice graphicsDevice, SpriteBatch sprite) {
        sprite.End();
        
        graphicsDevice.SetRenderTarget(null);
        graphicsDevice.Clear(Color.CornflowerBlue);
        sprite.Begin();
        sprite.Draw(this._renderTarget2D, new Vector2(0, 0), null, Color.White, 0.0F, Vector2.Zero, 1, SpriteEffects.None, 0.0F);
        sprite.End();
    }

    public double FpsCalculator(GameTime gameTime) {
        return 1 / gameTime.ElapsedGameTime.TotalSeconds;
    }
    
    public Matrix CreateMatrixPos(Vector3 pos) {
        return Matrix.CreateTranslation(pos);
    }

    public Viewport GetDisplayMode(GraphicsDevice graphicsDevice) {
        return graphicsDevice.Viewport;
    }

    public Camera GetCamera() {
        return this._camera;
    }

    public RenderTarget2D GetRenderTarget2D() {
        return this._renderTarget2D;
    }
}