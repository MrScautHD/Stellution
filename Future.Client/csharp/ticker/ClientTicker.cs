using Microsoft.Xna.Framework;

namespace Future.Client.csharp.ticker; 

public class ClientTicker : IClientTicker {
    
    private double _timer;
    private readonly double _delay = 1.0 / 60.0;
    
    public virtual void Update(GameTime gameTime) {
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
    protected virtual void FixedUpdate(GameTime gameTime) {
        
    }
}