using System;
using System.Collections.Generic;
using Future.Client.csharp.config;
using Future.Client.csharp.registry.types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.settings;

public class GraphicSettings {
    
    private GraphicsDeviceManager _graphics;
    private GameWindow _window;
    private GraphicsDevice _graphicsDevice;
    
    public int MultiSampling { get; private set; }

    public GraphicSettings(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice, GameWindow window) {
        this._graphics = graphics;
        this._window = window;
        this._graphicsDevice = graphicsDevice;
        this.ChangeAbleGraphicSettings();
    }

    private void ChangeAbleGraphicSettings() {
        this._graphics.GraphicsProfile = GraphicsProfile.HiDef;
        this._graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
        this._window.AllowUserResizing = true;

        GraphicConfig test = ClientConfigRegistry.GraphicConfig;
        
        //JsonNode jsonNode = ClientConfigRegistry.GraphicConfig.ReadJsonAsNode();

        this.MultiSampling = 32;
        this.SetWindowSize(1920, 1080);
        this._graphics.HardwareModeSwitch = false;
        this.SetVSync(false); // jsonNode["VSync"].GetValue<bool>()
        this._graphics.IsFullScreen = false; //jsonNode["FullScreen"].GetValue<bool>()

        this.Apply();
    }

    public void SetWindowSize(int width, int height) {
        this._graphics.PreferredBackBufferWidth = width;
        this._graphics.PreferredBackBufferHeight = height;
    }

    public KeyValuePair<int, int> GetWindowSize() {
        return new KeyValuePair<int, int>(this._graphics.PreferredBackBufferWidth, this._graphics.PreferredBackBufferHeight);
    }

    public int GetWidth() {
        return this._graphics.PreferredBackBufferWidth;
    }

    public int GetHeight() {
        return this._graphics.PreferredBackBufferHeight;
    }

    public void SetMaxFps(Game game, int frames) {
        game.TargetElapsedTime = TimeSpan.FromSeconds(1d / MathHelper.Min(60, frames));
        
        this._graphics.PreparingDeviceSettings += (sender, e) => {
            e.GraphicsDeviceInformation.PresentationParameters.PresentationInterval = PresentInterval.Two;
        };
    }

    public void SetMultiSamplingCount(int sampleCount) {
        this._graphicsDevice.PresentationParameters.MultiSampleCount = sampleCount; // 2, 4, 8, 16, 32
    }

    public int GetSampleCount() {
        return this._graphicsDevice.PresentationParameters.MultiSampleCount;
    }

    public void SetMultiSampling(bool sampling) {
        this._graphics.PreferMultiSampling = sampling;
    }

    public bool IsMultiSamplingEnabled() {
        return this._graphics.PreferMultiSampling;
    }

    public void SetVSync(bool vSync) {
        this._graphics.SynchronizeWithVerticalRetrace = vSync;
    }

    public bool IsVSyncEnabled() {
        return this._graphics.SynchronizeWithVerticalRetrace;
    }

    public void ToggleFullScreen() {
        this._graphics.ToggleFullScreen();
    }

    public bool IsFullScreen() {
        return this._graphics.IsFullScreen;
    }

    public void Apply() {
        this._graphics.ApplyChanges();
    }
}