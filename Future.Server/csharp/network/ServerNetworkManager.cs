using Easel.Core;
using Riptide.Utils;

namespace Future.Server.csharp.network; 

public class ServerNetworkManager {

    private Riptide.Server _server;

    public ServerNetworkManager() {
        this._server = new Riptide.Server();
        RiptideLogger.Initialize(Logger.Debug, Logger.Info, Logger.Warn, Logger.Error, false);
    }

    public void FixedUpdate() {
        this._server.Update();
    }

    public void Start(ushort port, ushort maxClientCount) {
        this._server.Start(port, maxClientCount);
    }

    public void Stop() {
        if (this._server.IsRunning) {
            this._server.Stop();
        }
    }
}