using Future.Common.csharp.file;

namespace Future.Client.csharp.config; 

public class GraphicConfig : AbstractConfig {
    
    public GraphicConfig(string directory, string name) : base(directory, name) {
        this._dictionary.Add("FullScreen", false);
        this._dictionary.Add("VSync", false);
        this._dictionary.Add("Multi-Sampling", 0);
        this._dictionary.Add("TEST", true);
        
        this.WriteConfig();
    }
}