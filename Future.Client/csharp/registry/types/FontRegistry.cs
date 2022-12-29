using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;

namespace Future.Client.csharp.registry.types; 

public class FontRegistry : IClientRegistry {
    
    public static SpriteFont Fontoe { get; private set; }

    public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content) {
        Fontoe = this.Register("fontoe", graphicsDevice, content, 25, 1024, 1024, new[] {
            CharacterRange.BasicLatin,
            CharacterRange.Latin1Supplement,
            CharacterRange.LatinExtendedA,
            CharacterRange.Cyrillic
        });
    }

    private SpriteFont Register(string name, GraphicsDevice graphicsDevice, ContentManager content, int fontPixelHeight, int bitmapWidth, int bitmapHeight, IEnumerable<CharacterRange> characterRanges) {
        TtfFontBakerResult fontResult = TtfFontBaker.Bake(File.ReadAllBytes(content.RootDirectory + "/fonts/" + name + ".ttf"), fontPixelHeight, bitmapWidth, bitmapHeight, characterRanges);
        
        SpriteFont font = fontResult.CreateSpriteFont(graphicsDevice);
        
        RegistryTypes.Fonts.Add(name, font);
        return font;
    }
}