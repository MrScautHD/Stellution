using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.registry.types; 

public class FontRegistry : IClientRegistry {
    
    public static DynamicSpriteFont Fontoe { get; private set; }

    public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content) {
        Fontoe = this.Register("fontoe", content, 18);
    }

    private DynamicSpriteFont Register(string name, ContentManager content, int size) {
        FontSystem font = new FontSystem();
        font.AddFont(File.ReadAllBytes(content.RootDirectory + "/fonts/" + name + ".ttf"));
        
        ClientRegistryTypes.Fonts.Add(name, font.GetFont(size));
        return font.GetFont(size);
    }
}