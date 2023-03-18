using System.Collections.Generic;
using Easel.Content;
using Easel.Graphics;
using Easel.Graphics.Materials;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientMaterialRegistry : Registry, IRegistry {
    
    public static readonly Dictionary<string, StandardMaterial> Materials = new();
    
    public static StandardMaterial CyberCarMaterial { get; private set; }
    public static StandardMaterial FemaleMaterial { get; private set; }
    
    public void Register(ContentManager content) {
        CyberCarMaterial = this.Register("cyber_car", Materials, new StandardMaterial(ClientTextureRegistry.Get(ClientTextureRegistry.CyberCarTexture, SamplerState.PointClamp)));
        FemaleMaterial = this.Register("female", Materials, new StandardMaterial(ClientTextureRegistry.Get(ClientTextureRegistry.FemaleTexture, SamplerState.PointClamp)));
    }
}