using Future.Common.csharp.file;

namespace Future.Common.csharp.config; 

public class ServerPropertyConfig : AbstractConfig {
    
    public ServerPropertyConfig(string directory, string name) : base(directory, name) {
        this._dictionary.Add("server-name", "Best server ever!");

        this.WriteConfig();
    }
}