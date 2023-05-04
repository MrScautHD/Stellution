using System;
using Easel.Graphics;
using Easel.GUI;
using Easel.Math;

namespace Stellution.Client.csharp.overlay.types; 

public class MapEditorOverlay : Overlay {
    
    public MapEditorOverlay(Font font) : base(font) {
    }

    public override void Draw() {
        this.DrawImage(Texture2D.Black, new Position(Anchor.TopLeft), new Size<int>(250, 350));
        this.DrawText("TEST", new Position(Anchor.CenterCenter), 18);
    }
}