using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Future.Server.csharp.server;

public class ServerTicker {
    
    private GameTime _gameTime;
    
    private int _updateFrameLag;

    private long _previousTicks;
    
    private Stopwatch _gameTimer;
    
    private TimeSpan _accumulatedElapsedTime;
    private TimeSpan _maxElapsedTime = TimeSpan.FromMilliseconds(500.0);
    public TimeSpan TargetElapsedTime;

    private double _timer;
    private readonly double _delay = 1.0 / 60.0;

    public ServerTicker() {
        this._gameTime = new GameTime();
    }

    protected void RunTick(bool isRuning) {
        /*while (isRuning) {
            //this._gameTimer += this._gameTime.ElapsedGameTime;

            // UPDATE
            this.Update(this._gameTime);

            // FIXED UPDATE
            this._timer += this._gameTime.ElapsedGameTime.TotalSeconds;

            if (this._timer >= this._delay) {
                this.FixedUpdate(this._gameTime);
                this._timer -= this._delay;
            }
        }*/
        
        this.SetupGameTime(isRuning);
    }

    private void SetupGameTime(bool isRuning) {
        while (isRuning) {
            this._gameTime.ElapsedGameTime = _gameTime.ElapsedGameTime;
        }

        //this._gameTime.ElapsedGameTime = TimeSpan.FromTicks(this.TargetElapsedTime.Ticks * (long) num);
    }

    // EVERY FRAME!
    protected virtual void Update(GameTime gameTime) {
        Console.WriteLine(gameTime);
    }

    // EVERY 60 TICKS PER SEC!
    protected virtual void FixedUpdate(GameTime gameTime) {
        
    }
}