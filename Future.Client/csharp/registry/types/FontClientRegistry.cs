using System.IO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SpriteFontPlus;

namespace Future.Client.csharp.registry.types; 

public class FontClientRegistry : ClientRegistry {
    
    public static SpriteFont Fontoe { get; private set; }

    public override void LoadContent(GraphicsDevice graphicsDevice, ContentManager content) {
        Fontoe = this.Register("fontoe", graphicsDevice, content);
    }

    private SpriteFont Register(string name, GraphicsDevice graphicsDevice, ContentManager content) {
        TtfFontBakerResult fontResult = TtfFontBaker.Bake(File.ReadAllBytes(content.RootDirectory + "/fonts/" + name + ".ttf"), 25, 1024, 1024, new[] { 
                CharacterRange.BasicLatin,
                CharacterRange.Latin1Supplement,
                CharacterRange.LatinExtendedA,
                CharacterRange.Cyrillic
            }
        );
        
        SpriteFont font = fontResult.CreateSpriteFont(graphicsDevice);
        
        RegistryTypes.Fonts.Add(name, font);
        return font;
    }
}