using Easel.Math;
using Pie;

namespace Easel.Graphics.Lighting;

public class ShadowMap
{
    private Pie.Texture _texture;
    private Framebuffer _framebuffer;

    public ShadowMap(Size<int> size)
    {
        GraphicsDevice device = EaselGraphics.Instance.PieGraphics;
        
        //_texture = device.CreateTexture(new TextureDescription(TextureType.Texture2D, size.Width, size.Height, ))
        
        //_framebuffer = device.CreateFramebuffer()
    }
}