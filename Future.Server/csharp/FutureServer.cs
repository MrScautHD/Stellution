using Future.Common.csharp.network;
using Future.Server.csharp.server;

namespace Future.Server.csharp; 

public class FutureServer : ServerTicker {

    private NetworkHandler _network;
    private ServerManager _serverManager;

    public FutureServer() {
        this._network = new NetworkHandler();
        this._network.CreateNetwork(true);

        this._serverManager = (ServerManager) this._network.GetNetwork(true);
        this._serverManager.Start("localhost", 4090);
        
        Console.WriteLine("Server Started!");
    }

    public void Run() {
        this.RunTick(this._serverManager.Network.IsRunning);
    }
}