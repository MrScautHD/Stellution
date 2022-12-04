namespace Future.Common.csharp.network; 

public class NetworkHandler : DistHelper {

    private static ServerManager _server;
    private static ClientManager _client;

    public void CreateNetwork(bool isServer) {
        if (isServer && _server == null) {
            _server = new ServerManager(this);
        }
        else if (!isServer && _client == null) {
            _client = new ClientManager(this);
        }
        
        this.Reload();
    }

    public void Reload() {
        
        // HOST
        if ((_client != null && _client.Network.IsRunning) && (_server != null && _server.Network.IsRunning)) {
            this.SetHost();
            return;
        }

        // SERVER
        if (_server != null && _server.Network.IsRunning) {
            this.ServerSide = true;
        }
        // CLIENT
        else {
            this.ClientSide = true;
        }
    }

    public NetworkManager GetNetwork(bool isServer) {
        return isServer ? _server : _client;
    }
}