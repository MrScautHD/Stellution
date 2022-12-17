using Future.Client.csharp.renderer.objects;
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
    
    // TICKER
    private double _timer;
    private readonly double _delay = 1.0 / 60.0;

    public FutureClient() {
        this._graphics = new GraphicsDeviceManager(this);
        this._graphicSettings = new GraphicSettings(this._graphics, this.GraphicsDevice, this.Window);
        
        // GAME PROPERTIES
        this.Content.RootDirectory = "content";
        this.IsFixedTimeStep = false;
        this.IsMouseVisible = true;
        
        // GRAPHIC
        this._graphicSettings.SetWindowSize(1920, 1080);
        this._graphicSettings.SetVSync(false);
        //this._graphicSettings.SetMultiSampling(true);
        
        this._graphicSettings.Apply();
        
        _graphics.PreferMultiSampling = true;
        _graphics.ApplyChanges();
    }

    protected override void Initialize() {
        base.Initialize();
        
        this._graphicSettings.SetMultiSamplingCount(8);
        //this._graphicSettings.Apply();
        
        GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
        _graphics.ApplyChanges();
    }

    protected override void LoadContent() {
        this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
    }

    protected override void Draw(GameTime gameTime) {
        this.GraphicsDevice.Clear(Color.CornflowerBlue);
    }

    /**
     * "Update" Method is not Limited.
     */
    protected override void Update(GameTime gameTime) {
        base.Update(gameTime);
        
        // FIXED UPDATE
        this.FixedTimeCalculator(gameTime);
    }

    private void FixedTimeCalculator(GameTime gameTime) {
        this._timer += gameTime.ElapsedGameTime.TotalSeconds;

        if (this._timer >= this._delay) {
            this.FixedUpdate(gameTime);
            this._timer -= this._delay;
        }
    }

    /**
     * Limit "FixedUpdate" Method to 60 FPS.
     */
    private void FixedUpdate(GameTime gameTime) {
        
    }
}