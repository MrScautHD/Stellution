using System.Collections.Generic;
using Stellution.Client.csharp.overlay;
using Stellution.Client.csharp.overlay.types;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry; 

public class OverlayRegistry : Registry {
    
    public static readonly Dictionary<string, Overlay> Overlays = new();
    
    public static TestOverlay TestOverlay { get; private set; }
    
    public override void Initialize() {
        TestOverlay = this.Register("test", Overlays, new TestOverlay(FontRegistry.Fontoe));
    }
}