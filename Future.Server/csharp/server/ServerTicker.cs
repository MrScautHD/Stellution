using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Future.Server.csharp.server;

public class ServerTicker {
    
    private GameTime _gameTime = new();
    private Stopwatch _gameTimer;
    
    private int _updateFrameLag;
    private long _previousTicks;
    
    private TimeSpan _accumulatedElapsedTime;
    
    private TimeSpan _targetElapsedTime = TimeSpan.FromTicks(166667L);
    private TimeSpan _maxElapsedTime = TimeSpan.FromMilliseconds(500.0);
    private TimeSpan _inactiveSleepTime = TimeSpan.FromSeconds(0.02);
    
    private double _timer;
    private readonly double _delay = 1.0 / 60.0;
    
    public TimeSpan InactiveSleepTime {
        get => this._inactiveSleepTime;
        set => this._inactiveSleepTime = !(value < TimeSpan.Zero) ? value : throw new ArgumentOutOfRangeException("The time must be positive.");
    }

    public TimeSpan TargetElapsedTime {
        get => this._targetElapsedTime;
        set {
            if (value <= TimeSpan.Zero) {
                throw new ArgumentOutOfRangeException("The time must be positive and non-zero.");
            }

            if (value > this._maxElapsedTime) {
                throw new ArgumentOutOfRangeException("The time can not be larger than MaxElapsedTime");
            }

            if (!(value != this._targetElapsedTime)) {
                return;
            }
            
            this._targetElapsedTime = value;
        }
    }

    protected void RunTick(bool isRuning) { 
        while (true) {
            do {
                if (this.InactiveSleepTime.TotalMilliseconds >= 1.0) {
                    Thread.Sleep((int) this.InactiveSleepTime.TotalMilliseconds);
                }
            
                if (this._gameTimer == null) {
                    this._gameTimer = new Stopwatch();
                    this._gameTimer.Start(); 
                }
          
                long ticks = this._gameTimer.Elapsed.Ticks;
                this._accumulatedElapsedTime += TimeSpan.FromTicks(ticks - this._previousTicks);
                this._previousTicks = ticks;

                if (!(this._accumulatedElapsedTime < this.TargetElapsedTime)) {
                    if (this._accumulatedElapsedTime > this._maxElapsedTime) {
                       this._accumulatedElapsedTime = this._maxElapsedTime;
                    }

                    this._gameTime.ElapsedGameTime = this.TargetElapsedTime;
      
                    int num = 0;
                    while (this._accumulatedElapsedTime >= this.TargetElapsedTime && isRuning) {
                        this._gameTime.TotalGameTime += this.TargetElapsedTime;
                        this._accumulatedElapsedTime -= this.TargetElapsedTime;
                        ++num;
          
                        // UPDATE
                        this.Update(this._gameTime);
          
                        // FIXED UPDATE
                        this._timer += this._gameTime.ElapsedGameTime.TotalSeconds;
        
                        if (this._timer >= this._delay) {
                            this.FixedUpdate(this._gameTime);
                            this._timer -= this._delay;
                        }
                    }
      
                    this._updateFrameLag += Math.Max(0, num - 1);
      
                    if (this._gameTime.IsRunningSlowly) {
                        if (this._updateFrameLag == 0) {
                            this._gameTime.IsRunningSlowly = false;
                        }
                    }
                    else if (this._updateFrameLag >= 5) {
                        this._gameTime.IsRunningSlowly = true;
                    }

                    if (num == 1 && this._updateFrameLag > 0) {
                        --this._updateFrameLag;
                    }

                    this._gameTime.ElapsedGameTime = TimeSpan.FromTicks(this.TargetElapsedTime.Ticks * (long) num);
                }
            }
        
            while ((this.TargetElapsedTime - this._accumulatedElapsedTime).TotalMilliseconds < 2.0);
            Thread.Sleep(1);
        }
    }

    // EVERY FRAME!
    protected virtual void Update(GameTime gameTime) {
        
    }

    // EVERY 60 TICKS PER SEC!
    protected virtual void FixedUpdate(GameTime gameTime) {
        
    }
}