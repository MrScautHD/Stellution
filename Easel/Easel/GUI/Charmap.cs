using System;
using System.Collections.Generic;
using System.Linq;
using Easel.Graphics;
using Easel.Math;
using Pie.Freetype;

namespace Easel.GUI;

public class Charmap : IDisposable
{
    public readonly Texture2D Texture;

    private readonly Dictionary<char, Character> _characters;

    public Charmap(Texture2D texture, Dictionary<char, Character> characters)
    {
        Texture = texture;
        _characters = characters;
    }

    public Character GetCharacter(char c)
    {
        if (!_characters.TryGetValue(c, out Character character))
            character = _characters['?'];
        return character;
    }

    public struct Character
    {
        public Rectangle<int> Source;
        public Vector2<int> Bearing;
        public int Advance;
    }

    public void Dispose()
    {
        Texture.Dispose();
    }
}