using Easel.Core;
using Riptide.Utils;

namespace Stellution.Client.csharp.network; 

public class ClientNetworkManager : Riptide.Client {
    
    public ClientNetworkManager() {
        RiptideLogger.Initialize(Logger.Debug, Logger.Info, Logger.Warn, Logger.Error, false);
    }
}