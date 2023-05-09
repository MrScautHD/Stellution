using Easel.Graphics.Renderers;
using Easel.GUI;
using Stellution.Client.csharp.registry.types;

namespace Stellution.Client.csharp.overlay.types; 

public class DebugOverlay : Overlay {
    
    public override void Draw(SpriteRenderer renderer) {
        this.DrawText(FontRegistry.Fontoe.Value, "FPS: ", new Position(Anchor.TopLeft), 25);
    }
}