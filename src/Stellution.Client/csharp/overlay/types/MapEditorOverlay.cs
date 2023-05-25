using Easel.Graphics;
using Easel.GUI;
using Easel.Math;
using Pie.Windowing;
using Stellution.Client.csharp.registry.types;

namespace Stellution.Client.csharp.overlay.types; 

public class MapEditorOverlay : Overlay {

    public override void Draw() {
        this.DrawText(FontRegistry.Fontoe.Value, "Map Editor", new Position(Anchor.TopLeft), 29);
        this.DrawImage(Texture2D.Black, new Position(Anchor.TopLeft), new Size<int>(105 * 2, 177 * 2));
    }

    protected void DrawButton(Texture2D texture, Position position, Size<int> size, Color color) {
        this.DrawImage(texture, position, size, color);
    }

    public override Anchor? GetAnchor() {
        return Anchor.TopLeft;
    }

    protected override void OnKeyPress(Key key) {
        if (key == Key.F8) {
            this.Enabled ^= true;
        }
    }
}