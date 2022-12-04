using LiteNetLib;

namespace Future.Common.csharp.network; 

public class NetworkManager {
    
    public NetManager Network { get; private set; }

    protected EventBasedNetListener _listener;
    
    protected NetworkHandler _networkHandler;
    
    public NetworkManager(NetworkHandler networkHandler) {
        this._listener = new EventBasedNetListener();
        this.Network = new NetManager(this._listener);
        this._networkHandler = networkHandler;
    }
    
    public virtual void Start(string address, int port) {
        
    }

    public void Stop() {
        this.Network.Stop();
        this._networkHandler.Reload();
    }
}