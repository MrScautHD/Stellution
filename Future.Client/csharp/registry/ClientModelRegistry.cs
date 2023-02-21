using System.Collections.Generic;
using Easel.Content;
using Easel.Graphics;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientModelRegistry : Registry, IRegistry {
    
    // REGISTRY LIST
    public static readonly Dictionary<string, Model> Models = new();

    // REGISTRIES
    public static Model CyberCarModel { get; private set; }
    
    public void Initialize(ContentManager content) {
        CyberCarModel = this.RegisterLoad("cyber_car", Models, content, "entity/vehicle/cyber_car.fbx");
    }
}