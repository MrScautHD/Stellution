using Easel.Core;
using Riptide.Utils;

namespace Stellution.Client.csharp.network; 

public class ClientNetworkManager {

    public Riptide.Client Client { get; private set; }

    public ClientNetworkManager() {
        RiptideLogger.Initialize(Logger.Debug, Logger.Info, Logger.Warn, Logger.Error, false);
    }

    public void FixedUpdate() {
        this.Client.Update();
    }

    public void Connect(string hostAddress) {
        this.Client = new Riptide.Client();
        this.Client.Connect(hostAddress);
    }

    public void Disconnect() {
        if (this.Client.IsConnected) {
            this.Client.Disconnect();
        }
    }
}