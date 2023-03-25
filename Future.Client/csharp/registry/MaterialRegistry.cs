using Easel.Content;
using Easel.Graphics;
using Easel.Graphics.Materials;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class MaterialRegistry : Registry, IRegistry {
    
    public static StandardMaterial CyberCar { get; private set; }
    public static TranslucentStandardMaterial Female { get; private set; }
    
    public void Initialize(ContentManager content) {
        CyberCar = new StandardMaterial(TextureRegistry.CyberCar);
        Female = new TranslucentStandardMaterial(Texture2D.Black, TextureRegistry.Female, Texture2D.White);
    }
}