using Easel;
using Easel.Entities;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;

namespace Stellution.Client.csharp.particle; 

public abstract class Particle {

    protected Texture2D Texture;
    protected Color Color;
    protected Size<int> Size;
    protected Transform Transform;
    protected float Gravity;
    
    protected EaselGraphics Graphics => EaselGame.Instance.Graphics;
    protected SpriteRenderer SpriteRenderer => EaselGame.Instance.Graphics.SpriteRenderer;

    protected Particle() {
        
    }

    public virtual void Draw() {
        
    }

    public virtual void Update() {
        
    }
}