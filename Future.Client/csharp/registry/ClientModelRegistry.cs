using Easel.Content.Builder;
using Easel.Graphics;
using Easel.Graphics.Materials;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientModelRegistry : Registry {
    
    public static readonly Model CyberCarModel = LoadModel(FutureClient.ContentBuilder, new ModelContent("models/entity/vehicle/cyber_car.glb", false), ClientMaterialRegistry.CyberCarMaterial);
    public static readonly Model FemaleModel = LoadModel(FutureClient.ContentBuilder, new ModelContent("models/entity/player/female.glb", false), ClientMaterialRegistry.FemaleMaterial);

    public static Model LoadModel(ContentBuilder builder, ModelContent modelContent, Material material) {
        Model model = Load<Model>(builder, modelContent);

        for (int i = 0; i < model.Materials.Length; i++) {
            ref Material modelMaterial = ref model.Materials[i];

            modelMaterial = material;
        }
        
        return model;
    }
}