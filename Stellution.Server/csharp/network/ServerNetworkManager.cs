using Easel.Core;
using Riptide.Utils;

namespace Stellution.Server.csharp.network; 

public class ServerNetworkManager {

    public Riptide.Server Server { get; private set; }

    public ServerNetworkManager() {
        this.Server = new Riptide.Server();
        RiptideLogger.Initialize(Logger.Debug, Logger.Info, Logger.Warn, Logger.Error, false);
    }

    public void FixedUpdate() {
        this.Server.Update();
    }

    public void Start(ushort port, ushort maxClientCount) {
        this.Server.Start(port, maxClientCount);
    }

    public void Stop() {
        if (this.Server.IsRunning) {
            this.Server.Stop();
        }
    }
}