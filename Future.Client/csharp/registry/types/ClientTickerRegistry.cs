using Future.Client.csharp.ticker;
using Microsoft.Xna.Framework;

namespace Future.Client.csharp.registry.types; 

public class ClientTickerRegistry : IClientRegistry {

    //public readonly AbstractClientTicker AbstractClientTicker = Register("client_ticker", new AbstractClientTicker());

    private static T Register<T>(string name, T ticker) where T : IClientTicker {
        ClientRegistryTypes.Ticker.Add(name, ticker);
        
        return ticker;
    }

    public void Update(GameTime time) {
        foreach (IClientTicker ticker in ClientRegistryTypes.Ticker.Values) {
            ticker.Update(time);
        }
    }
}