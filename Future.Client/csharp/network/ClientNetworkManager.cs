using System;
using System.Threading;
using LiteNetLib;

namespace Future.Client.csharp.network; 

public class ClientNetworkManager {

    private NetManager _netManager;

    private EventBasedNetListener _listener;
    
    public void Connect(string address, int port) { // address: "localhost" (host ip or name) port: 9050 
        this._netManager.Start();
        this._netManager.Connect(address, port, "SomeConnectionKey");

        // EVENT
        this._listener.NetworkReceiveEvent += (peer, reader, channel, deliveryMethod) => {
            Console.WriteLine("We got: {0}", reader.GetBool()); // Max Length of string
            reader.Recycle();
        };

        while (!Console.KeyAvailable) {
            this._netManager.PollEvents();
            Thread.Sleep(15);
        }
    }

    public void Disconnect() {
        this._netManager.Stop();
    }
}