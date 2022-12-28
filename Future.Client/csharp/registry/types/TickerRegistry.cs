using Future.Client.csharp.ticker;
using Microsoft.Xna.Framework;

namespace Future.Client.csharp.registry.types; 

public class TickerRegistry : IClientRegistry {

    public readonly ClientTicker ClientTicker = Register("client_ticker", new ClientTicker());

    private static T Register<T>(string name, T ticker) {
        RegistryTypes.Ticker.Add(name, (IClientTicker) ticker);
        
        return ticker;
    }

    public void Update(GameTime time) {
        foreach (IClientTicker ticker in RegistryTypes.Ticker.Values) {
            ticker.Update(time);
        }
    }
}