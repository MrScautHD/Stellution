using LiteNetLib.Utils;

namespace Future.Common.csharp.network.packets; 

public struct IsServerPacket : INetSerializable {
    public bool IsServer;

    public void Serialize(NetDataWriter writer) {
        writer.Put(this.IsServer);
    }

    public void Deserialize(NetDataReader reader) {
        this.IsServer = reader.GetBool();
    }
}