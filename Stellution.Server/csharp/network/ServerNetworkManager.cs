using Easel.Core;
using Riptide.Utils;

namespace Stellution.Server.csharp.network; 

public class ServerNetworkManager : Riptide.Server {
    
    public ServerNetworkManager() {
        RiptideLogger.Initialize(Logger.Debug, Logger.Info, Logger.Warn, Logger.Error, false);
    }
}