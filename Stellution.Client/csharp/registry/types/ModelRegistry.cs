using System;
using Easel.Content.Builder;
using Easel.Graphics;
using Easel.Graphics.Materials;

namespace Stellution.Client.csharp.registry.types; 

public class ModelRegistry : ContentRegistry {
    
    public static readonly string DefinitionName = "content/models";

    public static readonly Lazy<Model> CyberCar = ModelGetter(DefinitionName, "entity/vehicle/cyber_car", MaterialRegistry.CyberCar);
    public static readonly Lazy<Model> Female = ModelGetter(DefinitionName, "entity/player/female", MaterialRegistry.Female);

    public override void Initialize() {
        ContentDefinition definition = new ContentBuilder(DefinitionName)
            .Add(new ModelContent("entity/vehicle/cyber_car.glb", false))
            .Add(new ModelContent("entity/player/female.glb", false))
            .Build();

        Content.AddContent(definition);
    }
    
    /**
     * Use this to get the "MODEL" with the right "Material".
     */
    protected static Lazy<Model> ModelGetter(string definitionName, string path, Material material) {
        Lazy<Model> lazy = new Lazy<Model>(() => {
            Model model = Content.Load<Model>(definitionName, path);

            foreach (ModelMesh modelMesh in model.Meshes) {
                for (int i = 0; i < modelMesh.Meshes.Length; i++) {
                    modelMesh.Meshes[i].Material = material;
                }
            }

            return model;
        });

        return lazy;
    }
}