using LiteNetLib;

namespace Future.Common.csharp.network; 

public class ServerManager : NetworkManager {

    public ServerManager(NetworkHandler networkHandler) : base(networkHandler) {
        
    }

    public override void Start(string address, int port) {
        this.Network.Start(port); // Port: 9050
        this._networkHandler.Reload();

        this._listener.ConnectionRequestEvent += request => {
            if (this.Network.GetPeersCount(ConnectionState.Connected) > 10) { // 10 MAX PLAYERS
                request.AcceptIfKey("SomeConnectionKey");
            }
            else {
                request.Reject();
            }
        };

        while (!Console.KeyAvailable) {
            this.Network.PollEvents();
            Thread.Sleep(15);
        }
    }
}