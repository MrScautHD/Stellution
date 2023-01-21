using Future.Common.csharp.file;
using Future.Common.csharp.network;
using Future.Server.csharp.ticker;
using Microsoft.Xna.Framework;

namespace Future.Server.csharp;

public class FutureServer : ServerTicker {

    private NetworkHandler _network;
    private ServerManager _serverManager;

    public static Logger Logger = new("log", "logs.txt");

    public FutureServer() {
        this._network = new NetworkHandler();
        this._network.CreateNetwork(true);

        this._serverManager = (ServerManager) this._network.GetNetwork(true);
        this._serverManager.Start("localhost", 4090);
        
        Logger.Print("Server Started!", ConsoleColor.Green);
    }

    public void Run() {
        this.RunTick(this._serverManager.Network.IsRunning);
    }

    /**
     * The "Update" Method is limited to 60 FPS when "IsFixedTimeStep" (default) true.
     */
    protected override void Update(GameTime gameTime) {
        
    }
}