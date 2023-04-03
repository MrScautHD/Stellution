using Easel.Core;
using Riptide.Utils;

namespace Stellution.Client.csharp.network; 

public class ClientNetworkManager {

    private Riptide.Client _client;

    public ClientNetworkManager() {
        RiptideLogger.Initialize(Logger.Debug, Logger.Info, Logger.Warn, Logger.Error, false);
    }

    public void FixedUpdate() {
        this._client.Update();
    }

    public void Connect(string hostAddress) {
        this._client = new Riptide.Client();
        this._client.Connect(hostAddress);
    }

    public void Disconnect() {
        if (this._client.IsConnected) {
            this._client.Disconnect();
        }
    }
}