using Easel.Graphics;
using Easel.GUI;
using Easel.Math;
using Stellution.Client.csharp.registry.types;

namespace Stellution.Client.csharp.overlay.types; 

public class MapEditorOverlay : Overlay {

    public override void Draw() {
        this.DrawImage(Texture2D.Black, new Position(Anchor.TopLeft), new Size<int>(250, 350));
        this.DrawText(FontRegistry.Fontoe.Value, "TEST", new Position(Anchor.CenterCenter), 18);
    }
}