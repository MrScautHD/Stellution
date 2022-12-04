using Future.csharp.client.renderer.objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp;

public class FutureClient : Game {
    
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private StreetLightRenderer _renderer;
    
    private double _timer;
    private double _delay = 1.0 / 60.0;
    
    public FutureClient() {
        this._graphics = new GraphicsDeviceManager(this);
            
        this.Content.RootDirectory = "content";
        this.IsMouseVisible = true;
            
        this._graphics.PreferredBackBufferWidth = 1920;
        this._graphics.PreferredBackBufferHeight = 1080;
        
        //this._graphics.ToggleFullScreen();

        this._graphics.ApplyChanges();
        
        this._renderer = new StreetLightRenderer();
    }

    protected override void Initialize() {
        base.Initialize();
        
        // DEFAULT RENDERER
        this._renderer.Initialize(this.GraphicsDevice, this.Window);
    }

    protected override void LoadContent() {
        this._spriteBatch = new SpriteBatch(this.GraphicsDevice);

        // DEFAULT RENDERER
        this._renderer.LoadContent(this.GraphicsDevice, this.Content);
    }

    protected override void Draw(GameTime gameTime) {
        this.GraphicsDevice.Clear(Color.CornflowerBlue);
            
        // DEFAULT RENDERER
        this._renderer.Draw(this.GraphicsDevice, this._spriteBatch, gameTime);
    }
}