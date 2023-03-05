using Easel.Content.Builder;
using Easel.Graphics;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientTextureRegistry : Registry {
    
    public static readonly Texture2D CyberCarTexture = LoadTexture(FutureClient.ContentBuilder, SamplerState.PointClamp, new ImageContent("textures/entity/vehicle/cyber_car.png"));
    public static readonly Texture2D FemaleTexture = LoadTexture(FutureClient.ContentBuilder, SamplerState.PointClamp, new ImageContent("textures/entity/player/female.png"));
    public static readonly Texture2D LogoTexture = LoadTexture(FutureClient.ContentBuilder, SamplerState.PointClamp, new ImageContent("textures/logo/logo_transparent.png"));
    public static readonly Texture2D MenuBackgroundTexture = LoadTexture(FutureClient.ContentBuilder, SamplerState.AnisotropicClamp, new ImageContent("textures/gui/menu_background.png"));

    public static Texture2D LoadTexture(ContentBuilder builder, SamplerState state, ImageContent imageContent) {
        Texture2D texture = Load<Texture2D>(builder, imageContent);
        texture.SamplerState = state;
        
        return texture;
    }
}