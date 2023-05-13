using Easel.Core;
using Stellution.Common.csharp.file;

namespace Stellution.Client.csharp.config; 

public class GraphicConfig : AbstractConfig {
    
    public GraphicConfig(string directory, string name) : base(directory, name) {
        this._dictionary.Add("fullscreen", false);
        this._dictionary.Add("vsync", false);
        this._dictionary.Add("msaa", 0);
        this._dictionary.Add("width", 1920);
        this._dictionary.Add("height", 1080);
        this._dictionary.Add("screens", 1); //Change to the right screen screen 1 - 2...
        this._dictionary.Add("render_distance", 2); // 5% - 100%
        this._dictionary.Add("framerate", 300); // 60 FPS min - 300 then unlimited
        this._dictionary.Add("hardware_mode_switch", false);
        
        this.WriteConfig();
    }
}