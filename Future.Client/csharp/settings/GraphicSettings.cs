using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.settings; 

public class GraphicSettings {
    
    private GraphicsDeviceManager _graphics;
    private GameWindow _window;
    private GraphicsDevice _graphicsDevice;

    public GraphicSettings(GraphicsDeviceManager graphics, GraphicsDevice graphicsDevice, GameWindow window) {
        this._graphics = graphics;
        this._window = window;
        this._graphicsDevice = graphicsDevice;
        this._graphics.GraphicsProfile = GraphicsProfile.HiDef;
        this._window.AllowUserResizing = true;
        this.SetupDefaultSettingsOrSaved();
    }

    // ADD THE SAVE PART
    private void SetupDefaultSettingsOrSaved() {

        // GRAPHIC
        this.SetWindowSize(1920, 1080);
        this.SetVSync(false);

        this.Apply();

        //this._graphics.PreferMultiSampling = true;
        this._graphics.ApplyChanges();
        
        //this._graphicSettings.SetMultiSampling(true);
        //this._graphicSettings.SetMultiSamplingCount(8);
        //this._graphicSettings.Apply();
        
        //this.GraphicsDevice.PresentationParameters.MultiSampleCount = 8;
        //this._graphics.ApplyChanges();
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