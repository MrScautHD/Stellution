using FontStashSharp;
using Future.Client.csharp.registry.types;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Future.Client.csharp.renderer.overlay; 

public class DebugOverlay : DefaultRenderer {

    private int _lineDistance;

    public DebugOverlay() {
        this.Visible = false;
    }

    protected override void DrawOnScreen(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, Matrix view, Matrix projection, GameTime time) {
        base.DrawOnScreen(graphicsDevice, spriteBatch, view, projection, time);

        this.DrawDebugInfo(graphicsDevice, spriteBatch, "FPS: " + this.FpsCalculator(time));
        this.DrawDebugInfo(graphicsDevice, spriteBatch, "System Info: " + graphicsDevice.Adapter.Description);

        // RESET LINE DISTANCE
        this._lineDistance = 0;
    }

    private void DrawDebugInfo(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, string debugInfo) {
        this.DefaultBegin(spriteBatch, graphicsDevice, RasterizerState.CullCounterClockwise);
        this.DrawFont(FontRegistry.Fontoe.GetFont(20), spriteBatch, new Vector2(5, (this._lineDistance * 20) + 5), Color.White, debugInfo);
        this.DefaultEnd(spriteBatch, graphicsDevice);
        this._lineDistance += 1;
    }

    protected override void KeyToggle(Keys key, bool down) {
        if (key == Keys.F3 && !down) {
            this.Visible = !this.Visible;
        }
    }
}