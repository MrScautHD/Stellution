using System;
using Liru3D.Animations;
using Liru3D.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.renderer;

public class DefaultRenderer : IRenderer {

    private BasicCamera _camera;
    private RenderTarget2D _renderTarget2D;
    
    public virtual void Initialize(GraphicsDevice graphicsDevice, GameWindow window) {
        this._camera = new BasicCamera(graphicsDevice, window);
        this._camera.Position = new Vector3(2, 10, 52);
        this._camera.LookAtDirection = Vector3.Forward;
    }

    public virtual void LoadContent(GraphicsDevice graphicsDevice, ContentManager content) {
        int width = this.GetDisplayMode(graphicsDevice).Width;
        int height = this.GetDisplayMode(graphicsDevice).Height;

        this._renderTarget2D = new RenderTarget2D(graphicsDevice, width, height);
    }

    public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime time) {
        this._camera.Update(time);
        
        this.DrawInWorld(graphicsDevice, spriteBatch, this._camera.View, this._camera.Projection, time);

        // OVERLAYS, SCREENS...
        this.DrawOnScreen(graphicsDevice, spriteBatch, this._camera.View, this._camera.Projection, time);
        
        // ANIMATIONS
        this.Anim(graphicsDevice, spriteBatch, this._camera.View, this._camera.Projection, time);
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

    protected void DefaultBegin(SpriteBatch spriteBatch, Matrix? view = null) {
        if (view != null) {
            spriteBatch.Begin(transformMatrix: view, samplerState: SamplerState.PointClamp, rasterizerState: RasterizerState.CullNone);
            return;
        }

        spriteBatch.Begin(samplerState: SamplerState.PointClamp, rasterizerState: RasterizerState.CullNone);
    }
    
    protected void DefaultEnd(SpriteBatch spriteBatch) {
        spriteBatch.End();
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

    public double FpsCalculator(GameTime gameTime) {
        return 1 / gameTime.ElapsedGameTime.TotalSeconds;
    }
    
    protected Matrix CreateMatrixPos(Vector3 pos) {
        return Matrix.CreateTranslation(pos);
    }

    protected Viewport GetDisplayMode(GraphicsDevice graphicsDevice) {
        return graphicsDevice.Viewport;
    }

    public BasicCamera GetCamera() {
        return this._camera;
    }

    public RenderTarget2D GetRenderTarget2D() {
        return this._renderTarget2D;
    }
}