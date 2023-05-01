using Easel.Graphics;
using Easel.Graphics.Materials;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry; 

public class MaterialRegistry : Registry {
    
    public static TranslucentStandardMaterial CyberCar { get; private set; }
    public static TranslucentStandardMaterial Female { get; private set; }
    
    public override void Initialize() {
        CyberCar = new TranslucentStandardMaterial(TextureRegistry.CyberCar) { BlendState = BlendState.AlphaBlend };
        Female = new TranslucentStandardMaterial(TextureRegistry.Female) { BlendState = BlendState.AlphaBlend } ;
    }
}