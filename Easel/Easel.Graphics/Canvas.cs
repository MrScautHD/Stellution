using System.Runtime.CompilerServices;
using Easel.Math;
using Pie;

namespace Easel.Graphics;

/// <summary>
/// Software rendered canvas.
/// </summary>
public class Canvas
{
    private byte[] _backBuffer;

    public readonly Size<int> Size;

    public Rectangle<int> Scissor;

    public Canvas(Size<int> size)
    {
        _backBuffer = new byte[size.Width * size.Height * 4];

        Size = size;
        Scissor = new Rectangle<int>(Vector2<int>.Zero, Size);
    }

    private Canvas(Size<int> size, byte[] data)
    {
        Size = size;
        _backBuffer = data;
    }

    public void Clear(Color color)
    {
        for (int i = 0; i < _backBuffer.Length; i += 4)
        {
            _backBuffer[i + 0] = color.Rb;
            _backBuffer[i + 1] = color.Gb;
            _backBuffer[i + 2] = color.Bb;
            _backBuffer[i + 3] = color.Ab;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawPixelUnchecked(int x, int y, Color color)
    {
        int pos = (y * Size.Width + x) * 4;
        _backBuffer[pos + 0] = color.Rb;
        _backBuffer[pos + 1] = color.Gb;
        _backBuffer[pos + 2] = color.Bb;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawPixelUnchecked(int index, byte value)
    {
        // TODO: Alpha blending
        _backBuffer[index] = value;
    }

    public void FillRectangle(int x, int y, int width, int height, Color color)
    {
        int startX = x < Scissor.X ? 0 : x;
        int startY = y < Scissor.Y ? 0 : y;

        int endX = x + width > Scissor.Right ? Scissor.Right : x + width;
        int endY = y + height > Scissor.Bottom ? Scissor.Bottom : y + height;
            
        
        for (int pY = startY; pY < endY; pY++)
        {
            for (int pX = startX; pX < endX; pX++)
            {
                DrawPixelUnchecked(pX, pY, color);
            }
        }
    }

    public void DrawBitmap(int x, int y, Bitmap bitmap)
    {
        int startX = x < Scissor.X ? -x : 0;
        int startY = y < Scissor.Y ? -y : 0;

        int endX = x + bitmap.Size.Width > Scissor.Right ? Scissor.Right - x : bitmap.Size.Width;
        int endY = y + bitmap.Size.Height > Scissor.Bottom ? Scissor.Bottom - y : bitmap.Size.Height;

        for (int pY = startY; pY < endY; pY++)
        {
            for (int pX = startX; pX < endX; pX++)
            {
                int pos = ((y + pY) * Size.Width + (x + pX)) * 4;
                int texel = (pY * bitmap.Size.Width + pX) * 4;
                
                DrawPixelUnchecked(pos + 0, bitmap.Data[texel + 0]);
                DrawPixelUnchecked(pos + 1, bitmap.Data[texel + 1]);
                DrawPixelUnchecked(pos + 2, bitmap.Data[texel + 2]);
            }
        }
    }
    
    public void DrawBitmap(int x, int y, int width, int height, Bitmap bitmap)
    {
        int startX = x < Scissor.X ? -x : 0;
        int startY = y < Scissor.Y ? -y : 0;

        int endX = x + width > Scissor.Right ? Scissor.Right - x : width;
        int endY = y + height > Scissor.Bottom ? Scissor.Bottom - y : height;

        for (int pY = startY; pY < endY; pY++)
        {
            for (int pX = startX; pX < endX; pX++)
            {
                int pos = ((y + pY) * Size.Width + (x + pX)) * 4;

                int texX = (int) (pX * (bitmap.Size.Width / (float) width));
                int texY = (int) (pY * (bitmap.Size.Height / (float) height));
                int texel = (texY * bitmap.Size.Width + texX) * 4;

                DrawPixelUnchecked(pos + 0, bitmap.Data[texel + 0]);
                DrawPixelUnchecked(pos + 1, bitmap.Data[texel + 1]);
                DrawPixelUnchecked(pos + 2, bitmap.Data[texel + 2]);
            }
        }
    }

    public Bitmap ToBitmap() => new Bitmap(Size.Width, Size.Height, Format.R8G8B8A8_UNorm, _backBuffer);
    
    public static Canvas FromBitmap(Bitmap bitmap)
    {
        return new Canvas(bitmap.Size, bitmap.Data);
    }
}