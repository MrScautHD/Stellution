using System.Collections.Generic;
using Easel.Content;
using Easel.Graphics;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientTextureRegistry : Registry, IRegistry {
    
    public static readonly Dictionary<string, Texture2D> Textures = new();
    
    public static Texture2D CyberCarTexture { get; private set; }
    public static Texture2D FemaleTexture { get; private set; }
    
    public void InitializePre(ContentManager content) {
        CyberCarTexture = this.LoadTexture("cyber_car", Textures, content, SamplerState.PointClamp, "textures/entity/vehicle/cyber_car.png");
        FemaleTexture = this.LoadTexture("female", Textures, content, SamplerState.PointClamp, "textures/entity/player/female.png");
    }

    protected Texture2D LoadTexture(string key, Dictionary<string, Texture2D> registryList, ContentManager content, SamplerState state, string path) {
        Texture2D texture = content.Load<Texture2D>(path);
        texture.SamplerState = state;
        
        registryList.Add(key, texture);
        return texture;
    }
}