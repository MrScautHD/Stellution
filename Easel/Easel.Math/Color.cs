using System;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace Easel.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Color
{
    [XmlAttribute]
    public float R;

    [XmlAttribute]
    public float G;

    [XmlAttribute]
    public float B;

    [XmlAttribute]
    public float A;

    public byte Rb => (byte) (R * 255);

    public byte Gb => (byte) (G * 255);

    public byte Bb => (byte) (B * 255);

    public byte Ab => (byte) (A * 255);

    public Color(float r, float g, float b, float a)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public Color(byte r, byte g, byte b, byte a)
    {
        R = r / 255f;
        G = g / 255f;
        B = b / 255f;
        A = a / 255f;
    }

    public Color(byte r, byte g, byte b, float a)
    {
        R = r / 255f;
        G = g / 255f;
        B = b / 255f;
        A = a;
    }
    
    public Color(Color @base, float alpha) : this(@base.R, @base.G, @base.B, alpha) { }
    
    public Color(Color @base, byte alpha) : this(@base.Rb, @base.Gb, @base.Bb, alpha) { }

    public Color(uint rgbaColor) : 
        this((byte) (rgbaColor >> 24), (byte) ((rgbaColor & 0xFF0000) >> 16), (byte) ((rgbaColor & 0xFF00) >> 8), (byte) (rgbaColor & 0xFF)) { }

    public static Color FromHsv(float h, float s, float v)
    {
        float c = v * s;
        float x = c * (1 - MathF.Abs((h / 60f) % 2 - 1));
        float m = v - c;
        float r, g, b;

        switch (h)
        {
            case >= 0 and < 60:
                r = c;
                g = x;
                b = 0;
                break;
            case >= 60 and < 120:
                r = x;
                g = c;
                b = 0;
                break;
            case >= 120 and < 180:
                r = 0;
                g = c;
                b = x;
                break;
            case >= 180 and < 240:
                r = 0;
                g = x;
                b = c;
                break;
            case >= 240 and < 300:
                r = x;
                g = 0;
                b = c;
                break;
            case >= 300 and < 360:
                r = c;
                g = 0;
                b = x;
                break;
            default:
                r = 0;
                g = 0;
                b = 0;
                break;
        }

        return new Color(r + m, g + m, b + m, 1);
    }

    public static Color FromString(string text)
    {
        if (text.StartsWith("#") || text.StartsWith("0x"))
        {
            text = text
                .TrimStart('#')
                .TrimStart('0', 'x');

            if (!uint.TryParse(text, NumberStyles.HexNumber, null, out uint result))
                return Transparent;
            if (result <= 0xFFFFFF)
            {
                result <<= 8;
                result |= 255;
            }

            return new Color(result);
        }
        
        if (text.StartsWith("rgb(") && text.EndsWith(')'))
        {
            try
            {
                text = text["rgb(".Length..^1];
                string[] colors = text.Split(",");
                byte r = byte.Parse(colors[0]);
                byte g = byte.Parse(colors[1]);
                byte b = byte.Parse(colors[2]);
                return new Color(r, g, b, 255);
            }
            catch (Exception)
            {
                return Transparent;
            }
        }
        
        if (text.StartsWith("rgba(") && text.EndsWith(')'))
        {
            try
            {
                text = text["rgba(".Length..^1];
                string[] colors = text.Split(",");
                byte r = byte.Parse(colors[0]);
                byte g = byte.Parse(colors[1]);
                byte b = byte.Parse(colors[2]);
                byte a = byte.Parse(colors[3]);
                return new Color(r, g, b, a);
            }
            catch (Exception)
            {
                return Transparent;
            }
        }

        FieldInfo info = typeof(Color).GetField(text, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public);
        if (info == null)
            return Transparent;
        return (Color) (info.GetValue(null) ?? Transparent);
    }
    
    public static explicit operator System.Drawing.Color(Color color)
    {
        return System.Drawing.Color.FromArgb(color.Ab, color.Rb, color.Gb, color.Bb);
    }

    public static explicit operator Vector4(Color color)
    {
        return new Vector4(color.R, color.G, color.B, color.A);
    }

    public override string ToString()
    {
        return "Color(R: " + Rb + ", G: " + Gb + ", B: " + Bb + ", A: " + Ab + ")";
    }

    /// <summary>
    /// AliceBlue has an RGBA value of (240, 248, 255, 255) (hex #F0F8FFFF)
    /// </summary>
    public static readonly Color AliceBlue = new Color(0xF0F8FFFF);

    /// <summary>
    /// AntiqueWhite has an RGBA value of (250, 235, 215, 255) (hex #FAEBD7FF)
    /// </summary>
    public static readonly Color AntiqueWhite = new Color(0xFAEBD7FF);

    /// <summary>
    /// Aqua has an RGBA value of (0, 255, 255, 255) (hex #00FFFFFF)
    /// </summary>
    public static readonly Color Aqua = new Color(0x00FFFFFF);

    /// <summary>
    /// Aquamarine has an RGBA value of (127, 255, 212, 255) (hex #7FFFD4FF)
    /// </summary>
    public static readonly Color Aquamarine = new Color(0x7FFFD4FF);

    /// <summary>
    /// Azure has an RGBA value of (240, 255, 255, 255) (hex #F0FFFFFF)
    /// </summary>
    public static readonly Color Azure = new Color(0xF0FFFFFF);

    /// <summary>
    /// Beige has an RGBA value of (245, 245, 220, 255) (hex #F5F5DCFF)
    /// </summary>
    public static readonly Color Beige = new Color(0xF5F5DCFF);

    /// <summary>
    /// Bisque has an RGBA value of (255, 228, 196, 255) (hex #FFE4C4FF)
    /// </summary>
    public static readonly Color Bisque = new Color(0xFFE4C4FF);

    /// <summary>
    /// Black has an RGBA value of (0, 0, 0, 255) (hex #000000FF)
    /// </summary>
    public static readonly Color Black = new Color(0x000000FF);

    /// <summary>
    /// BlanchedAlmond has an RGBA value of (255, 235, 205, 255) (hex #FFEBCDFF)
    /// </summary>
    public static readonly Color BlanchedAlmond = new Color(0xFFEBCDFF);

    /// <summary>
    /// Blue has an RGBA value of (0, 0, 255, 255) (hex #0000FFFF)
    /// </summary>
    public static readonly Color Blue = new Color(0x0000FFFF);

    /// <summary>
    /// BlueViolet has an RGBA value of (138, 43, 226, 255) (hex #8A2BE2FF)
    /// </summary>
    public static readonly Color BlueViolet = new Color(0x8A2BE2FF);

    /// <summary>
    /// Brown has an RGBA value of (165, 42, 42, 255) (hex #A52A2AFF)
    /// </summary>
    public static readonly Color Brown = new Color(0xA52A2AFF);

    /// <summary>
    /// BurlyWood has an RGBA value of (222, 184, 135, 255) (hex #DEB887FF)
    /// </summary>
    public static readonly Color BurlyWood = new Color(0xDEB887FF);

    /// <summary>
    /// CadetBlue has an RGBA value of (95, 158, 160, 255) (hex #5F9EA0FF)
    /// </summary>
    public static readonly Color CadetBlue = new Color(0x5F9EA0FF);

    /// <summary>
    /// Chartreuse has an RGBA value of (127, 255, 0, 255) (hex #7FFF00FF)
    /// </summary>
    public static readonly Color Chartreuse = new Color(0x7FFF00FF);

    /// <summary>
    /// Chocolate has an RGBA value of (210, 105, 30, 255) (hex #D2691EFF)
    /// </summary>
    public static readonly Color Chocolate = new Color(0xD2691EFF);

    /// <summary>
    /// Coral has an RGBA value of (255, 127, 80, 255) (hex #FF7F50FF)
    /// </summary>
    public static readonly Color Coral = new Color(0xFF7F50FF);

    /// <summary>
    /// CornflowerBlue has an RGBA value of (100, 149, 237, 255) (hex #6495EDFF)
    /// </summary>
    public static readonly Color CornflowerBlue = new Color(0x6495EDFF);

    /// <summary>
    /// Cornsilk has an RGBA value of (255, 248, 220, 255) (hex #FFF8DCFF)
    /// </summary>
    public static readonly Color Cornsilk = new Color(0xFFF8DCFF);

    /// <summary>
    /// Crimson has an RGBA value of (220, 20, 60, 255) (hex #DC143CFF)
    /// </summary>
    public static readonly Color Crimson = new Color(0xDC143CFF);

    /// <summary>
    /// Cyan has an RGBA value of (0, 255, 255, 255) (hex #00FFFFFF)
    /// </summary>
    public static readonly Color Cyan = new Color(0x00FFFFFF);

    /// <summary>
    /// DarkBlue has an RGBA value of (0, 0, 139, 255) (hex #00008BFF)
    /// </summary>
    public static readonly Color DarkBlue = new Color(0x00008BFF);

    /// <summary>
    /// DarkCyan has an RGBA value of (0, 139, 139, 255) (hex #008B8BFF)
    /// </summary>
    public static readonly Color DarkCyan = new Color(0x008B8BFF);

    /// <summary>
    /// DarkGoldenRod has an RGBA value of (184, 134, 11, 255) (hex #B8860BFF)
    /// </summary>
    public static readonly Color DarkGoldenRod = new Color(0xB8860BFF);

    /// <summary>
    /// DarkGray has an RGBA value of (169, 169, 169, 255) (hex #A9A9A9FF)
    /// </summary>
    public static readonly Color DarkGray = new Color(0xA9A9A9FF);

    /// <summary>
    /// DarkGrey has an RGBA value of (169, 169, 169, 255) (hex #A9A9A9FF)
    /// </summary>
    public static readonly Color DarkGrey = new Color(0xA9A9A9FF);

    /// <summary>
    /// DarkGreen has an RGBA value of (0, 100, 0, 255) (hex #006400FF)
    /// </summary>
    public static readonly Color DarkGreen = new Color(0x006400FF);

    /// <summary>
    /// DarkKhaki has an RGBA value of (189, 183, 107, 255) (hex #BDB76BFF)
    /// </summary>
    public static readonly Color DarkKhaki = new Color(0xBDB76BFF);

    /// <summary>
    /// DarkMagenta has an RGBA value of (139, 0, 139, 255) (hex #8B008BFF)
    /// </summary>
    public static readonly Color DarkMagenta = new Color(0x8B008BFF);

    /// <summary>
    /// DarkOliveGreen has an RGBA value of (85, 107, 47, 255) (hex #556B2FFF)
    /// </summary>
    public static readonly Color DarkOliveGreen = new Color(0x556B2FFF);

    /// <summary>
    /// DarkOrange has an RGBA value of (255, 140, 0, 255) (hex #FF8C00FF)
    /// </summary>
    public static readonly Color DarkOrange = new Color(0xFF8C00FF);

    /// <summary>
    /// DarkOrchid has an RGBA value of (153, 50, 204, 255) (hex #9932CCFF)
    /// </summary>
    public static readonly Color DarkOrchid = new Color(0x9932CCFF);

    /// <summary>
    /// DarkRed has an RGBA value of (139, 0, 0, 255) (hex #8B0000FF)
    /// </summary>
    public static readonly Color DarkRed = new Color(0x8B0000FF);

    /// <summary>
    /// DarkSalmon has an RGBA value of (233, 150, 122, 255) (hex #E9967AFF)
    /// </summary>
    public static readonly Color DarkSalmon = new Color(0xE9967AFF);

    /// <summary>
    /// DarkSeaGreen has an RGBA value of (143, 188, 143, 255) (hex #8FBC8FFF)
    /// </summary>
    public static readonly Color DarkSeaGreen = new Color(0x8FBC8FFF);

    /// <summary>
    /// DarkSlateBlue has an RGBA value of (72, 61, 139, 255) (hex #483D8BFF)
    /// </summary>
    public static readonly Color DarkSlateBlue = new Color(0x483D8BFF);

    /// <summary>
    /// DarkSlateGray has an RGBA value of (47, 79, 79, 255) (hex #2F4F4FFF)
    /// </summary>
    public static readonly Color DarkSlateGray = new Color(0x2F4F4FFF);

    /// <summary>
    /// DarkSlateGrey has an RGBA value of (47, 79, 79, 255) (hex #2F4F4FFF)
    /// </summary>
    public static readonly Color DarkSlateGrey = new Color(0x2F4F4FFF);

    /// <summary>
    /// DarkTurquoise has an RGBA value of (0, 206, 209, 255) (hex #00CED1FF)
    /// </summary>
    public static readonly Color DarkTurquoise = new Color(0x00CED1FF);

    /// <summary>
    /// DarkViolet has an RGBA value of (148, 0, 211, 255) (hex #9400D3FF)
    /// </summary>
    public static readonly Color DarkViolet = new Color(0x9400D3FF);

    /// <summary>
    /// DeepPink has an RGBA value of (255, 20, 147, 255) (hex #FF1493FF)
    /// </summary>
    public static readonly Color DeepPink = new Color(0xFF1493FF);

    /// <summary>
    /// DeepSkyBlue has an RGBA value of (0, 191, 255, 255) (hex #00BFFFFF)
    /// </summary>
    public static readonly Color DeepSkyBlue = new Color(0x00BFFFFF);

    /// <summary>
    /// DimGray has an RGBA value of (105, 105, 105, 255) (hex #696969FF)
    /// </summary>
    public static readonly Color DimGray = new Color(0x696969FF);

    /// <summary>
    /// DimGrey has an RGBA value of (105, 105, 105, 255) (hex #696969FF)
    /// </summary>
    public static readonly Color DimGrey = new Color(0x696969FF);

    /// <summary>
    /// DodgerBlue has an RGBA value of (30, 144, 255, 255) (hex #1E90FFFF)
    /// </summary>
    public static readonly Color DodgerBlue = new Color(0x1E90FFFF);

    /// <summary>
    /// FireBrick has an RGBA value of (178, 34, 34, 255) (hex #B22222FF)
    /// </summary>
    public static readonly Color FireBrick = new Color(0xB22222FF);

    /// <summary>
    /// FloralWhite has an RGBA value of (255, 250, 240, 255) (hex #FFFAF0FF)
    /// </summary>
    public static readonly Color FloralWhite = new Color(0xFFFAF0FF);

    /// <summary>
    /// ForestGreen has an RGBA value of (34, 139, 34, 255) (hex #228B22FF)
    /// </summary>
    public static readonly Color ForestGreen = new Color(0x228B22FF);

    /// <summary>
    /// Fuchsia has an RGBA value of (255, 0, 255, 255) (hex #FF00FFFF)
    /// </summary>
    public static readonly Color Fuchsia = new Color(0xFF00FFFF);

    /// <summary>
    /// Gainsboro has an RGBA value of (220, 220, 220, 255) (hex #DCDCDCFF)
    /// </summary>
    public static readonly Color Gainsboro = new Color(0xDCDCDCFF);

    /// <summary>
    /// GhostWhite has an RGBA value of (248, 248, 255, 255) (hex #F8F8FFFF)
    /// </summary>
    public static readonly Color GhostWhite = new Color(0xF8F8FFFF);

    /// <summary>
    /// Gold has an RGBA value of (255, 215, 0, 255) (hex #FFD700FF)
    /// </summary>
    public static readonly Color Gold = new Color(0xFFD700FF);

    /// <summary>
    /// GoldenRod has an RGBA value of (218, 165, 32, 255) (hex #DAA520FF)
    /// </summary>
    public static readonly Color GoldenRod = new Color(0xDAA520FF);

    /// <summary>
    /// Gray has an RGBA value of (128, 128, 128, 255) (hex #808080FF)
    /// </summary>
    public static readonly Color Gray = new Color(0x808080FF);

    /// <summary>
    /// Grey has an RGBA value of (128, 128, 128, 255) (hex #808080FF)
    /// </summary>
    public static readonly Color Grey = new Color(0x808080FF);

    /// <summary>
    /// Green has an RGBA value of (0, 128, 0, 255) (hex #008000FF)
    /// </summary>
    public static readonly Color Green = new Color(0x008000FF);

    /// <summary>
    /// GreenYellow has an RGBA value of (173, 255, 47, 255) (hex #ADFF2FFF)
    /// </summary>
    public static readonly Color GreenYellow = new Color(0xADFF2FFF);

    /// <summary>
    /// HoneyDew has an RGBA value of (240, 255, 240, 255) (hex #F0FFF0FF)
    /// </summary>
    public static readonly Color HoneyDew = new Color(0xF0FFF0FF);

    /// <summary>
    /// HotPink has an RGBA value of (255, 105, 180, 255) (hex #FF69B4FF)
    /// </summary>
    public static readonly Color HotPink = new Color(0xFF69B4FF);

    /// <summary>
    /// IndianRed has an RGBA value of (205, 92, 92, 255) (hex #CD5C5CFF)
    /// </summary>
    public static readonly Color IndianRed = new Color(0xCD5C5CFF);

    /// <summary>
    /// Indigo has an RGBA value of (75, 0, 130, 255) (hex #4B0082FF)
    /// </summary>
    public static readonly Color Indigo = new Color(0x4B0082FF);

    /// <summary>
    /// Ivory has an RGBA value of (255, 255, 240, 255) (hex #FFFFF0FF)
    /// </summary>
    public static readonly Color Ivory = new Color(0xFFFFF0FF);

    /// <summary>
    /// Khaki has an RGBA value of (240, 230, 140, 255) (hex #F0E68CFF)
    /// </summary>
    public static readonly Color Khaki = new Color(0xF0E68CFF);

    /// <summary>
    /// Lavender has an RGBA value of (230, 230, 250, 255) (hex #E6E6FAFF)
    /// </summary>
    public static readonly Color Lavender = new Color(0xE6E6FAFF);

    /// <summary>
    /// LavenderBlush has an RGBA value of (255, 240, 245, 255) (hex #FFF0F5FF)
    /// </summary>
    public static readonly Color LavenderBlush = new Color(0xFFF0F5FF);

    /// <summary>
    /// LawnGreen has an RGBA value of (124, 252, 0, 255) (hex #7CFC00FF)
    /// </summary>
    public static readonly Color LawnGreen = new Color(0x7CFC00FF);

    /// <summary>
    /// LemonChiffon has an RGBA value of (255, 250, 205, 255) (hex #FFFACDFF)
    /// </summary>
    public static readonly Color LemonChiffon = new Color(0xFFFACDFF);

    /// <summary>
    /// LightBlue has an RGBA value of (173, 216, 230, 255) (hex #ADD8E6FF)
    /// </summary>
    public static readonly Color LightBlue = new Color(0xADD8E6FF);

    /// <summary>
    /// LightCoral has an RGBA value of (240, 128, 128, 255) (hex #F08080FF)
    /// </summary>
    public static readonly Color LightCoral = new Color(0xF08080FF);

    /// <summary>
    /// LightCyan has an RGBA value of (224, 255, 255, 255) (hex #E0FFFFFF)
    /// </summary>
    public static readonly Color LightCyan = new Color(0xE0FFFFFF);

    /// <summary>
    /// LightGoldenRodYellow has an RGBA value of (250, 250, 210, 255) (hex #FAFAD2FF)
    /// </summary>
    public static readonly Color LightGoldenRodYellow = new Color(0xFAFAD2FF);

    /// <summary>
    /// LightGray has an RGBA value of (211, 211, 211, 255) (hex #D3D3D3FF)
    /// </summary>
    public static readonly Color LightGray = new Color(0xD3D3D3FF);

    /// <summary>
    /// LightGrey has an RGBA value of (211, 211, 211, 255) (hex #D3D3D3FF)
    /// </summary>
    public static readonly Color LightGrey = new Color(0xD3D3D3FF);

    /// <summary>
    /// LightGreen has an RGBA value of (144, 238, 144, 255) (hex #90EE90FF)
    /// </summary>
    public static readonly Color LightGreen = new Color(0x90EE90FF);

    /// <summary>
    /// LightPink has an RGBA value of (255, 182, 193, 255) (hex #FFB6C1FF)
    /// </summary>
    public static readonly Color LightPink = new Color(0xFFB6C1FF);

    /// <summary>
    /// LightSalmon has an RGBA value of (255, 160, 122, 255) (hex #FFA07AFF)
    /// </summary>
    public static readonly Color LightSalmon = new Color(0xFFA07AFF);

    /// <summary>
    /// LightSeaGreen has an RGBA value of (32, 178, 170, 255) (hex #20B2AAFF)
    /// </summary>
    public static readonly Color LightSeaGreen = new Color(0x20B2AAFF);

    /// <summary>
    /// LightSkyBlue has an RGBA value of (135, 206, 250, 255) (hex #87CEFAFF)
    /// </summary>
    public static readonly Color LightSkyBlue = new Color(0x87CEFAFF);

    /// <summary>
    /// LightSlateGray has an RGBA value of (119, 136, 153, 255) (hex #778899FF)
    /// </summary>
    public static readonly Color LightSlateGray = new Color(0x778899FF);

    /// <summary>
    /// LightSlateGrey has an RGBA value of (119, 136, 153, 255) (hex #778899FF)
    /// </summary>
    public static readonly Color LightSlateGrey = new Color(0x778899FF);

    /// <summary>
    /// LightSteelBlue has an RGBA value of (176, 196, 222, 255) (hex #B0C4DEFF)
    /// </summary>
    public static readonly Color LightSteelBlue = new Color(0xB0C4DEFF);

    /// <summary>
    /// LightYellow has an RGBA value of (255, 255, 224, 255) (hex #FFFFE0FF)
    /// </summary>
    public static readonly Color LightYellow = new Color(0xFFFFE0FF);

    /// <summary>
    /// Lime has an RGBA value of (0, 255, 0, 255) (hex #00FF00FF)
    /// </summary>
    public static readonly Color Lime = new Color(0x00FF00FF);

    /// <summary>
    /// LimeGreen has an RGBA value of (50, 205, 50, 255) (hex #32CD32FF)
    /// </summary>
    public static readonly Color LimeGreen = new Color(0x32CD32FF);

    /// <summary>
    /// Linen has an RGBA value of (250, 240, 230, 255) (hex #FAF0E6FF)
    /// </summary>
    public static readonly Color Linen = new Color(0xFAF0E6FF);

    /// <summary>
    /// Magenta has an RGBA value of (255, 0, 255, 255) (hex #FF00FFFF)
    /// </summary>
    public static readonly Color Magenta = new Color(0xFF00FFFF);

    /// <summary>
    /// Maroon has an RGBA value of (128, 0, 0, 255) (hex #800000FF)
    /// </summary>
    public static readonly Color Maroon = new Color(0x800000FF);

    /// <summary>
    /// MediumAquaMarine has an RGBA value of (102, 205, 170, 255) (hex #66CDAAFF)
    /// </summary>
    public static readonly Color MediumAquaMarine = new Color(0x66CDAAFF);

    /// <summary>
    /// MediumBlue has an RGBA value of (0, 0, 205, 255) (hex #0000CDFF)
    /// </summary>
    public static readonly Color MediumBlue = new Color(0x0000CDFF);

    /// <summary>
    /// MediumOrchid has an RGBA value of (186, 85, 211, 255) (hex #BA55D3FF)
    /// </summary>
    public static readonly Color MediumOrchid = new Color(0xBA55D3FF);

    /// <summary>
    /// MediumPurple has an RGBA value of (147, 112, 219, 255) (hex #9370DBFF)
    /// </summary>
    public static readonly Color MediumPurple = new Color(0x9370DBFF);

    /// <summary>
    /// MediumSeaGreen has an RGBA value of (60, 179, 113, 255) (hex #3CB371FF)
    /// </summary>
    public static readonly Color MediumSeaGreen = new Color(0x3CB371FF);

    /// <summary>
    /// MediumSlateBlue has an RGBA value of (123, 104, 238, 255) (hex #7B68EEFF)
    /// </summary>
    public static readonly Color MediumSlateBlue = new Color(0x7B68EEFF);

    /// <summary>
    /// MediumSpringGreen has an RGBA value of (0, 250, 154, 255) (hex #00FA9AFF)
    /// </summary>
    public static readonly Color MediumSpringGreen = new Color(0x00FA9AFF);

    /// <summary>
    /// MediumTurquoise has an RGBA value of (72, 209, 204, 255) (hex #48D1CCFF)
    /// </summary>
    public static readonly Color MediumTurquoise = new Color(0x48D1CCFF);

    /// <summary>
    /// MediumVioletRed has an RGBA value of (199, 21, 133, 255) (hex #C71585FF)
    /// </summary>
    public static readonly Color MediumVioletRed = new Color(0xC71585FF);

    /// <summary>
    /// MidnightBlue has an RGBA value of (25, 25, 112, 255) (hex #191970FF)
    /// </summary>
    public static readonly Color MidnightBlue = new Color(0x191970FF);

    /// <summary>
    /// MintCream has an RGBA value of (245, 255, 250, 255) (hex #F5FFFAFF)
    /// </summary>
    public static readonly Color MintCream = new Color(0xF5FFFAFF);

    /// <summary>
    /// MistyRose has an RGBA value of (255, 228, 225, 255) (hex #FFE4E1FF)
    /// </summary>
    public static readonly Color MistyRose = new Color(0xFFE4E1FF);

    /// <summary>
    /// Moccasin has an RGBA value of (255, 228, 181, 255) (hex #FFE4B5FF)
    /// </summary>
    public static readonly Color Moccasin = new Color(0xFFE4B5FF);

    /// <summary>
    /// NavajoWhite has an RGBA value of (255, 222, 173, 255) (hex #FFDEADFF)
    /// </summary>
    public static readonly Color NavajoWhite = new Color(0xFFDEADFF);

    /// <summary>
    /// Navy has an RGBA value of (0, 0, 128, 255) (hex #000080FF)
    /// </summary>
    public static readonly Color Navy = new Color(0x000080FF);

    /// <summary>
    /// OldLace has an RGBA value of (253, 245, 230, 255) (hex #FDF5E6FF)
    /// </summary>
    public static readonly Color OldLace = new Color(0xFDF5E6FF);

    /// <summary>
    /// Olive has an RGBA value of (128, 128, 0, 255) (hex #808000FF)
    /// </summary>
    public static readonly Color Olive = new Color(0x808000FF);

    /// <summary>
    /// OliveDrab has an RGBA value of (107, 142, 35, 255) (hex #6B8E23FF)
    /// </summary>
    public static readonly Color OliveDrab = new Color(0x6B8E23FF);

    /// <summary>
    /// Orange has an RGBA value of (255, 165, 0, 255) (hex #FFA500FF)
    /// </summary>
    public static readonly Color Orange = new Color(0xFFA500FF);

    /// <summary>
    /// OrangeRed has an RGBA value of (255, 69, 0, 255) (hex #FF4500FF)
    /// </summary>
    public static readonly Color OrangeRed = new Color(0xFF4500FF);

    /// <summary>
    /// Orchid has an RGBA value of (218, 112, 214, 255) (hex #DA70D6FF)
    /// </summary>
    public static readonly Color Orchid = new Color(0xDA70D6FF);

    /// <summary>
    /// PaleGoldenRod has an RGBA value of (238, 232, 170, 255) (hex #EEE8AAFF)
    /// </summary>
    public static readonly Color PaleGoldenRod = new Color(0xEEE8AAFF);

    /// <summary>
    /// PaleGreen has an RGBA value of (152, 251, 152, 255) (hex #98FB98FF)
    /// </summary>
    public static readonly Color PaleGreen = new Color(0x98FB98FF);

    /// <summary>
    /// PaleTurquoise has an RGBA value of (175, 238, 238, 255) (hex #AFEEEEFF)
    /// </summary>
    public static readonly Color PaleTurquoise = new Color(0xAFEEEEFF);

    /// <summary>
    /// PaleVioletRed has an RGBA value of (219, 112, 147, 255) (hex #DB7093FF)
    /// </summary>
    public static readonly Color PaleVioletRed = new Color(0xDB7093FF);

    /// <summary>
    /// PapayaWhip has an RGBA value of (255, 239, 213, 255) (hex #FFEFD5FF)
    /// </summary>
    public static readonly Color PapayaWhip = new Color(0xFFEFD5FF);

    /// <summary>
    /// PeachPuff has an RGBA value of (255, 218, 185, 255) (hex #FFDAB9FF)
    /// </summary>
    public static readonly Color PeachPuff = new Color(0xFFDAB9FF);

    /// <summary>
    /// Peru has an RGBA value of (205, 133, 63, 255) (hex #CD853FFF)
    /// </summary>
    public static readonly Color Peru = new Color(0xCD853FFF);

    /// <summary>
    /// Pink has an RGBA value of (255, 192, 203, 255) (hex #FFC0CBFF)
    /// </summary>
    public static readonly Color Pink = new Color(0xFFC0CBFF);

    /// <summary>
    /// Plum has an RGBA value of (221, 160, 221, 255) (hex #DDA0DDFF)
    /// </summary>
    public static readonly Color Plum = new Color(0xDDA0DDFF);

    /// <summary>
    /// PowderBlue has an RGBA value of (176, 224, 230, 255) (hex #B0E0E6FF)
    /// </summary>
    public static readonly Color PowderBlue = new Color(0xB0E0E6FF);

    /// <summary>
    /// Purple has an RGBA value of (128, 0, 128, 255) (hex #800080FF)
    /// </summary>
    public static readonly Color Purple = new Color(0x800080FF);

    /// <summary>
    /// RebeccaPurple has an RGBA value of (102, 51, 153, 255) (hex #663399FF)
    /// </summary>
    public static readonly Color RebeccaPurple = new Color(0x663399FF);

    /// <summary>
    /// Red has an RGBA value of (255, 0, 0, 255) (hex #FF0000FF)
    /// </summary>
    public static readonly Color Red = new Color(0xFF0000FF);

    /// <summary>
    /// RosyBrown has an RGBA value of (188, 143, 143, 255) (hex #BC8F8FFF)
    /// </summary>
    public static readonly Color RosyBrown = new Color(0xBC8F8FFF);

    /// <summary>
    /// RoyalBlue has an RGBA value of (65, 105, 225, 255) (hex #4169E1FF)
    /// </summary>
    public static readonly Color RoyalBlue = new Color(0x4169E1FF);

    /// <summary>
    /// SaddleBrown has an RGBA value of (139, 69, 19, 255) (hex #8B4513FF)
    /// </summary>
    public static readonly Color SaddleBrown = new Color(0x8B4513FF);

    /// <summary>
    /// Salmon has an RGBA value of (250, 128, 114, 255) (hex #FA8072FF)
    /// </summary>
    public static readonly Color Salmon = new Color(0xFA8072FF);

    /// <summary>
    /// SandyBrown has an RGBA value of (244, 164, 96, 255) (hex #F4A460FF)
    /// </summary>
    public static readonly Color SandyBrown = new Color(0xF4A460FF);

    /// <summary>
    /// SeaGreen has an RGBA value of (46, 139, 87, 255) (hex #2E8B57FF)
    /// </summary>
    public static readonly Color SeaGreen = new Color(0x2E8B57FF);

    /// <summary>
    /// SeaShell has an RGBA value of (255, 245, 238, 255) (hex #FFF5EEFF)
    /// </summary>
    public static readonly Color SeaShell = new Color(0xFFF5EEFF);

    /// <summary>
    /// Sienna has an RGBA value of (160, 82, 45, 255) (hex #A0522DFF)
    /// </summary>
    public static readonly Color Sienna = new Color(0xA0522DFF);

    /// <summary>
    /// Silver has an RGBA value of (192, 192, 192, 255) (hex #C0C0C0FF)
    /// </summary>
    public static readonly Color Silver = new Color(0xC0C0C0FF);

    /// <summary>
    /// SkyBlue has an RGBA value of (135, 206, 235, 255) (hex #87CEEBFF)
    /// </summary>
    public static readonly Color SkyBlue = new Color(0x87CEEBFF);

    /// <summary>
    /// SlateBlue has an RGBA value of (106, 90, 205, 255) (hex #6A5ACDFF)
    /// </summary>
    public static readonly Color SlateBlue = new Color(0x6A5ACDFF);

    /// <summary>
    /// SlateGray has an RGBA value of (112, 128, 144, 255) (hex #708090FF)
    /// </summary>
    public static readonly Color SlateGray = new Color(0x708090FF);

    /// <summary>
    /// SlateGrey has an RGBA value of (112, 128, 144, 255) (hex #708090FF)
    /// </summary>
    public static readonly Color SlateGrey = new Color(0x708090FF);

    /// <summary>
    /// Snow has an RGBA value of (255, 250, 250, 255) (hex #FFFAFAFF)
    /// </summary>
    public static readonly Color Snow = new Color(0xFFFAFAFF);

    /// <summary>
    /// SpringGreen has an RGBA value of (0, 255, 127, 255) (hex #00FF7FFF)
    /// </summary>
    public static readonly Color SpringGreen = new Color(0x00FF7FFF);

    /// <summary>
    /// SteelBlue has an RGBA value of (70, 130, 180, 255) (hex #4682B4FF)
    /// </summary>
    public static readonly Color SteelBlue = new Color(0x4682B4FF);

    /// <summary>
    /// Tan has an RGBA value of (210, 180, 140, 255) (hex #D2B48CFF)
    /// </summary>
    public static readonly Color Tan = new Color(0xD2B48CFF);

    /// <summary>
    /// Teal has an RGBA value of (0, 128, 128, 255) (hex #008080FF)
    /// </summary>
    public static readonly Color Teal = new Color(0x008080FF);

    /// <summary>
    /// Thistle has an RGBA value of (216, 191, 216, 255) (hex #D8BFD8FF)
    /// </summary>
    public static readonly Color Thistle = new Color(0xD8BFD8FF);

    /// <summary>
    /// Tomato has an RGBA value of (255, 99, 71, 255) (hex #FF6347FF)
    /// </summary>
    public static readonly Color Tomato = new Color(0xFF6347FF);

    /// <summary>
    /// Turquoise has an RGBA value of (64, 224, 208, 255) (hex #40E0D0FF)
    /// </summary>
    public static readonly Color Turquoise = new Color(0x40E0D0FF);

    /// <summary>
    /// Violet has an RGBA value of (238, 130, 238, 255) (hex #EE82EEFF)
    /// </summary>
    public static readonly Color Violet = new Color(0xEE82EEFF);

    /// <summary>
    /// Wheat has an RGBA value of (245, 222, 179, 255) (hex #F5DEB3FF)
    /// </summary>
    public static readonly Color Wheat = new Color(0xF5DEB3FF);

    /// <summary>
    /// White has an RGBA value of (255, 255, 255, 255) (hex #FFFFFFFF)
    /// </summary>
    public static readonly Color White = new Color(0xFFFFFFFF);

    /// <summary>
    /// WhiteSmoke has an RGBA value of (245, 245, 245, 255) (hex #F5F5F5FF)
    /// </summary>
    public static readonly Color WhiteSmoke = new Color(0xF5F5F5FF);

    /// <summary>
    /// Yellow has an RGBA value of (255, 255, 0, 255) (hex #FFFF00FF)
    /// </summary>
    public static readonly Color Yellow = new Color(0xFFFF00FF);

    /// <summary>
    /// YellowGreen has an RGBA value of (154, 205, 50, 255) (hex #9ACD32FF)
    /// </summary>
    public static readonly Color YellowGreen = new Color(0x9ACD32FF);

    /// <summary>
    /// Transparent has an RGBA value of (0, 0, 0, 0) (hex #00000000)
    /// </summary>
    public static readonly Color Transparent = new Color(0x00000000);
}