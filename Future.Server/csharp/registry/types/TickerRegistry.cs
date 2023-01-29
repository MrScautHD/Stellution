using Future.Common.csharp.ticker;
using Microsoft.Xna.Framework;

namespace Future.Common.csharp.registry.types; 

public class TickerRegistry : IRegistry {

    //public readonly AbstractClientTicker AbstractClientTicker = Register("client_ticker", new AbstractClientTicker());

    private static T Register<T>(string name, T ticker) where T : ITicker {
        RegistryTypes.Ticker.Add(name, ticker);
        
        return ticker;
    }

    public void FixedUpdate(GameTime time) {
        foreach (ITicker ticker in RegistryTypes.Ticker.Values) {
            ticker.FixedUpdate(time);
        }
    }
}