using Easel;
using Easel.Content;
using Easel.Content.Builder;
using Easel.Graphics;
using Easel.Graphics.Materials;
using Stellution.Common.csharp.registry;

namespace Stellution.Client.csharp.registry; 

public class ModelRegistry : Registry, IRegistry {
    
    public static readonly string DefinitionName = "content/models";
    public static ContentManager Content => EaselGame.Instance.Content;

    public static Model CyberCar => ModelGetter(DefinitionName, "entity/vehicle/cyber_car", MaterialRegistry.CyberCar);
    public static Model Female => ModelGetter(DefinitionName, "entity/player/female", MaterialRegistry.Female);

    public void Initialize() {
        ContentDefinition definition = new ContentBuilder(DefinitionName)
            .Add(new ModelContent("entity/vehicle/cyber_car.glb", false))
            .Add(new ModelContent("entity/player/female.glb", false))
            .Build();

        Content.AddContent(definition);
    }
    
    /**
     * Use this to get the "MODEL" with the right "Material".
     */
    protected static Model ModelGetter(string definitionName, string friendlyName, Material material) {
        Model model = Content.Load<Model>(definitionName, friendlyName);

        foreach (ModelMesh modelMesh in model.Meshes) {
            for (int i = 0; i < modelMesh.Meshes.Length; i++) {
                modelMesh.Meshes[i].Material = material;
            }
        }

        return model;
    }
}