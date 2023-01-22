using Future.Common.csharp.file;

namespace Future.Client.csharp.config; 

public class GraphicConfig : AbstractConfig {
    
    public GraphicConfig(string directory, string name) : base(directory, name) {
        this._dictionary.Add("DirectX", 12);
        this._dictionary.Add("FOV", "80");
        this._dictionary.Add("FXAA", true);
        
        this.WriteConfig();
    }
}