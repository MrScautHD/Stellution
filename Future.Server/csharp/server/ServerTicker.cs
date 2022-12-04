using Microsoft.Xna.Framework;

namespace Future.Server.csharp.server;

public class ServerTicker {
    
    private GameTime _gameTime;
    private TimeSpan _gameTimer;

    private double _timer;
    private readonly double _delay = 1.0 / 60.0;

    public ServerTicker() {
        this._gameTime = new GameTime();
        this._gameTimer = this._gameTime.TotalGameTime;
    }

    protected void RunTick(bool isRuning) {
        while (isRuning) {
            this._gameTimer += this._gameTime.ElapsedGameTime;

            // UPDATE
            this.Update(this._gameTime);

            // FIXED UPDATE
            this._timer += this._gameTime.ElapsedGameTime.TotalSeconds;

            if (this._timer >= this._delay) {
                this.FixedUpdate(this._gameTime);
                this._timer -= this._delay;
            }
        }
    }

    // EVERY FRAME!
    protected virtual void Update(GameTime gameTime) {
        
    }

    // EVERY 60 TICKS PER SEC!
    protected virtual void FixedUpdate(GameTime gameTime) {
        Console.WriteLine(gameTime);
    }
}