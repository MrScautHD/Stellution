using Future.Client.csharp.ticker;
using Microsoft.Xna.Framework;

namespace Future.Client.csharp.registry.types; 

public class TickerRegistry : ClientRegistry {

    public static readonly ClientTicker ClientTicker = Register("client_ticker");

    private static ClientTicker Register(string name) {
        ClientTicker ticker = new ClientTicker();
        
        RegistryTypes.Ticker.Add(name, ticker);
        return ticker;
    }
    
    public override void Update(GameTime time) {
        foreach (ClientTicker ticker in RegistryTypes.Ticker.Values) {
            ticker.Update(time);
        }
    }
}