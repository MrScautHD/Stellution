using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.GUI;
using Easel.Math;

namespace Stellution.Client.csharp.overlay.types; 

public class TestOverlay : Overlay {
    
    public override void Draw(SpriteRenderer renderer) {
        this.DrawImage(renderer, Texture2D.Black, new Position(Anchor.BottomCenter), new Size<int>(100, 100));
        //this.DrawText(renderer, "TEST", new Position(Anchor.BottomLeft), 18, Color.White, true);
    }
}