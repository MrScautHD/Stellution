using Easel.Graphics;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry.types; 

public class SkyboxRegistry : Registry {
    
    public static Skybox EarthSkybox { get; private set; }

    public override void Initialize() {
        EarthSkybox = new Skybox(BitmapRegistry.SkyEarthSide.Value, BitmapRegistry.SkyEarthSide.Value, BitmapRegistry.SkyEarthTop.Value, BitmapRegistry.SkyEarthBottom.Value, BitmapRegistry.SkyEarthSide.Value, BitmapRegistry.SkyEarthSide.Value);
    }
}