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
        string rightKey = key + "_" + "right";
        string leftKey = key + "_" + "left";
        string topKey = key + "_" + "top";
        string bottomKey = key + "_" + "bottom";
        string frontKey = key + "_" + "front";
        string backKey = key + "_" + "back";
        
        Bitmap right = this.RegisterLoad(rightKey + "_map", ClientBitmapRegistry.Bitmaps, content, path + "/" + rightKey + ".bmp");
        Bitmap left = this.RegisterLoad(leftKey + "_map", ClientBitmapRegistry.Bitmaps, content, path + "/" + leftKey + ".bmp");
        Bitmap top = this.RegisterLoad(topKey + "_map", ClientBitmapRegistry.Bitmaps, content, path + "/" + topKey + ".bmp");
        Bitmap bottom = this.RegisterLoad(bottomKey + "_map", ClientBitmapRegistry.Bitmaps, content, path + "/" + bottomKey + ".bmp");
        Bitmap front = this.RegisterLoad(frontKey + "_map", ClientBitmapRegistry.Bitmaps, content, path + "/" + frontKey + ".bmp");
        Bitmap back = this.RegisterLoad(backKey + "_map", ClientBitmapRegistry.Bitmaps, content, path + "/" + backKey + ".bmp");

        return EarthSkybox = this.Register(key + "_skybox", registryList, new Skybox(right, left, top, bottom, front, back));
    }
}