using Easel.Content;
using Easel.Content.Builder;
using Easel.Graphics;
using Future.Common.csharp.registry;

namespace Future.Client.csharp.registry; 

public class ClientTextureRegistry : ContentRegistry, IContentRegistry {
    
    public static ImageContent CyberCarTexture { get; private set; }
    public static ImageContent FemaleTexture { get; private set; }
    public static ImageContent LogoTexture { get; private set; }
    public static ImageContent MenuBackgroundTexture { get; private set; }

    public void Load(ContentManager content) {
        CyberCarTexture = this.Register(FutureClient.ContentBuilder, new ImageContent("textures/entity/vehicle/cyber_car.png"));
        FemaleTexture = this.Register(FutureClient.ContentBuilder, new ImageContent("textures/entity/player/female.png"));
        LogoTexture = this.Register(FutureClient.ContentBuilder, new ImageContent("textures/logo/logo_transparent.png"));
        MenuBackgroundTexture = this.Register(FutureClient.ContentBuilder, new ImageContent("textures/gui/menu_background.png"));
    }

    public static Texture2D Get(ImageContent imageContent, SamplerState state = null) {
        Texture2D texture = ContentRegistry.Get<Texture2D>(imageContent);
        texture.SamplerState = state ?? SamplerState.PointClamp;
        
        return texture;
    }
}