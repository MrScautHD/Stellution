using System.Net;
using System.Net.Sockets;
using Easel.Core;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Future.Server.csharp.network; 

public class ServerNetworkManager : INetEventListener {

    private NetManager _server;

    private EventBasedNetListener _listener;
    private NetPeer _ourPeer;
    private NetDataWriter _dataWriter;
    private string _connectionKey;

    public void Start(int port, string connectionKey = "") { // address: "localhost" (host ip or name) port: 9050
        this._connectionKey = connectionKey;
        this._dataWriter = new NetDataWriter();
        this._server = new NetManager(this) {
            BroadcastReceiveEnabled = true,
            UpdateTime = 15
        };
        this._server.Start(port);
    }

    public void Update() {
        //this._server.PollEvents();
        
        if (this._ourPeer != null) {
            //_serverBall.transform.Translate(1f * Time.fixedDeltaTime, 0f, 0f);
            _dataWriter.Reset();
            //_dataWriter.Put(_serverBall.transform.position.x);
            _ourPeer.Send(_dataWriter, DeliveryMethod.Sequenced);
        }
    }

    public void Stop() {
        this._server.Stop();
    }

    public void OnPeerConnected(NetPeer peer) {
        Logger.Info("[SERVER] We have new peer " + peer.EndPoint);
        this._ourPeer = peer;
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketErrorCode) {
        Logger.Error("[SERVER] error " + socketErrorCode);
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber, DeliveryMethod deliveryMethod) {
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader, UnconnectedMessageType messageType) {
        if (messageType == UnconnectedMessageType.Broadcast) {
            Logger.Error("[SERVER] Received discovery request. Send discovery response");
            NetDataWriter dataWriter = new NetDataWriter();
            dataWriter.Put(1);
            
            this._server.SendUnconnectedMessage(dataWriter, remoteEndPoint);
        }
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency) {
    }

    public void OnConnectionRequest(ConnectionRequest request) {
        request.AcceptIfKey(this._connectionKey);
    }

    public void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo) {
        Logger.Info("[SERVER] peer disconnected " + peer.EndPoint + ", info: " + disconnectInfo.Reason);
        if (peer == this._ourPeer) {
            this._ourPeer = null;
        }
    }
}