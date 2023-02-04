using Future.Common.csharp.file;
using Future.Common.csharp.network;
using Future.Server.csharp.registry;
using Future.Server.csharp.registry.types;
using Future.Server.csharp.ticker;
using Microsoft.Xna.Framework;

namespace Future.Server.csharp;

public class FutureServer : ServerTicker {

    private NetworkHandler _network;
    private ServerManager _serverManager;

    public FutureServer() {
        this._network = new NetworkHandler();
        this._network.CreateNetwork(true);

        this._serverManager = (ServerManager) this._network.GetNetwork(true);
        this._serverManager.Start("localhost", 4090);
        
        Logger.Log.Print("Server Started!", ConsoleColor.Green);
        
        // REGISTRY
        IRegistry.Registries.Add(new ConfigRegistry());
        IRegistry.Registries.Add(new TickerRegistry());
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