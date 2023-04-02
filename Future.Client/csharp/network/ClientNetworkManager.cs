using System.Net;
using System.Net.Sockets;
using Easel.Core;
using LiteNetLib;

namespace Future.Client.csharp.network; 

public class ClientNetworkManager : INetEventListener {

    private NetManager _client;

    private EventBasedNetListener _listener;
    
    public void Connect(string address, int port, string connectionKey = "") { // address: "localhost" (host ip or name) port: 9050
        this._client = new NetManager(this) {
            UnconnectedMessagesEnabled = true,
            UpdateTime = 15
        };
        
        this._client.Start();
        this._client.Connect(address, port, connectionKey);
    }

    public void Disconnect() {
        this._client.Stop();
    }

    public void OnPeerConnected(NetPeer peer) {
        Logger.Info("[CLIENT] We connected to " + peer.EndPoint);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
        Logger.Info("[CLIENT] We disconnected because " + disconnectInfo.Reason);
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError) {
        Logger.Error("[CLIENT] We received error " + socketError);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod) {
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) {
        if (messageType == UnconnectedMessageType.BasicMessage && this._client.ConnectedPeersCount == 0 && reader.GetInt() == 1) {
            Logger.Info("[CLIENT] Received discovery response. Connecting to: " + remoteEndPoint);
            this._client.Connect(remoteEndPoint, "sample_app");
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency) {
    }

    public void OnConnectionRequest(ConnectionRequest request) {
    }
}