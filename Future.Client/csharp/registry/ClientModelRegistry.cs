using Easel.Content;
using Easel.Content.Builder;
using Easel.Graphics;
using Easel.Graphics.Materials;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientModelRegistry : ContentRegistry, IContentRegistry {
    
    public static ModelContent CyberCarModel { get; private set; }
    public static ModelContent FemaleModel { get; private set; }

    public void Load(ContentManager content) {
        CyberCarModel = this.Register(FutureClient.ContentBuilder, new ModelContent("models/entity/vehicle/cyber_car.glb", false));
        FemaleModel = this.Register(FutureClient.ContentBuilder, new ModelContent("models/entity/player/female.glb", false));
    }

    public static Model Get(ModelContent modelContent, Material material) {
        Model model = ContentRegistry.Get<Model>(modelContent);

        for (int i = 0; i < model.Materials.Length; i++) {
            ref Material modelMaterial = ref model.Materials[i];

            modelMaterial = material;
        }
        
        return model;
    }
}