using Future.Client.csharp.camera;
using Future.Client.csharp.registry;
using Future.Client.csharp.registry.types;
using Future.Client.csharp.settings;
using Future.Common.csharp.log;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp;

public class FutureClient : Game {
    
    private GraphicsDeviceManager _graphicDeviceManager;
    private SpriteBatch _spriteBatch;
    private RenderTarget2D _renderTarget2D;
    private GraphicSettings _graphicSettings;
    
    private readonly Camera _camera;
    
    public static Logger Logger = new Logger();

    public FutureClient() {
        this._graphicDeviceManager = new GraphicsDeviceManager(this);
        this._graphicSettings = new GraphicSettings(this._graphicDeviceManager, this.GraphicsDevice, this.Window);
        this._camera = new Camera(this.GraphicsDevice, 80, Vector3.Zero, Vector3.Forward);

        // GAME PROPERTIES
        this.Content.RootDirectory = "content";
        this.IsFixedTimeStep = false;
        this.IsMouseVisible = true;
        
        // REGISTRY
        IClientRegistry.Registries.Add(new DrawRegistry());
        IClientRegistry.Registries.Add(new FontRegistry());
        IClientRegistry.Registries.Add(new SoundRegistry());
        IClientRegistry.Registries.Add(new TickerRegistry());
    }

    protected override void Initialize() {
        base.Initialize();

        this._renderTarget2D = new RenderTarget2D(this.GraphicsDevice, 1920, 1080, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        
        // INIT REGISTRY
        foreach (IClientRegistry registry in IClientRegistry.Registries) {
            registry.Initialize(this.GraphicsDevice, this.Window, this._camera);
        }
    }

    protected override void LoadContent() {
        this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
        
        // LOAD REGISTRY
        foreach (IClientRegistry registry in IClientRegistry.Registries) {
            registry.LoadContent(this.GraphicsDevice, this.Content);
        }
    }

    protected override void Draw(GameTime gameTime) {
        this.GraphicsDevice.SetRenderTarget(this._renderTarget2D);
        this.GraphicsDevice.Clear(Color.CornflowerBlue);
        
        // DRAW REGISTRY
        foreach (IClientRegistry registry in IClientRegistry.Registries) {
            registry.Draw(this.GraphicsDevice, this._spriteBatch, gameTime);
        }

        this.GraphicsDevice.SetRenderTarget(null);
        this.GraphicsDevice.Clear(Color.CornflowerBlue);
        
        this._spriteBatch.Begin(samplerState: SamplerState.PointClamp, rasterizerState: RasterizerState.CullCounterClockwise);
        this._spriteBatch.Draw(this._renderTarget2D, new Vector2(0, 0), null, Color.White, 0.0F, Vector2.Zero, 1, SpriteEffects.None, 1.0F);
        this._spriteBatch.End();
    }
    
    protected override void Update(GameTime gameTime) {
        base.Update(gameTime);

        // UPDATE REGISTRY
        foreach (IClientRegistry registry in IClientRegistry.Registries) {
            registry.Update(gameTime);
        }
    }
}