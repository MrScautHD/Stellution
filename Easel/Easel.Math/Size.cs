using System;
using System.Numerics;
using System.Xml.Serialization;

namespace Easel.Math;

public struct Size<T> : IEquatable<Size<T>> where T : INumber<T>
{
    public static readonly Size<T> Zero = new Size<T>(T.Zero);
    
    [XmlAttribute]
    public T Width;
    
    [XmlAttribute]
    public T Height;

    public Size(T width, T height)
    {
        Width = width;
        Height = height;
    }

    public Size(T wh)
    {
        Width = wh;
        Height = wh;
    }
    
    public bool Equals(Size<T> other)
    {
        return Width == other.Width && Height == other.Height;
    }

    public override bool Equals(object obj)
    {
        return obj is Size<T> other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Width, Height);
    }

    public static bool operator ==(Size<T> left, Size<T> right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Size<T> left, Size<T> right)
    {
        return !left.Equals(right);
    }

    public static Size<T> operator +(Size<T> left, Size<T> right)
    {
        return new Size<T>(left.Width + right.Width, left.Height + right.Height);
    }

    public static Size<T> operator -(Size<T> left, Size<T> right)
    {
        return new Size<T>(left.Width - right.Width, left.Height - right.Height);
    }

    public static Size<T> operator *(Size<T> left, Size<T> right)
    {
        return new Size<T>(left.Width * right.Width, left.Height * right.Height);
    }

    public static Size<T> operator *(Size<T> left, T right)
    {
        return new Size<T>(left.Width * right, left.Height * right);
    }

    public static Size<T> operator /(Size<T> left, Size<T> right)
    {
        return new Size<T>(left.Width / right.Width, left.Height / right.Height);
    }

    public static Size<T> operator /(Size<T> left, T right)
    {
        return new Size<T>(left.Width / right, left.Height / right);
    }

    public static implicit operator Size<float>(Size<T> size)
    {
        float width = Convert.ToSingle(size.Width);
        float height = Convert.ToSingle(size.Height);
        return new Size<float>(width, height);
    }
    
    public static explicit operator Vector2<T>(Size<T> size) => new Vector2<T>(size.Width, size.Height);

    public static explicit operator Size<T>(Vector2<T> vector) => new Size<T>(vector.X, vector.Y);

    public static explicit operator System.Drawing.Size(Size<T> size)
    {
        int width = Convert.ToInt32(size.Width);
        int height = Convert.ToInt32(size.Height);
        
        return new System.Drawing.Size(width, height);
    }

    public static explicit operator Size<T>(System.Drawing.Size size) =>
        new Size<T>(T.CreateChecked(size.Width), T.CreateChecked(size.Height));

    public override string ToString()
    {
        return Width + "x" + Height;
    }
}