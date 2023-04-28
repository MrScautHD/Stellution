using Easel;
using Easel.Content;
using Easel.Graphics;
using Easel.Graphics.Materials;

namespace Stellution.Common.csharp.registry; 

public class Registry {
    
    protected static ContentManager Content => EaselGame.Instance.Content;
    
    public static readonly List<IRegistry> RegistryTypes = new();

    /**
     * Use this to get the "TEXTURE" with the right "Sample-State".
     */
    protected static Texture2D TextureGetter(string definitionName, string friendlyName, SamplerState? state = null) {
        Texture2D texture = Content.Load<Texture2D>(definitionName, friendlyName);
        texture.SamplerState = state ?? SamplerState.PointClamp;

        return texture;
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