using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Future.Server.csharp.ticker;

public class ServerTicker {
    
    private GameTime _gameTime = new();
    private Stopwatch _gameTimer;
    
    /**
     * Limit "Update" Method to 60 FPS if true, else is it unlimited.
     */
    public bool IsFixedTimeStep = true;

    private int _updateFrameLag;
    private long _previousTicks;
    
    private TimeSpan _accumulatedElapsedTime;
    
    private readonly TimeSpan _targetElapsedTime = TimeSpan.FromTicks(166667L);
    private readonly TimeSpan _maxElapsedTime = TimeSpan.FromMilliseconds(500.0);
    private readonly TimeSpan _inactiveSleepTime = TimeSpan.FromSeconds(0.02);
    
    private double _timer;
    private readonly double _delay = 1.0 / 60.0;

    protected void RunTick(bool isRunning) {
        while (true) {
            do {
                if (this._inactiveSleepTime.TotalMilliseconds >= 1.0) {
                    Thread.Sleep((int) this._inactiveSleepTime.TotalMilliseconds);
                }

                if (this._gameTimer == null) {
                    this._gameTimer = new Stopwatch();
                    this._gameTimer.Start();
                }

                long ticks = this._gameTimer.Elapsed.Ticks;
                this._accumulatedElapsedTime += TimeSpan.FromTicks(ticks - this._previousTicks);
                this._previousTicks = ticks;

                if (!this.IsFixedTimeStep || !(this._accumulatedElapsedTime < this._targetElapsedTime)) {
                    if (this._accumulatedElapsedTime > this._maxElapsedTime) {
                        this._accumulatedElapsedTime = this._maxElapsedTime;
                    }

                    if (this.IsFixedTimeStep) {
                        this._gameTime.ElapsedGameTime = this._targetElapsedTime;

                        int num = 0;
                        while (this._accumulatedElapsedTime >= this._targetElapsedTime && isRunning) {
                            this._gameTime.TotalGameTime += this._targetElapsedTime;
                            this._accumulatedElapsedTime -= this._targetElapsedTime;
                            ++num;

                            // FIXED UPDATE
                            this.Update(this._gameTime);
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

                        this._gameTime.ElapsedGameTime = TimeSpan.FromTicks(this._targetElapsedTime.Ticks * (long)num);
                    }
                    else {
                        this._gameTime.ElapsedGameTime = this._accumulatedElapsedTime;
                        this._gameTime.TotalGameTime += this._accumulatedElapsedTime;
                        this._accumulatedElapsedTime = TimeSpan.Zero;
                        
                        // UPDATE
                        this.Update(this._gameTime);
                    }
                }
            }
            
            while ((this._targetElapsedTime - this._accumulatedElapsedTime).TotalMilliseconds < 2.0);
            Thread.Sleep(1);
        }
    }
    
    protected virtual void Update(GameTime gameTime) {
        
    }
}