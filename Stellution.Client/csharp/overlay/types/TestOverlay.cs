using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.GUI;
using Easel.Math;

namespace Stellution.Client.csharp.overlay.types; 

public class TestOverlay : Overlay {
    
    public TestOverlay(Font font) : base(font) {
    }

    public override void Draw(SpriteRenderer renderer) {
        this.DrawImage(Texture2D.Black, new Position(Anchor.BottomCenter), new Size<int>(100, 100));
        this.DrawText("TEST", new Position(Anchor.CenterCenter), 18, Color.White);
    }
}