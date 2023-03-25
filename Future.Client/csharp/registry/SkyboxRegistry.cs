using Easel.Content;
using Easel.Graphics;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class SkyboxRegistry : Registry, IRegistry {
    
    public static Skybox EarthSkybox { get; private set; }

    public void Initialize(ContentManager content) {
        EarthSkybox = new Skybox(BitmapRegistry.SkyEarthSide, BitmapRegistry.SkyEarthSide, BitmapRegistry.SkyEarthTop, BitmapRegistry.SkyEarthBottom, BitmapRegistry.SkyEarthSide, BitmapRegistry.SkyEarthSide);
    }
}