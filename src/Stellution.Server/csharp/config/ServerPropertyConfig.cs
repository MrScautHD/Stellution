using System.Numerics;
using Stellution.Common.csharp.file;

namespace Stellution.Server.csharp.config;

public class ServerPropertyConfig : AbstractConfig {
    public ServerPropertyConfig(string directory, string name) : base(directory, name) {
        _dictionary.Add("name", "Stellution-Server");
        _dictionary.Add("description", "A Stellution Server!");
        _dictionary.Add("port", 7777);
        _dictionary.Add("max_client_count", 32);
        _dictionary.Add("start_spawn_point", new Vector3(0, 0, 0));

        WriteConfig();
    }
}