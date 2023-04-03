using Stellution.Common.csharp.file;

namespace Stellution.Server.csharp.config; 

public class ServerPropertyConfig : AbstractConfig {
    
    public ServerPropertyConfig(string directory, string name) : base(directory, name) {
        this._dictionary.Add("name", "Future-Server");
        this._dictionary.Add("description", "A Future Server!");
        this._dictionary.Add("slots", 16);

        this.WriteConfig();
    }
}