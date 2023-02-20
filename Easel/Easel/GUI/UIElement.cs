using System;
using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.Math;
using Pie.Windowing;

namespace Easel.GUI;

public abstract class UIElement
{
    public event OnClick Click;
    
    public Position Position;

    public Size<int> Size;
    
    public bool IsClicked;

    public bool IsHovering;

    protected bool IsMouseButtonHeld;

    protected Vector2<int> CalculatedScreenPos;

    public UITheme Theme;

    public Tooltip Tooltip;

    protected UIElement(Position position, Size<int> size)
    {
        Position = position;
        Size = size;
        // UITheme is purposefully a struct, copy it for each element.
        Theme = UI.Theme;
    }

    protected internal virtual void Update(ref bool mouseTaken, Rectangle<int> viewport)
    {
        Vector2<float> mousePos = Input.MousePosition;

        CalculatedScreenPos = Position.CalculatePosition(viewport, Size);

        IsClicked = false;
        IsHovering = false;
        
        if (!mouseTaken && mousePos.X >= CalculatedScreenPos.X && mousePos.X < CalculatedScreenPos.X + Size.Width &&
            mousePos.Y >= CalculatedScreenPos.Y && mousePos.Y < CalculatedScreenPos.Y + Size.Height)
        {
            mouseTaken = true;
            IsHovering = true;

            UI.CurrentTooltip = Tooltip;

            if (Input.MouseButtonDown(MouseButton.Left))
                IsMouseButtonHeld = true;
            else if (IsMouseButtonHeld)
            {
                Click?.Invoke(this);
                IsClicked = true;
                IsMouseButtonHeld = false;
            }
        }
    }

    protected internal virtual void Draw(SpriteRenderer renderer)
    {
        if (Theme.Blur != null)
        {
            _effect ??= new Effect("Easel.Graphics.Shaders.SpriteRenderer.Sprite.vert",
                "Easel.Graphics.Shaders.SpriteRenderer.Sprite.frag", defines: "BLUR");

            if (BlurTexture == null || Size != BlurTexture.Size)
            {
                BlurTexture?.Dispose();
                _writeBuffer?.Dispose();
                BlurTexture = new RenderTarget(Size);
                _writeBuffer = new RenderTarget(Size);
            }
            
            EaselGraphics graphics = EaselGame.Instance.GraphicsInternal;
            RenderTarget rt = graphics.Renderer.MainTarget;
        
            renderer.End();
        
            graphics.SetRenderTarget(BlurTexture);
            graphics.Viewport = new Rectangle<int>(new Vector2<int>(0, rt.Size.Height - Size.Height), rt.Size);
            renderer.Begin();
            renderer.Draw(rt, new Vector2<float>(0, 000), new Rectangle<int>(CalculatedScreenPos, Size), Color.White, 0, Vector2<float>.Zero, Vector2<float>.One);
            renderer.End();
            graphics.SetRenderTarget(null);

            for (int i = 0; i < Theme.Blur.Iterations; i++)
            {
                float radius = (Theme.Blur.Iterations - i - 1) * Theme.Blur.Radius;
                Vector2<float> direction = i % 2 == 0 ? new Vector2<float>(radius, 0) : new Vector2<float>(0, radius);
            
                graphics.SetRenderTarget(_writeBuffer);
                graphics.Viewport = new Rectangle<int>(new Vector2<int>(0, rt.Size.Height - Size.Height), rt.Size);
                renderer.Begin(effect: _effect);

                renderer.Draw(BlurTexture, Vector2<float>.Zero, null, Color.White, 0, Vector2<float>.Zero, Vector2<float>.One,
                    meta1: new Vector4((System.Numerics.Vector2) direction, Size.Width, Size.Height));
            
                renderer.End();
                graphics.SetRenderTarget(null);
                (BlurTexture, _writeBuffer) = (_writeBuffer, BlurTexture);
            }

            renderer.Begin();
        }
    }

    public delegate void OnClick(UIElement element);

    #region Blur

    protected RenderTarget BlurTexture;

    private static Effect _effect;
    
    private RenderTarget _writeBuffer;

    #endregion
}