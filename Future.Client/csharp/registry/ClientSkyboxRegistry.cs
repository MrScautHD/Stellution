using System.Collections.Generic;
using Easel.Content;
using Easel.Content.Builder;
using Easel.Graphics;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientSkyboxRegistry : ContentRegistry, IContentRegistry {
    
    public static readonly Dictionary<string, Dictionary<string, Skybox>> Skyboxes = new();
    
    public static KeyValuePair<string, List<ImageContent>> EarthSkybox { get; private set; }
    
    public void Load(ContentManager content) {
        EarthSkybox = this.RegisterSkybox("earth", FutureClient.ContentBuilder, "textures/sky/skybox/earth");
    }
    
    protected KeyValuePair<string, List<ImageContent>> RegisterSkybox(string key, ContentBuilder builder, string path) {
        string sideKey = key + "_" + "side";
        string topKey = key + "_" + "top";
        string bottomKey = key + "_" + "bottom";

        ImageContent side = this.Register(builder, new ImageContent(path + "/" + sideKey + ".bmp"));
        ImageContent top = this.Register(builder, new ImageContent(path + "/" + topKey + ".bmp"));
        ImageContent bottom = this.Register(builder, new ImageContent(path + "/" + bottomKey + ".bmp"));

        List<ImageContent> contentList = new List<ImageContent>();
        contentList.Add(side);
        contentList.Add(top);
        contentList.Add(bottom);
        
        return KeyValuePair.Create(key, contentList);
    }

    public static Skybox Get(KeyValuePair<string, List<ImageContent>> skyboxContent) {
        Bitmap side = ClientTextureRegistry.Get<Bitmap>(skyboxContent.Value[0]);
        Bitmap top = ClientTextureRegistry.Get<Bitmap>(skyboxContent.Value[1]);
        Bitmap bottom = ClientTextureRegistry.Get<Bitmap>(skyboxContent.Value[2]);

        if (Skyboxes.ContainsKey(skyboxContent.Key)) {
            return Skyboxes.GetValueOrDefault(skyboxContent.Key).GetValueOrDefault(skyboxContent.Key);
        }
        else {
            Skybox skybox = new Skybox(side, side, top, bottom, side, side);

            Dictionary<string, Skybox> dictionary = new Dictionary<string, Skybox>();
            dictionary.Add(skyboxContent.Key, skybox);
            
            Skyboxes.Add(skyboxContent.Key, dictionary);

            return skybox;
        }
    }
}