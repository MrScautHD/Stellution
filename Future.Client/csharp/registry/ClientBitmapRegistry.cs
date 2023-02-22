using System.Collections.Generic;
using Easel.Content;
using Easel.Graphics;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientBitmapRegistry : Registry, IRegistry {
    
    // REGISTRY LIST
    public static readonly Dictionary<string, Bitmap> Bitmaps = new();

    // REGISTRIES
    public static Bitmap EarthSkyboxMap { get; private set; }
    
    public void Initialize(ContentManager content) {
        //EarthSkyboxMap = this.RegisterLoad("earth_skybox", Bitmaps, content, "textures/sky/skybox/earth.bmp");
    }
}