using Easel.Core;
using Riptide;

namespace Stellution.Server.csharp.network.handlers;

public class AnimationHandler {
    [MessageHandler(1)]
    private static void MessageHandler(ushort fromClientId, Message message) {
        var animation = message.GetString();
        //StellutionServer.NetworkManager.SendToAll(message, fromClientId);
        Logger.Info("Client with the id: " + fromClientId + "play the animation: " + animation);
    }
}