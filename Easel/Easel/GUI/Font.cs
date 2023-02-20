using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using Easel.Graphics;
using Easel.Graphics.Renderers;
using Easel.GUI.BBCode;
using Easel.Math;
using Pie.Freetype;

namespace Easel.GUI;

public class Font : IDisposable
{
    public readonly Face Face;

    private Dictionary<uint, Charmap> _charmaps;

    public Font(string path)
    {
        Face = FontHelper.FreeType.CreateFace(path, 0);
        
        _charmaps = new Dictionary<uint, Charmap>();
    }

    public Font(byte[] data)
    {
        Face = FontHelper.FreeType.CreateFace(data, 0);
        
        _charmaps = new Dictionary<uint, Charmap>();
    }

    public void Draw(SpriteRenderer renderer, uint size, string text, Vector2<int> position, Color color)
    {
        if (!_charmaps.TryGetValue(size, out Charmap charmap))
        {
            Face.Size = (int) size;
            charmap = FontHelper.GenerateCharmapInRange(Face, (char) 0, (char) 127);
            _charmaps.Add(size, charmap);
        }

        Vector2<int> pos = position;
        int largestChar = 0;
        foreach (char c in text)
        {
            Charmap.Character chr = charmap.GetCharacter(c);
            if (chr.Source.Height > largestChar)
                largestChar = chr.Bearing.Y;
        }
        pos.Y += largestChar;

        foreach (char c in text)
        {
            switch (c)
            {
                case '\n':
                    pos.Y += (int) size;
                    pos.X = position.X;
                    continue;
            }
            
            Charmap.Character chr = charmap.GetCharacter(c);
            Vector2<int> charPos = new Vector2<int>(pos.X + chr.Bearing.X,
                pos.Y - chr.Source.Height + (chr.Source.Height - chr.Bearing.Y));
            renderer.Draw(charmap.Texture, (Vector2<float>) charPos, chr.Source, color, 0, Vector2<float>.Zero, Vector2<float>.One);
            pos.X += chr.Advance;
        }
    }

    public void DrawBBCode(SpriteRenderer renderer, uint size, string text, Vector2<int> position, Color initialColor)
    {
        if (!_charmaps.TryGetValue(size, out Charmap charmap))
        {
            Face.Size = (int) size;
            charmap = FontHelper.GenerateCharmapInRange(Face, (char) 0, (char) 127);
            _charmaps.Add(size, charmap);
        }

        BBCodeInstruction[] bbCode = BBCodeParser.Parse(text);

        Vector2<int> pos = position;
        int largestChar = 0;
        foreach (char c in text)
        {
            Charmap.Character chr = charmap.GetCharacter(c);
            if (chr.Source.Height > largestChar)
                largestChar = chr.Bearing.Y;
        }
        pos.Y += largestChar;
        
        ref Color currentColor = ref initialColor;

        foreach (BBCodeInstruction instruction in bbCode)
        {
            if (instruction == null)
                continue;
            
            switch (instruction.InstructionType)
            {
                case InstructionType.Text:
                    TextInstruction tInst = (TextInstruction) instruction;
                    foreach (char c in tInst.Text)
                    {
                        switch (c)
                        {
                            case '\n':
                                pos.Y += (int) size;
                                pos.X = position.X;
                                continue;
                        }

                        Charmap.Character chr = charmap.GetCharacter(c);
                        Vector2<int> charPos = new Vector2<int>(pos.X + chr.Bearing.X,
                            pos.Y - chr.Source.Height + (chr.Source.Height - chr.Bearing.Y));
                        renderer.Draw(charmap.Texture, (Vector2<float>) charPos, chr.Source, currentColor, 0,
                            Vector2<float>.Zero, Vector2<float>.One);
                        pos.X += chr.Advance;
                    }

                    break;
                case InstructionType.Color:
                    ColorInstruction cInst = (ColorInstruction) instruction;
                    if (cInst.IsExiting)
                        currentColor = ref initialColor;
                    else
                        currentColor = ref cInst.Color;
                    break;
            }
        }
    }

    public Size<int> MeasureString(uint size, string text)
    {
        if (!_charmaps.TryGetValue(size, out Charmap charmap))
        {
            Face.Size = (int) size;
            charmap = FontHelper.GenerateCharmapInRange(Face, (char) 0, (char) 127);
            _charmaps.Add(size, charmap);
        }

        int largestChar = 0;
        foreach (char c in text)
        {
            Charmap.Character chr = charmap.GetCharacter(c);
            if (chr.Source.Height > largestChar)
                largestChar = chr.Bearing.Y;
        }

        int pos = 0;
        Size<int> measuredSize = new Size<int>(0, largestChar);

        int i = 0;
        foreach (char c in text)
        {
            switch (c)
            {
                case '\n':
                    measuredSize.Height += (int) size;
                    pos = 0;
                    continue;
            }
            
            Charmap.Character chr = charmap.GetCharacter(c);
            pos += chr.Advance;
            if (pos > measuredSize.Width)
                measuredSize.Width = pos;
            i++;
        }

        return measuredSize;
    }

    public Size<int> MeasureStringBBCode(uint size, string text)
    {
        if (!_charmaps.TryGetValue(size, out Charmap charmap))
        {
            Face.Size = (int) size;
            charmap = FontHelper.GenerateCharmapInRange(Face, (char) 0, (char) 127);
            _charmaps.Add(size, charmap);
        }

        int largestChar = 0;
        foreach (char c in text)
        {
            Charmap.Character chr = charmap.GetCharacter(c);
            if (chr.Source.Height > largestChar)
                largestChar = chr.Bearing.Y;
        }

        int pos = 0;
        Size<int> measuredSize = new Size<int>(0, largestChar);

        foreach (BBCodeInstruction instruction in BBCodeParser.Parse(text))
        {
            if (instruction is not { InstructionType: InstructionType.Text })
                continue;
            
            foreach (char c in ((TextInstruction) instruction).Text)
            {
                switch (c)
                {
                    case '\n':
                        measuredSize.Height += (int) size;
                        pos = 0;
                        continue;
                }
            
                Charmap.Character chr = charmap.GetCharacter(c);
                pos += chr.Advance;
                if (pos > measuredSize.Width)
                    measuredSize.Width = pos;
            }
        }
        
        return measuredSize;
    }
    

    public void Dispose()
    {
        foreach ((_, Charmap charmap) in _charmaps)
            charmap.Dispose();
    }
}