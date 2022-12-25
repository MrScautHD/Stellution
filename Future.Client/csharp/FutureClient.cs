using Future.Client.csharp.registry;
using Future.Client.csharp.registry.types;
using Future.Client.csharp.settings;
using Future.Common.csharp.log;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp;

public class FutureClient : Game {
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private GraphicSettings _graphicSettings;
    
    public static Logger Logger = new Logger();

    public FutureClient() {
        this._graphics = new GraphicsDeviceManager(this);
        this._graphicSettings = new GraphicSettings(this._graphics, this.GraphicsDevice, this.Window);

        // GAME PROPERTIES
        this.Content.RootDirectory = "content";
        this.IsFixedTimeStep = false;
        this.IsMouseVisible = true;
        
        // REGISTRY
        ClientRegistry.Registries.Add(new DrawRegistry());
        ClientRegistry.Registries.Add(new FontClientRegistry());
        ClientRegistry.Registries.Add(new TickerRegistry());
    }

    protected override void Initialize() {
        base.Initialize();
        
        // INIT REGISTRY
        foreach (ClientRegistry registry in ClientRegistry.Registries) {
            registry.Initialize(this.GraphicsDevice, this.Window);
        }
    }

    protected override void LoadContent() {
        this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
        
        // LOAD REGISTRY
        foreach (ClientRegistry registry in ClientRegistry.Registries) {
            registry.LoadContent(this.GraphicsDevice, this.Content);
        }
    }

    protected override void Draw(GameTime gameTime) {
        this.GraphicsDevice.Clear(Color.CornflowerBlue);
        
        // DRAW REGISTRY
        foreach (ClientRegistry registry in ClientRegistry.Registries) {
            registry.Draw(this.GraphicsDevice, this._spriteBatch, gameTime);
        }
    }
    
    protected override void Update(GameTime gameTime) {
        base.Update(gameTime);

        // UPDATE REGISTRY
        foreach (ClientRegistry registry in ClientRegistry.Registries) {
            registry.Update(gameTime);
        }
    }
}