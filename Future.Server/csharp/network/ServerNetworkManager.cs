using LiteNetLib;

namespace Future.Server.csharp.network; 

public class ServerNetworkManager {
    
    public NetManager NetManager { get; private set; }

    public EventBasedNetListener Listener { get; private set; }

    public ServerNetworkManager() {
        this.Listener = new EventBasedNetListener();
        this.NetManager = new NetManager(this.Listener);
    }
    
    public void Start(string address, string password, int port = 9050) {
        this.NetManager.Start(port); //TODO FIX THAT ADRESS

        //EVENT
        this.Listener.ConnectionRequestEvent += request => {
            if (this.NetManager.GetPeersCount(ConnectionState.Connected) > 10) { // 10 MAX PLAYERS
                request.AcceptIfKey(password);
            }
            else {
                request.Reject();
            }
        };

        while (!Console.KeyAvailable) {
            this.NetManager.PollEvents();
            Thread.Sleep(15);
        }
    }

    public void Stop() {
        this.NetManager.Stop();
    }
}