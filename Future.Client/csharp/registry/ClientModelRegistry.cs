using System.Collections.Generic;
using Easel.Content;
using Easel.Graphics;
using Easel.Graphics.Materials;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientModelRegistry : Registry, IRegistry {
    
    // REGISTRY LIST
    public static readonly Dictionary<string, Model> Models = new();

    // REGISTRIES
    public static Model CyberCarModel { get; private set; }
    
    public void Initialize(ContentManager content) {
        CyberCarModel = this.LoadModel("cyber_car", Models, content, "models/entity/vehicle/cyber_car.gltf", ClientTextureRegistry.CyberCarTexture);
    }

    protected Model LoadModel(string key, Dictionary<string, Model> registryList, ContentManager content, string path, Texture2D texture, bool flipUvs = false) {
        Model model = new Model(content.GetFullPath(path), flipUvs);

        Material material = new StandardMaterial(texture, Texture2D.EmptyNormal, Texture2D.White); //TODO play around with this Values

        for (int i = 0; i < model.Meshes.Length; i++) {
            ref ModelMesh modelMesh = ref model.Meshes[i];

            for (int j = 0; j < modelMesh.Meshes.Length; j++) {
                ref Mesh mesh = ref modelMesh.Meshes[j];
                
                mesh.Material = material;
            }
        }

        registryList.Add(key, model);
        return model;
    }
}