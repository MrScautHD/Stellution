using Future.Common.csharp.network;
using Microsoft.Xna.Framework;

namespace Future.Server.csharp; 

public class FutureServer {

    private NetworkHandler _network;
    private ServerManager _serverManager;
    
    private double _timer;
    private readonly double _delay = 1.0 / 60.0;
    
    public FutureServer() {

        // START SERVER
        Console.WriteLine("Press any KEY to start the Server!");
        //Console.ReadKey(true);
        
        this._network = new NetworkHandler();
        this._network.CreateNetwork(true);

        this._serverManager = (ServerManager) this._network.GetNetwork(true);
        this._serverManager.Start("localhost", 4090);
        
        Console.WriteLine("Server Started!");
    }

    public void Run() {
        GameTime gameTime = new GameTime();
        
        while (this._serverManager.Network.IsRunning) {
            // UPDATE
            this.Update(gameTime);
        }
    }

    // EVERY FRAME!
    private void Update(GameTime gameTime) {

        // FIXED UPDATE
        this._timer += gameTime.ElapsedGameTime.TotalSeconds;
        
        Console.WriteLine(this._timer);

        if (this._timer >= this._delay) {
            this.FixedUpdate(gameTime);
            this._timer -= this._delay;
        }
    }

    // EVERY 60 TICKS PER SEC!
    private void FixedUpdate(GameTime gameTime) {
        Console.WriteLine("test");
    }
}