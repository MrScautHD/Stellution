using System.Collections.Generic;
using Easel.Graphics.Materials;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientMaterialRegistry : Registry {
    
    public static readonly Dictionary<string, StandardMaterial> Materials = new();
    
    public static readonly StandardMaterial CyberCarMaterial = Register("cyber_car", Materials, new StandardMaterial(ClientTextureRegistry.CyberCarTexture));
    public static readonly StandardMaterial FemaleMaterial = Register("female", Materials, new StandardMaterial(ClientTextureRegistry.FemaleTexture));
}