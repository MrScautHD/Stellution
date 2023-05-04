using Easel.GUI;
using Stellution.Client.csharp.registry.types;

namespace Stellution.Client.csharp.overlay.types; 

public class CrosshairOverlay : Overlay {
    
    public override void Draw() {
        this.DrawImage(TextureRegistry.Crosshair.Value, new Position(Anchor.CenterCenter));
        this.DrawText(FontRegistry.Fontoe.Value, "TEST", new Position(Anchor.BottomCenter), 18);
    }
}