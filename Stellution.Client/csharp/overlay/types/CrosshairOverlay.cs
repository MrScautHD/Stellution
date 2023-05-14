using Easel.GUI;
using Easel.Math;
using Stellution.Client.csharp.registry.types;

namespace Stellution.Client.csharp.overlay.types; 

public class CrosshairOverlay : Overlay {

    public override void Draw() {
        this.DrawImage(TextureRegistry.Crosshair.Value, new Position(Anchor.CenterCenter), new Size<int>(24));
    }
}