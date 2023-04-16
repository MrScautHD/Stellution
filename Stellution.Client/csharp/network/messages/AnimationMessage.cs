using Riptide;

namespace Stellution.Client.csharp.network.messages; 

public class AnimationMessage {

    public void SendMessage(string animation) {
        Message message = Message.Create(MessageSendMode.Unreliable, 1);
        message.AddString(animation);
        
        StellutionClient.NetworkManager.Send(message);
    }
}