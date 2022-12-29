using System.IO;
using FontStashSharp;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Future.Client.csharp.registry.types; 

public class FontRegistry : IClientRegistry {
    
    public static FontSystem Fontoe { get; private set; }

    public void LoadContent(GraphicsDevice graphicsDevice, ContentManager content) {
        Fontoe = this.Register("fontoe", content);
    }

    private FontSystem Register(string name, ContentManager content) {
        FontSystem font = new FontSystem();
        font.AddFont(File.ReadAllBytes(content.RootDirectory + "/fonts/" + name + ".ttf"));
        
        RegistryTypes.Fonts.Add(name, font);
        return font;
    }
}