using Easel.Graphics;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry; 

public class SkyboxRegistry : Registry {
    
    public static Skybox EarthSkybox { get; private set; }

    public override void Initialize() {
        EarthSkybox = new Skybox(BitmapRegistry.SkyEarthSide, BitmapRegistry.SkyEarthSide, BitmapRegistry.SkyEarthTop, BitmapRegistry.SkyEarthBottom, BitmapRegistry.SkyEarthSide, BitmapRegistry.SkyEarthSide);
    }
}