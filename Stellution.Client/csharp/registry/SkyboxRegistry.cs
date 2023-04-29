using Easel.Graphics;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry; 

public class SkyboxRegistry : Registry, IRegistry {
    
    public static Skybox EarthSkybox { get; private set; }

    public void Initialize() {
        EarthSkybox = new Skybox(BitmapRegistry.SkyEarthSide, BitmapRegistry.SkyEarthSide, BitmapRegistry.SkyEarthTop, BitmapRegistry.SkyEarthBottom, BitmapRegistry.SkyEarthSide, BitmapRegistry.SkyEarthSide);
    }
}