using System.Collections.Generic;
using Easel.Content;
using Easel.Graphics;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientSkyboxRegistry : Registry, IRegistry {
    
    // REGISTRY LIST
    public static readonly Dictionary<string, Skybox> Skyboxes = new();

    // REGISTRIES
    public static Skybox EarthSkybox { get; private set; }
    
    public void Initialize(ContentManager content) {
        EarthSkybox = this.RegisterSkybox("earth", Skyboxes, content, "textures/sky/skybox/earth");
    }

    protected Skybox RegisterSkybox(string key, Dictionary<string, Skybox> registryList, ContentManager content, string path) {
        string sideKey = key + "_" + "side";
        string topKey = key + "_" + "top";
        string bottomKey = key + "_" + "bottom";

        Bitmap side = this.RegisterLoad(sideKey + "_map", ClientBitmapRegistry.Bitmaps, content, path + "/" + sideKey + ".bmp");
        Bitmap top = this.RegisterLoad(topKey + "_map", ClientBitmapRegistry.Bitmaps, content, path + "/" + topKey + ".bmp");
        Bitmap bottom = this.RegisterLoad(bottomKey + "_map", ClientBitmapRegistry.Bitmaps, content, path + "/" + bottomKey + ".bmp");

        return EarthSkybox = this.Register(key + "_skybox", registryList, new Skybox(side, side, top, bottom, side, side));
    }
}