using System.Collections.Generic;
using Easel.Content.Builder;
using Easel.Graphics;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientSkyboxRegistry : Registry {
    
    public static readonly Dictionary<string, Skybox> Skyboxes = new();
    
    public static readonly Skybox EarthSkybox = RegisterSkybox("earth", Skyboxes, FutureClient.ContentBuilder, "textures/sky/skybox/earth");
    
    public static Skybox RegisterSkybox(string key, Dictionary<string, Skybox> registryList, ContentBuilder builder, string path) {
        string sideKey = key + "_" + "side";
        string topKey = key + "_" + "top";
        string bottomKey = key + "_" + "bottom";

        Bitmap side = Load<Bitmap>(builder, new ImageContent(path + "/" + sideKey + ".bmp"));
        Bitmap top = Load<Bitmap>(builder, new ImageContent(path + "/" + topKey + ".bmp"));
        Bitmap bottom = Load<Bitmap>(builder, new ImageContent(path + "/" + bottomKey + ".bmp"));

        return Register(key + "_skybox", registryList, new Skybox(side, side, top, bottom, side, side));
    }
}