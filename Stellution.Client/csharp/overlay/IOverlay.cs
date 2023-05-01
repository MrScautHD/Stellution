using Easel.Graphics.Renderers;

namespace Stellution.Client.csharp.overlay; 

public interface IOverlay {
    
    void Draw(SpriteRenderer renderer);
}