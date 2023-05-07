using Easel.GUI;
using Easel.Math;
using Stellution.Client.csharp.registry.types;

namespace Stellution.Client.csharp.overlay.types; 

public class CrosshairOverlay : Overlay {

    public override void Draw() {
        this.SpriteRenderer.Begin();
        this.DrawImage(TextureRegistry.Crosshair.Value, new Position(Anchor.CenterCenter), new Size<int>(24));
        this.DrawText(FontRegistry.Fontoe.Value, "TEST", new Position(Anchor.BottomCenter), 80, Color.Blue);
        this.SpriteRenderer.End();
    }
}