using Future.Common.csharp.file;
using Future.Common.csharp.ticker;

namespace Future.Server.csharp.registry; 

public class RegistryTypes {
    public static readonly Dictionary<string, ITicker> Ticker = new();
    public static readonly Dictionary<string, AbstractConfig> Configs = new();
}