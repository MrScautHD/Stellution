using System.Collections.Generic;
using Stellution.Client.csharp.overlay;
using Stellution.Client.csharp.overlay.types;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry.types; 

public class OverlayRegistry : Registry {
    
    public static readonly Dictionary<string, Overlay> Overlays = new();
    
    public static CrosshairOverlay CrosshairOverlay { get; private set; }
    public static MapEditorOverlay MapEditorOverlay { get; private set; }

    public override void Initialize() {
        MapEditorOverlay = this.Register("map_editor", Overlays, new MapEditorOverlay(FontRegistry.Fontoe.Value));
        CrosshairOverlay = this.Register("crosshair", Overlays, new CrosshairOverlay(FontRegistry.Fontoe.Value));
    }
}