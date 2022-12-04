namespace Future.Common.csharp.network; 

public class ClientManager : NetworkManager {
    
    public ClientManager(NetworkHandler networkHandler) : base(networkHandler) {
        
    }

    public override void Start(string address, int port) { // address: "localhost" (host ip or name) port: 9050 
        this.Network.Start();
        this._networkHandler.Reload();
        this.Network.Connect(address, port, "SomeConnectionKey"); /* Text Key or NetDataWriter */

        this._listener.NetworkReceiveEvent += (peer, reader, channel, deliveryMethod) => {
            Console.WriteLine("We got: {0}", reader.GetBool()); // Max Length of string
            reader.Recycle();
        };

        while (!Console.KeyAvailable) {
            this.Network.PollEvents();
            Thread.Sleep(15);
        }
    }
}