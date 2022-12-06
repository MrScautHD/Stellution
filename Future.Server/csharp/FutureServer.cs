using Future.Common.csharp.log;
using Future.Common.csharp.network;
using Future.Server.csharp.ticker;
using Microsoft.Xna.Framework;

namespace Future.Server.csharp; 

public class FutureServer : ServerTicker {

    private NetworkHandler _network;
    private ServerManager _serverManager;

    public static Logger Logger = new Logger();

    public FutureServer() {
        this._network = new NetworkHandler();
        this._network.CreateNetwork(true);

        this._serverManager = (ServerManager) this._network.GetNetwork(true);
        this._serverManager.Start("localhost", 4090);
        
        Logger.Print("Server Started!");
    }

    public void Run() {
        this.RunTick(this._serverManager.Network.IsRunning);
    }

    protected override void Update(GameTime gameTime) {
        Logger.Print("Update");
    }

    protected override void FixedUpdate(GameTime gameTime) {
        Logger.Print("FixedUpdate");
    }
}