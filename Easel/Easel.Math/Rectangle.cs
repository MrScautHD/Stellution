using System;
using System.Numerics;
using System.Xml.Serialization;

namespace Easel.Math;

public struct Rectangle<T> : IEquatable<Rectangle<T>> where T : INumber<T>
{
    [XmlAttribute]
    public T X;
    
    [XmlAttribute]
    public T Y;
    
    [XmlAttribute]
    public T Width;
    
    [XmlAttribute]
    public T Height;

    public Vector2<T> Location => new Vector2<T>(X, Y);

    public Size<T> Size => new Size<T>(Width, Height);

    public T Left => X;

    public T Top => Y;

    public T Right => X + Width;

    public T Bottom => Y + Height;

    public Rectangle(T x, T y, T width, T height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }

    public Rectangle(Vector2<T> location, Size<T> size) : this(location.X, location.Y, size.Width, size.Height) { }

    public bool Equals(Rectangle<T> other)
    {
        return X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
    }

    public override bool Equals(object obj)
    {
        return obj is Rectangle<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Width, Height);
    }

    public static bool operator ==(Rectangle<T> left, Rectangle<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Rectangle<T> left, Rectangle<T> right)
    {
        return !left.Equals(right);
    }

    public static explicit operator System.Drawing.Rectangle(Rectangle<T> rectangle)
    {
        int x = Convert.ToInt32(rectangle.X);
        int y = Convert.ToInt32(rectangle.Y);
        int w = Convert.ToInt32(rectangle.Width);
        int h = Convert.ToInt32(rectangle.Height);
        
        return new System.Drawing.Rectangle(x, y, w, h);
    }

    public static explicit operator Rectangle<T>(System.Drawing.Rectangle rectangle) =>
        new Rectangle<T>(T.CreateChecked(rectangle.X), T.CreateChecked(rectangle.Y), T.CreateChecked(rectangle.Width),
            T.CreateChecked(rectangle.Height));

    public override string ToString()
    {
        return "Rectangle(X: " + X + ", Y: " + Y + ", Width: " + Width + ", Height: " + Height + ")";
    }
}