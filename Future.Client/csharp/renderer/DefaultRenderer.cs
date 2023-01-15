using FontStashSharp;
using Future.Client.csharp.camera;
using Liru3D.Animations;
using Liru3D.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Future.Client.csharp.renderer;

public abstract class DefaultRenderer : IRenderer {
    
    protected Camera Camera { get; private set; }
    
    public bool Visible;

    protected DefaultRenderer() {
        this.Visible = true;
    }

    public virtual void Initialize(GraphicsDevice graphicsDevice, GameWindow window, Camera camera) {
        this.Camera = camera;
        window.KeyDown += (obj, input) => this.KeyToggle(input.Key, true);
        window.KeyUp += (obj, input) => this.KeyToggle(input.Key, false);
    }

    public virtual void LoadContent(GraphicsDevice graphicsDevice, ContentManager content) {
        
    }

    public void Draw(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, GameTime time) {
        if (this.Visible) {
            // MODELS
            this.EnableDepth(graphicsDevice);
            this.DrawInWorld(graphicsDevice, spriteBatch, this.Camera.View, this.Camera.Projection, time);
            this.DisableDepth(graphicsDevice);

            // OVERLAYS, SCREENS...
            this.DrawOnScreen(graphicsDevice, spriteBatch, this.Camera.View, this.Camera.Projection, time);

            // ANIMATIONS
            this.Anim(graphicsDevice, spriteBatch, this.Camera.View, this.Camera.Projection, time);
        }
    }
    
    protected void DrawModel(Model model, Texture2D texture, Matrix world, Matrix view, Matrix projection) {
        foreach (ModelMesh mesh in model.Meshes) {
            if (this.GetContainingBoundingFrustumMesh(mesh) != ContainmentType.Disjoint) {
                foreach (BasicEffect effect in mesh.Effects) {
                    effect.Texture = texture;
                    effect.TextureEnabled = true;
                    effect.World = world;
                    effect.View = view;
                    effect.Projection = projection;
                }
                
                mesh.Draw();
            }
        }
    }
    
    protected void DrawSkinnedModel(SkinnedModel model, AnimationPlayer animationPlayer, SkinnedEffect effect) {
        foreach (SkinnedMesh mesh in model.Meshes) {
            if (this.GetContainingBoundingFrustumSkinnedMesh(mesh) != ContainmentType.Disjoint) {
                animationPlayer.SetEffectBones(effect);
                effect.CurrentTechnique.Passes[0].Apply();
                mesh.Draw();
            }
        }
    }
    
    protected void DrawFont(DynamicSpriteFont font, SpriteBatch spriteBatch, Vector2 position, Color color, string text, Vector2? scale = null) {
        spriteBatch.DrawString(font, text, new Vector2(position.X, (position.Y + (2 * scale == null ? 1 : scale.Value.Y))), Color.Multiply(color, 0.2F), scale);
        spriteBatch.DrawString(font, text, position, color, scale);
    }


    protected virtual void DrawInWorld(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Matrix view, Matrix projection, GameTime time) {
        
    }

    protected virtual void DrawOnScreen(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Matrix view, Matrix projection, GameTime time) {

    }

    protected virtual void Anim(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Matrix view, Matrix projection, GameTime time) {
        
    }
    
    protected virtual void KeyToggle(Keys key, bool down) {
        
    }

    protected void EnableDepth(GraphicsDevice graphicsDevice) {
        graphicsDevice.DepthStencilState = DepthStencilState.Default;
    }

    protected void DisableDepth(GraphicsDevice graphicsDevice) {
        graphicsDevice.DepthStencilState = DepthStencilState.None;
    }

    protected void DefaultBegin(SpriteBatch spriteBatch, Matrix? view = null, Effect effect = null) {
        spriteBatch.Begin(transformMatrix: view, samplerState: SamplerState.PointClamp, effect: effect);
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

    protected ContainmentType GetContainingBoundingFrustumMesh(ModelMesh mesh) {
        return this.Camera.GetBoundingFrustum().Contains(mesh.BoundingSphere);
    }
    
    protected ContainmentType GetContainingBoundingFrustumSkinnedMesh(SkinnedMesh mesh) {
        return this.Camera.GetBoundingFrustum().Contains(mesh.BoundingSphere);
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
}